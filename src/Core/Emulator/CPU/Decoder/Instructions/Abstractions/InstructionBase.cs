using RealNES.Core.Emulator.Communication;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Resources;
using System.Text;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;

internal abstract class InstructionBase(InsructionServices services) : IInstruction
{
    protected AddressingType _addressingType;
    protected readonly CpuBus _bus = services.Bus;
    protected readonly FlagSetter _flagSetter = services.FlagSetter;
    protected readonly AddressingService _addrService = services.AddressingService;

    public abstract IReadOnlyList<byte> OpCodes { get; }
    /// <summary>
    /// Cycle start from 2
    /// </summary>
    /// <param name="state"></param>
    /// <param name="code"></param>
    public abstract void Process(ref readonly CpuState state);
    public void Step(ref readonly CpuState state)
    {
        if(state.Cycle == 1)
        {
            var opCode = _bus[state.Pc];
            _addressingType = GetAddressingType(opCode);
            state.Pc++;
            return;
        }
        Process(in state);
    }
    protected abstract AddressingType GetAddressingType(byte opCode);
    protected void EndInstr(ref readonly CpuState state)
    {
        OnEndInstr(in state);
        state.Cycle = 0;
    }

    protected virtual void OnEndInstr(ref readonly CpuState state) { }
}
record InsructionServices(CpuBus Bus, FlagSetter FlagSetter, AddressingService AddressingService);