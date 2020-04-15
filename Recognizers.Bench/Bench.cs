using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Recognizers;

namespace Recognizers.Bench
{
    //[SimpleJob(RuntimeMoniker.Net472, baseline: true)]
    //[SimpleJob(RuntimeMoniker.NetCoreApp30)]
    [RPlotExporter]
    public class Bench
    {
        const string regex = "[\x020]*[+]?[\x020]*[0-9]*[\x020]*([(][0-9]+[)])?[\x020]*[/]?([0-9-\x020])+";
        static string[] phonenos = new[]
        {
            "555-555",
            "(555) 555-555",
            "555 555-555",
            "555-555-555",
            "555-555 555",
            "+1 555-555 555",
            "+1 (555) 555 555",
            "(089) / 636-48018",
            "+49-89-636-48018",
            "19-49-89-636-48018",
            "191 541 754 3010",
        };

        [Benchmark]
        public void BenchRecognizers()
        {
            for (int i = 0; i < phonenos.Length; ++i)
            {
                var pos = new Position();
                var input = new Input(phonenos[i]);
                input.PhoneNumber(ref pos);
            }
        }

        [Benchmark]
        public void BenchRegex()
        {
            var rx = new System.Text.RegularExpressions.Regex(regex);
            for (int i = 0; i < phonenos.Length; ++i)
            {
                var input = phonenos[i];
                var match = rx.Match(input);
            }
        }

        [Benchmark]
        public void BenchRegexCompiled()
        {
            var rx = new System.Text.RegularExpressions.Regex(regex, System.Text.RegularExpressions.RegexOptions.Compiled);
            for (int i = 0; i < phonenos.Length; ++i)
            {
                var input = phonenos[i];
                var match = rx.Match(input);
            }
        }
    }
}
