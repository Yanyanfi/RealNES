using RealNES.Core.Emulator.CPU;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace RealNES.CoreTests.CpuTest.InstructionTest;

internal sealed class CpuStateSetter(Cpu cpu)
{
    public void SetState(InstrTestCase.State state)
    {
        var cpuInfo = typeof(Cpu).GetTypeInfo();
        cpuInfo.GetDeclaredField("_pc")!.SetValue(cpu, state.Pc);
        cpuInfo.GetDeclaredField("_a")!.SetValue(cpu, state.A);
        cpuInfo.GetDeclaredField("_x")!.SetValue(cpu, state.X);
        cpuInfo.GetDeclaredField("_y")!.SetValue(cpu, state.Y);
        cpuInfo.GetDeclaredField("_sp")!.SetValue(cpu, state.S);
        cpuInfo.GetDeclaredField("_p")!.SetValue(cpu, state.P);
        cpuInfo.GetDeclaredField("_cycle")!.SetValue(cpu, 0);
    }
}
