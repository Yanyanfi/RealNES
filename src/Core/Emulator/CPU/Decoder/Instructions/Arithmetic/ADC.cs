using RealNES.Core.Emulator.CPU.Decoder.Exceptions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Enums;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Arithmetic;

internal sealed class ADC(InstructionServices services) : InstructionBase(services)
{
    public override IReadOnlyList<byte> OpCodes { get; } = [0x69, 0x65, 0x75, 0x6d, 0x7d, 0x79, 0x61, 0x71];
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
    private void StepZp(ref readonly CpuState state)
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
    private void StepZpX(ref readonly CpuState state)
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
    private void StepAbs(ref readonly CpuState state)
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
                var operand = _bus[_ads.GetAbsAddr()];
                LastCycle(in state, operand);
                break;
        }
    }
    private void StepAbsX(ref readonly CpuState state)
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
                var operand = _bus[_ads.GetAbsXAddr2()];
                LastCycle(in state, operand);
                break;
        }
    }
    private void StepAbsY(ref readonly CpuState state)
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
                var operand = _bus[_ads.GetAbsYAddr2()];
                LastCycle(in state, operand);
                break;
        }
    }
    private void StepIndX(ref readonly CpuState state)
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
    private void StepIndY(ref readonly CpuState state)
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
                var num = _bus[_ads.GetIndirectYAddr2()];
                LastCycle(in state, num);
                break;
        }
    }
    private void LastCycle(ref readonly CpuState state, ushort operand)
    {
        var oddA = state.A;
        var sum = state.A + operand;
        if (_flagReader.ReadCarry(in state))
            sum++;
        _flagSetter.SetCarry(in state, sum > 0xff);
        state.A = (byte)sum;
        _flagSetter.SetNegativeByNumber(in state, state.A);
        _flagSetter.SetZeroByNumber(in state, state.A);
        _flagSetter.SetOverflow(in state, (~(oddA ^ operand) & (oddA ^ state.A) & 0x80) != 0);
        EndInstr(in state);
    }
    protected override AddressingType GetAddressingType(byte opCode)
    {
        return opCode switch
        {
            0x69 => AddressingType.Immediate,
            0x65 => AddressingType.ZeroPage,
            0x75 => AddressingType.ZeroPageX,
            0x6d => AddressingType.Absolute,
            0x7d => AddressingType.AbsoluteX,
            0x79 => AddressingType.AbsoluteY,
            0x61 => AddressingType.IndexedIndirect,
            0x71 => AddressingType.IndirectIndexed,
            _ => throw new MissingInstrException(opCode)
        };
    }
}
