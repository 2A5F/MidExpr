using Coplt.Union;

namespace MidExpr;

[Union2]
public unsafe partial struct MidType
{
    [UnionTemplate]
    private interface Template
    {
        void Void();
        void I1();
        void I2();
        void I4();
        void I8();
        void IPtr();
        void U1();
        void U2();
        void U4();
        void U8();
        void UPtr();
        void R4();
        void R8();
    }

    public int SizeOf => Tag switch {
        Tags.Void => 0,
        Tags.I1 => 1,
        Tags.I2 => 2,
        Tags.I4 => 4,
        Tags.I8 => 8,
        Tags.IPtr => sizeof(nint),
        Tags.U1 => 1,
        Tags.U2 => 2,
        Tags.U4 => 4,
        Tags.U8 => 8,
        Tags.UPtr => sizeof(nuint),
        Tags.R4 => 4,
        Tags.R8 => 8,
        _ => throw new ArgumentOutOfRangeException()
    };

    public int AlignOf => Tag switch {
        Tags.Void => 0,
        Tags.I1 => 1,
        Tags.I2 => 2,
        Tags.I4 => 4,
        Tags.I8 => 8,
        Tags.IPtr => sizeof(nint),
        Tags.U1 => 1,
        Tags.U2 => 2,
        Tags.U4 => 4,
        Tags.U8 => 8,
        Tags.UPtr => sizeof(nuint),
        Tags.R4 => 4,
        Tags.R8 => 8,
        _ => throw new ArgumentOutOfRangeException()
    };
}
