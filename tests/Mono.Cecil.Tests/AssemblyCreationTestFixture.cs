//
// AssemblyCreationTestFixture.cs
//
// Author:
//   Jb Evain (jb@nurv.fr)
//
// (C) 2007 Jb Evain
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

using System.IO;
using Mono.Bat;
using NUnit.Framework;

namespace Mono.Cecil.Tests {

	[TestFixture]
	public class AssemblyCreationTestFixture : AbstractAssemblyTestFixture {

		[Test]
		public void CreateHelloWorld ()
		{
			RunWriteAssemblyTestCase ("HelloWorld");
		}
		
		[Test]
		public void OptimizeLdc_I4 ()
		{
			RunWriteAssemblyTestCase ("OptimizeLdc_I4");
		}

		protected override string GetTestCasePath (string file)
		{
			return base.GetTestCasePath (Path.Combine ("Creation", file));
		}
	}
}
