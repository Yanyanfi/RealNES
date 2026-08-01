using RealNES.Core.Emulator.CPU.Decoder.Exceptions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Access;

internal sealed class STA(InstructionServices services) : InstructionBase(services)
{
    public override IReadOnlyList<byte> OpCodes { get; } = [0x85, 0x95, 0x8d, 0x9d, 0x99, 0x81, 0x91];
    private ushort _addr;
    private byte _arg1;
    private byte _arg2;
    private byte _low;
    private byte _high;
    public override void Process(ref readonly CpuState state)
    {
        switch (_addressingType)
        {
            case AddressingType.ZeroPage:
                StepZp(in state);
                return;
            case AddressingType.ZeroPageX:
                StepZpX(in state);
                return;
            case AddressingType.Absolute:
                StepAbs(in state);
                return;
            case AddressingType.AbsoluteX:
                StepAbsX(in state);
                return;
            case AddressingType.AbsoluteY:
                StepAbsY(in state);
                return;
            case AddressingType.IndexedIndirect:
                StepIndirectX(in state);
                return;
            case AddressingType.IndirectIndexed:
                StepIndirectY(in state);
                return;
        }
    }
    private void StepZp(ref readonly CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                _addr = _bus[state.Pc++];
                return;
            case 3:
                _bus[_addr] = state.A;
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
                _bus[addr] = state.A;
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
                _bus[_ads.GetAbsAddr()] = state.A;
                EndInstr(in state);
                return;
        }
    }
    private void StepAbsX(ref readonly CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                _arg1 = _bus[state.Pc++];
                return;
            case 3:
                _arg2 = _bus[state.Pc++];
                return;
            case 4:
                var tempAddr = ((_arg1 + state.X) & 0xff) + _arg2 * 256;
                _bus.Read((ushort)tempAddr);
                return;
            case 5:
                var addr = (_arg1 + _arg2 * 256 + state.X) & 0xffff;
                _bus[addr] = state.A;
                EndInstr(in state);
                return;
        }
    }
    private void StepAbsY(ref readonly CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                _arg1 = _bus[state.Pc++];
                return;
            case 3:
                _arg2 = _bus[state.Pc++];
                return;
            case 4:
                var tempAddr = ((_arg1 + state.Y) & 0xff) + _arg2 * 256;
                _bus.Read((ushort)tempAddr);
                return;
            case 5:
                var addr = (_arg1 + _arg2 * 256 + state.Y) & 0xffff;
                _bus[addr] = state.A;
                EndInstr(in state);
                return;
        }
    }
    private void StepIndirectX(ref readonly CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                _ads.StepIndirectX1(_bus, in state);
                return;
            case 3:
                _ads.StepIndirectX2(_bus);
                return;
            case 4:
                _ads.StepIndirectX3(_bus, in state);
                return;
            case 5:
                _ads.StepIndirectX4(_bus, in state);
                return;
            case 6:
                var addr = _ads.GetIndirectXAddr();
                _bus[addr] = state.A;
                EndInstr(in state);
                return;
        }
    }
    private void StepIndirectY(ref readonly CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                _arg1 = _bus[state.Pc++];
                return;
            case 3:
                _low = _bus[_arg1];
                return;
            case 4:
                _high = _bus[(_arg1 + 1) % 256];
                return;
            case 5:
                var tempLow = (_low + state.Y) & 0xff;
                var tempAddr = tempLow + _high * 256;
                _bus.Read((ushort)tempAddr);
                return;
            case 6:
                var addr = (_low + state.Y + _high * 256) & 0xffff;
                _bus[addr] = state.A;
                EndInstr(in state);
                return;
        }
    }
    protected override AddressingType GetAddressingType(byte opCode) => opCode switch
    {
        0x85 => AddressingType.ZeroPage,
        0x95 => AddressingType.ZeroPageX,
        0x8d => AddressingType.Absolute,
        0x9d => AddressingType.AbsoluteX,
        0x99 => AddressingType.AbsoluteY,
        0x81 => AddressingType.IndexedIndirect,
        0x91 => AddressingType.IndirectIndexed,
        _ => throw new MissingInstrException(opCode)
    };
}
