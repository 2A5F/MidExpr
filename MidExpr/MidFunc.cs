using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Coplt.Union;

namespace MidExpr;

public sealed unsafe partial class MidFunc
{
    #region Fields

    /// <summary>
    /// all local size
    /// </summary>
    internal int m_param_stack_size = 0;
    internal int m_ret_stack_size = 0;
    internal ushort[] m_code = [];
    internal LocalDefine[] m_locals = [];
    internal MidFunc[] m_fns = [];
    internal readonly MidType m_ret;
    internal readonly Parameter[] m_params;
    internal int m_init_stack_size = 0;
    internal string? m_name;
    internal bool m_finished;

    #endregion

    #region Props

    public int InitStackSize => m_init_stack_size;
    public int ParamStackSize => m_param_stack_size;

    public MidType ReturnType => m_ret;
    public ReadOnlySpan<Parameter> Parameters => m_params;

    public ReadOnlySpan<LocalDefine> Locals => m_locals;

    public string? Name => m_name;

    #endregion

    #region Ctor

    private MidFunc(MidType ReturnType, Parameter[] Parameters)
    {
        m_ret = ReturnType;
        m_params = Parameters;
    }

    #endregion

    #region Parameter

    public record struct Parameter(MidType Type, string? Name)
    {
        internal int m_local_index;

        public int LocalIndex => m_local_index;
    }

    #endregion

    #region Local Define

    [Flags]
    public enum LocalFlags
    {
        None,
        Return = 1 << 0,
        Argument = 1 << 1,
    }

    public record struct LocalDefine(int Offset, int Size, MidType Type, string? Name, LocalFlags Flags)
    {
        public int Offset = Offset;
        public int Size = Size;
        public MidType Type = Type;
        public string? Name = Name;
        public LocalFlags Flags = Flags;
    }

    #endregion

    #region OpData

    [Union2]
    internal partial struct OpData
    {
        [UnionTemplate]
        private interface Template
        {
            void Common(OpCode Op);
            void MarkLabel(LabelHandle Label);
            void Ret();
            void Arg(int Arg);
            void LdcX4(uint Value);
            void Call(MidFunc Func);
            void Bge(MidPrimitive Primitive, LabelHandle label);
            void Add(MidPrimitive Primitive);
            void Sub(MidPrimitive Primitive);
        }
    }

    #endregion

    #region Label

    internal record struct LabelDef(string? Name)
    {
        public int m_offset;
        public bool m_marked;
    }

    public record struct LabelHandle(int Index);

    #endregion

    #region Call

