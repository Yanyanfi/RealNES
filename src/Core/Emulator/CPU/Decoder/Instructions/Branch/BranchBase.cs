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
                var addr = (ushort)(pcHigh + (byte)sum);
                _bus.Read(addr);
                if (sum is <= 0xff and >= 0)
                {
                    state.Pc = addr;
                    EndInstr(in state);
                }
                break;
            case 4:
                addr = (ushort)(state.Pc + _offset);
                _bus.Read(addr);
                state.Pc = addr;
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
