using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Flags;

internal sealed class CLI(InstructionServices services) : FlagsBase(services)
{
    public override IReadOnlyList<byte> OpCodes { get; } = [0x58];

    protected override void SetOrClearFlag(in CpuState state)
    {
        _flagSetter.SetInterruptDisable(in state, false);
        state.DelayI = true;
    }
}