    public void CallAction()
    {
        var stack = ArrayPool<byte>.Shared.Rent(MidInterpreter.StackSize);
        try
        {
            fixed (byte* p_stack = stack)
            {
                UnsafeCall(p_stack);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(stack);
        }
    }

    public void CallAction<A0>(A0 a0)
        where A0 : unmanaged
    {
        var stack = ArrayPool<byte>.Shared.Rent(MidInterpreter.StackSize);
        try
        {
            fixed (byte* p_stack = stack)
            {
                *(A0*)(&p_stack[m_locals[m_params[0].m_local_index].Offset]) = a0;
                UnsafeCall(p_stack);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(stack);
        }
    }

    public R CallFunc<R, A0>(A0 a0)
        where R : unmanaged where A0 : unmanaged
    {
        var stack = ArrayPool<byte>.Shared.Rent(MidInterpreter.StackSize);
        try
        {
            fixed (byte* p_stack = stack)
            {
                *(A0*)(&p_stack[m_locals[m_params[0].m_local_index].Offset]) = a0;
                UnsafeCall(p_stack);
                return *(R*)p_stack;
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(stack);
        }
    }

    public void UnsafeCall(byte* stack) => MidInterpreter.Exec(this, stack);

    #endregion

    #region Builder

    public sealed class Builder
    {
        #region Fields

        internal readonly MidFunc m_inst;
        internal readonly List<OpData> m_ops = new();
        internal readonly List<LabelDef> m_labels = new();

        #endregion

        #region Props

        public string? Name
        {
            get => m_inst.m_name;
            set
            {
                if (m_inst.m_finished) throw new InvalidOperationException();
                m_inst.m_name = value;
            }
        }

        #endregion

        #region Ctor

        public Builder(
            MidType ReturnType,
            ReadOnlySpan<MidType> Params
        )
        {
            var args = new Parameter[Params.Length];
            for (var i = 0; i < Params.Length; i++)
            {
                args[i] = new(Params[i], null);
            }
            m_inst = new(ReturnType, args);
        }

        #endregion

        #region Build

        public MidFunc Build()
        {
            var code = new List<ushort>();
            var label_used = new List<(int, LabelHandle)>();
            var locs = new List<LocalDefine>();
            var fn_inc = 0;
            var fns = new Dictionary<MidFunc, int>();
            m_inst.m_ret_stack_size = m_inst.m_ret.SizeOf;
            var local_offset = 0;
            foreach (ref var param in m_inst.m_params.AsSpan())
            {
                var type = param.Type;
                ref var offset = ref local_offset;
                var size = type.SizeOf;
                var align = type.AlignOf;
                offset = offset.AlignUp(align);
                param.m_local_index = locs.Count;
                locs.Add(new()
                {
                    Offset = offset,
                    Size = size,
                    Type = type,
                    Name = param.Name,
                    Flags = LocalFlags.Argument,
                });
                offset += size;
            }
            m_inst.m_param_stack_size = local_offset;
            // todo locals
            m_inst.m_init_stack_size = local_offset;
            if (locs.Count > ushort.MaxValue) throw new OverflowException();
            foreach (var op in m_ops)
            {
                switch (op.Tag)
                {
                    case OpData.Tags.Common:
                    {
                        code.Add((ushort)op.Common.Op);
                        break;
                    }
                    case OpData.Tags.MarkLabel:
                    {
                        ref var label = ref CollectionsMarshal.AsSpan(m_labels)[op.MarkLabel.Label.Index];
                        label.m_offset = code.Count;
                        break;
                    }
                    case OpData.Tags.Ret:
                    {
                        code.Emit_Ret(m_inst.m_ret);
                        break;
                    }
                    case OpData.Tags.Arg:
                    {
                        ref readonly var arg = ref m_inst.Parameters[op.Arg.Arg];
                        code.Emit_LdLoc(arg.Type, arg.m_local_index); // arg is local n
                        break;
                    }
                    case OpData.Tags.LdcX4:
                    {
                        code.Emit_LdcX4(op.LdcX4.Value);
                        break;
                    }
                    case OpData.Tags.Call:
                    {
                        ref var fni = ref CollectionsMarshal.GetValueRefOrAddDefault(fns, op.Call.Func, out var exists);
                        if (!exists) fni = fn_inc++;
                        if (fni > ushort.MaxValue) throw new OverflowException();
                        code.Add((ushort)OpCode.Call);
                        code.Add((ushort)fni);
                        break;
                    }
                    case OpData.Tags.Bge:
                    {
                        var label = op.Bge.label;
                        label_used.Add((code.Count, label));
                        code.Emit_Bge(op.Bge.Primitive);
                        break;
                    }
                    case OpData.Tags.Add:
                    {
                        code.Emit_Add(op.Add.Primitive);
                        break;
                    }
                    case OpData.Tags.Sub:
                    {
                        code.Emit_Sub(op.Sub.Primitive);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            foreach (var (offset, label) in label_used)
            {
                ref readonly var lab = ref CollectionsMarshal.AsSpan(m_labels)[label.Index];
                code[offset + 1] = (ushort)(short)(lab.m_offset - offset);
            }
            m_inst.m_code = code.ToArray();
            m_inst.m_locals = locs.ToArray();
            m_inst.m_fns = fns.Keys.ToArray();
            m_inst.m_finished = true;
            return m_inst;
        }

        #endregion

        #region Args

        public Builder ArgName(int index, string name)
        {
            if (m_inst.m_finished) throw new InvalidOperationException();
            m_inst.m_params[index].Name = name;
            return this;
        }

        #endregion

        #region Def

        public LabelHandle DefLabel(string? name = null)
        {
            if (m_inst.m_finished) throw new InvalidOperationException();
            var index = m_labels.Count;
            m_labels.Add(new(name));
            return new(index);
        }

        #endregion

        #region Ret

        public Builder Ret()
        {
            if (m_inst.m_finished) throw new InvalidOperationException();
            m_ops.Add(OpData.Ret);
            return this;
        }

        #endregion

        #region LdcX4

        public Builder LdcX4(float value) => LdcX4(Unsafe.BitCast<float, uint>(value));

        public Builder LdcX4(int value) => LdcX4((uint)value);

        public Builder LdcX4(uint value)
        {
            if (m_inst.m_finished) throw new InvalidOperationException();
            m_ops.Add(OpData.LdcX4(value));
            return this;
        }

        #endregion

        #region LdArg

        public Builder LdArg(int arg)
        {
            if (m_inst.m_finished) throw new InvalidOperationException();
            m_ops.Add(OpData.Arg(arg));
            return this;
        }

        #endregion

        #region MarkLabel

        public Builder MarkLabel(LabelHandle label)
        {
            if (m_inst.m_finished) throw new InvalidOperationException();
            ref var lab = ref CollectionsMarshal.AsSpan(m_labels)[label.Index];
            if (lab.m_marked) throw new InvalidOperationException("Label already marked.");
            m_ops.Add(OpData.MarkLabel(label));
            lab.m_marked = true;
            return this;
        }

        #endregion

        #region Br

        public Builder Bge(MidPrimitive primitive, LabelHandle label)
        {
            if (m_inst.m_finished) throw new InvalidOperationException();
            m_ops.Add(OpData.Bge(primitive, label));
            return this;
        }

        #endregion

        #region Call

        public Builder Call(MidFunc func)
        {
            if (m_inst.m_finished) throw new InvalidOperationException();
            m_ops.Add(OpData.Call(func));
            return this;
        }

        public Builder Call(Builder func)
        {
            if (m_inst.m_finished) throw new InvalidOperationException();
            m_ops.Add(OpData.Call(func.m_inst));
            return this;
        }

        #endregion

        #region Calc

        public Builder Add(MidPrimitive primitive)
        {
            if (m_inst.m_finished) throw new InvalidOperationException();
            m_ops.Add(OpData.Add(primitive));
            return this;
        }

        public Builder Sub(MidPrimitive primitive)
        {
            if (m_inst.m_finished) throw new InvalidOperationException();
            m_ops.Add(OpData.Sub(primitive));
            return this;
        }

        #endregion
    }

    #endregion
}
