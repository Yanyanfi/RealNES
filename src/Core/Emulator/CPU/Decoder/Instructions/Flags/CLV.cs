using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Flags;

internal sealed class CLV(InstructionServices services) : FlagsBase(services)
{
    public override IReadOnlyList<byte> OpCodes { get; } = [0xb8];

    protected override void SetOrClearFlag(in CpuState state) => _flagSetter.SetOverflow(in state, false);
}
