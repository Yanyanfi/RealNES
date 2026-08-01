using RealNES.Core.Emulator.Communication;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions;

internal sealed class AddressingService
{
    private byte _arg1;
    private byte _arg2;
    private byte _temp;
    private ushort _addr;
    public void StepZpX1(CpuBus bus, ref readonly CpuState state) => _arg1 = bus[state.Pc++];
    public void StepZpX2(CpuBus bus) => _temp = bus[_arg1];
    public byte GetZpXAddr(ref readonly CpuState state) => (byte)((_temp + state.X) & 0xff);
    public void StepZpY1(CpuBus bus, ref readonly CpuState state) => _arg1 = bus[state.Pc++];
    public void StepZpY2(CpuBus bus) => _temp = bus[_arg1];
    public byte GetZpYAddr(CpuBus bus, ref readonly CpuState state) => (byte)((_temp + state.Y) & 0xff);
    public void StepAbs1(CpuBus bus, ref readonly CpuState state) => _arg1 = bus[state.Pc++];
    public void StepAbs2(CpuBus bus, ref readonly CpuState state) => _arg2 = bus[state.Pc++];
    public ushort GetAbsAddr() => (ushort)(_arg2 * 256 + _arg1);
    public void StepAbsX1(CpuBus bus, ref readonly CpuState state) => _arg1 = bus[state.Pc++];
    public void StepAbsX2(CpuBus bus, ref readonly CpuState state) => _arg2 = bus[state.Pc++];
    public bool GetAbsXAddr1(CpuBus bus,ref readonly CpuState state,out ushort addr)
    {
        var baseAddr = _arg1 + state.X;
        _addr = (ushort)(baseAddr + _arg2 * 256);
        if (baseAddr <= 255)
        {
            addr = _addr;
            return true;
        }
        bus.Read(0);
        addr = default;
        return false;
    }
    public ushort GetAbsXAddr2() => _addr;
    public void StepAbsY1(CpuBus bus, ref readonly CpuState state) => _arg1 = bus[state.Pc++];
    public void StepAbsY2(CpuBus bus, ref readonly CpuState state) => _arg2 = bus[state.Pc++];
    public bool GetAbsYAddr1(CpuBus bus, ref readonly CpuState state, out ushort addr)
    {
        var baseAddr = _arg1 + state.Y;
        _addr = (ushort)(baseAddr + _arg2 * 256);
        if (baseAddr <= 255)
        {
            addr = _addr;
            return true;
        }
        bus.Read(0);
        addr = default;
        return false;
    }
    public ushort GetAbsYAddr2() => _addr;
    public void StepIndirectX1(CpuBus bus, ref readonly CpuState state) => _arg1 = bus[state.Pc++];
    public void StepIndirectX2(CpuBus bus) => bus.Read(_arg1);
    public void StepIndirectX3(CpuBus bus,ref readonly CpuState state) => _addr = bus[(_arg1 + state.X) % 256];
    public void StepIndirectX4(CpuBus bus, ref readonly CpuState state) => _addr += (ushort)(bus[(_arg1 + state.X + 1) % 256]*256);
    public ushort GetIndirectXAddr() => _addr;
    public void StepIndirectY1(CpuBus bus, ref readonly CpuState state) => _arg1 = bus[state.Pc++];
    public void StepIndirectY2(CpuBus bus) => _addr = bus[_arg1];
    public void StepIndirectY3(CpuBus bus) => _addr += (ushort)(bus[(_arg1 + 1) % 256] * 256);
    public bool GetIndirectYAddr1(CpuBus bus,ref readonly CpuState state,out ushort addr)
    {
        var lowAddr = _addr & 0xff;
        if (lowAddr + state.Y < 256)
        {
            addr = (ushort)(_addr + state.Y);
            return true;
        }
        addr = default;
        _addr = (ushort)(_addr + state.Y - 256);
        bus.Read(_addr);
        return false;
    }
    public ushort GetIndirectYAddr2() => (ushort)(_addr + 256);


}
