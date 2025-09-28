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
}
