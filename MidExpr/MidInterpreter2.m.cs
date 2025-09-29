using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace MidExpr;

public static unsafe partial class MidInterpreter2
{
    public const int StackSize = 8 * 1024 * 1024;
    public const int FrameCount = 1024;

    internal struct Stack
    {
        public byte[]? Data;
        public Frame[]? Frames;
    }

    [ThreadStatic]
    private static Stack m_stack;

    internal static Stack GetStack()
    {
        ref var stack = ref m_stack;
        stack.Data ??= GC.AllocateUninitializedArray<byte>(StackSize);
        stack.Frames ??= new Frame[FrameCount];
        return stack;
    }

    public struct Regs
    {
        public Vector128<byte> r0;
        public Vector128<byte> r1;
        public Vector128<byte> r2;
        public Vector128<byte> r3;
        public Vector128<byte> r4;
        public Vector128<byte> r5;
        public Vector128<byte> r6;
        public Vector128<byte> r7;

        public void Set<T>(SigLoc loc, T value) where T : unmanaged
        {
            var v = sizeof(T) switch
            {
                1 => Vector128.CreateScalar(Unsafe.BitCast<T, byte>(value)),
                2 => Vector128.CreateScalar(Unsafe.BitCast<T, ushort>(value)).AsByte(),
                4 => Vector128.CreateScalar(Unsafe.BitCast<T, uint>(value)).AsByte(),
                8 => Vector128.CreateScalar(Unsafe.BitCast<T, ulong>(value)).AsByte(),
                _ => typeof(T) == typeof(float) || typeof(T) == typeof(double)
                    ? Vector128.CreateScalar(value).AsByte()
                    : throw new ArgumentOutOfRangeException()
            };
            switch (loc)
            {
                case SigLoc.R0:
                    r0 = v;
                    break;
                case SigLoc.R1:
                    r1 = v;
                    break;
                case SigLoc.R2:
                    r2 = v;
                    break;
                case SigLoc.R3:
                    r3 = v;
                    break;
                case SigLoc.R4:
                    r4 = v;
                    break;
                case SigLoc.R5:
                    r5 = v;
                    break;
                case SigLoc.R6:
                    r6 = v;
                    break;
                case SigLoc.R7:
                    r7 = v;
                    break;
                case SigLoc.Local:
                default:
                    throw new ArgumentOutOfRangeException(nameof(loc), loc, null);
            }
        }

        public T Get<T>(SigLoc loc) where T : unmanaged
        {
            var v = loc switch
            {
                SigLoc.R0 => r0,
                SigLoc.R1 => r1,
                SigLoc.R2 => r2,
                SigLoc.R3 => r3,
                SigLoc.R4 => r4,
                SigLoc.R5 => r5,
                SigLoc.R6 => r6,
                SigLoc.R7 => r7,
                _ => throw new ArgumentOutOfRangeException(nameof(loc), loc, null)
            };
            if (typeof(T) == typeof(float) || typeof(T) == typeof(double)) return v.As<byte, T>()[0];
            return sizeof(T) switch
            {
                1 => Unsafe.BitCast<byte, T>(v[0]),
                2 => Unsafe.BitCast<ushort, T>(v.AsUInt16()[0]),
                4 => Unsafe.BitCast<uint, T>(v.AsUInt32()[0]),
                8 => Unsafe.BitCast<ulong, T>(v.AsUInt64()[0]),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    public struct Frame
    {
        public MidFunc2 m_func;
        public byte* m_p_stack;
        public byte* m_p_calc_stack;
        public uint m_code_offset;
    }

    public static R CallFunc<R, A0>(MidFunc2 func, A0 a0)
        where R : unmanaged
        where A0 : unmanaged
    {
        Regs regs = default;
        var stack = GetStack();
        fixed (byte* p_data = stack.Data)
        {
            foreach (ref readonly var parameter in func.Parameters)
            {
                if (parameter.Place is SigLoc.Local) throw new NotImplementedException("todo");
                else regs.Set(parameter.Place, a0);
            }
            Exec(func, ref regs, p_data, stack.Frames!);
            if (func.m_return.Place is SigLoc.Local) throw new NotImplementedException("todo");
            else return regs.Get<R>(func.m_return.Place);
        }
    }
}
