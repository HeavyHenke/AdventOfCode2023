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
}

file class Rule
{
    private readonly Func<Part, string?> _ruleImpl;
    
    public Rule(string rule)
    {
        if (rule.Contains('<'))
        {
            var q = rule.Split('<');
            var attrib = q[0];
            var r = q[1].Split(':');
            var value = int.Parse(r[0]);
            var result = r[1];
            _ruleImpl = p => LessRule(attrib, value, result, p);
        }
        else if (rule.Contains('>'))
        {
            var q = rule.Split('>');
            var attrib = q[0];
            var r = q[1].Split(':');
            var value = int.Parse(r[0]);
            var result = r[1];
            _ruleImpl = p => GreaterRule(attrib, value, result, p);
        }
        else
        {
            _ruleImpl = _ => rule;
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