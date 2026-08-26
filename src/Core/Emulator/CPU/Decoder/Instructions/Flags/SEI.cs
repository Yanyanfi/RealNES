using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Flags;

internal sealed class SEI(InstructionServices services) : FlagsBase(services)
{
    public override IReadOnlyList<byte> OpCodes { get; } = [0x78];

    protected override void SetOrClearFlag(in CpuState state)
    {
        _flagSetter.SetInterruptDisable(in state, true);
        state.DelayI = true;
    }
}
