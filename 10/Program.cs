// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Runtime.CompilerServices;

// Read all lines form the input file
var inputFromFile = File.ReadAllLines(args.Length > 0 ? args[0] : "..\\..\\..\\input.txt").Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
int lineLength = inputFromFile[0].Length + 2;
var inputArray = string.Concat(Enumerable.Repeat('X', lineLength)) + string.Concat(inputFromFile.SelectMany(x => 'X' + x + 'X')) + string.Concat(Enumerable.Repeat('X', lineLength));

// Taken from Day 4
[MethodImpl(MethodImplOptions.AggressiveInlining)]
int[] MapDirection(CellDirection direction)
{
    // Format of [yOffset, xOffset]
    return direction switch
    {
        CellDirection.Up => [-1, 0],
        CellDirection.UpRight => [-1, 1],
        CellDirection.Right => [0, 1],
        CellDirection.DownRight => [1, 1],
        CellDirection.Down => [1, 0],
        CellDirection.DownLeft => [1, -1],
        CellDirection.Left => [0, -1],
        CellDirection.UpLeft => [-1, -1],
        _ => throw new ArgumentException("Invalid argument"),
    };
}

// Taken from Day 4
[MethodImpl(MethodImplOptions.AggressiveInlining)]
char GetGridValueInDirection(ReadOnlySpan<char> grid, int xPos, int yPos, CellDirection direction)
{
    int[] directionMappings = MapDirection(direction);
    return grid[(xPos + directionMappings[1]) + ((yPos + directionMappings[0]) * lineLength)];
}

// Taken from Day 4
[MethodImpl(MethodImplOptions.AggressiveInlining)]
char[] GetAdjacentCells(ReadOnlySpan<char> grid, int xPos, int yPos, int xSize, int ySize, CellDirection restrictedDirections)
{
    // A single dimensional array representing the char in each direction (clockwise, starting at the top left)
    char[] charsByDirection = new char[8];

    if (restrictedDirections.HasFlag(CellDirection.UpLeft))
        charsByDirection[0] = GetGridValueInDirection(grid, xPos, yPos, CellDirection.UpLeft);
    if (restrictedDirections.HasFlag(CellDirection.Up))
        charsByDirection[1] = GetGridValueInDirection(grid, xPos, yPos, CellDirection.Up);
    if (restrictedDirections.HasFlag(CellDirection.UpRight))
        charsByDirection[2] = GetGridValueInDirection(grid, xPos, yPos, CellDirection.UpRight);
    if (restrictedDirections.HasFlag(CellDirection.Right))
        charsByDirection[3] = GetGridValueInDirection(grid, xPos, yPos, CellDirection.Right);
    if (restrictedDirections.HasFlag(CellDirection.DownRight))
        charsByDirection[4] = GetGridValueInDirection(grid, xPos, yPos, CellDirection.DownRight);
    if (restrictedDirections.HasFlag(CellDirection.Down))
        charsByDirection[5] = GetGridValueInDirection(grid, xPos, yPos, CellDirection.Down);
    if (restrictedDirections.HasFlag(CellDirection.DownLeft))
        charsByDirection[6] = GetGridValueInDirection(grid, xPos, yPos, CellDirection.DownLeft);
    if (restrictedDirections.HasFlag(CellDirection.Left))
        charsByDirection[7] = GetGridValueInDirection(grid, xPos, yPos, CellDirection.Left);

    return charsByDirection;
}

int GetPathCount(ReadOnlySpan<char> inputSpan, int y, int x, bool[] visitedNines)
{
    if(inputSpan[x + (y * lineLength)] == '9')
    {
        visitedNines[x + (y * lineLength)] = true;
        return 1;
    }

    var adjacentCells = GetAdjacentCells(inputSpan, x, y, lineLength, lineLength, CellDirection.Up | CellDirection.Right | CellDirection.Down | CellDirection.Left);
    int paths = 0;
    for(int i = 1; i < 8; i += 2)
    {
        if(adjacentCells[i] == (inputSpan[x + (y * lineLength)] + 1))
        {
            int[] directionMappings = MapDirection((CellDirection)(1 << i));
            paths += GetPathCount(inputSpan, y + directionMappings[0], x + directionMappings[1], visitedNines);
        }
    }
    return paths;
}

void Part1And2()
{
    var inputSpan = inputArray.AsSpan();
    long totalP1 = 0;
    long totalP2 = 0;

    for(int y = 1; y < lineLength - 1; y++)
    {
        for(int x = 1; x < lineLength - 1; x++)
        {
            if(inputSpan[x + (y * lineLength)] == '0')
            {
                // Slight bodge for part 1
                bool[] visitedNines = new bool[inputArray.Length];
                totalP2 += GetPathCount(inputSpan, y, x, visitedNines);
                totalP1 += visitedNines.Count(x => x);
            }
        }
    }

    Console.WriteLine(totalP1);
    Console.WriteLine(totalP2);
}

Part1And2();

// Defines the direction of the cells, specifically in a clockwise manner
[Flags]
enum CellDirection
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