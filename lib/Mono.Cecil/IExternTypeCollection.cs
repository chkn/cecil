/*
 * Copyright (c) 2004, 2005 DotNetGuru and the individuals listed
 * on the ChangeLog entries.
 *
 * Authors :
 *   Jb Evain   (jbevain@gmail.com)
 *
 * This is a free software distributed under a MIT/X11 license
 * See LICENSE.MIT file for more details
 *
 * Generated by /CodeGen/cecil-gen.rb do not edit
 * Fri Aug 19 18:04:41 CEST 2005
 *
 *****************************************************************************/

namespace Mono.Cecil {

	using System.Collections;

	public interface IExternTypeCollection : ICollection, IReflectionVisitable {

		ITypeReference this [int index] { get; }
		ITypeReference this [string fullName] { get; }

		IModuleDefinition Container { get; }

		void Add (ITypeReference value);
		void Clear ();
		bool Contains (ITypeReference value);
		bool Contains (string fullName);
		int IndexOf (ITypeReference value);
		void Remove (ITypeReference value);
		void RemoveAt (int index);
	}
}
