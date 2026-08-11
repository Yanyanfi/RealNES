using RealNES.Core.Emulator.CPU;
using RealNES.CoreTests.CpuTest.Exceptions;
using System.Reflection;
namespace RealNES.CoreTests.CpuTest;

internal class CpuStateProvider(Cpu cpu)
{
    private static readonly TypeInfo _typeInfo = typeof(Cpu).GetTypeInfo();
    public State GetState()
    {
        var pc = GetValue<ushort>("_pc");
        var a = GetValue<byte>("_a");
        var x = GetValue<byte>("_x");
        var y = GetValue<byte>("_y");
        var sp = GetValue<byte>("_sp");
        var p = GetValue<byte>("_p");
        var instrCycle = GetValue<int>("_cycle");
        return new(pc, a, x, y, sp, p, instrCycle);
    }
    private TValue GetValue<TValue>(string fieldName) where TValue : struct =>
        (TValue)(_typeInfo.GetDeclaredField(fieldName)?.GetValue(cpu) ?? throw new FieldNotFoundException(fieldName));
}
