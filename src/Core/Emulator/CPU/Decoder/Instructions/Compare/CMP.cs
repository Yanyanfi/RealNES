using RealNES.Core.Emulator.CPU.Decoder.Exceptions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Enums;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Compare;

internal sealed class CMP(InstructionServices services) : InstructionBase(services)
{
    public override IReadOnlyList<byte> OpCodes { get; } = [0xc9, 0xc5, 0xd5, 0xcd, 0xdd, 0xd9, 0xc1, 0xd1];
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
            case AddressingType.ZeroPageX:
                StepZpX(in state);
                break;
            case AddressingType.Absolute:
                StepAbs(in state);
                break;
            case AddressingType.AbsoluteX:
                StepAbsX(in state);
                break;
            case AddressingType.AbsoluteY:
                StepAbsY(in state);
                break;
            case AddressingType.IndexedIndirect:
                StepIndX(in state);
                break;
            case AddressingType.IndirectIndexed:
                StepIndY(in state);
                break;
        }
    }
    private void StepImm(in CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                var value = _bus[state.Pc++];
                LastCycle(in state, value);
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
                var value = _bus[_addr];
                LastCycle(in state, value);
                break;
        }
    }
    private void StepZpX(in CpuState state)
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
                var addr = _ads.GetZpXAddr(in state);
                var value = _bus[addr];
                LastCycle(in state, value);
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
                    LastCycle(in state, _bus[addr]);
                break;
            case 5:
                addr = _ads.GetAbsXAddr2();
                var value = _bus[addr];
                LastCycle(in state, value);
                break;
        }
    }
    private void StepAbsY(in CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                _ads.StepAbsY1(_bus, in state);
                break;
            case 3:
                _ads.StepAbsY2(_bus, in state);
                break;
            case 4:
                if (_ads.GetAbsYAddr1(_bus, in state, out var addr))
                    LastCycle(in state, _bus[addr]);
                break;
            case 5:
                addr = _ads.GetAbsYAddr2();
                LastCycle(in state, _bus[addr]);
                break;
        }
    }
    private void StepIndX(in CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                _ads.StepIndirectX1(_bus, in state);
                break;
            case 3:
                _ads.StepIndirectX2(_bus);
                break;
            case 4:
                _ads.StepIndirectX3(_bus, in state);
                break;
            case 5:
                _ads.StepIndirectX4(_bus, in state);
                break;
            case 6:
                var addr = _ads.GetIndirectXAddr();
                LastCycle(in state, _bus[addr]);
                break;
        }
    }
    private void StepIndY(in CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                _ads.StepIndirectY1(_bus, in state);
                break;
            case 3:
                _ads.StepIndirectY2(_bus);
                break;
            case 4:
                _ads.StepIndirectY3(_bus);
                break;
            case 5:
                if (_ads.GetIndirectYAddr1(_bus, in state, out var addr))
                    LastCycle(in state, _bus[addr]);
                break;
            case 6:
                addr = _ads.GetIndirectYAddr2();
                LastCycle(in state, _bus[addr]);
                break;
        }
    }
    private void LastCycle(in CpuState state, byte value)
    {
        _flagSetter.SetCarry(in state, state.A >= value);
        var result = (byte)(state.A - value);
        _flagSetter.SetZeroByNumber(in state, result);
        _flagSetter.SetNegativeByNumber(in state, result);
        EndInstr(in state);
    }
    protected override AddressingType GetAddressingType(byte opCode)
    {
        return opCode switch
        {
            0xc9 => AddressingType.Immediate,
            0xc5 => AddressingType.ZeroPage,
            0xd5 => AddressingType.ZeroPageX,
            0xcd => AddressingType.Absolute,
            0xdd => AddressingType.AbsoluteX,
            0xd9 => AddressingType.AbsoluteY,
            0xc1 => AddressingType.IndexedIndirect,
            0xd1 => AddressingType.IndirectIndexed,
            _ => throw new MissingInstrException(opCode)
        };
    }
}
