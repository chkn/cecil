//
// cecil-echo.cs
// A program to test if Cecil can read an assembly
//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// (C) 2005 Jb Evain
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace Cecil.Tools {

	using System;

	using Mono.Cecil;
	using Mono.Cecil.Cil;

	class Echo {

		static void Main (string [] args)
		{
			if (args.Length == 0) {
				Console.WriteLine ("usage:mono cecil-echo.exe assembly");
				return;
			}

			try {
				AssemblyDefinition asm = AssemblyFactory.GetAssembly (args [0]);
				foreach (ModuleDefinition mod in asm.Modules) {
					foreach (TypeDefinition type in mod.Types) {
						foreach (MethodDefinition meth in type.Methods)
							Bang (meth.Body);
						foreach (MethodDefinition ctor in type.Constructors)
							Bang (ctor.Body);
					}
				}
				Console.WriteLine ("Assembly {0} succesfully loaded", asm.Name.FullName);
			} catch (Exception e) {
				Console.WriteLine ("Failed to load assembly {0}", args [0]);
				Console.WriteLine (e);
			}
		}

		static void Bang (MethodBody b)
		{
		}
	}
}