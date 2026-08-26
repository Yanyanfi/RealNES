using RealNES.Core.Emulator.Bus;
using RealNES.Core.Emulator.CPU;
using System.Text;
using Cycle = RealNES.CoreTests.CpuTest.InstructionTest.InstrTestCase.Cycle;
namespace RealNES.CoreTests.CpuTest.InstructionTest;

internal class InstrTestCasesRunner(
    InstrTestCaseProvider caseProvider,
    CpuStateProvider stateProvider,
    InstrInfoProvider infoProvider,
    CpuStateSetter cpuSetter,
    MemorySetter memSetter,
    Cpu cpu,
    CpuBus bus,
    string rptPath) : IDisposable
{
    private readonly StreamWriter _writer = new(rptPath, false);
    public bool Run(byte opCode)
    {
        var cases = caseProvider.GetInstrTestCases(opCode);
        foreach (var c in cases)
        {
            cpuSetter.SetState(c.Initial);
            memSetter.SetMemory(c.Initial);
            List<Cycle> realCycles = [];
            State state;
            do
            {
                bus.ClearIOState();
                cpu.Step();
                var cycle = GetCycleIo();
                realCycles.Add(cycle);
                state = stateProvider.GetState();
            }
            while (state.InstrCycle != 0&&state.InstrCycle<20);
            if (!Analyze(c, realCycles, state))
                return false;
        }
        return true;
    }
    private bool Analyze(InstrTestCase testCase, List<Cycle> realCycles, State finalState)
    {
        var name = infoProvider.GetInstrName();
        var addrMode = infoProvider.GetAddressingName();
        var a = finalState.A;
        var pc = finalState.Pc;
        var x = finalState.X;
        var y = finalState.Y;
        var sp = finalState.Sp;
        var p = finalState.P;
        var exp = testCase.Final;
        var init = testCase.Initial;
        if (HasError())
        {
            PrintInfo();
            return false;
        }
        return true;
        bool HasError()
        {
            var regErr = (pc, sp, p, a, x, y) != (exp.Pc, exp.S, exp.P, exp.A, exp.X, exp.Y);
            var cycleCountErr = testCase.Cycles.Count != realCycles.Count;
            var ioErr = testCase.Cycles.Select((c, i) => (c, i)).Any(e => e.c != realCycles[e.i]);
            return regErr || cycleCountErr || ioErr;
        }
        void PrintInfo()
        {
            _writer.WriteLine("----------------------------------");
            _writer.WriteLine($"Instruction: {name} {addrMode}");
            IEnumerable<string> regHeads = ["", "PC", "SP", "P", "A", "X", "Y"];
            IEnumerable<object> initRegLine = ["init", init.Pc, init.S, init.P,init.A, init.X, init.Y];
            object[] realRegLine = ["real", pc, sp, p, a, x, y];
            object[] expectedRegLine = ["exp", exp.Pc, exp.S, exp.P, exp.A, exp.X, exp.Y];
            IEnumerable<string> regCmpLine = Enumerable.Range(0, 7).Select(i =>
            {
                return (realRegLine[i], expectedRegLine[i]) switch
                {
                    (string a, string b) => "",
                    (var a, var b) => a.Equals(b) ? "PASS" : "FAIL"
                };
            });
            PrintLine(regHeads);
            PrintLine(initRegLine);
            PrintLine(expectedRegLine);
            PrintLine(realRegLine);
            PrintLine(regCmpLine);
            _writer.WriteLine("--------");
            if(testCase.Cycles.Count != realCycles.Count)
            {
                _writer.WriteLine("CYCLE COUNT ERROR!");
                _writer.WriteLine($"EXPTECT: {testCase.Cycles.Count}");
                _writer.WriteLine($"REAL:    {realCycles.Count}");
                _writer.Flush();
                return;
            }
            string[] busHead = ["cycle", "exp_a", "real_a", "exp_d", "real_d", "exp_m", "real_m"];
            PrintLine(busHead);
            var expCycles = testCase.Cycles;
            for (int i = 0; i < expCycles.Count; i++)
            {
                var exp = expCycles[i];
                var real = realCycles[i];
                var result = exp==real?"PASS":"FAIL";
                object[] line = [i+1, exp.AddrBus, real.AddrBus, exp.DataBus, real.DataBus, exp.Mode, real.Mode,result];
                PrintLine(line);
            }
            _writer.Flush();
            return;
            
        }
        void PrintLine<T>(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                _writer.Write(GetGrid(item));
            }
            _writer.WriteLine();
        }
        string GetGrid(object? item)
        {
            var str = item switch
            {
                byte v => v.ToString("X2"),
                ushort v => v.ToString("X4"),
                _ => item?.ToString() ?? "null"
            };
            return str.PadRight(8);
        }
    }
    private Cycle GetCycleIo()
    {
        var mode = (bus.IsRead, bus.IsWrite) switch
        {
            (true, false) => "read",
            (false, true) => "write",
            _ => "error"
        };
        return new(bus.LastAddress, bus.LastData, mode);
    }

    public void Dispose() => _writer.Dispose();
}
