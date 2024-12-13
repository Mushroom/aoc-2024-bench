// Read all lines in from the input file
var inputFromFile = File.ReadAllLines(args.Length > 0 ? args[0] : "..\\..\\..\\input.txt").Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

// The grid size to size everything to
var gridSize = inputFromFile.Length + 2;

// Pad the array
var inputCellArray = new string('#', gridSize) + string.Concat(inputFromFile.SelectMany(x => '#' + x + '#')) + new string('#', gridSize);

Tuple<int, int> CalculateAreaAndPerimeter(ReadOnlySpan<char> plotMap, Span<bool> visitedMap, Span<int> vertexMap, int y, int x)
{
    var currentArea = 1;
    var currentPerimeter = 0;

    // Get the current plot character
    char plotAtPoint = plotMap[x + (y * gridSize)];

    // Indicate we have visited this square
    visitedMap[x + (y * gridSize)] = true;

    // Count the corners by mapping vertices
    vertexMap[x + (y * gridSize)] += 1;
    vertexMap[x + ((y + 1) * gridSize)] += 1;
    vertexMap[x + 1 + (y * gridSize)] += 1;
    vertexMap[x + 1 + ((y + 1) * gridSize)] += 1;

    // Scan above and below the given location
    for (int yScan = y - 1; yScan <= y + 1; yScan++)
    {
        // Scan either side of the location (inline conditionals are used here to avoid scanning diagonals)
        for (int xScan = (yScan == y ? x - 1 : x); xScan <= (yScan == y ? x + 1 : x); xScan++)
        {
            // Don't scan the current cell
            if (yScan == y && xScan == x) continue;

            // If the cell at the scanned position is not the same type as this, we have hit an edge, so increment the perimeter
            if(plotMap[xScan + (yScan * gridSize)] != plotAtPoint)
            {
                ++currentPerimeter;
            }
            // Otherwise, as long as we have not visited the cell, continue scanning using that as a base point
            else if (!visitedMap[xScan + (yScan * gridSize)])
            {
                var returnedTuple = CalculateAreaAndPerimeter(plotMap, visitedMap, vertexMap, yScan, xScan);
                currentArea += returnedTuple.Item1;
                currentPerimeter += returnedTuple.Item2;
            }
        }
    }

    return new Tuple<int, int>(currentArea, currentPerimeter);
}

void Part1And2()
{
    long p1Total = 0;
    long p2Total = 0;
    // Create a Span<T> for speed
    ReadOnlySpan<char> plotMap = inputCellArray.AsSpan();

    // Create a "tracking" array for the cells we visited
    var visitedMap = new Span<bool>(new bool[plotMap.Length]);

    // Create an array to map our vertices
    var vertexMap = new Span<int>(new int[visitedMap.Length]);

    for(int y = 1; y < gridSize - 1; y++)
    {
        for(int x = 1; x < gridSize - 1; x++)
        {
            char plotChar = plotMap[x + (y * gridSize)];

            // Skip this cell if it's already part of an area we visited
            if(visitedMap[x + (y * gridSize)]) continue;

            // Calculate the area, and Part 1 perimiter
            var output = CalculateAreaAndPerimeter(plotMap, visitedMap, vertexMap, y, x);

            long p2CornerCount = 0;
            // Scan through our calculated vertex values
            for(int yPerimCheck = 1; yPerimCheck < gridSize; yPerimCheck++)
            {
                for(int xPerimCheck = 1; xPerimCheck < gridSize; xPerimCheck++)
                {
                    // If there are no vertices at this point, nothing to do
                    if (vertexMap[xPerimCheck + (yPerimCheck * gridSize)] == 0) continue;

                    // If there are an odd number of vertices, then this is a corner
                    if (vertexMap[xPerimCheck + (yPerimCheck * gridSize)] % 2 != 0)
                    {
                        ++p2CornerCount;
                    }
                    // Special case: region enclosed within another region, count the inside corners
                    else if(vertexMap[xPerimCheck + (yPerimCheck * gridSize)] == 2)
                    {
                        // Check if we're inside the region, and the relative plot character to our vertex is also in the region
                        if(((plotChar == plotMap[xPerimCheck - 1 + ((yPerimCheck - 1) * gridSize)]) &&
                            (plotChar == plotMap[xPerimCheck + (yPerimCheck * gridSize)])) ||
                        // Check if the relative plot characters immediately above and next to this one are also in the region
                            ((plotChar == plotMap[xPerimCheck + ((yPerimCheck - 1) * gridSize)]) &&
                                  (plotChar == plotMap[xPerimCheck - 1 + (yPerimCheck * gridSize)])))
                        {
                            p2CornerCount += 2;
                        }
                    }

                    // Reset this vertex mapping
                    vertexMap[xPerimCheck + (yPerimCheck * gridSize)] = 0;
                }
            }

            //Console.WriteLine($"Region '{plotChar}': Area {output.Item1} | Perim {output.Item2} | Vertices {p2CornerCount}");

            p1Total += output.Item1 * output.Item2;
            p2Total += output.Item1 * p2CornerCount;
        }
    }

    Console.WriteLine(p1Total);
    Console.WriteLine(p2Total);
}

Part1And2();
