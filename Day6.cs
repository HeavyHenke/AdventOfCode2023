public class Day6
{
    public string CalcA()
    {
        // var times = new[] { 7, 15, 30 };
        // var distances = new[] { 9, 40, 200 };
        var times = new[] { 46, 68, 98, 66 };
        var distances = new[] { 358, 1054, 1807, 1080 };

        int ret = 1;
        for (int i = 0; i < times.Length; i++)
        {
            int numWins = 0;
            for (int t = 1; t < times[i]; t++)
            {
                var dist = t * (times[i] - t);
                if (dist > distances[i])
                    numWins++;
            }

            ret *= numWins;
        }
        
        return ret.ToString();
    }
    
    public string CalcB()
    {
        long times = 46_689_866;
        var distances = 358_105_418_071_080;

        int numWins = 0;
        for (int t = 0; t < times; t++)
        {
            var dist = t * (times - t);
            if (dist > distances)
                numWins++;
        }

        return numWins.ToString();
    }
}