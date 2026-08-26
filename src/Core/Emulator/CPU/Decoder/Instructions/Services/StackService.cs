using RealNES.Core.Emulator.Bus;

namespace RealNES.Core.Emulator.CPU.Decoder.Instructions.Services;

internal sealed class StackService(CpuBus bus)
{
    private readonly CpuBus _bus = bus;
    public void Push(ref readonly CpuState state, byte value) => _bus[0x100 + state.Sp--] = value;
    public byte Pull(ref readonly CpuState state) => _bus[++state.Sp + 0x100];
    public ushort GetTopAddress(ref readonly CpuState state) => (ushort)(state.Sp + 0x100);
}