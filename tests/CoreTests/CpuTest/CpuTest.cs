using RealNES.Core.Emulator.Communication;
using RealNES.Core.Emulator.CPU;
using RealNES.Core.Emulator.Memory;

namespace RealNES.CoreTests.CpuTest;

public class CpuTest
{
    private readonly Cpu _cpu;
    private readonly CpuStateProvider _stateProvider;
    private readonly InstrInfoProvider _instrInfoProvider;
    private readonly InternalRam _ram;
    private readonly CpuBus _bus;
    private readonly StateLogger _logger;
    public CpuTest()
    {
        var ram = new InternalRam();
        _bus = new CpuBus(ram);
        var cpuFactory = new CpuFactory(_bus);
        _cpu = cpuFactory.Create();
        _stateProvider = new(_cpu);
        _instrInfoProvider = new(_cpu);
        _ram = ram;
        _logger = new(_stateProvider, _instrInfoProvider, _bus, @"C:\Users\wanya\Desktop\6502.csv");
    }
    [Fact]
    public void Test1()
    {
        _cpu.Reset();
        _ram.Load(@"C:\Users\wanya\Downloads\Access_JMP_loop1.bin");
        while (_instrInfoProvider.GetInstrName() != "JMP")
        {
            StepInstr();
        }
    }
    private void StepInstr()
    {
        do
        {
            _cpu.Step();
            _logger.LogLine();
            _bus.ClearIOState();
        }
        while (_stateProvider.GetState().InstrCycle != 0);
    }
}
