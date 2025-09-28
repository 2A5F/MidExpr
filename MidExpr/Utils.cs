using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace MidExpr;

internal static unsafe class Utils
{
    [StructLayout(LayoutKind.Sequential)]
    private struct AlignOfHelper<T> where T : struct
    {
        public byte dummy;
        public T data;
    }

    public static int AlignOf<T>() where T : struct
    {
        return sizeof(AlignOfHelper<T>) - sizeof(T);
    }

    public static int AlignUp(this int val, int alignment)
    {
        Debug.Assert(val >= 0 && alignment >= 0);

        // alignment must be a power of 2 for this implementation to work (need modulo otherwise)
        Debug.Assert(0 == (alignment & (alignment - 1)));
        int result = (val + (alignment - 1)) & ~(alignment - 1);
        Debug.Assert(result >= val); // check for overflow

        return result;
    }

    public static nuint AlignUp(this nuint val, nuint alignment)
    {
        Debug.Assert(val >= 0 && alignment >= 0);

        // alignment must be a power of 2 for this implementation to work (need modulo otherwise)
        Debug.Assert(0 == (alignment & (alignment - 1)));
        var result = (val + (alignment - 1)) & ~(alignment - 1);
        Debug.Assert(result >= val); // check for overflow

        return result;
    }

    public static byte* AlignUp(byte* val, int alignment)
    {
        return (byte*)AlignUp((nuint)val, (nuint)alignment);
    }
}
