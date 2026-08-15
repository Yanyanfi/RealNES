using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Branch;

internal sealed class BNE(InstructionServices services) : BranchBase(services)
{
    public override IReadOnlyList<byte> OpCodes { get; } = [0xd0];

    protected override bool NeedJump(ref readonly CpuState state) => !_flagReader.ReadZero(in state);
}
