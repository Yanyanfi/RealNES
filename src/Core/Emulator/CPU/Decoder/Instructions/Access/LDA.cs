using RealNES.Core.Emulator.CPU.Decoder.Exceptions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Enums;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Access;

internal sealed class LDA(InstructionServices services) : InstructionBase(services)
{
    public override IReadOnlyList<byte> OpCodes { get; } = [0xa9, 0xa5, 0xb5, 0xad, 0xbd, 0xb9, 0xa1, 0xb1];

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
                StepIndirectX(in state);
                break;
            case AddressingType.IndirectIndexed:
                StepIndirectY(in state);
                break;
        }
    }
    private byte _arg1;
    private byte _arg2;
    private ushort _addr;
    private byte _peekArg1;

    private void StepImm(ref readonly CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                state.A = _bus[state.Pc];
                state.Pc++;
                EndInstr(in state);
                break;
        }
    }
    private void StepZp(ref readonly CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                _arg1 = _bus[state.Pc];
                state.Pc++;
                return;
            case 3:
                state.A = _bus[_arg1];
                EndInstr(in state);
                return;
        }
    }
    private void StepZpX(ref readonly CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                _arg1 = _bus[state.Pc];
                state.Pc++;
                return;
            case 3:
                _peekArg1 = _bus[_arg1];
                return;
            case 4:
                state.A = _bus[(_peekArg1 + state.X) & 0xff];
                EndInstr(in state);
                return;
        }
    }
    private void StepAbs(ref readonly CpuState state)
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
                var addr = _arg2 * 256 + _arg1;
                state.A = _bus[addr];
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
                var baseAddr = _arg1 + state.X;
                _addr = (ushort)(baseAddr + _arg2 * 256);
                if (baseAddr <= 255)
                {
                    state.A = _bus[_addr];
                    EndInstr(in state);
                    return;
                }
                _bus.Read(0);
                return;
            case 5:
                state.A = _bus[_addr];
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
                var baseAddr = _arg1 + state.Y;
                _addr = (ushort)(baseAddr + _arg2 * 256);
                if (baseAddr <= 255)
                {
                    state.A = _bus[_addr];
                    EndInstr(in state);
                    return;
                }
                _bus.Read(0);
                return;
            case 5:
                state.A = _bus[_addr];
                EndInstr(in state);
                return;
        }
    }
    private void StepIndirectX(ref readonly CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                _arg1 = _bus[state.Pc++];
                return;
            case 3:
                _bus.Read(_arg1);
                return;
            case 4:
                _addr = _bus[(_arg1 + state.X) % 256];
                return;
            case 5:
                _addr += _bus[(_arg1 + state.X + 1) % 256];
                return;
            case 6:
                state.A = _bus[_addr];
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
                _addr = _bus[_arg1];
                return;
            case 4:
                _addr += (ushort)(_bus[(_arg1 + 1) % 256] * 256);
                return;
            case 5:
                var lowAddr = _addr & 0xff;
                if (lowAddr + state.Y < 256)
                {
                    state.A = _bus[_addr + state.Y];
                    EndInstr(in state);
                    return;
                }
                _addr = (ushort)(_addr + state.Y - 256);
                _bus.Read(_addr);
                return;
            case 6:
                state.A = _bus[_addr + 256];
                EndInstr(in state);
                return;
        }
    }
    protected override AddressingType GetAddressingType(byte opCode) => opCode switch
    {
        0xa9 => AddressingType.Immediate,
        0xa5 => AddressingType.ZeroPage,
        0xb5 => AddressingType.ZeroPageX,
        0xad => AddressingType.Absolute,
        0xbd => AddressingType.AbsoluteX,
        0xb9 => AddressingType.AbsoluteY,
        0xa1 => AddressingType.IndexedIndirect,
        0xb1 => AddressingType.IndirectIndexed,
        _ => throw new MissingInstrException(opCode)
    };
    protected override void OnEndInstr(ref readonly CpuState state)
    {
        _flagSetter.SetZero(in state, state.A == 0);
        _flagSetter.SetNegativeByNumber(in state, state.A);
    }
}
