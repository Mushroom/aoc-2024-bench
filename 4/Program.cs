using System.Runtime.CompilerServices;

const int lineLength = 140;

// Read all lines, stripping out any blank lines
var inputFromFile = File.ReadAllText(args.Length > 0 ? args[0] : "..\\..\\..\\input.txt").ReplaceLineEndings(string.Empty);

// Mappings for the offset of a letter in a 2D array, given a direction
[MethodImpl(MethodImplOptions.AggressiveInlining)]
int[] MapDirection(LetterDirection direction)
{
    // Format of [yOffset, xOffset]
    return direction switch
    {
        LetterDirection.Up => [-1, 0],
        LetterDirection.UpRight => [-1, 1],
        LetterDirection.Right => [0, 1],
        LetterDirection.DownRight => [1, 1],
        LetterDirection.Down => [1, 0],
        LetterDirection.DownLeft => [1, -1],
        LetterDirection.Left => [0, -1],
        LetterDirection.UpLeft => [-1, -1],
        _ => throw new ArgumentException("Invalid argument"),
    };
}

// Gets the value in a grid, given the direction
// NOTE: This does no bounds checking, it assumes it has been done already
[MethodImpl(MethodImplOptions.AggressiveInlining)]
char GetGridValueInDirection(ReadOnlySpan<char> grid, int xPos, int yPos, LetterDirection direction)
{
    int[] directionMappings = MapDirection(direction);
    return grid[(xPos + directionMappings[1]) + ((yPos + directionMappings[0]) * lineLength)];
    //return grid[yPos + directionMappings[0]][xPos + directionMappings[1]];
}

// Gets the letters around the given letter, indicating where they are, with optional restrictions
// Note: we don't care about binary size, so inline everything for AoCBench performance
[MethodImpl(MethodImplOptions.AggressiveInlining)]
//Dictionary<LetterDirection, char> GetAdjacentLetters(char[][] grid, int xPos, int yPos, int xSize, int ySize, LetterDirection restrictedDirections)
Dictionary<LetterDirection, char> GetAdjacentLetters(ReadOnlySpan<char> grid, int xPos, int yPos, int xSize, int ySize, LetterDirection restrictedDirections)
{
    // The directions to exclude (ie, at the edges of the grid)
    LetterDirection excludedDirections = LetterDirection.None;

    // If xPos is 0, then there are no letters on the left side
    if (xPos == 0)
    {
        excludedDirections |= LetterDirection.UpLeft | LetterDirection.Left | LetterDirection.DownLeft;
    }

    // If yPos is 0, then there are no letters on the top side
    if (yPos == 0)
    {
        excludedDirections |= LetterDirection.UpLeft | LetterDirection.Up | LetterDirection.UpRight;
    }

    // If xPos is max value, then there are no letters on the right side
    if (xPos == xSize - 1)
    {
        excludedDirections |= LetterDirection.UpRight | LetterDirection.Right | LetterDirection.DownRight;
    }

    // If yPos is max value, then there are no letters on the bottom side
    if (yPos == ySize - 1)
    {
        excludedDirections |= LetterDirection.DownRight | LetterDirection.Down | LetterDirection.DownLeft;
    }

    // The dictionary we will return (with a max of 8 directions)
    Dictionary<LetterDirection, char> adjacentLetters = new Dictionary<LetterDirection, char>(8);

    // Add each letter to the dictionary, provided it is not excluded by position, and satisfies the restriction
    // Note: "None" is always excluded
    foreach (LetterDirection direction in
        Enum.GetValues<LetterDirection>().Where(x =>
            (restrictedDirections == LetterDirection.None || restrictedDirections.HasFlag(x)) && !excludedDirections.HasFlag(x)))
    {
        adjacentLetters[direction] = GetGridValueInDirection(grid, xPos, yPos, direction);
    }

    return adjacentLetters;
}

void Part1And2()
{
    long p1Counter = 0;
    long p2Counter = 0;

    // Parse out the input into an array of arrays
    //var parsedArray = inputFromFile.Select(x => x.ToCharArray()).ToArray();
    var parsedArray = inputFromFile.AsSpan();

    // Go through each letter, and calculate for each part
    for (int y = 0; y < lineLength; y++)
    {
        for (int x = 0; x < lineLength; x++)
        {
            // Part 1 - Does it form an "XMAS" in any direction
            if (parsedArray[x + (y * lineLength)] == 'X')
            {
                // Get all the adjacent letters to the "X"
                var lettersAdjacentToX = GetAdjacentLetters(parsedArray, x, y, lineLength, lineLength, LetterDirection.None);

                // Any adjacent "M" could form an "XMAS" at this point
                foreach (var mValue in lettersAdjacentToX.Where(x => x.Value == 'M'))
                {
                    // Get the direction mapping offsets fir the direction the "M" is in
                    int[] directionMapping = MapDirection(mValue.Key);

                    // Find if there is an "A" adjacent to the "M", restricting to the same direction
                    var aAdjacentToM = GetAdjacentLetters(parsedArray, x + directionMapping[1], y + directionMapping[0], lineLength, lineLength, mValue.Key).Where(x => x.Value == 'A');
                    if (aAdjacentToM.Any())
                    {
                        // Find if there is an "S" adjacent to the "A", restricting to the same direction
                        var sAdjacentToA = GetAdjacentLetters(parsedArray, x + (directionMapping[1] * 2), y + (directionMapping[0] * 2), lineLength, lineLength, aAdjacentToM.Single().Key).Where(x => x.Value == 'S');

                        // If there is, we have found an "XMAS", so increment the P1 counter
                        if (sAdjacentToA.Any())
                        {
                            p1Counter++;
                        }
                    }
                }
            }

            // Part 2 - Does it form an "X" of "MAS"es
            if (parsedArray[x + (y * lineLength)] == 'A')
            {
                // Get the letters around the "A", with a restriction of them being diagonal to it
                var lettersDiagonallyAdjacentToA = GetAdjacentLetters(parsedArray, x, y, lineLength, lineLength, LetterDirection.UpRight | LetterDirection.DownRight | LetterDirection.DownLeft | LetterDirection.UpLeft).Select(x => x.Value).ToArray();
                if (lettersDiagonallyAdjacentToA.Length == 4) // We have an "X", now check it is a "MAS"
                {
                    // As we can assume that our dict values are ordered clockwise (as they are specified to be in the enum),
                    // we can just scan them via iteration and a modulo operator to wrap around
                    for (int i = 0; i < 4; i++)
                    {
                        // Scan for a sequential pair of "M"s and "S"es
                        if (lettersDiagonallyAdjacentToA[(i + 0) % 4] == 'M' && lettersDiagonallyAdjacentToA[(i + 1) % 4] == 'M' &&
                            lettersDiagonallyAdjacentToA[(i + 2) % 4] == 'S' && lettersDiagonallyAdjacentToA[(i + 3) % 4] == 'S')
                        {
                            p2Counter++;
                            break;
                        }
                    }
                }
            }
        }
    }

    Console.WriteLine(p1Counter);
    Console.WriteLine(p2Counter);
}

Part1And2();

// Defines the direction of the letters, specifically in a clockwise manner
[Flags]
enum LetterDirection
{
    None = 0,
    Up = 1,
    UpRight = 2,
    Right = 4,
    DownRight = 8,
    Down = 16,
    DownLeft = 32,
    Left = 64,
    UpLeft = 128
}
