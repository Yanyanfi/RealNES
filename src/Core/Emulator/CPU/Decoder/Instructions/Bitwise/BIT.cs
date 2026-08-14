using RealNES.Core.Emulator.CPU.Decoder.Exceptions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Bitwise;

internal sealed class BIT(InstructionServices services) : InstructionBase(services)
{
    public override IReadOnlyList<byte> OpCodes { get; } = [0x24, 0x2c];
    private ushort _addr;

    public override void Process(ref readonly CpuState state)
    {
        switch (_addressingType)
        {
            case AddressingType.ZeroPage:
                StepZp(in state);
                break;
            case AddressingType.Absolute:
                StepAbs(in state);
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
                var data = _bus[_addr];
                _flagSetter.SetOverflow(in state, (data & 0b1000000) != 0);
                _flagSetter.SetNegativeByNumber(in state, data);
                var result = (byte)(data & state.A);
                _flagSetter.SetZeroByNumber(in state, result);
                EndInstr(in state);
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
                var data = _bus[addr];
                _flagSetter.SetOverflow(in state, (data & 0b1000000) != 0);
                _flagSetter.SetNegativeByNumber(in state, data);
                var result = (byte)(data & state.A);
                _flagSetter.SetZeroByNumber(in state, result);
                EndInstr(in state);
                break;
        }
    }

    protected override AddressingType GetAddressingType(byte opCode)
    {
        return opCode switch
        {
            0x24 => AddressingType.ZeroPage,
            0x2c => AddressingType.Absolute,
            _ => throw new MissingInstrException(opCode)
        };
    }
}
