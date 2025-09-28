using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MidExpr;

internal static unsafe class MidInterpreter
{
    public const int StackSize = 8 * 1024 * 1024;
    public static void Exec(MidFunc func, byte* stack)
    {
        var calc_stack = stack + func.m_init_stack_size;
        ref var fns = ref func.m_fns[0];
        ref var locs = ref func.m_locals[0];
        fixed (ushort* code = func.m_code)
        {
            var code_i = 0;
            for (;;)
            {
                var op = (OpCode)code[code_i];
                switch (op)
                {
                    case OpCode.Nop:
                        code_i++;
                        continue;
                    case OpCode.Debug:
                        Debugger.Break();
                        code_i++;
                        continue;
                    case OpCode.Ret: return;
                    case OpCode.Ret_X1:
                    {
                        *stack = *(calc_stack - 1);
                        return;
                    }
                    case OpCode.Ret_X2:
                    {
                        *(ushort*)stack = *(ushort*)(calc_stack - 2);
                        return;
                    }
                    case OpCode.Ret_X4:
                    {
                        *(uint*)stack = *(uint*)(calc_stack - 4);
                        return;
                    }
                    case OpCode.Ret_X8:
                    {
                        *(ulong*)stack = *(ulong*)(calc_stack - 8);
                        return;
                    }
                    case OpCode.Ret_XPtr:
                    {
                        *(nuint*)stack = *(nuint*)(calc_stack - sizeof(nuint));
                        return;
                    }
                    case OpCode.Call:
                    {
                        var fn = Unsafe.Add(ref fns, code[code_i + 1]);
                        Exec(fn, calc_stack -= fn.m_param_stack_size);
                        calc_stack += fn.m_ret_stack_size;
                        code_i += 2;
                        continue;
                    }
                    case OpCode.CallInd:
                        break;
                    case OpCode.LdLoc_X1:
                    {
                        ref var loc = ref Unsafe.Add(ref locs, code[code_i + 1]);
                        *calc_stack = stack[loc.Offset];
                        calc_stack += 1;
                        code_i += 2;
                        continue;
                    }
                    case OpCode.LdLoc_X2:
                    {
                        ref var loc = ref Unsafe.Add(ref locs, code[code_i + 1]);
                        *(ushort*)calc_stack = *(ushort*)&stack[loc.Offset];
                        calc_stack += 2;
                        code_i += 2;
                        continue;
                    }
                    case OpCode.LdLoc_X4:
                    {
                        ref var loc = ref Unsafe.Add(ref locs, code[code_i + 1]);
                        *(uint*)calc_stack = *(uint*)&stack[loc.Offset];
                        calc_stack += 4;
                        code_i += 2;
                        continue;
                    }
                    case OpCode.LdLoc_X8:
                    {
                        ref var loc = ref Unsafe.Add(ref locs, code[code_i + 1]);
                        *(ulong*)calc_stack = *(ulong*)&stack[loc.Offset];
                        calc_stack += 8;
                        code_i += 2;
                        continue;
                    }
                    case OpCode.LdLoc_XPtr:
                    {
                        ref var loc = ref Unsafe.Add(ref locs, code[code_i + 1]);
                        *(nuint*)calc_stack = *(nuint*)&stack[loc.Offset];
                        calc_stack += sizeof(nuint);
                        code_i += 2;
                        continue;
                    }
                    case OpCode.LdLocA:
                    {
                        ref var loc = ref Unsafe.Add(ref locs, code[code_i + 1]);
                        *(nuint*)calc_stack = (nuint)(&stack[loc.Offset]);
                        calc_stack += sizeof(nuint);
                        code_i += 2;
                        continue;
                    }
                    case OpCode.LdNull:
                    {
                        *(nuint*)calc_stack = 0;
                        calc_stack += sizeof(nuint);
                        code_i++;
                        continue;
                    }
                    case OpCode.Ldc_I4_M1:
                    {
                        *(int*)calc_stack = -1;
                        calc_stack += sizeof(int);
                        code_i++;
                        continue;
                    }
                    case OpCode.Ldc_I4_0:
                    {
                        *(int*)calc_stack = 0;
                        calc_stack += sizeof(int);
                        code_i++;
                        continue;
                    }
                    case OpCode.Ldc_I4_1:
                    {
                        *(int*)calc_stack = 1;
                        calc_stack += sizeof(int);
                        code_i++;
                        continue;
                    }
                    case OpCode.Ldc_I4_2:
                    {
                        *(int*)calc_stack = 2;
                        calc_stack += sizeof(int);
                        code_i++;
                        continue;
                    }
                    case OpCode.Ldc_I4_3:
                    {
                        *(int*)calc_stack = 3;
                        calc_stack += sizeof(int);
                        code_i++;
                        continue;
                    }
                    case OpCode.Ldc_I4_4:
                    {
                        *(int*)calc_stack = 4;
                        calc_stack += sizeof(int);
                        code_i++;
                        continue;
                    }
                    case OpCode.Ldc_I4_5:
                    {
                        *(int*)calc_stack = 5;
                        calc_stack += sizeof(int);
                        code_i++;
                        continue;
                    }
                    case OpCode.Ldc_I4_6:
                    {
                        *(int*)calc_stack = 6;
                        calc_stack += sizeof(int);
                        code_i++;
                        continue;
                    }
                    case OpCode.Ldc_I4_7:
                    {
                        *(int*)calc_stack = 7;
                        calc_stack += sizeof(int);
                        code_i++;
                        continue;
                    }
                    case OpCode.Ldc_I4_8:
                    {
                        *(int*)calc_stack = 8;
                        calc_stack += sizeof(int);
                        code_i++;
                        continue;
                    }
                    case OpCode.Ldc_R4_1:
                    {
                        *(float*)calc_stack = 1;
                        calc_stack += sizeof(float);
                        code_i++;
                        continue;
                    }
                    case OpCode.Ldc_R4_M1:
                    {
                        *(float*)calc_stack = -1;
                        calc_stack += sizeof(float);
                        code_i++;
                        continue;
                    }
                    case OpCode.Ldc_R4_2:
                    {
                        *(float*)calc_stack = 2;
                        calc_stack += sizeof(float);
                        code_i++;
                        continue;
                    }
                    case OpCode.Ldc_R4_05:
                    {
                        *(float*)calc_stack = 0.5f;
                        calc_stack += sizeof(float);
                        code_i++;
                        continue;
                    }
                    case OpCode.Ldc_R4_M05:
                    {
                        *(float*)calc_stack = -0.5f;
                        calc_stack += sizeof(float);
                        code_i++;
                        continue;
                    }
                    case OpCode.Ldc_R4_Inf:
                    {
                        *(float*)calc_stack = float.PositiveInfinity;
                        calc_stack += sizeof(float);
                        code_i++;
                        continue;
                    }
                    case OpCode.Ldc_R4_NInf:
                    {
                        *(float*)calc_stack = float.NegativeInfinity;
                        calc_stack += sizeof(float);
                        code_i++;
                        continue;
                    }
                    case OpCode.Ldc_X4:
                    {
                        *(uint*)calc_stack = *(uint*)(code + 1);
                        calc_stack += sizeof(uint);
                        code_i += 3;
                        continue;
                    }
                    case OpCode.Ldc_X8:
                    {
                        *(ulong*)calc_stack = *(ulong*)(code + 1);
                        calc_stack += sizeof(ulong);
                        code_i += 7;
                        continue;
                    }
                    case OpCode.LdInd_X1:
                        break;
                    case OpCode.LdInd_X2:
                        break;
                    case OpCode.LdInd_X4:
                        break;
                    case OpCode.LdInd_X8:
                        break;
                    case OpCode.LdInd_Ptr:
                        break;
                    case OpCode.StInd_X1:
                        break;
                    case OpCode.StInd_X2:
                        break;
                    case OpCode.StInd_X4:
                        break;
                    case OpCode.StInd_X8:
                        break;
                    case OpCode.StInd_XPtr:
                        break;
                    case OpCode.LdFld:
                        break;
                    case OpCode.LdFldA:
                        break;
                    case OpCode.StFld:
                        break;
                    case OpCode.LdSFld:
                        break;
                    case OpCode.LdSFldA:
                        break;
                    case OpCode.LdFtn:
                        break;
                    case OpCode.LdVtFtn:
                        break;
                    case OpCode.SizeOf:
                        break;
                    case OpCode.Dup_X1:
                    {
                        *calc_stack = *(calc_stack - 1);
                        calc_stack += 1;
                        code_i++;
                        continue;
                    }
                    case OpCode.Dup_X2:
                    {
                        *(ushort*)calc_stack = *(ushort*)(calc_stack - 2);
                        calc_stack += 2;
                        code_i++;
                        continue;
                    }
                    case OpCode.Dup_X4:
                    {
                        *(uint*)calc_stack = *(uint*)(calc_stack - 4);
                        calc_stack += 4;
                        code_i++;
                        continue;
                    }
                    case OpCode.Dup_X8:
                    {
                        *(ulong*)calc_stack = *(ulong*)(calc_stack - 8);
                        calc_stack += 8;
                        code_i++;
                        continue;
                    }
                    case OpCode.Dup_XPtr:
                    {
                        *(nuint*)calc_stack = *(nuint*)(calc_stack - sizeof(nuint));
                        calc_stack += sizeof(nuint);
                        code_i++;
                        continue;
                    }
                    case OpCode.Dup_T:
                        break;
                    case OpCode.Pop_X1:
                    {
                        calc_stack -= 1;
                        code_i++;
                        continue;
                    }
                    case OpCode.Pop_X2:
                    {
                        calc_stack -= 2;
                        code_i++;
                        continue;
                    }
                    case OpCode.Pop_X4:
                    {
                        calc_stack -= 4;
                        code_i++;
                        continue;
                    }
                    case OpCode.Pop_X8:
                    {
                        calc_stack -= 8;
                        code_i++;
                        continue;
                    }
                    case OpCode.Pop_XPtr:
                    {
                        calc_stack -= sizeof(nuint);
                        code_i++;
                        continue;
                    }
                    case OpCode.Pop_T:
                        break;
                    case OpCode.Add_I1:
                    {
                        calc_stack -= 1;
                        *(sbyte*)(calc_stack - 1) += *(sbyte*)calc_stack;
                        code_i++;
                        continue;
                    }
                    case OpCode.Add_I2:
                    {
                        calc_stack -= 2;
                        *(short*)(calc_stack - 2) += *(short*)calc_stack;
                        code_i++;
                        continue;
                    }
                    case OpCode.Add_I4:
                    {
                        calc_stack -= 4;
                        (*(int*)(calc_stack - 4)) += *(int*)calc_stack;
                        code_i++;
                        continue;
                    }
                    case OpCode.Add_I8:
                    {
                        calc_stack -= 8;
                        (*(long*)(calc_stack - 8)) += *(long*)calc_stack;
                        code_i++;
                        continue;
                    }
                    case OpCode.Add_IPtr:
                    {
                        calc_stack -= sizeof(nint);
                        (*(nint*)(calc_stack - sizeof(nint))) += *(nint*)calc_stack;
                        code_i++;
                        continue;
                    }
                    case OpCode.Add_U1:
                    {
                        calc_stack -= 1;
                        *(calc_stack - 1) += *calc_stack;
                        code_i++;
                        continue;
                    }
                    case OpCode.Add_U2:
                    {
                        calc_stack -= 2;
                        *(ushort*)(calc_stack - 2) += *(ushort*)calc_stack;
                        code_i++;
                        continue;
                    }
                    case OpCode.Add_U4:
                    {
                        calc_stack -= 4;
                        *(uint*)(calc_stack - 4) += *(uint*)calc_stack;
                        code_i++;
                        continue;
                    }
                    case OpCode.Add_U8:
                    {
                        calc_stack -= 8;
                        *(ulong*)(calc_stack - 8) += *(ulong*)calc_stack;
                        code_i++;
                        continue;
                    }
                    case OpCode.Add_UPtr:
                    {
                        calc_stack -= sizeof(nuint);
                        *(nuint*)(calc_stack - sizeof(nuint)) += *(nuint*)calc_stack;
                        code_i++;
                        continue;
                    }
                    case OpCode.Add_R4:
                    {
                        calc_stack -= 4;
                        *(float*)(calc_stack - 4) += *(float*)calc_stack;
                        code_i++;
                        continue;
                    }
                    case OpCode.Add_R8:
                    {
                        calc_stack -= 8;
                        *(double*)(calc_stack - 8) += *(double*)calc_stack;
                        code_i++;
                        continue;
                    }
                    case OpCode.Sub_I1:
                    {
                        calc_stack -= 1;
                        *(sbyte*)(calc_stack - 1) -= *(sbyte*)calc_stack;
                        code_i++;
                        continue;
                    }
                    case OpCode.Sub_I2:
                    {
                        calc_stack -= 2;
                        *(short*)(calc_stack - 2) -= *(short*)calc_stack;
                        code_i++;
                        continue;
                    }
                    case OpCode.Sub_I4:
                    {
                        calc_stack -= 4;
                        *(int*)(calc_stack - 4) -= *(int*)calc_stack;
                        code_i++;
                        continue;
                    }
                    case OpCode.Sub_I8:
                    {
                        calc_stack -= 8;
                        *(long*)(calc_stack - 8) -= *(long*)calc_stack;
                        code_i++;
                        continue;
                    }
                    case OpCode.Sub_IPtr:
                    {
                        calc_stack -= sizeof(nint);
                        *(nint*)(calc_stack - sizeof(nint)) -= *(nint*)calc_stack;
                        code_i++;
                        continue;
                    }
                    case OpCode.Sub_U1:
                    {
                        calc_stack -= 1;
                        *(calc_stack - 1) -= *calc_stack;
                        code_i++;
                        continue;
                    }
                    case OpCode.Sub_U2:
                    {
                        calc_stack -= 2;
                        *(ushort*)(calc_stack - 2) -= *(ushort*)calc_stack;
                        code_i++;
                        continue;
                    }
                    case OpCode.Sub_U4:
                    {
                        calc_stack -= 4;
                        *(uint*)(calc_stack - 4) -= *(uint*)calc_stack;
                        code_i++;
                        continue;
                    }
                    case OpCode.Sub_U8:
                    {
                        calc_stack -= 8;
                        *(ulong*)(calc_stack - 8) -= *(ulong*)calc_stack;
                        code_i++;
                        continue;
                    }
                    case OpCode.Sub_UPtr:
                    {
                        calc_stack -= sizeof(nuint);
                        *(nuint*)(calc_stack - sizeof(nuint)) -= *(nuint*)calc_stack;
                        code_i++;
                        continue;
                    }
                    case OpCode.Sub_R4:
                    {
                        calc_stack -= 4;
                        *(float*)(calc_stack - 4) -= *(float*)calc_stack;
                        code_i++;
                        continue;
                    }
                    case OpCode.Sub_R8:
                    {
                        calc_stack -= 4;
                        *(double*)(calc_stack - 4) -= *(double*)calc_stack;
                        code_i++;
                        continue;
                    }
                    case OpCode.Mul_I1:
                        break;
                    case OpCode.Mul_I2:
                        break;
                    case OpCode.Mul_I4:
                        break;
                    case OpCode.Mul_I8:
                        break;
                    case OpCode.Mul_IPtr:
                        break;
                    case OpCode.Mul_U1:
                        break;
                    case OpCode.Mul_U2:
                        break;
                    case OpCode.Mul_U4:
                        break;
                    case OpCode.Mul_U8:
                        break;
                    case OpCode.Mul_UPtr:
                        break;
                    case OpCode.Mul_R1:
                        break;
                    case OpCode.Mul_R2:
                        break;
                    case OpCode.Div_I1:
                        break;
                    case OpCode.Div_I2:
                        break;
                    case OpCode.Div_I4:
                        break;
                    case OpCode.Div_I8:
                        break;
                    case OpCode.Div_IPtr:
                        break;
                    case OpCode.Div_U1:
                        break;
                    case OpCode.Div_U2:
                        break;
                    case OpCode.Div_U4:
                        break;
                    case OpCode.Div_U8:
                        break;
                    case OpCode.Div_UPtr:
                        break;
                    case OpCode.Div_R1:
                        break;
                    case OpCode.Div_R2:
                        break;
                    case OpCode.Rem_I1:
                        break;
                    case OpCode.Rem_I2:
                        break;
                    case OpCode.Rem_I4:
                        break;
                    case OpCode.Rem_I8:
                        break;
                    case OpCode.Rem_IPtr:
                        break;
                    case OpCode.Rem_U1:
                        break;
                    case OpCode.Rem_U2:
                        break;
                    case OpCode.Rem_U4:
                        break;
                    case OpCode.Rem_U8:
                        break;
                    case OpCode.Rem_UPtr:
                        break;
                    case OpCode.Rem_R1:
                        break;
                    case OpCode.Rem_R2:
                        break;
                    case OpCode.And_X1:
                        break;
                    case OpCode.And_X2:
                        break;
                    case OpCode.And_X4:
                        break;
                    case OpCode.And_X8:
                        break;
                    case OpCode.And_XPtr:
                        break;
                    case OpCode.Or_X1:
                        break;
                    case OpCode.Or_X2:
                        break;
                    case OpCode.Or_X4:
                        break;
                    case OpCode.Or_X8:
                        break;
                    case OpCode.Or_XPtr:
                        break;
                    case OpCode.Xor_X1:
                        break;
                    case OpCode.Xor_X2:
                        break;
                    case OpCode.Xor_X4:
                        break;
                    case OpCode.Xor_X8:
                        break;
                    case OpCode.Xor_XPtr:
                        break;
                    case OpCode.Shl_X1:
                        break;
                    case OpCode.Shl_X2:
                        break;
                    case OpCode.Shl_X4:
                        break;
                    case OpCode.Shl_X8:
                        break;
                    case OpCode.Shl_XPtr:
                        break;
                    case OpCode.Shr_I1:
                        break;
                    case OpCode.Shr_I2:
                        break;
                    case OpCode.Shr_I4:
                        break;
                    case OpCode.Shr_I8:
                        break;
                    case OpCode.Shr_IPtr:
                        break;
                    case OpCode.Shr_U1:
                        break;
                    case OpCode.Shr_U2:
                        break;
                    case OpCode.Shr_U4:
                        break;
                    case OpCode.Shr_U8:
                        break;
                    case OpCode.Shr_UPtr:
                        break;
                    case OpCode.Rol_X1:
                        break;
                    case OpCode.Rol_X2:
                        break;
                    case OpCode.Rol_X4:
                        break;
                    case OpCode.Rol_X8:
                        break;
                    case OpCode.Rol_XPtr:
                        break;
                    case OpCode.Ror_X1:
                        break;
                    case OpCode.Ror_X2:
                        break;
                    case OpCode.Ror_X4:
                        break;
                    case OpCode.Ror_X8:
                        break;
                    case OpCode.Ror_XPtr:
                        break;
                    case OpCode.Not_X1:
                        break;
                    case OpCode.Not_X2:
                        break;
                    case OpCode.Not_X4:
                        break;
                    case OpCode.Not_X8:
                        break;
                    case OpCode.Not_XPtr:
                        break;
                    case OpCode.Neg_I1:
                        break;
                    case OpCode.Neg_I2:
                        break;
                    case OpCode.Neg_I4:
                        break;
                    case OpCode.Neg_I8:
                        break;
                    case OpCode.Neg_IPtr:
                        break;
                    case OpCode.Neg_R4:
                        break;
                    case OpCode.Neg_R8:
                        break;
                    case OpCode.Conv_X1_X2:
                        break;
                    case OpCode.Conv_X1_X4:
                        break;
                    case OpCode.Conv_X1_X8:
                        break;
                    case OpCode.Conv_X1_XPtr:
                        break;
                    case OpCode.Conv_X2_X1:
                        break;
                    case OpCode.Conv_X2_X4:
                        break;
                    case OpCode.Conv_X2_X8:
                        break;
                    case OpCode.Conv_X2_XPtr:
                        break;
                    case OpCode.Conv_X4_X1:
                        break;
                    case OpCode.Conv_X4_X2:
                        break;
                    case OpCode.Conv_X4_X8:
                        break;
                    case OpCode.Conv_X4_XPtr:
                        break;
                    case OpCode.Conv_X8_X1:
                        break;
                    case OpCode.Conv_X8_X2:
                        break;
                    case OpCode.Conv_X8_X4:
                        break;
                    case OpCode.Conv_X8_XPtr:
                        break;
                    case OpCode.Conv_XPtr_X1:
                        break;
                    case OpCode.Conv_XPtr_X2:
                        break;
                    case OpCode.Conv_XPtr_X4:
                        break;
                    case OpCode.Conv_XPtr_X8:
                        break;
                    case OpCode.Conv_I1_R4:
                        break;
                    case OpCode.Conv_I1_R8:
                        break;
                    case OpCode.Conv_I2_R4:
                        break;
                    case OpCode.Conv_I2_R8:
                        break;
                    case OpCode.Conv_I4_R4:
                        break;
                    case OpCode.Conv_I4_R8:
                        break;
                    case OpCode.Conv_I8_R4:
                        break;
                    case OpCode.Conv_I8_R8:
                        break;
                    case OpCode.Conv_IPtr_R4:
                        break;
                    case OpCode.Conv_IPtr_R8:
                        break;
                    case OpCode.Conv_U1_R4:
                        break;
                    case OpCode.Conv_U1_R8:
                        break;
                    case OpCode.Conv_U2_R4:
                        break;
                    case OpCode.Conv_U2_R8:
                        break;
                    case OpCode.Conv_U4_R4:
                        break;
                    case OpCode.Conv_U4_R8:
                        break;
                    case OpCode.Conv_U8_R4:
                        break;
                    case OpCode.Conv_U8_R8:
                        break;
                    case OpCode.Conv_UPtr_R4:
                        break;
                    case OpCode.Conv_UPtr_R8:
                        break;
                    case OpCode.Conv_R4_I1:
                        break;
                    case OpCode.Conv_R4_I2:
                        break;
                    case OpCode.Conv_R4_I4:
                        break;
                    case OpCode.Conv_R4_I8:
                        break;
                    case OpCode.Conv_R4_IPtr:
                        break;
                    case OpCode.Conv_R4_U1:
                        break;
                    case OpCode.Conv_R4_U2:
                        break;
                    case OpCode.Conv_R4_U4:
                        break;
                    case OpCode.Conv_R4_U8:
                        break;
                    case OpCode.Conv_R4_UPtr:
                        break;
                    case OpCode.Conv_R8_I1:
                        break;
                    case OpCode.Conv_R8_I2:
                        break;
                    case OpCode.Conv_R8_I4:
                        break;
                    case OpCode.Conv_R8_I8:
                        break;
                    case OpCode.Conv_R8_IPtr:
                        break;
                    case OpCode.Conv_R8_U1:
                        break;
                    case OpCode.Conv_R8_U2:
                        break;
                    case OpCode.Conv_R8_U4:
                        break;
                    case OpCode.Conv_R8_U8:
                        break;
                    case OpCode.Conv_R8_UPtr:
                        break;
                    case OpCode.Ceq_X1:
                        break;
                    case OpCode.Ceq_X2:
                        break;
                    case OpCode.Ceq_X4:
                        break;
                    case OpCode.Ceq_X8:
                        break;
                    case OpCode.Ceq_XPtr:
                        break;
                    case OpCode.Ceq_R4:
                        break;
                    case OpCode.Ceq_R8:
                        break;
                    case OpCode.Cne_X1:
                        break;
                    case OpCode.Cne_X2:
                        break;
                    case OpCode.Cne_X4:
                        break;
                    case OpCode.Cne_X8:
                        break;
                    case OpCode.Cne_XPtr:
                        break;
                    case OpCode.Cne_R4:
                        break;
                    case OpCode.Cne_R8:
                        break;
                    case OpCode.Cle_I1:
                        break;
                    case OpCode.Cle_I2:
                        break;
                    case OpCode.Cle_I4:
                        break;
                    case OpCode.Cle_I8:
                        break;
                    case OpCode.Cle_IPtr:
                        break;
                    case OpCode.Cle_U1:
                        break;
                    case OpCode.Cle_U2:
                        break;
                    case OpCode.Cle_U4:
                        break;
                    case OpCode.Cle_U8:
                        break;
                    case OpCode.Cle_UPtr:
                        break;
                    case OpCode.Cle_R4:
                        break;
                    case OpCode.Cle_R8:
                        break;
                    case OpCode.Clt_I1:
                        break;
                    case OpCode.Clt_I2:
                        break;
                    case OpCode.Clt_I4:
                        break;
                    case OpCode.Clt_I8:
                        break;
                    case OpCode.Clt_IPtr:
                        break;
                    case OpCode.Clt_U1:
                        break;
                    case OpCode.Clt_U2:
                        break;
                    case OpCode.Clt_U4:
                        break;
                    case OpCode.Clt_U8:
                        break;
                    case OpCode.Clt_UPtr:
                        break;
                    case OpCode.Clt_R4:
                        break;
                    case OpCode.Clt_R8:
                        break;
                    case OpCode.Cge_I1:
                        break;
                    case OpCode.Cge_I2:
                        break;
                    case OpCode.Cge_I4:
                        break;
                    case OpCode.Cge_I8:
                        break;
                    case OpCode.Cge_IPtr:
                        break;
                    case OpCode.Cge_U1:
                        break;
                    case OpCode.Cge_U2:
                        break;
                    case OpCode.Cge_U4:
                        break;
                    case OpCode.Cge_U8:
                        break;
                    case OpCode.Cge_UPtr:
                        break;
                    case OpCode.Cge_R4:
                        break;
                    case OpCode.Cge_R8:
                        break;
                    case OpCode.Cgt_I1:
                        break;
                    case OpCode.Cgt_I2:
                        break;
                    case OpCode.Cgt_I4:
                        break;
                    case OpCode.Cgt_I8:
                        break;
                    case OpCode.Cgt_IPtr:
                        break;
                    case OpCode.Cgt_U1:
                        break;
                    case OpCode.Cgt_U2:
                        break;
                    case OpCode.Cgt_U4:
                        break;
                    case OpCode.Cgt_U8:
                        break;
                    case OpCode.Cgt_UPtr:
                        break;
                    case OpCode.Cgt_R4:
                        break;
                    case OpCode.Cgt_R8:
                        break;
                    case OpCode.Br_True_X1:
                        break;
                    case OpCode.Br_True_X2:
                        break;
                    case OpCode.Br_True_X4:
                        break;
                    case OpCode.Br_True_X8:
                        break;
                    case OpCode.Br_True_XPtr:
                        break;
                    case OpCode.Br_False_X1:
                        break;
                    case OpCode.Br_False_X2:
                        break;
                    case OpCode.Br_False_X4:
                        break;
                    case OpCode.Br_False_X8:
                        break;
                    case OpCode.Br_False_XPtr:
                        break;
                    case OpCode.Beq_X1:
                        break;
                    case OpCode.Beq_X2:
                        break;
                    case OpCode.Beq_X4:
                        break;
                    case OpCode.Beq_X8:
                        break;
                    case OpCode.Beq_XPtr:
                        break;
                    case OpCode.Beq_R4:
                        break;
                    case OpCode.Beq_R8:
                        break;
                    case OpCode.Bne_X1:
                        break;
                    case OpCode.Bne_X2:
                        break;
                    case OpCode.Bne_X4:
                        break;
                    case OpCode.Bne_X8:
                        break;
                    case OpCode.Bne_XPtr:
                        break;
                    case OpCode.Bne_R4:
                        break;
                    case OpCode.Bne_R8:
                        break;
                    case OpCode.Ble_I1:
                        break;
                    case OpCode.Ble_I2:
                        break;
                    case OpCode.Ble_I4:
                        break;
                    case OpCode.Ble_I8:
                        break;
                    case OpCode.Ble_IPtr:
                        break;
                    case OpCode.Ble_U1:
                        break;
                    case OpCode.Ble_U2:
                        break;
                    case OpCode.Ble_U4:
                        break;
                    case OpCode.Ble_U8:
                        break;
                    case OpCode.Ble_UPtr:
                        break;
                    case OpCode.Ble_R4:
                        break;
                    case OpCode.Ble_R8:
                        break;
                    case OpCode.Blt_I1:
                        break;
                    case OpCode.Blt_I2:
                        break;
                    case OpCode.Blt_I4:
                        break;
                    case OpCode.Blt_I8:
                        break;
                    case OpCode.Blt_IPtr:
                        break;
                    case OpCode.Blt_U1:
                        break;
                    case OpCode.Blt_U2:
                        break;
                    case OpCode.Blt_U4:
                        break;
                    case OpCode.Blt_U8:
                        break;
                    case OpCode.Blt_UPtr:
                        break;
                    case OpCode.Blt_R4:
                        break;
                    case OpCode.Blt_R8:
                        break;
                    case OpCode.Bge_I1:
                    {
                        calc_stack -= 2;
                        if (*(sbyte*)calc_stack >= *(sbyte*)(calc_stack + 1)) code_i += code[code_i + 1];
                        else code_i += 2;
                        continue;
                    }
                    case OpCode.Bge_I2:
                    {
                        calc_stack -= 4;
                        if (*(short*)calc_stack >= *(short*)(calc_stack + 2)) code_i += code[code_i + 1];
                        else code_i += 2;
                        continue;
                    }
                    case OpCode.Bge_I4:
                    {
                        calc_stack -= 8;
                        if (*(int*)calc_stack >= *(int*)(calc_stack + 4)) code_i += code[code_i + 1];
                        else code_i += 2;
                        continue;
                    }
                    case OpCode.Bge_I8:
                    {
                        calc_stack -= 16;
                        if (*(long*)calc_stack >= *(long*)(calc_stack + 8)) code_i += code[code_i + 1];
                        else code_i += 2;
                        continue;
                    }
                    case OpCode.Bge_IPtr:
                    {
                        calc_stack -= sizeof(nint) * 2;
                        if (*(nint*)calc_stack >= *(nint*)(calc_stack + sizeof(nint))) code_i += code[code_i + 1];
                        else code_i += 2;
                        continue;
                    }
                    case OpCode.Bge_U1:
                    {
                        calc_stack -= 2;
                        if (*calc_stack >= *(calc_stack + 1)) code_i += code[code_i + 1];
                        else code_i += 2;
                        continue;
                    }
                    case OpCode.Bge_U2:
                    {
                        calc_stack -= 4;
                        if (*(ushort*)calc_stack >= *(ushort*)(calc_stack + 2)) code_i += code[code_i + 1];
                        else code_i += 2;
                        continue;
                    }
                    case OpCode.Bge_U4:
                    {
                        calc_stack -= 8;
                        if (*(uint*)calc_stack >= *(uint*)(calc_stack + 4)) code_i += code[code_i + 1];
                        else code_i += 2;
                        continue;
                    }
                    case OpCode.Bge_U8:
                    {
                        calc_stack -= 16;
                        if (*(ulong*)calc_stack >= *(ulong*)(calc_stack + 8)) code_i += code[code_i + 1];
                        else code_i += 2;
                        continue;
                    }
                    case OpCode.Bge_UPtr:
                    {
                        calc_stack -= sizeof(nuint) * 2;
                        if (*(nuint*)calc_stack >= *(nuint*)(calc_stack + sizeof(nuint))) code_i += code[code_i + 1];
                        else code_i += 2;
                        continue;
                    }
                    case OpCode.Bge_R4:
                    {
                        calc_stack -= 8;
                        if (*(float*)calc_stack >= *(float*)(calc_stack + 4)) code_i += code[code_i + 1];
                        else code_i += 2;
                        continue;
                    }
                    case OpCode.Bge_R8:
                    {
                        calc_stack -= 16;
                        if (*(double*)calc_stack >= *(double*)(calc_stack + 8)) code_i += code[code_i + 1];
                        else code_i += 2;
                        continue;
                    }
                    case OpCode.Bgt_I1:
                        break;
                    case OpCode.Bgt_I2:
                        break;
                    case OpCode.Bgt_I4:
                        break;
                    case OpCode.Bgt_I8:
                        break;
                    case OpCode.Bgt_IPtr:
                        break;
                    case OpCode.Bgt_U1:
                        break;
                    case OpCode.Bgt_U2:
                        break;
                    case OpCode.Bgt_U4:
                        break;
                    case OpCode.Bgt_U8:
                        break;
                    case OpCode.Bgt_UPtr:
                        break;
                    case OpCode.Bgt_R4:
                        break;
                    case OpCode.Bgt_R8:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                throw new NotImplementedException("todo");
            }
        }
    }
}
