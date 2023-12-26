using System.Text.RegularExpressions;

public class Day19
{
    public object CalcA()
    {
        var lines = File.ReadAllLines("Day19.txt");
        var ix = 0;

        var workFlows = new Dictionary<string, WorkFlow>();
        while (lines[ix] != "")
        {
            var wf = new WorkFlow(lines[ix]);
            workFlows.Add(wf.Name, wf);
            ix++;
        }
        ix++;

        long sum = 0;
        while (ix < lines.Length)
        {
            var part = new Part(lines[ix++]);
            var wf = "in";
            while (wf != "A" && wf != "R")
            {
                wf = workFlows[wf].Run(part);
            }

            if (wf == "A")
                sum += part.Sum();
        } 
        
        
        return sum;
    }
    
    
    public object CalcB()
    {
        var lines = File.ReadAllLines("Day19.txt");
        var ix = 0;

        var workFlows = new Dictionary<string, WorkFlow>();
        while (lines[ix] != "")
        {
            var wf = new WorkFlow(lines[ix]);
            workFlows.Add(wf.Name, wf);
            ix++;
        }

        var accepted = new List<Part2>();
        var start = new Part2();
        var searchStack = new Stack<Part2>();
        searchStack.Push(start);
        while (searchStack.Count > 0)
        {
            var node = searchStack.Pop();
            var wf = workFlows[node.AtWorkFlow];
            var children = wf.Run(node);
            foreach (var child in children)
            {
                switch (child.AtWorkFlow)
                {
                    case "R":
                        break;
                    case "A":
                        accepted.Add(child);
                        break;
                    default:
                        searchStack.Push(child);
                        break;
                }
            }
        }
        

        long sum = 0;
        for (int i = 0; i < accepted.Count; i++)
        {
            var part = accepted[i];
            sum += part.Combos();
            for (int j = 0; j < i; j++)
            {
                sum -= 2*part.ComboOverlap(accepted[j]);
            }
        }
        
        return sum;
    }
    
}

file class WorkFlow
{
    public readonly string Name;
    private readonly List<Rule> _rules;

    public WorkFlow(string workFlow)
    {
        var curlIx = workFlow.IndexOf('{');
        Name = workFlow[..curlIx];

        workFlow = workFlow[(curlIx + 1)..^1];
        _rules = workFlow.Split(',').Select(p => new Rule(p)).ToList();
    }

    public string Run(Part p)
    {
        foreach (var rule in _rules)
        {
            var result = rule.TryRule(p);
            if (result != null)
                return result;
        }

        throw new Exception("Knas");
    }

    public IEnumerable<Part2> Run(Part2 p)
    {
        foreach (var rule in _rules)
        {
            var result = rule.ApplyRule(p);
            if (result.truePart != null)
                yield return result.truePart;

            if (result.falsePart == null)
                yield break;

            p = result.falsePart;
        }
    }
}

file class Rule
{
    private readonly Func<Part, string?> _ruleImpl;
    private readonly string _rule;
    private readonly short _value;
    private readonly string _destination;

    public Rule(string rule)
    {
        _rule = rule;
        if (rule.Contains('<'))
        {
            var q = rule.Split('<');
            var attrib = q[0];
            var r = q[1].Split(':');
            _value = short.Parse(r[0]);
            _destination = r[1];
            _ruleImpl = p => LessRule(attrib, _value, _destination, p);
        }
        else if (rule.Contains('>'))
        {
            var q = rule.Split('>');
            var attrib = q[0];
            var r = q[1].Split(':');
            _value = short.Parse(r[0]);
            _destination = r[1];
            _ruleImpl = p => GreaterRule(attrib, _value, _destination, p);
        }
        else
        {
            _ruleImpl = _ => rule;
            _destination = rule;
            _value = 0;
        }
    }

    public string? TryRule(Part p)
    {
        return _ruleImpl(p);
    }

    private static string? LessRule(string attribute, int value, string result, Part part)
    {
        if (part.GetAttribute(attribute) < value)
            return result;
        return null;
    }
    private static string? GreaterRule(string attribute, int value, string result, Part part)
    {
        if (part.GetAttribute(attribute) > value)
            return result;
        return null;
    }


    public (Part2? truePart, Part2? falsePart) ApplyRule(Part2 part)
    {
        if (_value == 0)
            return (part.CloneWithNewDestination(_destination), null);
        
        if (_rule.StartsWith("x<"))
        {
            var truePart = part.CreateWithXMax(_value - 1, _destination);
            var falsePart = part.CreateWithXMin(_value, part.AtWorkFlow);
            return (truePart, falsePart);
        }
        if (_rule.StartsWith("x>"))
        {
            var truePart = part.CreateWithXMin(_value + 1, _destination);
            var falsePart = part.CreateWithXMax(_value, part.AtWorkFlow);
            return (truePart, falsePart);
        }
        
        if (_rule.StartsWith("m<"))
        {
            var truePart = part.CreateWithMMax(_value - 1, _destination);
            var falsePart = part.CreateWithMMin(_value, part.AtWorkFlow);
            return (truePart, falsePart);
        }
        if (_rule.StartsWith("m>"))
        {
            var truePart = part.CreateWithMMin(_value + 1, _destination);
            var falsePart = part.CreateWithMMax(_value, part.AtWorkFlow);
            return (truePart, falsePart);
        }
        
        if (_rule.StartsWith("a<"))
        {
            var truePart = part.CreateWithAMax(_value - 1, _destination);
            var falsePart = part.CreateWithAMin(_value, part.AtWorkFlow);
            return (truePart, falsePart);
        }
        if (_rule.StartsWith("a>"))
        {
            var truePart = part.CreateWithAMin(_value + 1, _destination);
            var falsePart = part.CreateWithAMax(_value, part.AtWorkFlow);
            return (truePart, falsePart);
        }
        
        if (_rule.StartsWith("s<"))
        {
            var truePart = part.CreateWithSMax(_value - 1, _destination);
            var falsePart = part.CreateWithSMin(_value, part.AtWorkFlow);
            return (truePart, falsePart);
        }
        if (_rule.StartsWith("s>"))
        {
            var truePart = part.CreateWithSMin(_value + 1, _destination);
            var falsePart = part.CreateWithSMax(_value, part.AtWorkFlow);
            return (truePart, falsePart);
        }

        throw new Exception("Knas");
    }
    
}


