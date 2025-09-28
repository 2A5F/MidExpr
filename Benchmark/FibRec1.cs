using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using MidExpr;

namespace Benchmark;

[DisassemblyDiagnoser(syntax: DisassemblySyntax.Intel, maxDepth: 100), MemoryDiagnoser]
public class FibRec1
{
    private MidFunc func;

    public FibRec1()
    {
        var builder = new MidFunc.Builder(MidType.I4, [MidType.I4]) { Name = "FibRec" };
        var l = builder.DefLabel();
        func = builder
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
    }

    public int FibRec(int n)
    {
        if (n < 3) return 1;
        return FibRec(n - 1) + FibRec(n - 2);
    }

    [Benchmark(Baseline = true)]
    public int Csharp() => FibRec(8);

    [Benchmark]
    public int Mid() => func.CallFunc<int, int>(8);
}
