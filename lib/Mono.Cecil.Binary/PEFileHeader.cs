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
 * Tue Mar 01 01:11:32 Paris, Madrid 2005
 *
 *****************************************************************************/


namespace Mono.Cecil.Binary {

    public sealed class PEFileHeader : IHeader, IBinaryVisitable {

        public ushort Machine;
        public ushort NumberOfSections;
        public uint TimeDateStamp;
        public uint PointerToSymbolTable;
        public uint NumberOfSymbols;
        public ushort OptionalHeaderSize;
        public ImageCharacteristics Characteristics;

        internal PEFileHeader ()
        {
        }

        public void SetDefaultValues ()
        {
            Machine = 0x14c;
            PointerToSymbolTable = 0;
            NumberOfSymbols = 0;
        }

        public void Accept (IBinaryVisitor visitor)
        {
            visitor.Visit (this);
        }
    }
}
