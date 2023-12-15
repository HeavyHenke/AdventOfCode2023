using System.Diagnostics;
using AdventOfCode2023;
using NodaTime;

var sw = Stopwatch.StartNew();

var result = new Day12().CalcB();

sw.Stop();

new Clipboard.WindowsClipboard(SystemClock.Instance).Write(result);
Console.WriteLine(result);
Console.WriteLine($"It took {sw.Elapsed}");