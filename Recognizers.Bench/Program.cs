using System;
using BenchmarkDotNet;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace Recognizers.Bench
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            //ManualConfig.CreateEmpty()
            //    .AddJob(Job.Default.WithJit(BenchmarkDotNet.Environments.Jit.RyuJit)
            BenchmarkRunner.Run<Bench>();
        }
    }
}
