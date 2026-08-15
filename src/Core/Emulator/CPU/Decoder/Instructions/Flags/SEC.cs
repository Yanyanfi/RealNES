using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Flags;

internal sealed class SEC(InstructionServices services) : FlagsBase(services)
{
    public override IReadOnlyList<byte> OpCodes { get; } = [0x38];

    protected override void SetOrClearFlag(in CpuState state)
    {
        _flagSetter.SetCarry(in state, true);
    }
}
