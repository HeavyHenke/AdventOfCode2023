using System.Text.RegularExpressions;
using MoreLinq;

public class Day8
{
    public string CalcA()
    {
        var lines = File.ReadAllLines("Day8.txt");
        var instruction = lines[0];
        Dictionary<string, (string left, string right)> map = new();
        
        for (int i = 2; i < lines.Length; i++)
        {
            var m = Regex.Match(lines[i], @"(?<node>\w{3}) = \((?<left>\w{3}), (?<right>\w{3})\)");
            map.Add(m.Groups["node"].Value, (m.Groups["left"].Value, m.Groups["right"].Value));
        }

        var pos = "AAA";
        int num = 0;
        int ipos = 0;
        while (pos != "ZZZ")
        {
            var instr = instruction[ipos++];
            if (ipos == instruction.Length)
                ipos = 0;

            if (instr == 'L')
                pos = map[pos].left;
            else
                pos = map[pos].right;
            num++;
        }

        return num.ToString();
    }
    
    public string CalcB()
    {
        var lines = File.ReadAllLines("Day8.txt");
        var instruction = lines[0];
        Dictionary<string, (string left, string right)> map = new();
        
        for (int i = 2; i < lines.Length; i++)
        {
            var m = Regex.Match(lines[i], @"(?<node>\w{3}) = \((?<left>\w{3}), (?<right>\w{3})\)");
            map.Add(m.Groups["node"].Value, (m.Groups["left"].Value, m.Groups["right"].Value));
        }

        var poses = map.Keys.Where(k => k[2] == 'A').ToArray();
        var beginAgainAt = new int[poses.Length];
        var posOrder = new List<string>[poses.Length];
        var whereIsZ = new int[poses.Length];
        
        // Find periodicity of each
        for (int i = 0; i < poses.Length; i++)
        {
            var visited = new Dictionary<(string str, int ipos), int>();
            var pos = poses[i];
            posOrder[i] = new List<string>();
            int ipos = 0;
            while (true)
            {
                if (pos[2] == 'Z')
                    whereIsZ[i] = posOrder[i].Count;
                
                if (visited.TryGetValue((pos, ipos), out var beginVal))
                {
                    beginAgainAt[i] = beginVal;
                    break;
                }
                posOrder[i].Add(pos);
                visited.Add((pos, ipos), posOrder[i].Count - 1);
                
                var instr = instruction[ipos++];
                if (ipos == instruction.Length)
                    ipos = 0;
                if (instr == 'L')
                    pos = map[pos].left;
                else
                    pos = map[pos].right;
            }
        }
        
        // Iterate numbers where one of them always ends with Z
        var allIndexes = Enumerable.Range(0, poses.Length).ToArray();

        var repeatLengths = allIndexes.Select(ix => posOrder[ix].Count - beginAgainAt[ix]).ToArray();
        var largestRepeatLengthIx = repeatLengths.Index().MaxBy(z => z.Value).Key;
        
        long num = whereIsZ[largestRepeatLengthIx];
        var posesInIteration = allIndexes
            .Select(ix => ((num - beginAgainAt[ix]) % repeatLengths[ix]) + beginAgainAt[ix])
            .Select((n, i) => posOrder[i][(int)n]);
        while (posesInIteration.All(p => p[2] == 'Z') == false)
        {
            num += repeatLengths[largestRepeatLengthIx];
            posesInIteration = allIndexes
                .Select(ix => ((num - beginAgainAt[ix]) % repeatLengths[ix]) + beginAgainAt[ix])
                .Select((n ,i) => posOrder[i][(int)n]);
        }
        
        return num.ToString();
    }

}