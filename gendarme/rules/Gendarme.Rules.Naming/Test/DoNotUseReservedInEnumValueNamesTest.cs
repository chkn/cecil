//
// Unit tests for DoNotUseReservedInEnumValueNamesRule
//
// Authors:
//	Andreas Noever <andreas.noever@gmail.com>
//
//  (C) 2008 Andreas Noever
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
using System.Reflection;

using Gendarme.Framework;
using Gendarme.Rules.Naming;
using Gendarme.Framework.Rocks;
using Mono.Cecil;
using NUnit.Framework;

namespace Test.Rules.Naming {

	[TestFixture]
	public class DoNotUseReservedInEnumValueNamesTest {

		private DoNotUseReservedInEnumValueNamesRule rule;
		private AssemblyDefinition assembly;
		private TypeDefinition type;


		[TestFixtureSetUp]
		public void FixtureSetUp ()
		{
			string unit = Assembly.GetExecutingAssembly ().Location;
			assembly = AssemblyFactory.GetAssembly (unit);
			type = assembly.MainModule.Types ["Test.Rules.Naming.DoNotUseReservedInEnumValueNamesTest"];
			rule = new DoNotUseReservedInEnumValueNamesRule ();
		}

		private TypeDefinition GetTest (string name)
		{
			foreach (TypeDefinition nestedType in type.NestedTypes) {
				if (nestedType.Name == name)
					return nestedType;
			}
			return null;
		}

		struct NonEnum {
			int Reserved, NonEnum2;
		}

		[Test]
		public void TestNonEnum ()
		{
			TypeDefinition type = GetTest ("NonEnum");
			Assert.IsNull (rule.CheckType (type, new MinimalRunner ()));
		}

		enum FalsePositive {
			A,
			B,
			C
		}

		[Test]
		public void TestFalsePositive ()
		{
			TypeDefinition type = GetTest ("FalsePositive");
			Assert.IsNull (rule.CheckType (type, new MinimalRunner ()));
		}

		enum Reserved {
			A,
			Reserved,
			B
		}

		[Test]
		public void TestReserved ()
		{
			TypeDefinition type = GetTest ("Reserved");
			Assert.IsNotNull (rule.CheckType (type, new MinimalRunner ()));
		}
	}
}
