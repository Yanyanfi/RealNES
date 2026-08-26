using RealNES.Core.Emulator.Bus;
using RealNES.Core.Emulator.CPU.Decoder.Exceptions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;

namespace RealNES.Core.Emulator.CPU.Decoder;

internal sealed class Decoder : IDecoder
{
    private readonly IInstruction[] _instructionTable = new IInstruction[256];
    private readonly CpuBus _cpuBus;
    private IInstruction? _currentInstr;
    ///<exception cref="RepeatOpCodeException"/>
    public Decoder(IEnumerable<IInstruction> instructions, CpuBus cpuBus)
    {
        foreach (var instr in instructions)
        {
            var codes = instr.OpCodes;
            foreach (var code in codes)
            {
                if (_instructionTable[code] is not null)
                    throw new RepeatOpCodeException(code, _instructionTable[code].GetType().Name, instr.GetType().Name);
                _instructionTable[code] = instr;
            }
        }
        _cpuBus = cpuBus;
    }
    ///<exception cref="MissingInstrException"/>
    public void Step(ref readonly CpuState state)
    {
        if (state.Cycle == 0)
        {
            var code = _cpuBus[state.Pc];
#if DEBUG
            if (_instructionTable[code] is null)
                throw new MissingInstrException(code);
#endif
            _currentInstr = _instructionTable[code];
        }
        state.Cycle++;
        _currentInstr?.Step(in state);
    }
}
