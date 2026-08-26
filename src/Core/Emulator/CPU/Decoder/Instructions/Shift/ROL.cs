using RealNES.Core.Emulator.CPU.Decoder.Exceptions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Enums;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Shift;

internal sealed class ROL(InstructionServices services) : InstructionBase(services)
{
    public override IReadOnlyList<byte> OpCodes { get; } = [0x2a, 0x26, 0x36, 0x2e, 0x3e];
    private byte _data;
    private ushort _addr;

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
                _bus.Read(state.Pc);
                var carry = _flagReader.ReadCarry(in state);
                var flag = state.A >= 128;
                _flagSetter.SetCarry(in state, flag);
                state.A <<= 1;
                if (carry)
                    state.A++;
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
        var carry = _flagReader.ReadCarry(in state);
        var flag = _data >= 128;
        _flagSetter.SetCarry(in state, flag);
        _data <<= 1;
        if (carry)
            _data++;
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
            0x2a => AddressingType.Accumulator,
            0x26 => AddressingType.ZeroPage,
            0x36 => AddressingType.ZeroPageX,
            0x2e => AddressingType.Absolute,
            0x3e => AddressingType.AbsoluteX,
            _ => throw new MissingInstrException(opCode)
        };
    }
}
