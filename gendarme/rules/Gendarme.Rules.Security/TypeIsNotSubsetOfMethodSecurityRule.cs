//
// Gendarme.Rules.Security.TypeIsNotSubsetOfMethodSecurityRule
//
// Authors:
//	Sebastien Pouliot <sebastien@ximian.com>
//
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
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

using System;
using System.Collections;
using System.Security;
using System.Text;

using Mono.Cecil;
using Gendarme.Framework;

namespace Gendarme.Rules.Security {

	public class TypeIsNotSubsetOfMethodSecurityRule : ITypeRule {

		public IList CheckType (IAssemblyDefinition assembly, IModuleDefinition module, ITypeDefinition type, Runner runner)
		{
			// #1 - this rules apply if type as security permissions
			if (type.SecurityDeclarations.Count == 0)
				return runner.RuleSuccess;

			PermissionSet assert = null;
			PermissionSet deny = null;
			PermissionSet permitonly = null;
			PermissionSet demand = null;
			bool apply = false;

			foreach (ISecurityDeclaration declsec in type.SecurityDeclarations) {
				switch (declsec.Action) {
				case Mono.Cecil.SecurityAction.Assert:
					assert = declsec.PermissionSet;
					apply = true;
					break;
				case Mono.Cecil.SecurityAction.Deny:
					deny = declsec.PermissionSet;
					apply = true;
					break;
				case Mono.Cecil.SecurityAction.PermitOnly:
					permitonly = declsec.PermissionSet;
					apply = true;
					break;
				case Mono.Cecil.SecurityAction.Demand:
					demand = declsec.PermissionSet;
					apply = true;
					break;
				}
			}
			
			// #2 - this rules doesn't apply to LinkDemand (both are executed) 
			// and to InheritanceDemand (both are executed at different time).
			if (!apply)
				return runner.RuleSuccess;

			// *** ok, the rule applies! ***

			// ensure that method-level security doesn't replace type-level security
			// with a subset of the original check
			foreach (IMethodDefinition method in type.Methods) {
				if (method.SecurityDeclarations.Count == 0)
					continue;

				foreach (ISecurityDeclaration declsec in method.SecurityDeclarations) {
					switch (declsec.Action) {
					case Mono.Cecil.SecurityAction.Assert:
						if (assert == null)
							continue;
						if (!assert.IsSubsetOf (declsec.PermissionSet))
							return runner.RuleFailure;
						break;
					case Mono.Cecil.SecurityAction.Deny:
						if (deny == null)
							continue;
						if (!deny.IsSubsetOf (declsec.PermissionSet))
							return runner.RuleFailure;
						break;
					case Mono.Cecil.SecurityAction.PermitOnly:
						if (permitonly == null)
							continue;
						if (!permitonly.IsSubsetOf (declsec.PermissionSet))
							return runner.RuleFailure;
						break;
					case Mono.Cecil.SecurityAction.Demand:
					case Mono.Cecil.SecurityAction.NonCasDemand:
						if (demand == null)
							continue;
						if (!demand.IsSubsetOf (declsec.PermissionSet))
							return runner.RuleFailure;
						break;
					}
				}
			}
			// other types security applies
			return runner.RuleSuccess;
		}
	}
}