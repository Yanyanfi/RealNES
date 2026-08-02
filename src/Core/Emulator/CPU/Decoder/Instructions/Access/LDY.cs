using RealNES.Core.Emulator.CPU.Decoder.Exceptions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Access;

internal sealed class LDY(InstructionServices services) : InstructionBase(services)
{
    private byte _arg1;
    public override IReadOnlyList<byte> OpCodes { get; } =[0xa0,0xa4,0xb4,0xac,0xbc];

    public override void Process(ref readonly CpuState state)
    {
        switch (_addressingType)
        {
            case AddressingType.Immediate:
                StepImm(in state);
                return;
            case AddressingType.ZeroPage:
                StepZp(in state);
                return;
            case AddressingType.ZeroPageX:
                StepZpX(in state);
                break;
            case AddressingType.Absolute:
                StepAbs(in state);
                break;
            case AddressingType.AbsoluteX:
                StepAbsX(in state);
                break;
        }
    }

    private void StepImm(ref readonly CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                state.Y = _bus[state.Pc++];
                EndInstr(in state);
                return;
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
                state.Y = _bus[_arg1];
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
                state.Y = _bus[addr];
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
                state.Y = _bus[addr];
                EndInstr(in state);
                return;
        }
    }
    private void StepAbsX(ref readonly CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                _ads.StepAbsX1(_bus, in state);
                return;
            case 3:
                _ads.StepAbsX2(_bus, in state);
                return;
            case 4:
                if (_ads.GetAbsXAddr1(_bus, in state, out var addr))
                {
                    state.Y = _bus[addr];
                    EndInstr(in state);
                }
                return;
            case 5:
                var addr2 = _ads.GetAbsXAddr2();
                state.Y = _bus[addr2];
                EndInstr(in state);
                return;
        }
    }
    protected override AddressingType GetAddressingType(byte opCode)
    {
        return opCode switch
        {
            0xa0 => AddressingType.Immediate,
            0xa4 => AddressingType.ZeroPage,
            0xb4 => AddressingType.ZeroPageX,
            0xac => AddressingType.Absolute,
            0xbc => AddressingType.AbsoluteX,
            _ => throw new MissingInstrException(opCode)
        };
    }
    protected override void OnEndInstr(ref readonly CpuState state)
    {
        _flagSetter.SetZeroByNumber(in state, state.Y);
        _flagSetter.SetNegativeByNumber(in state, state.Y);
    }
}
