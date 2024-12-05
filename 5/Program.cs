using System.Buffers;
using System.Runtime.CompilerServices;

// Read out all the lines from the input
var inputFromFile = File.ReadAllLines(args.Length > 0 ? args[0] : "..\\..\\..\\input.txt");

// Parse out the rules into int[2] arrays
var orderingRules = inputFromFile.TakeWhile(x => !string.IsNullOrWhiteSpace(x)).Select(y => Array.ConvertAll(y.Split('|'), int.Parse));

// Group them by the first half of the rule (Part 1)
var orderingRulesBefore = orderingRules.GroupBy(z => z[0]).ToList();

// Parse out the page numbers
var pageNumbers = inputFromFile.SkipWhile(x => !string.IsNullOrWhiteSpace(x)).Skip(1).Select(y => Array.ConvertAll(y.Split(','), int.Parse));

// Checks if pages are in order
// Note: We don't care about binary size or compilation time for AoCbench, so force inlining
[MethodImpl(MethodImplOptions.AggressiveInlining)]
bool ArePagesInOrder(int[] pages)
{
    // Loop through every page and check if it violates any rule numbers
    for (int i = 0; i < pages.Length; i++)
    {
        // Find the relevant rules (first-element grouped) for this page number
        var rulesForThisNumber = orderingRulesBefore.Where(x => x.Key == pages[i]);
        if (rulesForThisNumber.Any())
        {
            // Look back at previous pages to see if they violate the rules
            Span<int> slicedPages = pages[..i];
            foreach (var rule in rulesForThisNumber)
            {
                // See if there is a rule that matches - if there is, this is an invalid sequence of pages
                var pageRuleIndex = slicedPages.IndexOfAny(rule.Select(x => x[1]).ToArray());
                if(pageRuleIndex != -1) return false;
            }
        }
    }

    return true;
}

void Part1And2()
{
    long p1Counter = 0;
    long p2Counter = 0;

    // Loop through all the pages
    foreach (var pageNumberSet in pageNumbers)
    {
        // If the pages are in order, then we do Part 1
        if(ArePagesInOrder(pageNumberSet))
        {
            p1Counter += pageNumberSet[pageNumberSet.Length / 2];
        }
        // Otherwise we use a nice little trick for Part 2
        else
        {
            // This is a really interesting property of the data/rules I noticed while debugging that results in a ridiculous (and likely unintended) perf win:
            // You don't actually need to order it - for all possible valid rules for this number set, when grouped by the second number in the rule - 
            // the group with the same number of rules as the index of the middle value of the array, matches the correct middle number
            p2Counter += orderingRules.Where(x => x.All(y => pageNumberSet.Contains(y))).GroupBy(z => z[1]).Single(w => w.Count() == pageNumberSet.Length / 2).Key;
        }
    }

    Console.WriteLine(p1Counter);
    Console.WriteLine(p2Counter);
}

Part1And2();
