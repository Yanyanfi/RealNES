using BenchmarkDotNet.Attributes;
using Iced.Intel;
using RealNES.Core.Emulator;
using RealNES.Core.Emulator.Communication;
using RealNES.Core.Emulator.CPU;
using RealNES.Core.Emulator.Memory;
namespace RealNES.CoreBenchmarks.CpuBenchmark;

[MemoryDiagnoser(false)]
public class CpuBenchmark
{
    private readonly Cpu _cpu;
    public CpuBenchmark()
    {
        var ram = new InternalRam();
        ram.Load(@"C:\Users\wanya\Downloads\Access_JMP_loop1.bin");
        var bus = new CpuBus(ram);
        var cpuFactory = new CpuFactory(bus);
        _cpu = cpuFactory.Create();
        _cpu.Reset();
    }
    [Benchmark]
    public void StepCpu()
    {
        _cpu.Step();
    }
}
