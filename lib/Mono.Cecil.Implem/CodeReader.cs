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

namespace Mono.Cecil.Implem {

    using System;
    using System.IO;

    using Mono.Cecil;
    using Mono.Cecil.Binary;
    using Mono.Cecil.Cil;
    using Mono.Cecil.Metadata;
    using Mono.Cecil.Signatures;

    internal sealed class CodeReader : ICodeVisitor {

        private ReflectionReader m_reflectReader;
        private MetadataRoot m_root;

        public CodeReader (ReflectionReader reflectReader)
        {
            m_reflectReader = reflectReader;
            m_root = m_reflectReader.MetadataRoot;
        }

        public void Visit (IMethodBody body)
        {
            MethodDefinition meth = body.Method as MethodDefinition;
            MethodBody methBody = body as MethodBody;
            BinaryReader br = m_reflectReader.Module.Reader.GetReader ();
            br.BaseStream.Position = m_reflectReader.Module.Reader.Image.ResolveVirtualAddress (meth.RVA);

            // lets read the method
            int flags = br.ReadByte ();
            switch (flags & 0x7) {
            case (int) MethodHeaders.TinyFormat :
                methBody.CodeSize = flags >> 2;
                methBody.MaxStack = 8;
                ReadCilBody (methBody, br);
                return;
            case (int) MethodHeaders.FatFormat :
                br.BaseStream.Position--;
                int fatflags = br.ReadUInt16 ();
                //int headersize = (fatflags >> 12) & 0xf;
                methBody.MaxStack = br.ReadUInt16 ();
                methBody.CodeSize = br.ReadInt32 ();
                methBody.LocalVarToken = br.ReadInt32 ();
                body.InitLocals = (fatflags & (int) MethodHeaders.InitLocals) != 0;
                Visit (methBody.Variables);
                ReadCilBody (methBody, br);
                if ((fatflags & (int) MethodHeaders.MoreSects) != 0)
                    ReadExceptionHandlers (methBody, br);
                return;
            }
        }

        private int GetRid (int token)
        {
            return token & 0x00ffffff;
        }

        private bool IsToken (int token, TokenType t)
        {
            return token >> 24 == (int) t >> 24;
        }

        private void ReadCilBody (MethodBody body, BinaryReader br)
        {
            long start = br.BaseStream.Position;
            while (br.BaseStream.Position < start + body.CodeSize) {
                OpCode op;
                int cursor = br.ReadByte ();
                if (cursor == 0xfe)
                    op = OpCodes.Cache.Instance.TwoBytesOpCode [br.ReadByte ()];
                else
                    op = OpCodes.Cache.Instance.OneByteOpCode [cursor];

                Instruction instr = new Instruction (cursor, op);
                switch (op.OperandType) {
                case OperandType.InlineNone :
                    break;
                case OperandType.InlineBrTarget :
                    instr.Operand = br.ReadInt32 ();
                    break;
                case OperandType.InlineSwitch :
                    uint length = br.ReadUInt32 ();
                    int [] branches = new int [length];
                    for (int i = 0; i < length; i++)
                        branches [i] = br.ReadInt32 ();
                    instr.Operand = branches;
                    break;
                case OperandType.ShortInlineVar :
                case OperandType.ShortInlineBrTarget :
                case OperandType.ShortInlineI :
                    instr.Operand = br.ReadByte ();
                    break;
                case OperandType.InlineSig :
                case OperandType.InlineVar :
                case OperandType.InlineI :
                    instr.Operand = br.ReadInt32 ();
                    break;
                case OperandType.InlineI8 :
                    instr.Operand = br.ReadInt64 ();
                    break;
                case OperandType.ShortInlineR :
                    instr.Operand = br.ReadSingle ();
                    break;
                case OperandType.InlineR :
                    instr.Operand = br.ReadDouble ();
                    break;
                case OperandType.InlineString :
                    instr.Operand = m_root.Streams.UserStringsHeap [GetRid (br.ReadInt32 ())];
                    break;
                case OperandType.InlineField :
                    int field = br.ReadInt32 ();
                    if (IsToken (field, TokenType.Field))
                        instr.Operand = m_reflectReader.GetFieldDefAt (GetRid (field));
                    else
                        instr.Operand = m_reflectReader.GetMemberRefAt (GetRid (field));
                    break;
                case OperandType.InlineMethod :
                    int meth = br.ReadInt32 ();
                    if (IsToken (meth, TokenType.Method))
                        instr.Operand = m_reflectReader.GetMethodDefAt (GetRid (meth));
                    else
                        instr.Operand = m_reflectReader.GetMemberRefAt (GetRid (meth));
                    break;
                case OperandType.InlineType :
                    int type = br.ReadInt32 ();
                    if (IsToken (type, TokenType.TypeDef))
                        instr.Operand = m_reflectReader.GetTypeDefAt (GetRid (type));
                    else
                        instr.Operand = m_reflectReader.GetTypeRefAt (GetRid (type));
                    break;
                case OperandType.InlineTok :
                    int token = br.ReadInt32 ();
                    if (IsToken (token, TokenType.TypeDef))
                        instr.Operand = m_reflectReader.GetTypeDefAt (GetRid (token));
                    else if (IsToken (token, TokenType.TypeRef))
                        instr.Operand = m_reflectReader.GetTypeRefAt (GetRid (token));
                    else if (IsToken (token, TokenType.Method))
                        instr.Operand = m_reflectReader.GetMethodDefAt (GetRid (token));
                    else if (IsToken (token, TokenType.MemberRef))
                        instr.Operand = m_reflectReader.GetMemberRefAt (GetRid (token));
                    else
                        throw new ReflectionException ("Wrong token following ldtoken");
                    break;
                }

                // resolve branches & switchs
            }
        }

        private void ReadExceptionHandlers (MethodBody body, BinaryReader br)
        {

        }

        public void Visit (IInstructionCollection instructions)
        {
        }

        public void Visit (IInstruction instr)
        {
        }

        public void Visit (IExceptionHandlerCollection seh)
        {
        }

        public void Visit (IExceptionHandler eh)
        {
        }

        public void Visit (IVariableDefinitionCollection variables)
        {
            MethodBody body = variables.Container as MethodBody;
            MethodDefinition meth = body.Method as MethodDefinition;
            StandAloneSigTable sasTable = m_root.Streams.TablesHeap [typeof (StandAloneSigTable)] as StandAloneSigTable;
            StandAloneSigRow sasRow = sasTable [GetRid (body.LocalVarToken)];
            LocalVarSig sig = m_reflectReader.SigReader.GetLocalVarSig (sasRow.Signature);
            for (int i = 0; i < sig.Count; i++) {
                LocalVarSig.LocalVariable lv = sig.LocalVariables [i];
                ITypeReference varType = m_reflectReader.GetTypeRefFromSig(lv.Type);
                if (lv.ByRef)
                    varType = new ReferenceType (varType);
                if ((lv.Constraint & Constraint.Pinned) != 0)
                    varType = new PinnedType (varType);
                variables.Add (new VariableDefinition (string.Concat ("V_", i), meth, varType));
            }
        }

        public void Visit (IVariableDefinition var)
        {
        }
    }
}
