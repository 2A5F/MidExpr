using MidExpr;

namespace Test1;

public class Tests
{
    public int FibRec(int n)
    {
        if (n < 3) return 1;
        return FibRec(n - 1) + FibRec(n - 2);
    }

    [Test]
    public void TestFib1()
    {
        var builder = new MidFunc.Builder(MidType.I4, [MidType.I4]) { Name = "FibRec" };
        var l = builder.DefLabel();
        var func = builder
            .LdArg(0)
            .LdcX4(3)
            .Bge(MidPrimitive.I4, l)
            .LdcX4(1)
            .Ret()
            .MarkLabel(l)
            .LdArg(0)
            .LdcX4(1)
            .Sub(MidPrimitive.I4)
            .Call(builder)
            .LdArg(0)
            .LdcX4(2)
            .Sub(MidPrimitive.I4)
            .Call(builder)
            .Add(MidPrimitive.I4)
            .Ret()
            .Build();
        Console.WriteLine(func);
        var ret = func.CallFunc<int, int>(8);
        Console.WriteLine(ret);

        var csr = FibRec(8);
        Console.WriteLine(csr);
        Assert.That(ret, Is.EqualTo(csr));
    }

    [Test]
    public void TestFib2()
    {
        var builder = new MidFunc2.Builder(new(MidType.I4, SigLoc.R0), [new(MidType.I4, SigLoc.R1)]) { Name = "FibRec" };
        var l = builder.DefLabel("l");
        var func = builder
            .Emit(OpCode2.Ldc_R2_I4_3) //           r2 = 3
            .Emit(OpCode2.BgeI_R1_R2, l) //         if (r1 >= r2) goto l
            .Emit(OpCode2.Ldc_R0_I4_1) //           r0 = 1
            .Emit(OpCode2.Ret) //                   ret
            .MarkLabel(l) //                    l:
            .Emit(OpCode2.Push_R1_X4) //            push(r1)
            .Emit(OpCode2.Ldc_R2_I4_1) //           r2 = 1
            .Emit(OpCode2.SubI_R1_R2_X4) //         r1 -= r2
            .Emit(OpCode2.Call, builder) //         call => ret: r0; n: r1
            .Emit(OpCode2.Pop_R2_X4) //             r2 = pop()
            .Emit(OpCode2.Push_R0_X4) //            push(r0)
            .Emit(OpCode2.Ldc_R1_X4, -2) //         r1 = -2
            .Emit(OpCode2.AddI_R1_R2_X4) //         r1 += r2
            .Emit(OpCode2.Call, builder) //         call => ret: r0; n: r1
            .Emit(OpCode2.Pop_R2_X4) //             r2 = pop()
            .Emit(OpCode2.AddI_R0_R2_X4) //         r0 += r2
            .Emit(OpCode2.Ret) //                   ret
            .Build();
        Console.WriteLine(func);
        var ret = func.CallFunc<int, int>(8);
        Console.WriteLine(ret);

        var csr = FibRec(8);
        Console.WriteLine(csr);
        Assert.That(ret, Is.EqualTo(csr));
    }

    [Test]
    public void TestFib3()
    {
        var builder = new MidFunc3.Builder(new(MidType.I4, SigLoc.R0), [new(MidType.I4, SigLoc.R1)]) { Name = "FibRec" };
        var l = builder.DefLabel("l");
        var func = builder
            .Emit(OpCode2.Ldc_R2_I4_3) //           r2 = 3
            .Emit(OpCode2.BgeI_R1_R2, l) //         if (r1 >= r2) goto l
            .Emit(OpCode2.Ldc_R0_I4_1) //           r0 = 1
            .Emit(OpCode2.Ret) //                   ret
            .MarkLabel(l) //                    l:
            .Emit(OpCode2.Push_R1_X4) //            push(r1)
            .Emit(OpCode2.Ldc_R2_I4_1) //           r2 = 1
            .Emit(OpCode2.SubI_R1_R2_X4) //         r1 -= r2
            .Emit(OpCode2.Call, builder) //         call => ret: r0; n: r1
            .Emit(OpCode2.Pop_R2_X4) //             r2 = pop()
            .Emit(OpCode2.Push_R0_X4) //            push(r0)
            .Emit(OpCode2.Ldc_R1_X4, -2) //         r1 = -2
            .Emit(OpCode2.AddI_R1_R2_X4) //         r1 += r2
            .Emit(OpCode2.Call, builder) //         call => ret: r0; n: r1
            .Emit(OpCode2.Pop_R2_X4) //             r2 = pop()
            .Emit(OpCode2.AddI_R0_R2_X4) //         r0 += r2
            .Emit(OpCode2.Ret) //                   ret
            .Build();
        Console.WriteLine(func);
        var ret = func.CallFunc<int, int>(8);
        Console.WriteLine(ret);

        var csr = FibRec(8);
        Console.WriteLine(csr);
        Assert.That(ret, Is.EqualTo(csr));
    }
}
