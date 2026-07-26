using RealNES.Core.Emulator.Communication;
using RealNES.Core.Emulator.CPU.Decoder.Exceptions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;

namespace RealNES.Core.Emulator.CPU.Decoder;

internal sealed class Decoder : IDecoder
{
    private readonly Dictionary<byte, IInstruction> _instructionTable = [];
    private readonly CpuBus _cpuBus;
    private IInstruction? _currentInstr;
    ///<exception cref="RepeatOpCodeException"/>
    public Decoder(IEnumerable<IInstruction> instructions,CpuBus cpuBus)
    {
        foreach(var instr in instructions)
        {
            var codes = instr.OpCodes;
            foreach(var code in codes)
            {
                if (_instructionTable.TryGetValue(code, out var repeatInstr))
                    throw new RepeatOpCodeException(code, repeatInstr.GetType().Name, instr.GetType().Name);
                _instructionTable[code] = instr;
            }
        }
        _cpuBus = cpuBus;
    }
    ///<exception cref="MissingInstrException"/>
    public void Step(ref readonly CpuState state)
    {
        var code = _cpuBus[state.Pc];
        if (state.Cycle == 0)
        {
#if DEBUG
            if (!_instructionTable.ContainsKey(code))
                throw new MissingInstrException(code);
#endif
            _currentInstr = _instructionTable[code];
        }
        _currentInstr?.Step(in state);
    }
}
