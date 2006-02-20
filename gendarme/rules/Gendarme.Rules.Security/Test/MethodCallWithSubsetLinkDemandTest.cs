//
// Unit tests for MethodCallWithSubsetLinkDemandRule
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
using System.Collections.Specialized;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using SSP = System.Security.Permissions;

using Gendarme.Framework;
using Gendarme.Rules.Security;
using Mono.Cecil;
using NUnit.Framework;

namespace Test.Rules.Security {

	[TestFixture]
	public class MethodCallWithSubsetLinkDemandTest {

		public class BaseClass {

			[SecurityPermission (SSP.SecurityAction.LinkDemand, ControlAppDomain = true)]
			public virtual void VirtualMethod ()
			{
			}

			[SecurityPermission (SSP.SecurityAction.LinkDemand, ControlAppDomain = true)]
			protected void ProtectedMethod ()
			{
			}
		}

		public class SubsetInheritClass: BaseClass  {

			[SecurityPermission (SSP.SecurityAction.LinkDemand, Unrestricted = true)]
			public override void VirtualMethod ()
			{
				base.VirtualMethod ();
			}

			[SecurityPermission (SSP.SecurityAction.LinkDemand, Unrestricted = true)]
			public void CallProtectedMethod ()
			{
				base.ProtectedMethod ();
			}
		}

		public class NotASubsetInheritClass: BaseClass {

			[SecurityPermission (SSP.SecurityAction.LinkDemand, ControlThread = true)]
			public override void VirtualMethod ()
			{
				base.VirtualMethod ();
			}

			[SecurityPermission (SSP.SecurityAction.LinkDemand, ControlThread = true)]
			public void CallProtectedMethod ()
			{
				base.ProtectedMethod ();
			}
		}

		public class SubsetCallClass {

			[SecurityPermission (SSP.SecurityAction.LinkDemand, Unrestricted = true)]
			public void Method ()
			{
				new BaseClass ().VirtualMethod ();
			}
		}

		public class NotASubsetCallClass {

			[SecurityPermission (SSP.SecurityAction.LinkDemand, ControlThread = true)]
			public void Method ()
			{
				new BaseClass ().VirtualMethod ();
			}
		}

		private IMethodRule rule;
		private IAssemblyDefinition assembly;
		private IModuleDefinition module;

		[TestFixtureSetUp]
		public void FixtureSetUp ()
		{
			string unit = Assembly.GetExecutingAssembly ().Location;
			assembly = AssemblyFactory.GetAssembly (unit);
			module = assembly.MainModule;
			rule = new MethodCallWithSubsetLinkDemandRule ();
		}

		private ITypeDefinition GetTest (string name)
		{
			string fullname = "Test.Rules.Security.MethodCallWithSubsetLinkDemandTest/" + name;
			return assembly.MainModule.Types[fullname];
		}

		[Test]
		public void SubsetInherit ()
		{
			ITypeDefinition type = GetTest ("SubsetInheritClass");
			foreach (IMethodDefinition method in type.Methods) {
				Assert.IsNotNull (rule.CheckMethod (assembly, module, type, method, new MinimalRunner()));
			}
		}

		[Test]
		public void NotASubsetInherit ()
		{
			ITypeDefinition type = GetTest ("NotASubsetInheritClass");
			foreach (IMethodDefinition method in type.Methods) {
				Assert.IsNull (rule.CheckMethod (assembly, module, type, method, new MinimalRunner()));
			}
		}

		[Test]
		public void SubsetCall ()
		{
			ITypeDefinition type = GetTest ("SubsetCallClass");
			foreach (IMethodDefinition method in type.Methods) {
				Assert.IsNotNull (rule.CheckMethod (assembly, module, type, method, new MinimalRunner()));
			}
		}

		[Test]
		public void NotASubsetCall ()
		{
			ITypeDefinition type = GetTest ("NotASubsetCallClass");
			foreach (IMethodDefinition method in type.Methods) {
				Assert.IsNull (rule.CheckMethod (assembly, module, type, method, new MinimalRunner()));
			}
		}
	}
}