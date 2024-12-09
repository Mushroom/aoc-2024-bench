var inputFromFile = File.ReadAllText(args.Length > 0 ? args[0] : "..\\..\\..\\input.txt").TrimEnd();

// Parse out the array (convert char to int using ASCII values)
int[] blockMapP1 = inputFromFile.Select(x => x - 48).ToArray();
// Make a copy for part 2
int[] blockMapP2 = (int[])blockMapP1.Clone();

long CalculateChecksum(int[] blockMap, bool part2)
{
    // The current length of the block map (ie, with the data blocks taken off the end)
    int currentBlockMapLength = blockMap.Length - 1;

    // The checksum result
    long checksum = 0;

    // The nuber of data blocks we have looking for a space
    int blocksOwed = 0;

    for (int i = 0; i <= currentBlockMapLength; i++)
    {
        // If it's a data block
        if (i % 2 == 0)
        {
            // While the block still has things left to do (ie, data in it needs a free space, or it still has data in it)
            while (blockMap[i] != 0)
            {
                // This is a data block looking for free space, so adjust the block counter, and break out so we can fill the space
                if (blockMap[i] < 0)
                {
                    blocksOwed -= blockMap[i];
                    break;
                }

                // Add it to the checksum
                checksum += blocksOwed * (i / 2);
                ++blocksOwed;

                // Indicate we have taken some data from this block
                --blockMap[i];
            }
        }
        // If it's a free space block
        else
        {
            // Fill in the free space, using the data at the end, decreasing
            for (int j = currentBlockMapLength; j > i; j -= 2)
            {
                // Part 2: If either data is owed (ie, a block looking for a space), or the current free block is less than this data block, skip it
                if(part2 && (blockMap[j] < 0 || blockMap[j] > blockMap[i])) continue;

                int fillCount = 0;

                // While there is space in the free block, and data to be put in it
                while (blockMap[i] != 0 && (blockMap[j] > 0))
                {
                    // Add to the checksum
                    checksum += blocksOwed * (j / 2);

                    // Keep track of what we've done
                    ++blocksOwed;
                    ++fillCount;

                    // Reduce the free space remaining, and the data in the last block
                    --blockMap[i];
                    --blockMap[j];
                }

                // We have emptied this block, indicate we have data from it looking for space to go
                if (blockMap[j] == 0)
                {
                    blockMap[j] -= fillCount;
                }

                // The last data block has been emptied and a place found for all of it's data
                if (j == currentBlockMapLength && blockMap[j] == 0)
                {
                    currentBlockMapLength -= 2;
                }
            }

            // Adjust the block counter to reflect the actions taken this iteration
            blocksOwed += blockMap[i];
        }
    }

    return checksum;
}

void Part1And2()
{
    long p1Result = CalculateChecksum(blockMapP1, false);
    long p2Result = CalculateChecksum(blockMapP2, true);

    Console.WriteLine(p1Result);
    Console.WriteLine(p2Result);
}

Part1And2();