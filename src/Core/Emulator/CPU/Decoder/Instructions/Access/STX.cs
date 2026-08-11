using RealNES.Core.Emulator.CPU.Decoder.Exceptions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Enums;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Access;

internal sealed class STX(InstructionServices services) : InstructionBase(services)
{
    public override IReadOnlyList<byte> OpCodes { get; } = [0x86, 0x96, 0x8e];
    private byte _arg1;
    public override void Process(ref readonly CpuState state)
    {
        switch (_addressingType)
        {
            case AddressingType.ZeroPage:
                StepZp(in state);
                break;
            case AddressingType.ZeroPageY:
                StepZpY(in state);
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
                _bus[_arg1] = state.X;
                EndInstr(in state);
                return;
        }
    }
    private void StepZpY(ref readonly CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                _ads.StepZpY1(_bus, in state);
                return;
            case 3:
                _ads.StepZpY2(_bus);
                return;
            case 4:
                var addr = _ads.GetZpYAddr(in state);
                _bus[addr] = state.X;
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
                _bus[addr] = state.X;
                EndInstr(in state);
                return;
        }
    }
    protected override AddressingType GetAddressingType(byte opCode)
    {
        return opCode switch
        {
            0x86 => AddressingType.ZeroPage,
            0x96 => AddressingType.ZeroPageY,
            0x8e => AddressingType.Absolute,
            _ => throw new MissingInstrException(opCode)
        };
    }
}
