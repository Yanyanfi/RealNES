using RealNES.Core.Emulator.CPU.Decoder.Exceptions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Enums;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Jump;

internal sealed class JMP(InstructionServices services) : InstructionBase(services)
{
    public override IReadOnlyList<byte> OpCodes { get; } = [0x4c, 0x6c];
    private ushort _pointer;
    private ushort _addr;
    public override void Process(ref readonly CpuState state)
    {
        switch (_addressingType)
        {
            case AddressingType.Absolute:
                StepAbs(in state);
                break;
            case AddressingType.Indirect:
                StepInd(in state);
                break;
        }
    }
    public void StepInd(ref readonly CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                _pointer = _bus[state.Pc++];
                break;
            case 3:
                _pointer += (ushort)(_bus[state.Pc++] << 8);
                break;
            case 4:
                _addr = _bus[_pointer];
                break;
            case 5:
                var pointerLow = (byte)((_pointer & 0xff) + 1);
                var pointerHigh = _pointer & 0xff00;
                var pointer = (ushort)(pointerLow + pointerHigh);
                _addr += (ushort)(_bus[pointer] << 8);
                state.Pc = _addr;
                EndInstr(in state);
                break;
        }
    }
    public void StepAbs(ref readonly CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                _addr = _bus[state.Pc++];
                break;
            case 3:
                _addr += (ushort)(_bus[state.Pc] << 8);
                state.Pc = _addr;
                EndInstr(in state);
                break;
        }
    }
    protected override AddressingType GetAddressingType(byte opCode)
    {
        return opCode switch
        {
            0x4c => AddressingType.Absolute,
            0x6c => AddressingType.Indirect,
            _ => throw new MissingInstrException(opCode)
        };
    }
}
