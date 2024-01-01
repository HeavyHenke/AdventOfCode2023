public class Day20
{
    public object CalcA()
    {
        var modules = File.ReadLines("Day20.txt")
            .Select(s => new Module(s))
            .ToDictionary(key => key.Id);

        foreach (var m in modules)
            m.Value.SetInputsForConjunctionModule(modules.Values);

        var broadcaster = modules["broadcaster"];

        long numHighSignals = 0;
        long numLowSignals = 0;

        List<(string dest, string source, bool signal)> next = [];
        List<(string dest, string source, bool signal)> continueWith = [];

        for (int i = 0; i < 1_000; i++)
        {
            next.Clear();
            next.AddRange(broadcaster.GetOutput("button", false));

            numLowSignals++;

            while (next.Count > 0)
            {
                var highSignals = next.Count(o => o.signal);
                numHighSignals += highSignals;
                numLowSignals += next.Count - highSignals;

                continueWith.Clear();
                foreach (var g in next)
                {
                    if (modules.TryGetValue(g.dest, out var module))
                    {
                        var output = module.GetOutput(g.source, g.signal);
                        continueWith.AddRange(output);
                    }
                }

                (next, continueWith) = (continueWith, next);
            }
        }

        return numHighSignals * numLowSignals;
    }

    public object CalcB()
    {
        var modules = File.ReadLines("Day20.txt")
            .Select(s => new Module(s))
            .ToDictionary(key => key.Id);

        foreach (var m in modules)
            m.Value.SetInputsForConjunctionModule(modules.Values);

        var broadcaster = modules["broadcaster"];


        List<(string dest, string source, bool signal)> next = [];
        List<(string dest, string source, bool signal)> continueWith = [];

        var modulesInputRm = modules.Values.Where(m => m.Outputs.Contains("rm")).Select(key => key.Id).ToHashSet();
        var cycleLengths = new Dictionary<string, long>();
        var cycles = new Dictionary<string, long>();
        
        for (long buttonPress = 1; buttonPress < 1024*1024; buttonPress++)
        {
            next.Clear();
            next.AddRange(broadcaster.GetOutput("button", false));

            while (next.Count > 0)
            {
                continueWith.Clear();
                foreach (var g in next)
                {
                    if (g.signal == false && modulesInputRm.Contains(g.dest))
                    {
                        if (cycleLengths.TryGetValue(g.dest, out var cycleLength))
                        {
                            var expected = (cycles[g.dest] + 1) * cycleLength;
                            if (buttonPress != expected)
                                throw new Exception("Unexpected cycle length");
                            cycles[g.dest]++;

                            if (cycles.Count == modulesInputRm.Count && cycles.Values.All(v => v >= 3))
                            {
                                long result = 1;
                                foreach (var c in cycleLengths.Values)
                                    result *= c;
                                return result;
                            }
                        }
                        else
                        {
                            cycleLengths[g.dest] = buttonPress;
                            cycles[g.dest] = 1;
                        }
                    }
                    
                    if (modules.TryGetValue(g.dest, out var module))
                    {
                        var output = module.GetOutput(g.source, g.signal);
                        continueWith.AddRange(output);
                    }
                    else if (g is { dest: "rx", signal: false })
                        return buttonPress;
                }

                (next, continueWith) = (continueWith, next);
            }
        }

        throw new Exception("Knas");
    }

    class Module
    {
        public string Id { get; }

        public readonly string[] Outputs;
        private readonly bool _isFlipFlop;

        private bool _flipFlopState;

        private HashSet<string>? _setConjunctionInputs;
        private int _numConjunctionInputs;
        

        public Module(string inp)
        {
            if (inp.StartsWith("broadcaster"))
            {
                Id = "broadcaster";
            }
            else
            {
                Id = inp.Split(" -> ")[0][1..];
                _isFlipFlop = inp[0] == '%';
            }

            Outputs = inp.Split("->")[1].Split(", ").Select(s => s.Trim()).ToArray();
        }

        public void SetInputsForConjunctionModule(IEnumerable<Module> allModules)
        {
            if (Id == "broadcaster" || _isFlipFlop)
                return;

            foreach (var m in allModules)
            {
                if (m.Outputs.Contains(Id))
                    _numConjunctionInputs++;
            }

            if (_numConjunctionInputs > 1)
                _setConjunctionInputs = new();
        }

        public IEnumerable<(string dest, string source, bool signal)> GetOutput(string source, bool signal)
        {
            if (Id == "broadcaster")
            {
                foreach (var o in Outputs)
                    yield return (o, Id, false);
                yield break;
            }

            if (_isFlipFlop)
            {
                if (signal == false)
                {
                    _flipFlopState = !_flipFlopState;
                    foreach (var o in Outputs)
                        yield return (o, Id, _flipFlopState);
                }

                yield break;
            }

            bool outputSignal;
            if (_setConjunctionInputs == null) // Meaning only one input
            {
                _flipFlopState = signal;
                outputSignal = !signal;
            }
            else
            {
                if (signal)
                    _setConjunctionInputs.Add(source);
                else
                    _setConjunctionInputs.Remove(source);

                outputSignal = _setConjunctionInputs.Count != _numConjunctionInputs;
            }
            
            foreach (var o in Outputs)
                yield return (o, Id, outputSignal);
        }

        public override string ToString()
        {
            if (_isFlipFlop)
                return $"{Id} flipflop {(_flipFlopState ? "1" : "0")}";
            if (Id == "broadcaster")
                return "broadcaster";
            
            if(_setConjunctionInputs != null)
                return $"{Id} conjunctions ({_numConjunctionInputs}) {string.Join(", ", _setConjunctionInputs)}";
            
            return $"{Id} conjunction {(_flipFlopState ? "1" : "0")}";
        }
    }
   
}