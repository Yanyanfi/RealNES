using RealNES.Core.Emulator.CPU.Decoder.Exceptions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Enums;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Compare;

internal sealed class CPX(InstructionServices services) : InstructionBase(services)
{
    public override IReadOnlyList<byte> OpCodes { get; } = [0xe0, 0xe4, 0xec];
    private ushort _addr;
    public override void Process(ref readonly CpuState state)
    {
        switch (_addressingType)
        {
            case AddressingType.Immediate:
                StepImm(in state);
                break;
            case AddressingType.ZeroPage:
                StepZp(in state);
                break;
            case AddressingType.Absolute:
                StepAbs(in state);
                break;
        }
    }
    private void StepImm(in CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                LastCycle(in state, _bus[state.Pc++]);
                break;
        }
    }
    private void StepZp(in CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                _addr = _bus[state.Pc++];
                break;
            case 3:
                LastCycle(in state, _bus[_addr]);
                break;
        }
    }
    private void StepAbs(in CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                _ads.StepAbs1(_bus, in state);
                break;
            case 3:
                _ads.StepAbs2(_bus, in state);
                break;
            case 4:
                var addr = _ads.GetAbsAddr();
                var value = _bus[addr];
                LastCycle(in state, value);
                break;
        }
    }
    private void LastCycle(in CpuState state, byte value)
    {
        _flagSetter.SetCarry(in state, state.X >= value);
        var result = (byte)(state.X - value);
        _flagSetter.SetZeroByNumber(in state, result);
        _flagSetter.SetNegativeByNumber(in state, result);
        EndInstr(in state);
    }
    protected override AddressingType GetAddressingType(byte opCode)
    {
        return opCode switch
        {
            0xe0 => AddressingType.Immediate,
            0xe4 => AddressingType.ZeroPage,
            0xec => AddressingType.Absolute,
            _ => throw new MissingInstrException(opCode)
        };
    }
}
