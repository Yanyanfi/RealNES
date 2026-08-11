using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Enums;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Stack;

internal sealed class PLP(InstructionServices services) : InstructionBase(services)
{
    public override IReadOnlyList<byte> OpCodes { get; } = [0x28];

    public override void Process(ref readonly CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                _bus.Read(state.Pc);
                break;
            case 3:
                _bus.Read(_stack.GetTopAddress(in state));
                break;
            case 4:
                state.P = _stack.Pull(in state);
                state.DelayI = true;
                EndInstr(in state);
                break;
        }
    }

    protected override AddressingType GetAddressingType(byte opCode)
    {
        return AddressingType.Implicit;
    }
}
