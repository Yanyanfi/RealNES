namespace RealNES.CoreTests.CpuTest.Exceptions;

internal class FieldNotFoundException(string fieldName) : Exception($"field {fieldName} not found")
{
    public string FieldName => fieldName;
}
