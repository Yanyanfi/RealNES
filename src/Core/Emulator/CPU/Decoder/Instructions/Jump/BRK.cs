using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Enums;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Jump;

internal sealed class BRK(InstructionServices services) : InstructionBase(services)
{
    public override IReadOnlyList<byte> OpCodes { get; } = [0x00];
    private ushort _addr;
    public override void Process(ref readonly CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                _bus.Read(state.Pc++);
                break;
            case 3:
                var pcHigh = (byte)(state.Pc >> 8);
                _stack.Push(in state, pcHigh);
                break;
            case 4:
                var pcLow = (byte)state.Pc;
                _stack.Push(in state, pcLow);
                break;
            case 5:
                var flag = (byte)(state.P | 0b110000);
                _stack.Push(in state, flag);
                _flagSetter.SetInterruptDisable(in state, true);
                break;
            case 6:
                _addr = _bus[0xfffe];
                break;
            case 7:
                _addr += (ushort)(_bus[0xffff] * 256);
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
