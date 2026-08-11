using RealNES.Core.Emulator.Communication;
using System.Text;

namespace RealNES.CoreTests.CpuTest;

internal sealed class StateLogger(CpuStateProvider cpuStateProvider, InstrInfoProvider instrInfoProvider, CpuBus bus, string filePath) : IDisposable
{
    private readonly StreamWriter _writer = new(filePath, false);
    private static readonly string _head = "cycle,ab,db,r/w,fetch,pc,a,x,y,s,p";
    private bool _isFirstLine = true;
    private long _cycle;

    public void LogLine()
    {
        if (_isFirstLine)
        {
            _isFirstLine = false;
            _writer.WriteLine(_head);
        }
        _writer.WriteLine(GetLine());
        _writer.Flush();
        _cycle++;
    }
    private string GetLine()
    {
        var cpuState = cpuStateProvider.GetState();
        var instrName = instrInfoProvider.GetInstrName();
        var addrName = instrInfoProvider.GetAddressingName();
        var sb = new StringBuilder();
        sb.Append(_cycle); sb.Append(',');
        sb.Append(bus.LastAddress); sb.Append(',');
        sb.Append(bus.LastData); sb.Append(',');
        var io = (bus.IsReadOrWriteLastCycle, bus.IsWrite) switch
        {
            (false, _) => 'N',
            (_, true) => 'w',
            _ => 'r'
        };
        sb.Append(io); sb.Append(',');
        sb.Append($"{instrName} {addrName}"); sb.Append(',');
        sb.Append(cpuState.Pc); sb.Append(',');
        sb.Append(cpuState.A); sb.Append(',');
        sb.Append(cpuState.X); sb.Append(',');
        sb.Append(cpuState.Y); sb.Append(',');
        sb.Append(cpuState.Sp); sb.Append(',');
        sb.Append(cpuState.P);
        return sb.ToString();
    }
    public void Dispose() => _writer.Dispose();

}
