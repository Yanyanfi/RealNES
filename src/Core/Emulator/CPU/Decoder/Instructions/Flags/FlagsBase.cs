using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Enums;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Flags;

internal abstract class FlagsBase(InstructionServices services) : InstructionBase(services)
{
    public override void Process(ref readonly CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                SetOrClearFlag(in state);
                EndInstr(in state);
                break;
        }
    }
    protected abstract void SetOrClearFlag(in CpuState state);
    protected override AddressingType GetAddressingType(byte opCode) => AddressingType.Implicit;
}
