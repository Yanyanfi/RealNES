using RealNES.Core.Emulator.CPU.Decoder.Exceptions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Enums;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Access;

internal sealed class LDX(InstructionServices services) : InstructionBase(services)
{
    private byte _arg1;
    public override IReadOnlyList<byte> OpCodes { get; } = [0xa2, 0xa6, 0xb6, 0xae, 0xbe];

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
            case AddressingType.ZeroPageY:
                StepZpY(in state);
                break;
            case AddressingType.Absolute:
                StepAbs(in state);
                break;
            case AddressingType.AbsoluteY:
                StepAbsY(in state);
                break;
        }
    }

    private void StepImm(ref readonly CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                state.X = _bus[state.Pc++];
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
                state.X = _bus[_arg1];
                EndInstr(in state);
                return;
        }
    }
    private void StepZpY(ref readonly CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                _ads.StepZpY1(_bus, in state);
                return;
            case 3:
                _ads.StepZpY2(_bus);
                return;
            case 4:
                var addr = _ads.GetZpYAddr(in state);
                state.X = _bus[addr];
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
                state.X = _bus[addr];
                EndInstr(in state);
                return;
        }
    }
    private void StepAbsY(ref readonly CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                _ads.StepAbsY1(_bus, in state);
                return;
            case 3:
                _ads.StepAbsY2(_bus, in state);
                return;
            case 4:
                if (_ads.GetAbsYAddr1(_bus, in state, out var addr))
                {
                    state.X = _bus[addr];
                    EndInstr(in state);
                }
                return;
            case 5:
                var addr2 = _ads.GetAbsYAddr2();
                state.X = _bus[addr2];
                EndInstr(in state);
                return;
        }
    }
    protected override AddressingType GetAddressingType(byte opCode)
    {
        return opCode switch
        {
            0xa2 => AddressingType.Immediate,
            0xa6 => AddressingType.ZeroPage,
            0xb6 => AddressingType.ZeroPageY,
            0xae => AddressingType.Absolute,
            0xbe => AddressingType.AbsoluteY,
            _ => throw new MissingInstrException(opCode)
        };
    }
    protected override void OnEndInstr(ref readonly CpuState state)
    {
        _flagSetter.SetZeroByNumber(in state, state.X);
        _flagSetter.SetNegativeByNumber(in state, state.X);
    }
}
