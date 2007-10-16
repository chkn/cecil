// 
// Gendarme.Rules.Naming.DetectNonAlphaNumericsInTypeNamesRule
//
// Authors:
//	Nidhi Rawal <sonu2404@gmail.com>
//	Sebastien Pouliot <sebastien@ximian.com>
//
// Copyright (c) <2007> Nidhi Rawal
// Copyright (C) 2007 Novell, Inc (http://www.novell.com)
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

using Mono.Cecil;
using Mono.Cecil.Cil;
using Gendarme.Framework;

namespace Gendarme.Rules.Naming {

	public class DetectNonAlphaNumericsInTypeNamesRule: IMethodRule, ITypeRule {

		// Compiler generates an error for any other non alpha-numerics than underscore ('_'), so we just need to check the presence of underscore in method names
		private bool CheckName (string name)
		{
			return (name.IndexOf ("_") == -1);
		}
		
		public MessageCollection CheckType (TypeDefinition type, Runner runner)
		{
			// type must be public and, if nested, public too
			if (type.IsNotPublic || type.IsNestedPrivate)
				return runner.RuleSuccess;

			// check the type name
			if (CheckName (type.Name))
				return runner.RuleSuccess;

			Location location = new Location (type.FullName, type.Name, 0);
			Message message = new Message ("Type name contains underscore", location, MessageType.Error);
			return new MessageCollection (message);
		}
		
		public MessageCollection CheckMethod (MethodDefinition method, Runner runner)
		{
			// exclude private methods and special names (like Getter and Setter)
			if (method.IsPrivate || method.IsSpecialName)
				return runner.RuleSuccess;

			// check the method name
			if (CheckName (method.Name))
				return runner.RuleSuccess;

			Location location = new Location (method.DeclaringType.ToString(), method.Name, 0);
			Message message = new Message ("Method name contains an underscore.", location, MessageType.Error);
			return new MessageCollection (message);
		}
	}
}