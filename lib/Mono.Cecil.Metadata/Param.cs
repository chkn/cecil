/*
 * Copyright (c) 2004 DotNetGuru and the individuals listed
 * on the ChangeLog entries.
 *
 * Authors :
 *   Jb Evain   (jb.evain@dotnetguru.org)
 *
 * This is a free software distributed under a MIT/X11 license
 * See LICENSE.MIT file for more details
 *
 * Generated by /CodeGen/cecil-gen.rb do not edit
 * Tue Mar 01 00:32:45 Paris, Madrid 2005
 *
 *****************************************************************************/

namespace Mono.Cecil.Metadata {

    using Mono.Cecil;

    [RId (0x08)]
    public sealed class ParamTable : IMetadataTable {

        private RowCollection m_rows;

        public ParamRow this [int index] {
            get { return m_rows [index] as ParamRow; }
            set { m_rows [index] = value; }
        }

        public RowCollection Rows {
            get { return m_rows; }
            set { m_rows = value; }
        }

        internal ParamTable ()
        {
        }

        public void Accept (IMetadataTableVisitor visitor)
        {
            visitor.Visit (this);
            this.Rows.Accept (visitor.GetRowVisitor ());
        }
    }

    public sealed class ParamRow : IMetadataRow {

        public static readonly int RowSize = 8;
        public static readonly int RowColumns = 3;

        public ParamAttributes Flags;
        public ushort Sequence;
        public uint Name;

        public int Size {
            get { return ParamRow.RowSize; }
        }

        public int Columns {
            get { return ParamRow.RowColumns; }
        }

        internal ParamRow ()
        {
        }

        public void Accept (IMetadataRowVisitor visitor)
        {
            visitor.Visit (this);
        }
    }
}
