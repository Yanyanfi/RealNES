using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Flags;

internal sealed class SED(InstructionServices services) : FlagsBase(services)
{
    public override IReadOnlyList<byte> OpCodes { get; } = [0xf8];

    protected override void SetOrClearFlag(in CpuState state) => _flagSetter.SetDecimal(in state, true);
}
