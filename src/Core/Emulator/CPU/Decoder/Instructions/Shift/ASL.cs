using RealNES.Core.Emulator.CPU.Decoder.Exceptions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Enums;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Shift;

internal sealed class ASL(InstructionServices services) : InstructionBase(services)
{
    public override IReadOnlyList<byte> OpCodes { get; } = [0x0a, 0x06, 0x16, 0x0e, 0x1e];
    private ushort _addr;
    private byte _data;
    public override void Process(ref readonly CpuState state)
    {
        switch (_addressingType)
        {
            case AddressingType.Accumulator:
                StepA(in state);
                break;
            case AddressingType.ZeroPage:
                StepZp(in state);
                break;
            case AddressingType.ZeroPageX:
                StepZpx(in state);
                break;
            case AddressingType.Absolute:
                StepAbs(in state);
                break;
            case AddressingType.AbsoluteX:
                StepAbsX(in state);
                break;
        }
    }

    private void StepA(in CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                _flagSetter.SetCarry(in state, state.A >= 128);
                state.A <<= 1;
                _flagSetter.SetZeroByNumber(in state, state.A);
                _flagSetter.SetNegativeByNumber(in state, state.A);
                EndInstr(in state);
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
                _data = _bus[_addr];
                break;
            case 4:
                MemoryLast2Cycle(in state);
                break;
            case 5:
                MemoryLastCycle(in state);
                break;
        }
    }
    private void StepZpx(in CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                _ads.StepZpX1(_bus, in state);
                break;
            case 3:
                _ads.StepZpX2(_bus);
                break;
            case 4:
                _addr = _ads.GetZpXAddr(in state);
                _data = _bus[_addr];
                break;
            case 5:
                MemoryLast2Cycle(in state);
                break;
            case 6:
                MemoryLastCycle(in state);
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
                _addr = _ads.GetAbsAddr();
                _data = _bus[_addr];
                break;
            case 5:
                MemoryLast2Cycle(in state);
                break;
            case 6:
                MemoryLastCycle(in state);
                break;
        }
    }
    private void StepAbsX(in CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                _ads.StepAbsX1(_bus, in state);
                break;
            case 3:
                _ads.StepAbsX2(_bus, in state);
                break;
            case 4:
                if (_ads.GetAbsXAddr1(_bus, in state, out var addr))
                    _bus.Read(addr);
                break;
            case 5:
                _addr = _ads.GetAbsXAddr2();
                _data = _bus[_addr];
                break;
            case 6:
                MemoryLast2Cycle(in state);
                break;
            case 7:
                MemoryLastCycle(in state);
                break;
        }
    }
    private void MemoryLast2Cycle(ref readonly CpuState state)
    {
        _bus[_addr] = _data;
        _flagSetter.SetCarry(in state, _data >= 128);
        _data <<= 1;
        _flagSetter.SetZeroByNumber(in state, _data);
        _flagSetter.SetNegativeByNumber(in state, _data);
    }
    private void MemoryLastCycle(ref readonly CpuState state)
    {
        _bus[_addr] = _data;
        EndInstr(in state);
    }
    protected override AddressingType GetAddressingType(byte opCode)
    {
        return opCode switch
        {
            0x0a => AddressingType.Accumulator,
            0x06 => AddressingType.ZeroPage,
            0x16 => AddressingType.ZeroPageX,
            0x0e => AddressingType.Absolute,
            0x1e => AddressingType.AbsoluteX,
            _ => throw new MissingInstrException(opCode)
        };
    }
}
