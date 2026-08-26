using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Enums;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Jump;

internal sealed class RTS(InstructionServices services) : InstructionBase(services)
{
    public override IReadOnlyList<byte> OpCodes { get; } = [0x60];
    private ushort _addr;
    public override void Process(ref readonly CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                _bus.Read(state.Pc);
                break;
            case 3:
                _bus.Read(_stack.GetTopAddress(in state));
                break;
            case 4:
                _addr = _stack.Pull(in state);
                break;
            case 5:
                _addr += (ushort)(_stack.Pull(in state) << 8);
                state.Pc = _addr;
                break;
            case 6:
                _bus.Read(state.Pc++);
                EndInstr(in state);
                break;
        }
    }

    protected override AddressingType GetAddressingType(byte opCode)
    {
        return AddressingType.Implicit;
    }
}
