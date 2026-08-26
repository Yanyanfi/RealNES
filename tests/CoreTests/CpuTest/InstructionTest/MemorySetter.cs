using RealNES.Core.Emulator.Bus;

namespace RealNES.CoreTests.CpuTest.InstructionTest;

internal class MemorySetter(CpuBus bus)
{
    public void SetMemory(InstrTestCase.State state)
    {
        foreach (var item in state.Ram)
        {
            bus[item[0]] = (byte)item[1];
        }
    }
}
