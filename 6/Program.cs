using System.Runtime.CompilerServices;

// Read out all the lines from the input
var inputFromFile = File.ReadAllLines(args.Length > 0 ? args[0] : "..\\..\\..\\input.txt");
const int gridSize = 130;

// Flatten the array
char[] grid = inputFromFile.SelectMany(x => x.ToCharArray()).ToArray();

// Finds the guard
[MethodImpl(MethodImplOptions.AggressiveInlining)]
int[] FindGuard()
{
    for (int y = 0; y < gridSize; y++)
    {
        for (int x = 0; x < gridSize; x++)
        {
            if (grid[x + (y * gridSize)] == '^') return [x, y];
        }
    }

    throw new Exception("No guard starting position present in given array");
}

[MethodImpl(MethodImplOptions.AggressiveInlining)]
char[]? PlotGuardPathWithLoops(int[] guardStartingPosition)
{
    // Guard direction is as follows:
    // 1 << 0 = North
    // 1 << 1 = East
    // 1 << 2 = South
    // 1 << 3 = West
    byte currentGuardDirection = 1;

    // Create a fresh "visited" grid, as this permutation hasn't visited anything yet
    char[] visitedGrid = new char[grid.Length];

    // Get the starting position (and copy the values to avoid overwriting the original array)
    int[] guardPos = [guardStartingPosition[0], guardStartingPosition[1]];

    // Indicate the guard has visited the starting position facing north
    visitedGrid[guardPos[0] + (guardPos[1] * gridSize)] = (char)(1 << 0);

    // Keep iterating until the guard has walked off the edge or hit a loop
    while (true)
    {
        switch (currentGuardDirection)
        {
            // North
            case 1 << 0:
                // If we're already at the top edge, then the guard has left the area
                if (guardPos[1] == 0) return visitedGrid;

                // We have hit an obstacle
                if (grid[guardPos[0] + ((guardPos[1] - 1) * gridSize)] == '#')
                {
                    // So rotate the guard and skip to next movement cycle
                    currentGuardDirection = 1 << 1;
                    continue;
                }

                // Otherwise, the guard can move one step north
                guardPos[1] -= 1;

                break;

            case 1 << 1:
                // If we're already at the east edge, then the guard has left the area
                if (guardPos[0] == gridSize - 1) return visitedGrid;

                // We have hit an obstacle
                if (grid[(guardPos[0] + 1) + (guardPos[1] * gridSize)] == '#')
                {
                    // So rotate the guard and skip to next movement cycle
                    currentGuardDirection = 1 << 2;
                    continue;
                }

                // Otherwise, the guard can move one step east
                guardPos[0] += 1;

                break;
            case 1 << 2:
                // If we're already at the south edge, then the guard has left the area
                if (guardPos[1] == gridSize - 1) return visitedGrid;

                // We have hit an obstacle
                if (grid[guardPos[0] + ((guardPos[1] + 1) * gridSize)] == '#')
                {
                    // So rotate the guard and skip to next movement cycle
                    currentGuardDirection = 1 << 3;
                    continue;
                }

                // Otherwise, the guard can move one step south
                guardPos[1] += 1;

                break;
            case 1 << 3:
                // If we're already at the west edge, then the guard has left the area
                if (guardPos[0] == 0) return visitedGrid;

                // We have hit an obstacle
                if (grid[(guardPos[0] - 1) + (guardPos[1] * gridSize)] == '#')
                {
                    // So rotate the guard and skip to next movement cycle
                    currentGuardDirection = 1 << 0;
                    continue;
                }

                // Otherwise, the guard can move one step east
                guardPos[0] -= 1;

                break;
        }

        // Exit out if we have hit a loop (ie, the guard has already been on this tile facing the same direction)
        if ((visitedGrid[guardPos[0] + (guardPos[1] * gridSize)] & currentGuardDirection) == currentGuardDirection)
        {
            return null;
        }
        // Otherwise, store that the guard has visited this tile facing this direction
        else
        {
            visitedGrid[guardPos[0] + (guardPos[1] * gridSize)] |= (char)currentGuardDirection;
        }
    }
}

void Part1(int[] guardStartingPos)
{
    // Find the guard and plot their path, and sum all visited tiles
    var sum = PlotGuardPathWithLoops(guardStartingPos)!.Count(y => y > 0b000 && y <= 0b1111);
    Console.WriteLine(sum);
}

void Part2(int[] guardStartingPos)
{
    long loopCount = 0;

    for (int y = 0; y < gridSize; y++)
    {
        for (int x = 0; x < gridSize; x++)
        {
            if (grid[x + (y * gridSize)] == '#' || grid[x + (y * gridSize)] == '^') continue;

            grid[x + (y * gridSize)] = '#';
            if (PlotGuardPathWithLoops(guardStartingPos) == null) loopCount++;
            grid[x + (y * gridSize)] = '.';
        }
    }

    Console.WriteLine(loopCount);
}

int[] guardStartPos = FindGuard();
Part1(guardStartPos);
Part2(guardStartPos);
