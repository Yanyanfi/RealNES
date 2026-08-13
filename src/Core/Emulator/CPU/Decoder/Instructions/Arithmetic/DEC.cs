using RealNES.Core.Emulator.CPU.Decoder.Exceptions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Enums;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Arithmetic;

internal sealed class DEC(InstructionServices services) : InstructionBase(services)
{
    public override IReadOnlyList<byte> OpCodes { get; } = [0xc6, 0xd6, 0xce, 0xde];
    private ushort _addr;
    private byte _data;

    public override void Process(ref readonly CpuState state)
    {
        switch (_addressingType)
        {
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
                _data = _bus[_addr];
                break;
            case 4:
                _bus[_addr] = _data;
                _data--;
                _flagSetter.SetZeroByNumber(in state, _data);
                _flagSetter.SetZeroByNumber(in state, _data);
                break;
            case 5:
                _bus[_addr] = _data;
                EndInstr(in state);
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
                _addr = _ads.GetZpXAddr(in state);
                _data = _bus[_addr];
                break;
            case 5:
                _bus[_addr] = _data;
                _data--;
                _flagSetter.SetZeroByNumber(in state, _data);
                _flagSetter.SetNegativeByNumber(in state, _data);
                break;
            case 6:
                _bus[_addr] = _data;
                EndInstr(in state);
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
                _addr = _ads.GetAbsAddr();
                _data = _bus[_addr];
                break;
            case 5:
                _bus[_addr] = _data;
                _data--;
                _flagSetter.SetZeroByNumber(in state, _data);
                _flagSetter.SetNegativeByNumber(in state, _data);
                break;
            case 6:
                _bus[_addr] = _data;
                EndInstr(in state);
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
                if (_ads.GetAbsXAddr1(_bus, in state, out _addr))
                    _bus.Read(_addr);
                break;
            case 5:
                _addr = _ads.GetAbsXAddr2();
                _data = _bus[_addr];
                break;
            case 6:
                _bus[_addr] = _data;
                _data--;
                _flagSetter.SetZeroByNumber(in state, _data);
                _flagSetter.SetNegativeByNumber(in state, _data);
                break;
            case 7:
                _bus[_addr] = _data;
                EndInstr(in state);
                break;
        }
    }

    protected override AddressingType GetAddressingType(byte opCode)
    {
        return opCode switch
        {
            0xc6 => AddressingType.ZeroPage,
            0xd6 => AddressingType.ZeroPageX,
            0xce => AddressingType.Absolute,
            0xde => AddressingType.AbsoluteX,
            _ => throw new MissingInstrException(opCode)
        };
    }
}
