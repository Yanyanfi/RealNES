using RealNES.Core.Emulator.CPU.Decoder.Exceptions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Enums;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Access;

internal sealed class STY(InstructionServices services) : InstructionBase(services)
{
    private byte _arg1;

    public override IReadOnlyList<byte> OpCodes { get; } = [0x84, 0x94, 0x8c];

    public override void Process(ref readonly CpuState state)
    {
        switch (_addressingType)
        {
            case AddressingType.ZeroPage:
                StepZp(in state);
                break;
            case AddressingType.ZeroPageX:
                StepZpX(in state);
                break;
            case AddressingType.Absolute:
                StepAbs(in state);
                break;
        }
    }
    private void StepZp(ref readonly CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                _arg1 = _bus[state.Pc++];
                return;
            case 3:
                _bus[_arg1] = state.Y;
                EndInstr(in state);
                return;
        }
    }
    private void StepZpX(ref readonly CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                _ads.StepZpX1(_bus, in state);
                return;
            case 3:
                _ads.StepZpX2(_bus);
                return;
            case 4:
                var addr = _ads.GetZpXAddr(in state);
                _bus[addr] = state.Y;
                EndInstr(in state);
                return;
        }
    }
    private void StepAbs(ref readonly CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                _ads.StepAbs1(_bus, in state);
                return;
            case 3:
                _ads.StepAbs2(_bus, in state);
                return;
            case 4:
                var addr = _ads.GetAbsAddr();
                _bus[addr] = state.Y;
                EndInstr(in state);
                return;
        }
    }

    protected override AddressingType GetAddressingType(byte opCode)
    {
        return opCode switch
        {
            0x84 => AddressingType.ZeroPage,
            0x94 => AddressingType.ZeroPageX,
            0x8c => AddressingType.Absolute,
            _ => throw new MissingInstrException(opCode)
        };
    }
}
