using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Enums;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Transfer;

internal sealed class TAX(InstructionServices services) : InstructionBase(services)
{
    public override IReadOnlyList<byte> OpCodes { get; } = [0xaa];

    public override void Process(ref readonly CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                _bus.Read(state.Pc);
                state.X = state.A;
                EndInstr(in state);
                break;
        }
    }
    protected override void OnEndInstr(ref readonly CpuState state)
    {
        _flagSetter.SetZeroByNumber(in state, state.X);
        _flagSetter.SetNegativeByNumber(in state, state.X);
    }

    protected override AddressingType GetAddressingType(byte opCode)
    {
        return AddressingType.Implicit;
    }
}
