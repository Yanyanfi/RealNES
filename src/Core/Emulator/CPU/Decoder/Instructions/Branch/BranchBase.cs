using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Enums;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Branch;

internal abstract class BranchBase(InstructionServices services) : InstructionBase(services)
{
    private sbyte _offset;
    public override void Process(ref readonly CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                _offset = (sbyte)_bus.Read(state.Pc++);
                if (!NeedJump(in state))
                    EndInstr(in state);
                break;
            case 3:
                var pcLow = state.Pc & 0xff;
                var pcHigh = state.Pc & 0xff00;
                var sum = pcLow + _offset;
                var target = (ushort)(pcHigh + (byte)sum);
                _bus.Read(state.Pc);
                if (sum is <= 0xff and >= 0)
                {
                    state.Pc = target;
                    EndInstr(in state);
                }
                break;
            case 4:
                target = (ushort)(state.Pc + _offset);
                var dummy = (ushort)((state.Pc & 0xff00) | (target & 0x00ff));
                _bus.Read(dummy);
                state.Pc = target;
                EndInstr(in state);
                break;
        }
    }
    protected abstract bool NeedJump(ref readonly CpuState state);
    protected override AddressingType GetAddressingType(byte opCode)
    {
        return AddressingType.Relative;
    }
}
