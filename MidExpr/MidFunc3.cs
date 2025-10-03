using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Coplt.Dropping;
using Coplt.Union;

namespace MidExpr;

public sealed unsafe partial class MidFunc3
{
    #region Fields

    internal NativeHandle m_native = new();
    internal ReturnDefine m_return;
    internal readonly ParameterDefine[] m_parameters;
    internal LocalDefine[] m_locals = [];
    internal MidFunc3[] m_fns = [];
    internal string? m_name;
    internal int m_param_local_size = 0;
    internal int m_all_local_size = 0;
    internal bool m_finished;

    #endregion

    #region Handle

    [Dropping(Unmanaged = true)]
    internal sealed partial class NativeHandle
    {
        #region Fields

        internal IntPtr m_ptr;

        #endregion

        #region Ctor

        internal NativeHandle()
        {
            m_ptr = Coplt_MidFunc_Alloc();
            return;

            [DllImport("MidExpr.Native")]
            static extern IntPtr Coplt_MidFunc_Alloc();
        }

        #endregion

        #region Drop

        [Drop]
        private void Drop()
        {
            var old = Interlocked.Exchange(ref m_ptr, 0);
            if (old == 0) return;
            Coplt_MidFunc_Release(old);
            return;

            [DllImport("MidExpr.Native")]
            static extern void Coplt_MidFunc_Release(IntPtr self);
        }

        #endregion
    }

    #endregion

    #region Ctor

    internal MidFunc3(ReturnDefine ret, ParameterDefine[] parameters)
    {
        m_return = ret;
        m_parameters = parameters;
    }

    #endregion

    #region Props

    public ref readonly ReturnDefine Return => ref m_return;
    public ReadOnlySpan<ParameterDefine> Parameters => m_parameters.AsSpan();
    public ReadOnlySpan<LocalDefine> Locals => m_locals.AsSpan();

    public string? Name => m_name;

    #endregion

    #region Return

    public record struct ReturnDefine(MidType Type, SigLoc Place)
    {
        public MidType Type = Type;
        public SigLoc Place = Place;
        public int Index;
    }

    #endregion

    #region Parameter

    public record struct ParameterDefine(MidType Type, SigLoc Place, string? Name)
    {
        public MidType Type = Type;
        public SigLoc Place = Place;
        public int Index;
        public string? Name = Name;
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
        // stride by 4
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
            OpCode2 Op();
            void LdcX4(OpCode2 Op, uint Value);
            void Br(OpCode2 Op, LabelHandle Label);
            void Call(OpCode2 Op, MidFunc3 Func);
            void MarkLabel(LabelHandle Label);
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

    #region Builder

    public sealed unsafe partial class Builder
    {
        #region Fields

        internal readonly MidFunc3 m_inst;
        internal List<OpData> m_ops = new();
        internal readonly List<LabelDef> m_labels = new();

        #endregion

        #region Ctor

        public Builder(RetDecl Return, ReadOnlySpan<ParamDecl> Parameters)
        {
            var args = new ParameterDefine[Parameters.Length];
            for (var i = 0; i < Parameters.Length; i++)
            {
                var p = Parameters[i];
                args[i] = new(p.Type, p.Place, p.Name);
            }
            m_inst = new(new ReturnDefine(Return.Type, Return.Place), args);
        }

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

        #region Parameter

        public Builder ParamName(int index, string? Name)
        {
            if (m_inst.m_finished) throw new InvalidOperationException();
            m_inst.m_parameters[index].Name = Name;
            return this;
        }

        #endregion

        // todo local

        #region DefLabel

