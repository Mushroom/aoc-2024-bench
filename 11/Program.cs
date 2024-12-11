// Read all text in from the input file
var inputFromFile = File.ReadAllText(args.Length > 0 ? args[0] : "..\\..\\..\\input.txt").TrimEnd();

// Parse it into a lit of longs
var stoneList = inputFromFile.Split(' ').Select(x => long.Parse(x)).ToList();

// Initialise the dictionary that keeps track of how many stones of each number there are
// Note: We pre-allocate 4500 entries, which from testing should avoid resizing
Dictionary<long, long> stoneTracker = new Dictionary<long, long>(4500);

// Does the rules for the required number of iterations on the stones
Tuple<long, long> PerformRulesOnList()
{
    long p1Count = 0;

    // Initialise the initial counters for the stone tracker
    stoneList.ForEach(x => AddToDictValue(x, 1));

    for(int i = 0; i < 75; i++)
    {
        // Loop through each stone value
        // Note: We convert to list here, as it creates a copy - this is needed as we will be modifying the dictionary
        var debugList = stoneTracker.Where(x => x.Value > 0).ToList();
        foreach(var stoneCounter in stoneTracker.ToList())
        {
            // If there are no stones with this value, then don't do anything with it
            if(stoneCounter.Value == 0) continue;

            // Reset the counter for this stone value iteration
            // Note: This is not a straight "set to 0", as previous iterations might have changed this counter
            stoneTracker[stoneCounter.Key] -= stoneCounter.Value;

            // Rule 1: D̶o̶ ̶n̶o̶t̶ ̶t̶a̶l̶k̶ ̶a̶b̶o̶u̶t̶ ̶f̶i̶g̶h̶t̶ ̶c̶l̶u̶b̶  Stones engraved with '0' become '1'
            if(stoneCounter.Key == 0)
            {
                AddToDictValue(1, stoneCounter.Value);
                continue;
            }

            // Rule 2: Split a stone with an even number of digits
            string stoneString = stoneCounter.Key.ToString();
            if(stoneString.Length % 2 == 0)
            {
                var leftHalf = long.Parse(stoneString[..(stoneString.Length / 2)]);
                var rightHalf = long.Parse(stoneString[(stoneString.Length / 2)..]);
                AddToDictValue(leftHalf, stoneCounter.Value);
                AddToDictValue(rightHalf, stoneCounter.Value);
                continue;
            }

            // Rule 3: Otherwise, multiply by 2024
            AddToDictValue(stoneCounter.Key * 2024, stoneCounter.Value);
        }

        // Part 1
        if(i == 24)
        {
            p1Count = stoneTracker.Values.Sum();
        }

        /*for (int j = 0; j < stoneList.Count; j++)
        {
            // Rule 1 - Stones engraved with '0' become '1'
            if (stoneList[j] == 0)
            {
                stoneList[j] = 1;
                continue;
            }

            // Rule 2 - Split a stone with an even number of digits
            string stoneString = stoneList[j].ToString();
            if(stoneString.Length % 2 == 0)
            {
                var leftHalf = long.Parse(stoneString[..(stoneString.Length / 2)]);
                var rightHalf = long.Parse(stoneString[(stoneString.Length / 2)..]);
                stoneList[j] = rightHalf;
                stonesToAdd.Add(leftHalf);
                continue;
            }

            // Rule 3 - Otherwise, multiply by 2024
            stoneList[j] *= 2024;
        }

        stoneList.AddRange(stonesToAdd);*/
    }

    return new(p1Count, stoneTracker.Values.Sum());
}

// Adds a value to the given dict entry, optionally initialising the value if it doesn't exist
void AddToDictValue(long stoneNumber, long count)
{
    stoneTracker.TryAdd(stoneNumber, 0);
    stoneTracker[stoneNumber] += count;
}

void Part1And2()
{
    var results = PerformRulesOnList();
    Console.WriteLine(results.Item1);
    Console.WriteLine(results.Item2);
}

Part1And2();
