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

    [RId (0x11)]
    public sealed class StandAloneSigTable : IMetadataTable {

        private RowCollection m_rows;

        public StandAloneSigRow this [int index] {
            get { return m_rows [index] as StandAloneSigRow; }
            set { m_rows [index] = value; }
        }

        public RowCollection Rows {
            get { return m_rows; }
            set { m_rows = value; }
        }

        internal StandAloneSigTable ()
        {
        }

        public void Accept (IMetadataTableVisitor visitor)
        {
            visitor.Visit (this);
            this.Rows.Accept (visitor.GetRowVisitor ());
        }
    }

    public sealed class StandAloneSigRow : IMetadataRow {

        public static readonly int RowSize = 4;
        public static readonly int RowColumns = 1;

        public uint Signature;

        public int Size {
            get { return StandAloneSigRow.RowSize; }
        }

        public int Columns {
            get { return StandAloneSigRow.RowColumns; }
        }

        internal StandAloneSigRow ()
        {
        }

        public void Accept (IMetadataRowVisitor visitor)
        {
            visitor.Visit (this);
        }
    }
}