        public LabelHandle DefLabel(string? name = null)
        {
            if (m_inst.m_finished) throw new InvalidOperationException();
            var index = m_labels.Count;
            m_labels.Add(new(name));
            return new(index);
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

        #region Emit

        public Builder Emit(OpCode2 op)
        {
            if (m_inst.m_finished) throw new InvalidOperationException();
            m_ops.Add(OpData.Op(op));
            return this;
        }

        public Builder Emit(OpCode2 op, int Value) => Emit(op, (uint)Value);

        public Builder Emit(OpCode2 op, uint Value)
        {
            if (m_inst.m_finished) throw new InvalidOperationException();
            m_ops.Add(OpData.LdcX4(op, Value));
            return this;
        }

        public Builder Emit(OpCode2 op, float Value) => Emit(op, Unsafe.BitCast<float, uint>(Value));

        public Builder Emit(OpCode2 op, LabelHandle label)
        {
            if (m_inst.m_finished) throw new InvalidOperationException();
            m_ops.Add(OpData.Br(op, label));
            return this;
        }

        public Builder Emit(OpCode2 op, MidFunc3 func)
        {
            if (m_inst.m_finished) throw new InvalidOperationException();
            m_ops.Add(OpData.Call(op, func));
            return this;
        }

        public Builder Emit(OpCode2 op, Builder func) => Emit(op, func.m_inst);

        #endregion

        #region Build

        private struct MidFuncInitInfo
        {
            public ushort* p_code;
            public IntPtr* p_func;
            public uint code_len;
            public uint func_len;
            public uint all_local_size;
        }

        public MidFunc3 Build()
        {
            if (m_inst.m_finished) throw new InvalidOperationException();
            var code = new List<ushort>();
            var label_used = new List<(int, LabelHandle)>();
            var locs = new List<LocalDefine>();
            var fn_inc = 0;
            var fns = new Dictionary<MidFunc3, int>();
            var local_offset = 0;
            foreach (ref var param in m_inst.m_parameters.AsSpan())
            {
                if (param.Place is not SigLoc.Local) continue;
                var type = param.Type;
                ref var offset = ref local_offset;
                var size = type.SizeOf;
                var align = type.AlignOf;
                offset = offset.AlignUp(Math.Max(align, 4));
                param.Index = locs.Count;
                locs.Add(new()
                {
                    Offset = offset / 4,
                    Size = size,
                    Type = type,
                    Name = param.Name,
                    Flags = LocalFlags.Argument,
                });
                offset += size;
            }
            m_inst.m_param_local_size = local_offset;
            // todo locals
            m_inst.m_all_local_size = local_offset;
            if ((local_offset + 3) / 4 > ushort.MaxValue) throw new OverflowException();
            foreach (var op in m_ops)
            {
                switch (op.Tag)
                {
                    case OpData.Tags.Op:
                        code.Add((ushort)op.Op);
                        break;
                    case OpData.Tags.LdcX4:
                    {
                        code.Add((ushort)op.LdcX4.Op);
                        code.AddRange(MemoryMarshal.Cast<uint, ushort>(new Span<uint>(ref op.LdcX4.Value)));
                        break;
                    }
                    case OpData.Tags.Br:
                    {
                        var label = op.Br.Label;
                        label_used.Add((code.Count, label));
                        code.Add((ushort)op.Br.Op);
                        code.Add(0); // delay set
                        break;
                    }
                    case OpData.Tags.Call:
                    {
                        ref var fni = ref CollectionsMarshal.GetValueRefOrAddDefault(fns, op.Call.Func, out var exists);
                        if (!exists) fni = fn_inc++;
                        if (fni > ushort.MaxValue) throw new OverflowException();
                        code.Add((ushort)op.Call.Op);
                        code.Add((ushort)fni);
                        break;
                    }
                    case OpData.Tags.MarkLabel:
                    {
                        ref var label = ref CollectionsMarshal.AsSpan(m_labels)[op.MarkLabel.Label.Index];
                        label.m_offset = code.Count;
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
            m_inst.m_locals = locs.ToArray();
            m_inst.m_fns = fns.Keys.ToArray();
            var tmp_fns = ArrayPool<IntPtr>.Shared.Rent(m_inst.m_fns.Length);
            try
            {
                for (var i = 0; i < m_inst.m_fns.Length; i++)
                {
                    tmp_fns[i] = m_inst.m_fns[i].m_native.m_ptr;
                }
                fixed (ushort* p_code = CollectionsMarshal.AsSpan(code))
                fixed (IntPtr* p_fns = tmp_fns)
                {
                    MidFuncInitInfo info = new()
                    {
                        p_code = p_code,
                        p_func = p_fns,
                        code_len = (uint)code.Count,
                        func_len = (uint)m_inst.m_fns.Length,
                        all_local_size = (uint)m_inst.m_all_local_size,
                    };
                    Coplt_MidFunc_Init(m_inst.m_native.m_ptr, &info);
                }
            }
            finally
            {
                ArrayPool<IntPtr>.Shared.Return(tmp_fns);
            }
            m_inst.m_finished = true;
            return m_inst;

            [DllImport("MidExpr.Native")]
            static extern void Coplt_MidFunc_Init(IntPtr self, MidFuncInitInfo* info);
        }

        #endregion
    }

    #endregion

    #region Call

    public R CallFunc<R, A0>(A0 a0)
        where R : unmanaged
        where A0 : unmanaged
        => MidInterpreter3.CallFunc<R, A0>(this, a0);

    #endregion
}
