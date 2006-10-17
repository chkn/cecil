//
// Node.cs
//
// Authors:
//	Sebastien Pouliot <sebastien@ximian.com>
//
// Copyright (C) 2005-2006 Novell, Inc (http://www.novell.com)
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
using System.IO;

namespace Monoxide.Framework.Dot {

	public class Node {

		private string _label;
		private Hashtable _attributes;

		public string Label {
			get { return _label; }
			set { _label = value; }
		}

		public IDictionary Attributes {
			get {
				if (_attributes == null)
					_attributes = new Hashtable ();
				return _attributes;
			}
		}

		internal string GetAttributes ()
		{
			if (_attributes == null)
				return String.Empty;

			bool first = true;
			StringWriter sw = new StringWriter ();
			foreach (DictionaryEntry de in _attributes) {
				if (first) {
					first = false;
				} else {
					sw.Write (",");
				}
				sw.Write ("{0}={1}", de.Key, de.Value);
			}
			return sw.ToString ();
		}

		internal string ToString (string prefix)
		{
			StringWriter sw = new StringWriter ();
			String attrs = GetAttributes ();
			if (attrs.Length > 0)
				sw.WriteLine ("{0}{1} [{2}];", prefix, _label, attrs);
			else
				sw.WriteLine ("{0}{1};", prefix, _label);
			return sw.ToString ();
		}

		public override string ToString ()
		{
			return ToString (String.Empty);
		}
	}
}
