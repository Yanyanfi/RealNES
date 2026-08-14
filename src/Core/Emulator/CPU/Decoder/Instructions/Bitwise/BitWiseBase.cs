using RealNES.Core.Emulator.CPU.Decoder.Exceptions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Abstractions;
using RealNES.Core.Emulator.CPU.Decoder.Instructions.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Bitwise;

internal abstract class BitWiseBase(InstructionServices services) : InstructionBase(services)
{
    private ushort _addr;
    public override void Process(ref readonly CpuState state)
    {
        throw new NotImplementedException();
    }
    private void StepImm(in CpuState state)
    {
        switch (state.Cycle)
        {
            case 2:
                var data = _bus[state.Pc++];
                ProcessData(in state, data);
                _flagSetter.SetZeroByNumber(in state, state.A);
                _flagSetter.SetNegativeByNumber(in state, state.A);
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
                ProcessData(in state, data);
                _flagSetter.SetZeroByNumber(in state, state.A);
                _flagSetter.SetNegativeByNumber(in state, state.A);
                EndInstr(in state);
                break;
        }
    }
    private void StepZpX(in CpuState state)
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
                var data = _bus[addr];
                ProcessData(in state, data);
                _flagSetter.SetZeroByNumber(in state, state.A);
                _flagSetter.SetNegativeByNumber(in state, state.A);
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
                ProcessData(in state, data);
                _flagSetter.SetZeroByNumber(in state, state.A);
                _flagSetter.SetNegativeByNumber(in state, state.A);
                EndInstr(in state);
                break;
        }
    }
    private void StepAbsX(in CpuState state)
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
                {
                    var data = _bus[addr];
                    ProcessData(in state, data);
                    _flagSetter.SetZeroByNumber(in state, state.A);
                    _flagSetter.SetNegativeByNumber(in state, state.A);
                    EndInstr(in state);
                }
                break;
            case 5:
                addr = _ads.GetAbsXAddr2();
                ProcessData(in state, _bus[addr]);
                _flagSetter.SetZeroByNumber(in state, state.A);
                _flagSetter.SetNegativeByNumber(in state, state.A);
                EndInstr(in state);
                break;
        }
    }
    private void StepAbsY(in CpuState state)
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
                {
                    var data = _bus[addr];
                    ProcessData(in state, data);
                    _flagSetter.SetZeroByNumber(in state, state.A);
                    _flagSetter.SetNegativeByNumber(in state, state.A);
                    EndInstr(in state);
                }
                break;
            case 5:
                addr = _ads.GetAbsYAddr2();
                ProcessData(in state, _bus[addr]);
                _flagSetter.SetZeroByNumber(in state, state.A);
                _flagSetter.SetNegativeByNumber(in state, state.A);
                EndInstr(in state);
                break;
        }
    }
    private void StepIndX(in CpuState state)
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
                var data = _bus[addr];
                ProcessData(in state, data);
                _flagSetter.SetZeroByNumber(in state, state.A);
                _flagSetter.SetNegativeByNumber(in state, state.A);
                EndInstr(in state);
                break;
        }
    }
    private void StepIndY(in CpuState state)
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
                if(_ads.GetIndirectYAddr1(_bus,in state,out var addr))
                {
                    var data = _bus[addr];
                    ProcessData(in state, data);
                    _flagSetter.SetZeroByNumber(in state, state.A);
                    _flagSetter.SetNegativeByNumber(in state, state.A);
                    EndInstr(in state);
                }
                break;
            case 6:
                addr = _ads.GetIndirectYAddr2();
                ProcessData(in state, _bus[addr]);
                _flagSetter.SetZeroByNumber(in state, state.A);
                _flagSetter.SetNegativeByNumber(in state, state.A);
                EndInstr(in state);
                break;
        }
    }
    protected abstract void ProcessData(in CpuState state, byte data);
}
