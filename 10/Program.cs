// Read all lines form the input file
var inputFromFile = File.ReadAllLines(args.Length > 0 ? args[0] : "..\\..\\..\\input.txt").Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
int lineLength = inputFromFile.Length + 2;

// Create the 1D input array, padding the data
var inputArray = new string('X', lineLength) + string.Concat(inputFromFile.SelectMany(x => 'X' + x + 'X')) + new string('X', lineLength);

// Gets the number of paths for the given position
int GetPathCount(ReadOnlySpan<char> inputSpan, int y, int x, Span<bool> visitedNines)
{
    // We've hit the end of the path
    if (inputSpan[x + (y * lineLength)] == '9')
    {
        // Record in the given array that there was a '9' at this location (Part 1)
        visitedNines[x + (y * lineLength)] = true;
        return 1;
    }

    int paths = 0;

    // Scan above and below the given location
    for (int yScan = y - 1; yScan <= y + 1; yScan++)
    {
        // Scan either side of the location (inline conditionals are used here to avoid scanning diagonals)
        for (int xScan = (yScan == y ? x - 1 : x); xScan <= (yScan == y ? x + 1 : x); xScan++)
        {
            // If the value at the selected position is an increase of 1 from the given position
            if ((inputSpan[x + (y * lineLength)] + 1) == inputSpan[xScan + (yScan * lineLength)])
            {
                // Then recursively check any paths from that position
                paths += GetPathCount(inputSpan, yScan, xScan, visitedNines);
            }
        }
    }

    return paths;
}

void Part1And2()
{
    var inputSpan = inputArray.AsSpan();
    long totalP1 = 0;
    long totalP2 = 0;
    Span<bool> visitedNines = new bool[inputArray.Length * inputArray.Length];

    // Scan through the padded array
    for (int y = 1; y < lineLength - 1; y++)
    {
        for (int x = 1; x < lineLength - 1; x++)
        {
            // We have found the start of a trail
            if (inputSpan[x + (y * lineLength)] == '0')
            {
                Span<bool> ninesSlice = visitedNines.Slice((x + (y * lineLength)) * inputArray.Length, inputArray.Length);
                // Get the number of paths (P2)
                totalP2 += GetPathCount(inputSpan, y, x, ninesSlice);
                // Count the number of nines we visited, and add to the total
                totalP1 += ninesSlice.Count(true);
            }
        }
    }

    Console.WriteLine(totalP1);
    Console.WriteLine(totalP2);
}

Part1And2();
