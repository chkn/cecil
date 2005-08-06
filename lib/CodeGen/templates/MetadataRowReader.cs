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
 * <%=Time.now%>
 *
 *****************************************************************************/

namespace Mono.Cecil.Metadata {

	using System;
	using System.IO;

	using Mono.Cecil.Binary;

	internal sealed class MetadataRowReader : IMetadataRowVisitor {

		private MetadataTableReader m_mtrv;
		private BinaryReader m_binaryReader;
		private MetadataRoot m_metadataRoot;

		private int m_blobHeapIdxSz;
		private int m_stringsHeapIdxSz;
		private int m_guidHeapIdxSz;

		public MetadataRowReader (MetadataTableReader mtrv)
		{
			m_mtrv = mtrv;
			m_binaryReader = mtrv.GetReader ();
			m_metadataRoot = mtrv.GetMetadataRoot ();
		}

		private int GetIndexSize (Type table)
		{
			return m_mtrv.GetNumberOfRows (table) < (1 << 16) ? 2 : 4;
		}

		private int GetCodedIndexSize (CodedIndex ci)
		{
			return Utilities.GetCodedIndexSize (ci, m_mtrv.GetNumberOfRows);
		}

		private uint ReadByIndexSize (int size)
		{
			if (size == 2) {
				return (uint) m_binaryReader.ReadUInt16 ();
			} else if (size == 4) {
				return m_binaryReader.ReadUInt32 ();
			} else {
				throw new MetadataFormatException ("Non valid size for indexing");
			}
		}

		public void Visit (RowCollection coll)
		{
			m_blobHeapIdxSz = m_metadataRoot.Streams.BlobHeap != null ?
				m_metadataRoot.Streams.BlobHeap.IndexSize : 2;
			m_stringsHeapIdxSz = m_metadataRoot.Streams.StringsHeap != null ?
				m_metadataRoot.Streams.StringsHeap.IndexSize : 2;
			m_guidHeapIdxSz = m_metadataRoot.Streams.GuidHeap != null ?
				m_metadataRoot.Streams.GuidHeap.IndexSize : 2;
		}

<% $tables.each { |table| %>		public void Visit (<%=table.row_name%> row)
		{
<% table.columns.each { |col|
 if (col.target.nil?)
%>			row.<%=col.property_name%> = <%=col.read_binary("m_binaryReader")%>;
<% elsif (col.target == "BlobHeap")
%>			row.<%=col.property_name%> = ReadByIndexSize (m_blobHeapIdxSz);
<% elsif (col.target == "StringsHeap")
%>			row.<%=col.property_name%> = ReadByIndexSize (m_stringsHeapIdxSz);
<% elsif (col.target == "GuidHeap")
%>			row.<%=col.property_name%> = ReadByIndexSize (m_guidHeapIdxSz);
<% elsif (col.type == "MetadataToken")
%>			row.<%=col.property_name%> = Utilities.GetMetadataToken (CodedIndex.<%=col.target%>,
				ReadByIndexSize (GetCodedIndexSize (CodedIndex.<%=col.target%>)));
<% else
%>			row.<%=col.property_name%> = ReadByIndexSize (GetIndexSize (typeof (<%=col.target%>Table)));
<% end
}%>		}
<%  print("\n") ; } %>		public void Terminate (RowCollection coll)
		{
		}
	}
}
