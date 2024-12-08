using System.Runtime.CompilerServices;

// Read out all the lines from the input
var inputFromFile = File.ReadAllLines(args.Length > 0 ? args[0] : "..\\..\\..\\input.txt").Where(x => !string.IsNullOrEmpty(x)).ToArray();
const int inputLength = 50;

// Flatten down the lines into a single array
char[] antennaMap = inputFromFile.SelectMany(x => x.ToCharArray()).ToArray();

// Gets the position in a 1D array, given 2D co-ordinates
[MethodImpl(MethodImplOptions.AggressiveInlining)]
int GetPosition(int y, int x)
{
    return x + (y * inputLength);
}

// Gets the antinodes, given antenna types and positions (including harmonics)
Tuple<int, int> GetAntinodeCounts(Dictionary<char, List<Tuple<int, int>>> antennaPositions)
{
    Span<bool> antinodeGrid = stackalloc bool[inputLength * inputLength];
    Span<bool> harmonicAntinodeGrid = stackalloc bool[inputLength * inputLength];

    // Go through every type of antenna and get the antinodes for that type
    foreach (var antennaPosition in antennaPositions)
    {
        var currentPositions = antennaPosition.Value;

        // If there's only 1 type of an antenna, skip it, as there are no antinodes
        if (currentPositions.Count == 1) continue;

        // Go through all the positions
        for (int j = 0; j < currentPositions.Count; j++)
        {
            // We only want to compare positions later in the list than this one, to avoid repetition
            for (int k = j + 1; k < currentPositions.Count; k++)
            {
                // Work out the difference in x and y (essentially gradient, but split into it's component parts)
                int diffX = currentPositions[k].Item2 - currentPositions[j].Item2;
                int diffY = currentPositions[k].Item1 - currentPositions[j].Item1;

                // Extrapolate in each direction
                for (int dir = 0; dir < 2; dir++)
                {
                    // The position to extrapolate from
                    var curPos = dir == 0 ? currentPositions[j] : currentPositions[k];

                    // The multiplier for the delta values (ie, how many steps in that direction to check)
                    int curOffsetMultiplier = 0;

                    // Keep going until we go OOB
                    while (true)
                    {
                        // Calculate the possible position for the next antinode in the sequence
                        int antinodeCandidatePositionY = curPos.Item1 - (diffY * curOffsetMultiplier);
                        int antinodeCandidatePositionX = curPos.Item2 - (diffX * curOffsetMultiplier);

                        // See if it's in bounds
                        if (antinodeCandidatePositionY >= 0 && antinodeCandidatePositionY < inputLength &&
                            antinodeCandidatePositionX >= 0 && antinodeCandidatePositionX < inputLength)
                        {
                            // If it is, indicate this co-ordinate is an antinode (and it's the first non-antenna antinode, note it in the non-harmonic array)
                            if (curOffsetMultiplier == 1 || curOffsetMultiplier == -1) antinodeGrid[GetPosition(antinodeCandidatePositionY, antinodeCandidatePositionX)] = true;
                            harmonicAntinodeGrid[GetPosition(antinodeCandidatePositionY, antinodeCandidatePositionX)] = true;
                        }
                        else
                        {
                            break;
                        }

                        // Take another step in the given direction
                        curOffsetMultiplier += dir == 0 ? 1 : -1;
                    }
                }
            }
        }
    }

    // Return both the non-harmonic and harmonic antinode grids
    return new Tuple<int, int>(antinodeGrid.Count(true), harmonicAntinodeGrid.Count(true));
}

void Part1And2()
{
    // A dictionary of each antenna type, and the positions of each antenna of that type
    Dictionary<char, List<Tuple<int, int>>> antennaPositions = new Dictionary<char, List<Tuple<int, int>>>(60);

    // Scan through the given grid
    for (int y = 0; y < inputLength; y++)
    {
        for (int x = 0; x < inputLength; x++)
        {
            char charAtPosition = antennaMap[GetPosition(y, x)];
            if (charAtPosition == '.') continue;

            // Record the position of the antenna, and it's type
            if (!antennaPositions.TryGetValue(charAtPosition, out List<Tuple<int, int>>? value))
            {
                value = new List<Tuple<int, int>>(6);

                // Testing on my own input indicated there were never more than 4 items, so go for 6, just in case
                antennaPositions[charAtPosition] = value;
            }

            value.Add(new (y, x));
        }
    }

    // Get the grids and count the values required for the answers
    var antinodeGrids = GetAntinodeCounts(antennaPositions);

    Console.WriteLine(antinodeGrids.Item1);
    Console.WriteLine(antinodeGrids.Item2);
}

Part1And2();
