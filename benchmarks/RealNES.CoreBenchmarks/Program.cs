using BenchmarkDotNet.Running;
using Microsoft.Diagnostics.Tracing.Parsers.FrameworkEventSource;
using RealNES.CoreBenchmarks.CpuBenchmark;

BenchmarkRunner.Run<CpuBenchmark>();
//var ben = new CpuBenchmark();
//while (true)
//{
//    ben.StepCpu();
//}