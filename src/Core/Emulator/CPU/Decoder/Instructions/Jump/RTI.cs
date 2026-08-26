using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Enums;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Jump;

internal sealed class RTI(InstructionServices services) : InstructionBase(services)
{
    public override IReadOnlyList<byte> OpCodes { get; } = [0x40];
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
                state.P = (byte)((_stack.Pull(in state) & 0xCF) | 0x20);
                break;
            case 5:
                _addr = _stack.Pull(in state);
                break;
            case 6:
                _addr += (ushort)(_stack.Pull(in state) << 8);
                state.Pc = _addr;
                EndInstr(in state);
                break;
        }
    }

    protected override AddressingType GetAddressingType(byte opCode)
    {
        return AddressingType.Implicit;
    }
}
