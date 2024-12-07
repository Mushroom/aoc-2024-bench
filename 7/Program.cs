using System.Diagnostics;

// Read in the input, and then split it to tuples of the desired value and the numbers to make it
var inputFromFile = File.ReadAllLines(args.Length > 0 ? args[0] : "../../../input.txt");
var scoreAndValues = inputFromFile.Select(x => x.Split(':')).Select(y => new Tuple<long, long[]>(long.Parse(y[0]), Array.ConvertAll(y[1].Trim().Split(' '), long.Parse)));

// Checks if a sum is valid
bool IsSumValid(long requiredTotal, Span<long> values, bool concat)
{
    // Get the last value in the array - if it's the only one, check if it's the required total
    long lastValue = values[values.Length - 1];
    if(values.Length == 1) return values[0] == requiredTotal;

    // If the division of the total by the last value results in an integer, recursively check the array without the last number
    if(requiredTotal % lastValue == 0 && IsSumValid(requiredTotal/lastValue, values[..^1], concat))
    {
        return true;
    }

    // Part 2:
    // If the test value difference ends with 10 ^ the number of digits in the last value
    // and we do the same recursive check as above, then this is a valid sum
    if(concat &&
        (requiredTotal - lastValue) % Math.Pow(10, lastValue.ToString().Length) == 0 &&
            IsSumValid(requiredTotal / (long)Math.Pow(10, lastValue.ToString().Length), values[..^1], concat))
        {
            return true;
        }

    // Otherwise, we recursively check without the final value
    return IsSumValid(requiredTotal - values[^1], values[..^1], concat);
}

void Part1And2()
{
    long total1 = 0;
    long total2 = 0;
    foreach(var equationCombo in scoreAndValues)
    {
        if(IsSumValid(equationCombo.Item1, equationCombo.Item2, false))
        {
            total1 += equationCombo.Item1;
            total2 += equationCombo.Item1;
        }
        else if (IsSumValid(equationCombo.Item1, equationCombo.Item2, true))
        {
            total2 += equationCombo.Item1;
        }
    }

    Console.WriteLine(total1);
    Console.WriteLine(total2);
}

Part1And2();
