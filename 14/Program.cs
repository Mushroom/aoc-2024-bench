// Read all lines in from the input file
var inputFromFile = File.ReadAllLines(args.Length > 0 ? args[0] : "..\\..\\..\\input.txt").Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
const int xSize = 101;
const int ySize = 103;

List<Tuple<int, Robot>> initialRobotMap = new List<Tuple<int, Robot>>(xSize * ySize);

// Parse out the starting positions and their velocity
foreach(var startingData in inputFromFile)
{
    string[] splitPosAndVel = startingData.Split(' ');
    var startPos = splitPosAndVel[0][2..].Split(',').Select(x => int.Parse(x)).ToArray();
    var vel = splitPosAndVel[1][2..].Split(',').Select(x => int.Parse(x)).ToArray();
    initialRobotMap.Add(new (startPos[0] + (startPos[1] * xSize), new Robot(vel[0], vel[1])));
}

void Part1And2(List<Tuple<int, Robot>> initialMap)
{
    // The list tracking the robot positions
    List<Tuple<int, Robot>> currentMap = initialMap;

    // To record the snapshot at the 100th iteration for Part 1
    List<Tuple<int, Robot>> p1Map = null;
    long p2Value = 0;

    // Do the simulation
    for(int i = 0; true; i++)
    {
        // Create the new map for the robots after this simulation step
        List<Tuple<int, Robot>> newMap = new List<Tuple<int, Robot>>();

        // Simulate the movements
        foreach(var (currentPos, robot) in currentMap)
        {
            var (y, x) = Math.DivRem(currentPos, xSize);

            var newPositionY = (((y + robot.VelY) % ySize) + ySize) % ySize;
            var newPositionX = (((x + robot.VelX) % xSize) + xSize) % xSize;

            newMap.Add(new (newPositionX + (newPositionY * xSize), robot));
        }

        // Take a snapshot for Part 1
        if (i == 99) p1Map = newMap;

        // Part 2: Check if all robots are in unique positions
        var groupedInput = newMap.GroupBy(x => x.Item1);
        if (groupedInput.All(y => y.Count() == 1))
        {
            // If they are, find a run of at least 10 sequential values (this indicates a chirstmas tree)
            var groupedInputOrdered = groupedInput.OrderBy(x => x.Key);

            int count = 0;
            int firstItem = 0;

            // Run detection (here, a run of 10 is considered sufficient for a christmas tree)
            foreach(var group in groupedInputOrdered)
            {
                // If this is the first ever run of this checker
                if(count == 0)
                {
                    firstItem = group.Key;
                    count = 1;
                    continue;
                }

                // This is a continuation, increase the counter
                if(group.Key == firstItem + count)
                {
                    ++count;
                    continue;
                }

                // There has been a run of at least 10
                if (count >= 10) break;

                // Otherwise we have hit a new sequence, so skip
                count = 1;
                firstItem = group.Key;
            }

            // Record we have found the christmas tree
            if(count >= 10)
            {
                p2Value = i + 1;
                break;
            }
        }

        currentMap = newMap;
    }

    // Part 1 - Do the counts for each quadrant
    long q1Count, q2Count, q3Count, q4Count;
    q1Count = q2Count = q3Count = q4Count = 0;

    // Count the number of robots in each quadrant
    foreach (var (currentPos, robot) in p1Map!)
    {
        var yQuadrantLine = ySize / 2;
        var xQuadrantLine = xSize / 2;

        var (y, x) = Math.DivRem(currentPos, xSize);

        // Ignore the centre line robots
        if (y == yQuadrantLine || x == xQuadrantLine) continue;

        // Count the quadrants
        if (y < yQuadrantLine && x < xQuadrantLine) ++q1Count;
        else if (y < yQuadrantLine && x > xQuadrantLine) ++q2Count;
        else if (y > yQuadrantLine && x < xQuadrantLine) ++q3Count;
        else if (y > yQuadrantLine && x > xQuadrantLine) ++q4Count;
    }

    Console.WriteLine(q1Count * q2Count * q3Count * q4Count);
    Console.WriteLine(p2Value);
}

Part1And2(initialRobotMap);

record Robot(int VelX, int VelY);