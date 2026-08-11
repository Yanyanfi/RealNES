using RealNES.Core.Emulator.Communication;
using RealNES.Core.Emulator.CPU.Decoder.Instructions;

namespace RealNES.Core.Emulator.CPU.Decoder;

internal sealed class DecoderFactory(CpuBus bus)
{
    private readonly CpuBus _bus = bus;
    public IDecoder Create()
    {
        var instrProvider = new InstructionsProvider(_bus);
        var instructions = instrProvider.GetInstructions();
        var decoder = new Decoder(instructions, _bus);
        return decoder;
    }
}
