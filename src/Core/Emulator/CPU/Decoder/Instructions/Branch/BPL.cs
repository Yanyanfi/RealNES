using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Branch;

internal sealed class BPL(InstructionServices services) : BranchBase(services)
{
    public override IReadOnlyList<byte> OpCodes { get; } = [0x10];

    protected override bool NeedJump(ref readonly CpuState state) => !_flagReader.ReadNegative(in state);
}
