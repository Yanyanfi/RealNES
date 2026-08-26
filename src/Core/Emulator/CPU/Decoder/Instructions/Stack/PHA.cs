using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Enums;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Stack;

internal sealed class PHA(InstructionServices services) : InstructionBase(services)
{
    public override IReadOnlyList<byte> OpCodes { get; } = [0x48];

    public override void Process(ref readonly CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                _bus.Read(state.Pc);
                break;
            case 3:
                _stack.Push(in state, state.A);
                EndInstr(in state);
                break;
        }
    }

    protected override AddressingType GetAddressingType(byte opCode) => AddressingType.Implicit;
}
