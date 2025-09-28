using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MidExpr;

public enum OpCode : ushort
{
    Nop,
    Debug,

    Ret,
    Ret_X1,
    Ret_X2,
    Ret_X4,
    Ret_X8,
    Ret_XPtr,

    Call,
    CallInd,

    LdLoc_X1,
    LdLoc_X2,
    LdLoc_X4,
    LdLoc_X8,
    LdLoc_XPtr,

    StLoc_X1,
    StLoc_X2,
    StLoc_X4,
    StLoc_X8,
    StLoc_XPtr,

    LdLocA,

    LdNull,

    Ldc_I4_M1,
    Ldc_I4_0,
    Ldc_I4_1,
    Ldc_I4_2,
    Ldc_I4_3,
    Ldc_I4_4,
    Ldc_I4_5,
    Ldc_I4_6,
    Ldc_I4_7,
    Ldc_I4_8,
    Ldc_R4_1,
    Ldc_R4_M1,
    Ldc_R4_2,
    // 0.5
    Ldc_R4_05,
    // -0.5
    Ldc_R4_M05,
    Ldc_R4_Inf,
    Ldc_R4_NInf,
    Ldc_X4,
    Ldc_X8,

    LdInd_X1,
    LdInd_X2,
    LdInd_X4,
    LdInd_X8,
    LdInd_Ptr,

    StInd_X1,
    StInd_X2,
    StInd_X4,
    StInd_X8,
    StInd_XPtr,

    // <code>op u4</code>
    LdFld,
    LdFldA,
    StFld,
    LdSFld,
    LdSFldA,

    // <code>op u4</code>
    LdFtn,
    LdVtFtn,

    // <code>op u4</code>
    SizeOf,

    Dup_X1,
    Dup_X2,
    Dup_X4,
    Dup_X8,
    Dup_XPtr,
    Dup_T,

    Pop_X1,
    Pop_X2,
    Pop_X4,
    Pop_X8,
    Pop_XPtr,
    Pop_T,

    Add_I1,
    Add_I2,
    Add_I4,
    Add_I8,
    Add_IPtr,
    Add_U1,
    Add_U2,
    Add_U4,
    Add_U8,
    Add_UPtr,
    Add_R4,
    Add_R8,

    Sub_I1,
    Sub_I2,
    Sub_I4,
    Sub_I8,
    Sub_IPtr,
    Sub_U1,
    Sub_U2,
    Sub_U4,
    Sub_U8,
    Sub_UPtr,
    Sub_R4,
    Sub_R8,

    Mul_I1,
    Mul_I2,
    Mul_I4,
    Mul_I8,
    Mul_IPtr,
    Mul_U1,
    Mul_U2,
    Mul_U4,
    Mul_U8,
    Mul_UPtr,
    Mul_R1,
    Mul_R2,

    Div_I1,
    Div_I2,
    Div_I4,
    Div_I8,
    Div_IPtr,
    Div_U1,
    Div_U2,
    Div_U4,
    Div_U8,
    Div_UPtr,
    Div_R1,
    Div_R2,

    Rem_I1,
    Rem_I2,
    Rem_I4,
    Rem_I8,
    Rem_IPtr,
    Rem_U1,
    Rem_U2,
    Rem_U4,
    Rem_U8,
    Rem_UPtr,
    Rem_R1,
    Rem_R2,

    And_X1,
    And_X2,
    And_X4,
    And_X8,
    And_XPtr,

    Or_X1,
    Or_X2,
    Or_X4,
    Or_X8,
    Or_XPtr,

    Xor_X1,
    Xor_X2,
    Xor_X4,
    Xor_X8,
    Xor_XPtr,

    Shl_X1,
    Shl_X2,
    Shl_X4,
    Shl_X8,
    Shl_XPtr,

    Shr_I1,
    Shr_I2,
    Shr_I4,
    Shr_I8,
    Shr_IPtr,

    Shr_U1,
    Shr_U2,
    Shr_U4,
    Shr_U8,
    Shr_UPtr,

    Rol_X1,
    Rol_X2,
    Rol_X4,
    Rol_X8,
    Rol_XPtr,

    Ror_X1,
    Ror_X2,
    Ror_X4,
    Ror_X8,
    Ror_XPtr,

    Not_X1,
    Not_X2,
    Not_X4,
    Not_X8,
    Not_XPtr,

    Neg_I1,
    Neg_I2,
    Neg_I4,
    Neg_I8,
    Neg_IPtr,
    Neg_R4,
    Neg_R8,

    Conv_X1_X2,
    Conv_X1_X4,
    Conv_X1_X8,
    Conv_X1_XPtr,

    Conv_X2_X1,
    Conv_X2_X4,
    Conv_X2_X8,
    Conv_X2_XPtr,

