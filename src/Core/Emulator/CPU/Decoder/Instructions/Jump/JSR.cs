using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Enums;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Jump;

internal sealed class JSR(InstructionServices services) : InstructionBase(services)
{
    public override IReadOnlyList<byte> OpCodes { get; } = [0x20];
    private ushort _addr;
    public override void Process(ref readonly CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                _addr = _bus.Read(state.Pc++);
                break;
            case 3:
                _bus.Read(_stack.GetTopAddress(in state));
                break;
            case 4:
                _stack.Push(in state, (byte)(state.Pc >> 8));
                break;
            case 5:
                _stack.Push(in state, (byte)state.Pc);
                break;
            case 6:
                _addr += (ushort)(_bus[state.Pc] << 8);
                state.Pc = _addr;
                EndInstr(in state);
                break;
        }
    }

    protected override AddressingType GetAddressingType(byte opCode)
    {
        return AddressingType.Absolute;
    }
}
