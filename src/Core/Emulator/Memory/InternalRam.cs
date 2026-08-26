namespace RealNES.Core.Emulator.Memory;

internal sealed class InternalRam
{
    private readonly byte[] _memory = new byte[0x10000];
    public byte Read(ushort address) => _memory[address];

    public void Write(ushort address, byte value) => _memory[address] = value;

    public byte this[ushort index]
    {
        get => Read(index);
        set => Write(index, value);
    }
    public void Load(string path) => File.ReadAllBytes(path).CopyTo(_memory);

}
