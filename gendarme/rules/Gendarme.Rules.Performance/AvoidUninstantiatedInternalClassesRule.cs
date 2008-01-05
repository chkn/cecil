//
// Gendarme.Rules.Performance.AvoidUninstantiatedInternalClassesRule
//
// Authors:
//	Nidhi Rawal <sonu2404@gmail.com>
//	Sebastien Pouliot <sebastien@ximian.com>
//
// Copyright (c) <2007> Nidhi Rawal
// Copyright (C) 2007-2008 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Gendarme.Framework;
using Gendarme.Framework.Rocks;

namespace Gendarme.Rules.Performance {

	public class AvoidUninstantiatedInternalClassesRule: ITypeRule {

		private static bool CheckSpecialTypes (TypeDefinition type)
		{
			// <Module> isn't tagged as generated by the compiler but still must be ignored
			if (type.Name == Constants.ModuleType)
				return true;

			// some stuff, like arrays, is nested under <PrivateImplementationDetails>
			if (type.IsNested)
				return type.IsGeneratedCode () || type.DeclaringType.IsGeneratedCode ();

			return type.IsGeneratedCode ();
		}

		// we use this to cache the information about the assembly
		// i.e. all types instantiated by the assembly
		private Dictionary<AssemblyDefinition,List<TypeReference>> cache;

		public void CacheInstantiationFromAssembly (AssemblyDefinition assembly)
		{
			if (cache == null)
				cache = new Dictionary<AssemblyDefinition,List<TypeReference>> ();
			else if (cache.ContainsKey (assembly))
				return;

			List<TypeReference> list = new List<TypeReference> ();

			foreach (ModuleDefinition module in assembly.Modules) {
				foreach (TypeDefinition type in module.Types) {
					foreach (MethodDefinition method in type.Constructors) {
						ProcessMethod (method, list);
					}
					foreach (MethodDefinition method in type.Methods) {
						ProcessMethod (method, list);
					}
				}
			}

			cache.Add (assembly, list);
		}

		private static void ProcessMethod (MethodDefinition method, List<TypeReference> list)
		{
			if (!method.HasBody)
				return;

			// this is needed in case we return an enum
			TypeReference t = method.ReturnType.ReturnType;
			if (!list.Contains (t))
				list.Add (t);

			foreach (Instruction ins in method.Body.Instructions) {
				if (ins.OpCode == OpCodes.Newobj) {
					t = ins.Operand as TypeReference;
					if (t == null) {
						MethodReference m = ins.Operand as MethodReference;
						if (m != null)
							t = m.DeclaringType;
					}

					if ((t != null) &&  !list.Contains (t))
						list.Add (t);
				}
			}
		}

		public MessageCollection CheckType (TypeDefinition type, Runner runner)
		{
			// rule apply to non-public types
			// note: isAbstract also excludes static types (2.0)
			if (type.IsPublic || type.IsAbstract)
				return runner.RuleSuccess;

			// some types are created by compilers and should be ignored
			if (CheckSpecialTypes (type))
				return runner.RuleSuccess;

			// rule doesn't apply if the assembly open up itself to others using [InternalsVisibleTo]
			if (type.Module.Assembly.HasAttribute ("System.Runtime.CompilerServices.InternalsVisibleToAttribute"))
				return runner.RuleSuccess;

			// rule applies

			// if the type holds the Main entry point then it is considered useful
			MethodDefinition entry_point = type.Module.Assembly.EntryPoint;
			if ((entry_point != null) && (entry_point.DeclaringType == type))
				return runner.RuleSuccess;

			// create a cache of all type instantiation inside this 
			AssemblyDefinition assembly = type.Module.Assembly;
			CacheInstantiationFromAssembly (assembly);

			List<TypeReference> list = null;
			if (cache.ContainsKey (assembly))
				list = cache [assembly];

			// if we can't find the non-public type being used in the assembly then the rule fails
			if (list == null || !list.Contains (type)) {
				Location location = new Location (type.FullName, null);
				Message message = new Message ("There is no call for any of the types constructor found", location, MessageType.Error);
				return new MessageCollection (message);
			}

			return runner.RuleSuccess;
		}
	}
}