file record Part
{
    private readonly int _x;
    private readonly int _m;
    private readonly int _a;
    private readonly int _s;
    
    public Part(string data)
    {
        var m = new Regex(@"\{x=(?<x>\d+),m=(?<m>\d+),a=(?<a>\d+),s=(?<s>\d+)\}").Match(data);
        _x = int.Parse(m.Groups["x"].Value);
        _m = int.Parse(m.Groups["m"].Value);
        _a = int.Parse(m.Groups["a"].Value);
        _s = int.Parse(m.Groups["s"].Value);
    }

    public int Sum() => _x + _m + _a + _s;

    public int GetAttribute(string attrib)
    {
        return attrib switch
        {
            "x" => _x,
            "m" => _m,
            "a" => _a,
            "s" => _s,
            _ => throw new ArgumentException("Fett knas: " + attrib)
        };
    }
}


file record Part2
{
    private Range _x;
    private Range _m;
    private Range _a;
    private Range _s;
    
    public string AtWorkFlow { get; private set; }
    
    public Part2()
    {
        _x = _m = _a = _s = new Range(1, 4000);
        AtWorkFlow = "in";
    }

    public long Combos()
    {
        return _x.Size * _m.Size * _a.Size * _s.Size;
    }

    public long ComboOverlap(Part2 part2)
    {
        var x = _x.GetOverlap(part2._x);
        var m = _m.GetOverlap(part2._m);
        var a = _a.GetOverlap(part2._a);
        var s = _s.GetOverlap(part2._s);
        return x * m * a * s;
    }

    
    public bool ContainsAll(Part2 p2)
    {
        return _x.ContainsAll(p2._x) && _m.ContainsAll(p2._m) && _a.ContainsAll(p2._a) && _s.ContainsAll(p2._s);
    }
    

    public Part2 CloneWithNewDestination(string destination)
    {
        var ret = (Part2)MemberwiseClone();
        ret.AtWorkFlow = destination;
        return ret;
    }

    public Part2? CreateWithXMax(int xMax, string destination)
    {
        if (_x.IsLess(xMax))
            return null;

        var ret = CloneWithNewDestination(destination);
        ret._x = _x.WithMax(xMax);
        return ret;
    }
    
    public Part2? CreateWithXMin(int xMin, string destination)
    {
        if (_x.IsGreater(xMin))
            return null;

        var ret = CloneWithNewDestination(destination);
        ret._x = _x.WithMin(xMin);
        return ret;
    }
   
    public Part2? CreateWithMMax(int mMax, string destination)
    {
        if (_m.IsLess(mMax))
            return null;

        var ret = CloneWithNewDestination(destination);
        ret._m = _m.WithMax(mMax);
        return ret;
    }
    
    public Part2? CreateWithMMin(int mMin, string destination)
    {
        if (_m.IsGreater(mMin))
            return null;

        var ret = CloneWithNewDestination(destination);
        ret._m = _m.WithMin(mMin);
        return ret;
    }
    
    public Part2? CreateWithAMax(int aMax, string destination)
    {
        if (_a.IsLess(aMax))
            return null;

        var ret = CloneWithNewDestination(destination);
        ret._a = _a.WithMax(aMax);
        return ret;
    }
    
    public Part2? CreateWithAMin(int aMin, string destination)
    {
        if (_a.IsGreater(aMin))
            return null;

        var ret = CloneWithNewDestination(destination);
        ret._a = _a.WithMin(aMin);
        return ret;
    }
    
    public Part2? CreateWithSMax(int sMax, string destination)
    {
        if (_s.IsLess(sMax))
            return null;

        var ret = CloneWithNewDestination(destination);
        ret._s = _s.WithMax(sMax);
        return ret;
    }
    
    public Part2? CreateWithSMin(int sMin, string destination)
    {
        if (_s.IsGreater(sMin))
            return null;

        var ret = CloneWithNewDestination(destination);
        ret._s = _s.WithMin(sMin);
        return ret;
    }
}


file readonly struct Range(short min, short max)
{
    private readonly short _min = min;
    private readonly short _max = max;

    public long Size => _max - _min + 1;
    
    public long GetOverlap(Range r2)
    {
        var xMin = Math.Max(_min, r2._min);
        var xMax = Math.Min(_max, r2._max);
        if (xMax < xMin) return 0;
        return xMax - xMin + 1; 
    }

    public bool ContainsAll(Range r2)
    {
        return r2._min >= _min && r2._max <= _max;
    }

    public bool IsLess(int num)
    {
        return num < _min;
    }
    
    public bool IsGreater(int num)
    {
        return num > _max;
    }

    public Range WithMax(int max)
    {
        return new Range(_min, (short)Math.Min(max, _max));
    }

    public Range WithMin(int min)
    {
        return new Range((short)Math.Max(_min, min), _max);
    }
}