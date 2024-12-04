// Read all lines, stripping out any blank lines
var inputFromFile = File.ReadAllLines(args.Length > 0 ? args[0] : "..\\..\\..\\input.txt").Where(x => x.Trim() != string.Empty);

// Checks if a report is considered valid
bool IsValid(List<int> reportLevels)
{
    // Values of "direction" are as follows:
    // -1 = Descending
    //  0 = Unset
    // +1 = Ascending
    int direction = 0;

    // Step through the report values, starting from index 1 (as we look backwards)
    for (int i = 1; i < reportLevels.Count(); i++)
    {
        var difference = reportLevels[i] - reportLevels[i - 1];

        // Reports are ALWAYS invalid if there is no difference
        // So we check for lack of difference, or if it exceeds given values
        if (difference == 0 || difference > 3 || difference < -3)
        {
            return false;
        }

        // Check if the first diff was an increase or decrease and set direction accordingly
        if (direction == 0)
        {
            direction = difference > 0 ? 1 : -1;
        }

        // Check if the trend from the first pair has been broken
        if (!(direction == 1 && difference > 0) && !(direction == -1 && difference < 0))
        {
            return false;
        }
    }

    return true;
}

void Part1And2()
{
    long validCountP1 = 0;
    long validCountP2 = 0;

    // Check each report string
    foreach (var report in inputFromFile)
    {
        // Split the string into numbers, and parse them into a list
        var reportLevels = report.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList();

        // Check if the report in it's raw form is valid (satisfies both parts)
        if(IsValid(reportLevels))
        {
            validCountP1++;
            validCountP2++;
            continue;
        }

        // Now check if the report is valid with any single value removed (ie, "dampened" for P2)
        for(int i = 0; i < reportLevels.Count; i++)
        {
            // We step through each index and recreate the list without the value at that index
            if(IsValid(reportLevels.Where((_, index) => index != i).ToList()))
            {
                validCountP2++;
                break;
            }
        }
    }

    Console.WriteLine(validCountP1);
    Console.WriteLine(validCountP2);
}

Part1And2();