    Conv_X4_X1,
    Conv_X4_X2,
    Conv_X4_X8,
    Conv_X4_XPtr,

    Conv_X8_X1,
    Conv_X8_X2,
    Conv_X8_X4,
    Conv_X8_XPtr,

    Conv_XPtr_X1,
    Conv_XPtr_X2,
    Conv_XPtr_X4,
    Conv_XPtr_X8,

    Conv_I1_R4,
    Conv_I1_R8,
    Conv_I2_R4,
    Conv_I2_R8,
    Conv_I4_R4,
    Conv_I4_R8,
    Conv_I8_R4,
    Conv_I8_R8,
    Conv_IPtr_R4,
    Conv_IPtr_R8,

    Conv_U1_R4,
    Conv_U1_R8,
    Conv_U2_R4,
    Conv_U2_R8,
    Conv_U4_R4,
    Conv_U4_R8,
    Conv_U8_R4,
    Conv_U8_R8,
    Conv_UPtr_R4,
    Conv_UPtr_R8,

    Conv_R4_I1,
    Conv_R4_I2,
    Conv_R4_I4,
    Conv_R4_I8,
    Conv_R4_IPtr,
    Conv_R4_U1,
    Conv_R4_U2,
    Conv_R4_U4,
    Conv_R4_U8,
    Conv_R4_UPtr,

    Conv_R8_I1,
    Conv_R8_I2,
    Conv_R8_I4,
    Conv_R8_I8,
    Conv_R8_IPtr,
    Conv_R8_U1,
    Conv_R8_U2,
    Conv_R8_U4,
    Conv_R8_U8,
    Conv_R8_UPtr,

    Ceq_X1,
    Ceq_X2,
    Ceq_X4,
    Ceq_X8,
    Ceq_XPtr,
    Ceq_R4,
    Ceq_R8,

    Cne_X1,
    Cne_X2,
    Cne_X4,
    Cne_X8,
    Cne_XPtr,
    Cne_R4,
    Cne_R8,

    Cle_I1,
    Cle_I2,
    Cle_I4,
    Cle_I8,
    Cle_IPtr,
    Cle_U1,
    Cle_U2,
    Cle_U4,
    Cle_U8,
    Cle_UPtr,
    Cle_R4,
    Cle_R8,

    Clt_I1,
    Clt_I2,
    Clt_I4,
    Clt_I8,
    Clt_IPtr,
    Clt_U1,
    Clt_U2,
    Clt_U4,
    Clt_U8,
    Clt_UPtr,
    Clt_R4,
    Clt_R8,

    Cge_I1,
    Cge_I2,
    Cge_I4,
    Cge_I8,
    Cge_IPtr,
    Cge_U1,
    Cge_U2,
    Cge_U4,
    Cge_U8,
    Cge_UPtr,
    Cge_R4,
    Cge_R8,

    Cgt_I1,
    Cgt_I2,
    Cgt_I4,
    Cgt_I8,
    Cgt_IPtr,
    Cgt_U1,
    Cgt_U2,
    Cgt_U4,
    Cgt_U8,
    Cgt_UPtr,
    Cgt_R4,
    Cgt_R8,

    // <code>op u4</code>
    Br_True_X1,
    Br_True_X2,
    Br_True_X4,
    Br_True_X8,
    Br_True_XPtr,

    // <code>op u4</code>
    Br_False_X1,
    Br_False_X2,
    Br_False_X4,
    Br_False_X8,
    Br_False_XPtr,

    // <code>op u4</code>
    Beq_X1,
    Beq_X2,
    Beq_X4,
    Beq_X8,
    Beq_XPtr,
    Beq_R4,
    Beq_R8,

    // <code>op u4</code>
    Bne_X1,
    Bne_X2,
    Bne_X4,
    Bne_X8,
    Bne_XPtr,
    Bne_R4,
    Bne_R8,

    // <code>op u4</code>
    Ble_I1,
    Ble_I2,
    Ble_I4,
    Ble_I8,
    Ble_IPtr,
    Ble_U1,
    Ble_U2,
    Ble_U4,
    Ble_U8,
    Ble_UPtr,
    Ble_R4,
    Ble_R8,

    // <code>op u4</code>
    Blt_I1,
    Blt_I2,
    Blt_I4,
    Blt_I8,
    Blt_IPtr,
    Blt_U1,
    Blt_U2,
    Blt_U4,
    Blt_U8,
    Blt_UPtr,
    Blt_R4,
    Blt_R8,

    // <code>op u4</code>
    Bge_I1,
    Bge_I2,
    Bge_I4,
    Bge_I8,
    Bge_IPtr,
    Bge_U1,
    Bge_U2,
    Bge_U4,
    Bge_U8,
    Bge_UPtr,
    Bge_R4,
    Bge_R8,

