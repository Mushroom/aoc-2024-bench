using System.Runtime.CompilerServices;

const int lineLength = 142;

// Read all lines, and add padding characters
// This is a terrible hack for padding, but I'm trying to get a perf win here
const string emptyRowStart = "OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO";
const string emptyRowEnd = "OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO";
var inputFromFile = emptyRowStart + File.ReadAllText(args.Length > 0 ? args[0] : "..\\..\\..\\input.txt").ReplaceLineEndings("OO") + emptyRowEnd;

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
}

// Gets the letters around the given letter, indicating where they are, with optional restrictions
// Note: we don't care about binary size, so inline everything for AoCBench performance
[MethodImpl(MethodImplOptions.AggressiveInlining)]
//Dictionary<LetterDirection, char> GetAdjacentLetters(char[][] grid, int xPos, int yPos, int xSize, int ySize, LetterDirection restrictedDirections)
char[] GetAdjacentLetters(ReadOnlySpan<char> grid, int xPos, int yPos, int xSize, int ySize, LetterDirection restrictedDirections)
{
    // A single dimensional array representing the char in each direction (clockwise, starting at the top left)
    char[] charsByDirection = new char[8];

    if (restrictedDirections.HasFlag(LetterDirection.UpLeft))
        charsByDirection[0] = GetGridValueInDirection(grid, xPos, yPos, LetterDirection.UpLeft);
    if (restrictedDirections.HasFlag(LetterDirection.Up))
        charsByDirection[1] = GetGridValueInDirection(grid, xPos, yPos, LetterDirection.Up);
    if (restrictedDirections.HasFlag(LetterDirection.UpRight))
        charsByDirection[2] = GetGridValueInDirection(grid, xPos, yPos, LetterDirection.UpRight);
    if (restrictedDirections.HasFlag(LetterDirection.Right))
        charsByDirection[3] = GetGridValueInDirection(grid, xPos, yPos, LetterDirection.Right);
    if (restrictedDirections.HasFlag(LetterDirection.DownRight))
        charsByDirection[4] = GetGridValueInDirection(grid, xPos, yPos, LetterDirection.DownRight);
    if (restrictedDirections.HasFlag(LetterDirection.Down))
        charsByDirection[5] = GetGridValueInDirection(grid, xPos, yPos, LetterDirection.Down);
    if (restrictedDirections.HasFlag(LetterDirection.DownLeft))
        charsByDirection[6] = GetGridValueInDirection(grid, xPos, yPos, LetterDirection.DownLeft);
    if (restrictedDirections.HasFlag(LetterDirection.Left))
        charsByDirection[7] = GetGridValueInDirection(grid, xPos, yPos, LetterDirection.Left);

    return charsByDirection;
}

void Part1And2()
{
    long p1Counter = 0;
    long p2Counter = 0;

    // Parse out the input into an array of arrays
    //var parsedArray = inputFromFile.Select(x => x.ToCharArray()).ToArray();
    var parsedArray = inputFromFile.AsSpan();

    // Go through each letter, and calculate for each part
    for (int y = 1; y < lineLength - 1; y++)
    {
        for (int x = 1; x < lineLength - 1; x++)
        {
            // Part 1 - Does it form an "XMAS" in any direction
            if (parsedArray[x + (y * lineLength)] == 'X')
            {
                // Get all the adjacent letters to the "X"
                var lettersAdjacentToX = GetAdjacentLetters(parsedArray, x, y, lineLength, lineLength, LetterDirection.All);

                // Scan through all the letters
                for (int mValIndex = 0; mValIndex < 8; mValIndex++)
                {
                    char letterAtPositionM = lettersAdjacentToX[mValIndex];
                    // Any adjacent "M" could form an "XMAS" at this point
                    if (letterAtPositionM != 'M') continue;

                    // Get the direction mapping offsets for the direction the "M" is in
                    int[] directionMapping = MapDirection((LetterDirection)(1 << mValIndex));

                    // Find if there is an "A" adjacent to the "M", restricting to the same direction
                    var lettersAdjacentToM = GetAdjacentLetters(parsedArray, x + directionMapping[1], y + directionMapping[0], lineLength, lineLength, (LetterDirection)(1 << mValIndex));
                    if (lettersAdjacentToM[mValIndex] != 'A') continue;

                    // There is one, so we continue and look for an "S"
                    var lettersAdjacentToA = GetAdjacentLetters(parsedArray, x + (directionMapping[1] * 2), y + (directionMapping[0] * 2), lineLength, lineLength, (LetterDirection)(1 << mValIndex));
                    if (lettersAdjacentToA[mValIndex] == 'S') p1Counter++;
                }
            }

            // Part 2 - Does it form an "X" of "MAS"es
            if (parsedArray[x + (y * lineLength)] == 'A')
            {
                // Get the letters around the "A", with a restriction of them being diagonal to it
                var lettersDiagonallyAdjacentToA = GetAdjacentLetters(parsedArray, x, y, lineLength, lineLength, LetterDirection.UpRight | LetterDirection.DownRight | LetterDirection.DownLeft | LetterDirection.UpLeft); //.Select(x => x.Value).ToArray();

                // As we can assume that our dict values are ordered clockwise (as they are specified to be in the enum),
                // we can just scan them via iteration and a modulo operator to wrap around
                for (int i = 0; i < 8; i += 2)
                {
                    // Scan for a sequential pair of "M"s and "S"es
                    if (lettersDiagonallyAdjacentToA[(i + 0) % 8] == 'M' && lettersDiagonallyAdjacentToA[(i + 2) % 8] == 'M' &&
                        lettersDiagonallyAdjacentToA[(i + 4) % 8] == 'S' && lettersDiagonallyAdjacentToA[(i + 6) % 8] == 'S')
                    {
                        p2Counter++;
                        break;
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
    UpLeft = 1,
    Up = 2,
    UpRight = 4,
    Right = 8,
    DownRight = 16,
    Down = 32,
    DownLeft = 64,
    Left = 128,
    All = ~0
}
