using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Branch;

internal sealed class BVS(InstructionServices services) : BranchBase(services)
{
    public override IReadOnlyList<byte> OpCodes { get; } = [0x70];

    protected override bool NeedJump(ref readonly CpuState state) => _flagReader.ReadOverflow(in state);
}
