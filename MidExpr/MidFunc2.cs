using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Coplt.Union;

namespace MidExpr;

public sealed unsafe partial class MidFunc2
{
    #region Fields

    internal ushort[] m_code = [];
    internal ReturnDefine m_return;
    internal readonly ParameterDefine[] m_parameters;
    internal LocalDefine[] m_locals = [];
    internal MidFunc2[] m_fns = [];
    internal string? m_name;
    internal int m_param_local_size = 0;
    internal int m_all_local_size = 0;
    internal bool m_finished;

    #endregion

    #region Props

    internal ReadOnlySpan<OpCode2> DebugCode => MemoryMarshal.Cast<ushort, OpCode2>(m_code.AsSpan());

    public ref readonly ReturnDefine Return => ref m_return;
    public ReadOnlySpan<ParameterDefine> Parameters => m_parameters.AsSpan();
    public ReadOnlySpan<LocalDefine> Locals => m_locals.AsSpan();

    public string? Name => m_name;

    #endregion

    #region Ctor

    internal MidFunc2(ReturnDefine ret, ParameterDefine[] parameters)
    {
        m_return = ret;
        m_parameters = parameters;
    }

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
            void Call(OpCode2 Op, MidFunc2 Func);
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

        internal readonly MidFunc2 m_inst;
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

        public Builder Emit(OpCode2 op, MidFunc2 func)
        {
            if (m_inst.m_finished) throw new InvalidOperationException();
            m_ops.Add(OpData.Call(op, func));
            return this;
        }

        public Builder Emit(OpCode2 op, Builder func) => Emit(op, func.m_inst);

        #endregion

        #region Build

        public MidFunc2 Build()
        {
            if (m_inst.m_finished) throw new InvalidOperationException();
            var code = new List<ushort>();
            var label_used = new List<(int, LabelHandle)>();
            var locs = new List<LocalDefine>();
            var fn_inc = 0;
            var fns = new Dictionary<MidFunc2, int>();
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
            m_inst.m_code = code.ToArray();
            m_inst.m_locals = locs.ToArray();
            m_inst.m_fns = fns.Keys.ToArray();
            m_inst.m_finished = true;
            return m_inst;
        }

        #endregion
    }

    #endregion

    #region Call

    public R CallFunc<R, A0>(A0 a0)
        where R : unmanaged
        where A0 : unmanaged
        => MidInterpreter2.CallFunc<R, A0>(this, a0);

    #endregion
}

#region ParameterDeclare

[Flags]
public enum SigLoc
{
    Local,
    R0,
    R1,
    R2,
    R3,
    R4,
    R5,
    R6,
    R7,
}

public record struct RetDecl(MidType Type, SigLoc Place);
public record struct ParamDecl(MidType Type, SigLoc Place, string? Name = null);

#endregion
