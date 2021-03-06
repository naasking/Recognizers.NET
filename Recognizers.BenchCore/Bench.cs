﻿using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Recognizers;
using System.Text.RegularExpressions;

namespace Recognizers.Bench
{
    //[SimpleJob(RuntimeMoniker.Net472, baseline: true)]
    //[SimpleJob(RuntimeMoniker.NetCoreApp30)]
    [RPlotExporter]
    public class Bench
    {
        const string regex = "[ ]*[+]?[ ]*[0-9]*[ ]*([(][0-9]+[)])?[ ]*/?([0-9- ])+";
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
        static Regex rxi = new Regex(regex, System.Text.RegularExpressions.RegexOptions.None);
        static Regex rxc = new Regex(regex, System.Text.RegularExpressions.RegexOptions.Compiled);

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
            for (int i = 0; i < phonenos.Length; ++i)
            {
                var input = phonenos[i];
                var match = rxi.Match(input);
            }
        }

        [Benchmark]
        public void BenchRegexCompiled()
        {
            for (int i = 0; i < phonenos.Length; ++i)
            {
                var input = phonenos[i];
                var match = rxc.Match(input);
            }
        }
    }
}
