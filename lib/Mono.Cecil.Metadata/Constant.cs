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
 * Mon Jan 31 16:56:39 Paris, Madrid 2005
 *
 *****************************************************************************/

namespace Mono.Cecil.Metadata {

    using Mono.Cecil.Signatures;

    [RId (0x0b)]
    internal sealed class ConstantTable : IMetadataTable {

        private RowCollection m_rows;

        public ConstantRow this [int index] {
            get { return m_rows [index] as ConstantRow; }
            set { m_rows [index] = value; }
        }

        public RowCollection Rows {
            get { return m_rows; }
            set { m_rows = value; }
        }

        public void Accept (IMetadataTableVisitor visitor)
        {
            visitor.Visit (this);
            this.Rows.Accept (visitor.GetRowVisitor ());
        }
    }

    internal sealed class ConstantRow : IMetadataRow {

        public static readonly int RowSize = 10;
        public static readonly int RowColumns = 3;

        [Column] private ElementType m_type;
        [Column] private MetadataToken m_parent;
        [Column] private uint m_value;

        public ElementType Type {
            get { return m_type; }
            set { m_type = value; }
        }

        public MetadataToken Parent {
            get { return m_parent; }
            set { m_parent = value; }
        }

        public uint Value {
            get { return m_value; }
            set { m_value = value; }
        }

        public int Size {
            get { return ConstantRow.RowSize; }
        }

        public int Columns {
            get { return ConstantRow.RowColumns; }
        }

        public void Accept (IMetadataRowVisitor visitor)
        {
            visitor.Visit (this);
        }
    }
}
