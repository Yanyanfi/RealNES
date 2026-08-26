using System.Text.Json;
using System.Text.Json.Serialization;

namespace RealNES.CoreTests.CpuTest.InstructionTest;

internal record class InstrTestCase
{
    required public string Name { get; init; }

    required public State Initial { get; init; }
    required public State Final { get; init; }
    required public List<Cycle> Cycles { get; init; }
    internal record struct State
    {
        public ushort Pc { get; init; }
        public byte S { get; init; }
        public byte A { get; init; }
        public byte X { get; init; }
        public byte Y { get; init; }
        public byte P { get; init; }
        required public List<ushort[]> Ram { get; init; }
    }
    [JsonConverter(typeof(CycleJsonConverter))]
    internal record struct Cycle(ushort AddrBus, byte DataBus, string Mode);
    private class CycleJsonConverter : JsonConverter<Cycle>
    {
        public override Cycle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.Read();
            var addr = reader.GetUInt16();
            reader.Read();
            var data = reader.GetByte();
            reader.Read();
            var mode = reader.GetString() ?? throw new JsonException();
            reader.Read();
            if (reader.TokenType != JsonTokenType.EndArray)
                throw new JsonException();
            return new(addr, data, mode);
        }

        public override void Write(Utf8JsonWriter writer, Cycle value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