    // <code>op u4</code>
    Bgt_I1,
    Bgt_I2,
    Bgt_I4,
    Bgt_I8,
    Bgt_IPtr,
    Bgt_U1,
    Bgt_U2,
    Bgt_U4,
    Bgt_U8,
    Bgt_UPtr,
    Bgt_R4,
    Bgt_R8,
}

internal static class OpCodeExtension
{
    extension(List<ushort> code)
    {
        internal void Emit_LdcX4(uint value)
        {
            switch (value)
            {
                case uint.MaxValue:
                    code.Add((ushort)OpCode.Ldc_I4_M1);
                    return;
                case 0:
                    code.Add((ushort)OpCode.Ldc_I4_0);
                    return;
                case 1:
                    code.Add((ushort)OpCode.Ldc_I4_1);
                    return;
                case 2:
                    code.Add((ushort)OpCode.Ldc_I4_2);
                    return;
                case 3:
                    code.Add((ushort)OpCode.Ldc_I4_3);
                    return;
                case 4:
                    code.Add((ushort)OpCode.Ldc_I4_4);
                    return;
                case 5:
                    code.Add((ushort)OpCode.Ldc_I4_5);
                    return;
                case 6:
                    code.Add((ushort)OpCode.Ldc_I4_6);
                    return;
                case 7:
                    code.Add((ushort)OpCode.Ldc_I4_7);
                    return;
                case 8:
                    code.Add((ushort)OpCode.Ldc_I4_8);
                    return;
            }
            switch (Unsafe.BitCast<uint, float>(value))
            {
                case 1f:
                    code.Add((ushort)OpCode.Ldc_R4_1);
                    return;
                case -1f:
                    code.Add((ushort)OpCode.Ldc_R4_M1);
                    return;
                case 2f:
                    code.Add((ushort)OpCode.Ldc_R4_2);
                    return;
                case 0.5f:
                    code.Add((ushort)OpCode.Ldc_R4_05);
                    return;
                case -0.5f:
                    code.Add((ushort)OpCode.Ldc_R4_M05);
                    return;
                case float.PositiveInfinity:
                    code.Add((ushort)OpCode.Ldc_R4_Inf);
                    return;
                case float.NegativeInfinity:
                    code.Add((ushort)OpCode.Ldc_R4_NInf);
                    return;
            }
            code.Add((ushort)OpCode.Ldc_X4);
            code.AddRange(MemoryMarshal.Cast<uint, ushort>(new Span<uint>(ref value)));
        }
        internal void Emit_Ret(in MidType type)
        {
            switch (type.Tag)
            {
                case MidType.Tags.Void:
                    code.Add((ushort)OpCode.Ret);
                    break;
                case MidType.Tags.I1 or MidType.Tags.U1:
                    code.Add((ushort)OpCode.Ret_X1);
                    break;
                case MidType.Tags.I2 or MidType.Tags.U2:
                    code.Add((ushort)OpCode.Ret_X2);
                    break;
                case MidType.Tags.I4 or MidType.Tags.U4 or MidType.Tags.R4:
                    code.Add((ushort)OpCode.Ret_X4);
                    break;
                case MidType.Tags.I8 or MidType.Tags.U8 or MidType.Tags.R8:
                    code.Add((ushort)OpCode.Ret_X8);
                    break;
                case MidType.Tags.IPtr or MidType.Tags.UPtr:
                    code.Add((ushort)OpCode.Ret_XPtr);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        internal void Emit_LdLoc(in MidType type, int local)
        {
            switch (type.Tag)
            {
                case MidType.Tags.I1 or MidType.Tags.U1:
                    code.Add((ushort)OpCode.LdLoc_X1);
                    break;
                case MidType.Tags.I2 or MidType.Tags.U2:
                    code.Add((ushort)OpCode.LdLoc_X2);
                    break;
                case MidType.Tags.I4 or MidType.Tags.U4 or MidType.Tags.R4:
                    code.Add((ushort)OpCode.LdLoc_X4);
                    break;
                case MidType.Tags.I8 or MidType.Tags.U8 or MidType.Tags.R8:
                    code.Add((ushort)OpCode.LdLoc_X8);
                    break;
                case MidType.Tags.IPtr or MidType.Tags.UPtr:
                    code.Add((ushort)OpCode.LdLoc_XPtr);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            code.Add((ushort)local);
        }
        internal void Emit_StLoc(in MidType type, int local)
        {
            switch (type.Tag)
            {
                case MidType.Tags.I1 or MidType.Tags.U1:
                    code.Add((ushort)OpCode.StLoc_X1);
                    break;
                case MidType.Tags.I2 or MidType.Tags.U2:
                    code.Add((ushort)OpCode.StLoc_X2);
                    break;
                case MidType.Tags.I4 or MidType.Tags.U4 or MidType.Tags.R4:
                    code.Add((ushort)OpCode.StLoc_X4);
                    break;
                case MidType.Tags.I8 or MidType.Tags.U8 or MidType.Tags.R8:
                    code.Add((ushort)OpCode.StLoc_X8);
                    break;
                case MidType.Tags.IPtr or MidType.Tags.UPtr:
                    code.Add((ushort)OpCode.StLoc_XPtr);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            code.Add((ushort)local);
        }
        internal void Emit_Bge(MidPrimitive primitive)
        {
            switch (primitive)
            {
                case MidPrimitive.I1:
                    code.Add((ushort)OpCode.Bge_I1);
                    break;
                case MidPrimitive.I2:
                    code.Add((ushort)OpCode.Bge_I2);
                    break;
                case MidPrimitive.I4:
                    code.Add((ushort)OpCode.Bge_I4);
                    break;
                case MidPrimitive.I8:
                    code.Add((ushort)OpCode.Bge_I8);
                    break;
                case MidPrimitive.IPtr:
                    code.Add((ushort)OpCode.Bge_IPtr);
                    break;
                case MidPrimitive.U1:
                    code.Add((ushort)OpCode.Bge_U1);
                    break;
                case MidPrimitive.U2:
                    code.Add((ushort)OpCode.Bge_U2);
                    break;
                case MidPrimitive.U4:
                    code.Add((ushort)OpCode.Bge_U4);
                    break;
                case MidPrimitive.U8:
                    code.Add((ushort)OpCode.Bge_U8);
                    break;
                case MidPrimitive.UPtr:
                    code.Add((ushort)OpCode.Bge_UPtr);
                    break;
                case MidPrimitive.R4:
                    code.Add((ushort)OpCode.Bge_R4);
                    break;
                case MidPrimitive.R8:
                    code.Add((ushort)OpCode.Bge_R8);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(primitive), primitive, null);
            }
            code.Add(0); // empty label
        }
        internal void Emit_Add(MidPrimitive primitive)
        {
            switch (primitive)
            {
                case MidPrimitive.I1:
                    code.Add((ushort)OpCode.Add_I1);
                    break;
                case MidPrimitive.I2:
                    code.Add((ushort)OpCode.Add_I2);
                    break;
                case MidPrimitive.I4:
                    code.Add((ushort)OpCode.Add_I4);
                    break;
                case MidPrimitive.I8:
                    code.Add((ushort)OpCode.Add_I8);
                    break;
                case MidPrimitive.IPtr:
                    code.Add((ushort)OpCode.Add_IPtr);
                    break;
                case MidPrimitive.U1:
                    code.Add((ushort)OpCode.Add_U1);
                    break;
                case MidPrimitive.U2:
                    code.Add((ushort)OpCode.Add_U2);
                    break;
                case MidPrimitive.U4:
                    code.Add((ushort)OpCode.Add_U4);
                    break;
                case MidPrimitive.U8:
                    code.Add((ushort)OpCode.Add_U8);
                    break;
                case MidPrimitive.UPtr:
                    code.Add((ushort)OpCode.Add_UPtr);
                    break;
                case MidPrimitive.R4:
                    code.Add((ushort)OpCode.Add_R4);
                    break;
                case MidPrimitive.R8:
                    code.Add((ushort)OpCode.Add_R8);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(primitive), primitive, null);
            }
        }
        internal void Emit_Sub(MidPrimitive primitive)
        {
            switch (primitive)
            {
                case MidPrimitive.I1:
                    code.Add((ushort)OpCode.Sub_I1);
                    break;
                case MidPrimitive.I2:
                    code.Add((ushort)OpCode.Sub_I2);
                    break;
                case MidPrimitive.I4:
                    code.Add((ushort)OpCode.Sub_I4);
                    break;
                case MidPrimitive.I8:
                    code.Add((ushort)OpCode.Sub_I8);
                    break;
                case MidPrimitive.IPtr:
                    code.Add((ushort)OpCode.Sub_IPtr);
                    break;
                case MidPrimitive.U1:
                    code.Add((ushort)OpCode.Sub_U1);
                    break;
                case MidPrimitive.U2:
                    code.Add((ushort)OpCode.Sub_U2);
                    break;
                case MidPrimitive.U4:
                    code.Add((ushort)OpCode.Sub_U4);
                    break;
                case MidPrimitive.U8:
                    code.Add((ushort)OpCode.Sub_U8);
                    break;
                case MidPrimitive.UPtr:
                    code.Add((ushort)OpCode.Sub_UPtr);
                    break;
                case MidPrimitive.R4:
                    code.Add((ushort)OpCode.Sub_R4);
                    break;
                case MidPrimitive.R8:
                    code.Add((ushort)OpCode.Sub_R8);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(primitive), primitive, null);
            }
        }
    }
}
