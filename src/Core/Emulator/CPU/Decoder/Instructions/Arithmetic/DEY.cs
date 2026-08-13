using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Enums;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Arithmetic;

internal sealed class DEY(InstructionServices services) : InstructionBase(services)
{
    public override IReadOnlyList<byte> OpCodes { get; } = [0x88];

    public override void Process(ref readonly CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                state.Y--;
                _flagSetter.SetZeroByNumber(in state, state.Y);
                _flagSetter.SetNegativeByNumber(in state, state.Y);
                EndInstr(in state);
                break;
        }
    }

    protected override AddressingType GetAddressingType(byte opCode)
    {
        return AddressingType.Implicit;
    }
}
