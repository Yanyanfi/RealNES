using RealNES.Core.Emulator.Bus;
using RealNES.Core.Emulator.CPU;
using RealNES.Core.Emulator.Memory;
using RealNES.CoreTests.CpuTest.InstructionTest;
using System.Security.AccessControl;
using Xunit.Abstractions;

namespace RealNES.CoreTests.CpuTest;

public class CpuTest
{
    private static readonly string _instrTestRptPath = @"C:\Users\wanya\Desktop\instr_error.txt";
    private static readonly string _testCaseDirectory = @"C:\Users\wanya\Desktop\instrs_testcases";
    private static readonly string _logPath = @"C:\Users\wanya\Desktop\6502.csv";
    private static readonly string _programPath = @"C:\Users\wanya\Downloads\all_instr.bin";
    private readonly Cpu _cpu;
    private readonly CpuStateProvider _stateProvider;
    private readonly InstrInfoProvider _instrInfoProvider;
    private readonly InternalRam _ram;
    private readonly CpuBus _bus;
    private readonly StateLogger _logger;
    private readonly OpCodesProvider _codeProvider;
    private readonly InstrTestCasesRunner _runner;
    public CpuTest()
    {
        var ram = new InternalRam();
        var caseProvider = new InstrTestCaseProvider(_testCaseDirectory);
        _bus = new CpuBus(ram);
        var cpuFactory = new CpuFactory(_bus);
        _cpu = cpuFactory.Create();
        _stateProvider = new(_cpu);
        _instrInfoProvider = new(_cpu);
        _ram = ram;
        _logger = new(_stateProvider, _instrInfoProvider, _bus, _logPath);
        var cpuSetter = new CpuStateSetter(_cpu);
        var memSetter = new MemorySetter(_bus);
        _runner = new(caseProvider, _stateProvider, _instrInfoProvider, cpuSetter, memSetter, _cpu, _bus, _instrTestRptPath);
        _codeProvider = new(_cpu);
    }
    [Fact]
    public void Test1()
    {
        _cpu.Reset();
        _ram.Load(_programPath);
        while (_instrInfoProvider.GetInstrName() != "JMP")
        {
            StepInstr();
        }
    }
    [Fact]
    public void InstrTest()
    {
        var codes = _codeProvider.GetOpCodes();
        var errorCount = 0;
        foreach (var code in codes)
        {
            errorCount += _runner.Run(code) ? 0 : 1;
        }
        Assert.Equal(0,errorCount);
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
