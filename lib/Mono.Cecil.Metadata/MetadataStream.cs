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
 *****************************************************************************/

namespace Mono.Cecil.Metadata {

    public class MetadataStream : IMetadataVisitable {

        private MetadataStreamHeader m_header;
        private MetadataHeap m_heap;

        private MetadataRoot m_root;

        public MetadataStreamHeader Header {
            get { return m_header; }
            set { m_header = value; }
        }

        public MetadataHeap Heap {
            get { return m_heap; }
            set { m_heap = value; }
        }

        internal MetadataStream (MetadataRoot root)
        {
            m_root = root;
        }

        public void Accept (IMetadataVisitor visitor)
        {
            visitor.Visit (this);

            m_header.Accept (visitor);
            if (m_heap != null)
                m_heap.Accept (visitor);
        }

        public class MetadataStreamHeader : IMetadataVisitable {

            public uint Offset;
            public uint Size;
            public string Name;

            private MetadataStream m_stream;

            public MetadataStream Stream {
                get { return m_stream; }
            }

            internal MetadataStreamHeader (MetadataStream stream)
            {
                m_stream = stream;
            }

            public void Accept (IMetadataVisitor visitor)
            {
                visitor.Visit (this);
            }
        }
    }
}
