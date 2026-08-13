using RealNES.Core.Emulator.CPU.Decoder.Exceptions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Enums;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Arithmetic;

internal sealed class SBC(InstructionServices services) : InstructionBase(services)
{
    public override IReadOnlyList<byte> OpCodes { get; } = [0xe9, 0xe5, 0xf5, 0xed, 0xfd, 0xf9, 0xe1, 0xf1];
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
    private void StepImm(ref readonly CpuState state)
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
                LastCycle(in state, _bus[addr]);
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
                LastCycle(in state, _bus[addr]);
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
                LastCycle(in state, _bus[_ads.GetAbsXAddr2()]);
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
                LastCycle(in state, _bus[_ads.GetAbsYAddr2()]);
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
    private void LastCycle(ref readonly CpuState state, byte operand)
    {
        var diff = state.A - operand - (1 - (_flagReader.ReadCarry(in state) ? 1 : 0));
        _flagSetter.SetCarry(in state, diff >= 0);
        _flagSetter.SetZeroByNumber(in state, (byte)diff);
        _flagSetter.SetNegativeByNumber(in state, (byte)diff);
        _flagSetter.SetOverflow(in state, ((state.A ^ operand) & (state.A ^ diff) & 0x80) != 0);
        state.A = (byte)diff;
        EndInstr(in state);
    }
    protected override AddressingType GetAddressingType(byte opCode)
    {
        return opCode switch
        {
            0xe9 => AddressingType.Immediate,
            0xe5 => AddressingType.ZeroPage,
            0xf5 => AddressingType.ZeroPageX,
            0xed => AddressingType.Absolute,
            0xfd => AddressingType.AbsoluteX,
            0xf9 => AddressingType.AbsoluteY,
            0xe1 => AddressingType.IndexedIndirect,
            0xf1 => AddressingType.IndirectIndexed,
            _ => throw new MissingInstrException(opCode)
        };
    }
}
