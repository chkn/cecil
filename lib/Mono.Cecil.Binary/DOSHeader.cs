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

namespace Mono.Cecil.Binary {

    public sealed class DOSHeader : IHeader, IBinaryVisitable {

        public byte [] Start;
        public byte [] End;

        public uint Lfanew;

        internal DOSHeader ()
        {
        }

        public void SetDefaultValues ()
        {
            Start = new byte [60] {
                0x4d, 0x5a, 0x90, 0x00, 0x03, 0x00, 0x00,
                0x00, 0x04, 0x00, 0x00, 0x00, 0xff, 0xff,
                0x00, 0x00, 0xb8, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00
            };
            Lfanew = 128;
            End = new byte [64] {
                0x0e, 0x1f, 0xba, 0x0e, 0x00, 0xb4, 0x09,
                0xcd, 0x21, 0xb8, 0x01, 0x4c, 0xcd, 0x21,
                0x54, 0x68, 0x69, 0x73, 0x20, 0x70, 0x72,
                0x6f, 0x67, 0x72, 0x61, 0x6d, 0x20, 0x63,
                0x61, 0x6e, 0x6e, 0x6f, 0x74, 0x20, 0x62,
                0x65, 0x20, 0x72, 0x75, 0x6e, 0x20, 0x69,
                0x6e, 0x20, 0x44, 0x4f, 0x53, 0x20, 0x6d,
                0x6f, 0x64, 0x65, 0x2e, 0x0d, 0x0d, 0x0a,
                0x24, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00
            };
        }

        public void Accept (IBinaryVisitor visitor)
        {
            visitor.Visit (this);
        }
    }
}
