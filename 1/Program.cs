// Read all lines, stripping out any blank lines
var inputFromFile = File.ReadAllLines("..\\..\\..\\input.txt").Where(x => x.Trim() != string.Empty);

var inputLeftColumn = new List<long>(inputFromFile.Count());
var inputRightColumn = new List<long>(inputFromFile.Count());

// Parse out each line, and put the numbers into their relevant columns
foreach (var inputLine in inputFromFile)
{
    var splitNumbers = inputLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    inputLeftColumn.Add(long.Parse(splitNumbers[0]));
    inputRightColumn.Add(long.Parse(splitNumbers[1]));
}

void Part1()
{
    long total = 0;

    // Sort the columns
    var sortedLeftColumn = inputLeftColumn.Order().ToArray();
    var sortedRightColumn = inputRightColumn.Order().ToArray();

    // Work out the difference and make absolute, then add to total
    for(int i = 0; i < sortedLeftColumn.Count(); i++)
    {
        total += Math.Abs(sortedLeftColumn[i] - sortedRightColumn[i]);
    }

    Console.WriteLine(total);
}

void Part2()
{
    // Gets the frequency of each value in the right column by grouping them by themselves,
    // then counting the number of values in each group
    Dictionary<long, int> valuesByFrequency = inputRightColumn.GroupBy(x => x).ToDictionary(y => y.Key, y => y.Count());

    long total = 0;

    // Work out the similarity score for each number, then add to total
    foreach(long value in inputLeftColumn)
    {
        total += value * valuesByFrequency.GetValueOrDefault(value, 0);
    }

    Console.WriteLine(total);
}

Part1();
Part2();
