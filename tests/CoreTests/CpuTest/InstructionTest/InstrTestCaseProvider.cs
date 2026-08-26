using System.Text.Json;

namespace RealNES.CoreTests.CpuTest.InstructionTest;

internal sealed class InstrTestCaseProvider(string directory)
{
    ///<exception cref="JsonException"/>
    public IReadOnlyList<InstrTestCase> GetInstrTestCases(byte opCode)
    {
        var name = opCode.ToString("x2") + ".json";
        var path = Path.Join(directory, name);
        using var fs = File.OpenRead(path);
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        return JsonSerializer.Deserialize<List<InstrTestCase>>(fs,options) ?? throw new JsonException();
    }
}
