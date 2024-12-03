using System.Text.RegularExpressions;

internal partial class Program
{
    // Generate regexes at compile time for perf (SYSLIB1045)
    [GeneratedRegex("mul\\((\\d+)\\,(\\d+)\\)")]
    private static partial Regex mulRegex();

    [GeneratedRegex("do\\(\\)")]
    private static partial Regex doRegex();

    [GeneratedRegex("don\\'t\\(\\)")]
    private static partial Regex dontRegex();

    private static void Main(string[] args)
    {
        // Read all text, stripping blank lines from the end
        var inputFromFile = File.ReadAllText("input.txt").TrimEnd();

        // Do both parts at the same time for efficiency
        Part1And2(inputFromFile);
    }

    private static void Part1And2(string inputFromFile)
    {
        long totalP1 = 0;
        long totalP2 = 0;

        // Regex out the individual instructions we need to (mul capturing digits inside)
        MatchCollection mulMatches = mulRegex().Matches(inputFromFile);
        MatchCollection doMatches = doRegex().Matches(inputFromFile);
        MatchCollection dontMatches = dontRegex().Matches(inputFromFile);

        // Concat all matches into tuples indicating their type, ordered by index in original string
        // Types are as follows:
        // 0 = "mul" instruction
        // 1 = "do" instruction
        // 2 = "don't" instruction
        var allMatches = mulMatches.Select(x => new Tuple<Match, byte>(x, 0))
            .Concat(doMatches.Select(x => new Tuple<Match, byte>(x, 1)))
            .Concat(dontMatches.Select(x => new Tuple<Match, byte>(x, 2)))
            .OrderBy(x => x.Item1.Index);

        // As we step through, indicate if mul is enabled at this time
        bool isMulEnabledP2 = true;

        // Step through all concatenated and ordered matches
        foreach (var match in allMatches)
        {
            // Check which type of match it is
            switch (match.Item2)
            {
                case 0:
                    // Calculate the result of the "mul" instruction based on the values in the capture groups
                    long mulResult = long.Parse(match.Item1.Groups[1].ValueSpan) * long.Parse(match.Item1.Groups[2].ValueSpan);

                    // Add it to the totals for each part (but only if enabled for P2)
                    totalP1 += mulResult;
                    if (isMulEnabledP2) totalP2 += mulResult;
                    break;
                case 1:
                    // Enable "mul" when we have a "do"
                    isMulEnabledP2 = true;
                    break;
                case 2:
                    // Disable "mul" when we have a "don't"
                    isMulEnabledP2 = false;
                    break;
            }
        }

        Console.WriteLine(totalP1);
        Console.WriteLine(totalP2);
    }
}