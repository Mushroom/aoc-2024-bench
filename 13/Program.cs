// Read all lines in from the input file
var inputFromFile = File.ReadAllLines(args.Length > 0 ? args[0] : "..\\..\\..\\input.txt").Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

var machines = new ClawMachine[inputFromFile.Length / 3];

// Loop through all the input and parse it into machines
for(int i = 2; i < inputFromFile.Length; i+=3)
{
    // Parse out each button into a pair of strings, that then go into the list as a ClawMachine object
    var buttonA = inputFromFile[i-2][10..].Split(", ");
    var buttonB = inputFromFile[i-1][10..].Split(", ");
    var prize = inputFromFile[i][7..].Split(", ");
    machines[i / 3] = new (long.Parse(buttonA[0][2..]), long.Parse(buttonA[1][2..]), long.Parse(buttonB[0][2..]), long.Parse(buttonB[1][2..]), long.Parse(prize[0][2..]), long.Parse(prize[1][2..]));
}

long SolveUsingCramersRule(bool withOffset)
{
    long result = 0;

    foreach(var machine in machines)
    {
        // Solve using Cramer's Rule
        // ax + by = e
        // cx + dy = f
        //
        // Equating to
        // deltaAX(x) + deltaBX(y) = prizeX(e)
        // deltaAY(x) + deltaBY(y) = prizeY(f)
        //
        //    det|e b| |    det|a e|
        //       |f d| |       |c f|
        // x=    ----- | y=    -----
        //    det|a b| |    det|a b|
        //       |c d| |       |c d|

        var detABCD = (machine.ButtonADeltaX * machine.ButtonBDeltaY) - (machine.ButtonADeltaY * machine.ButtonBDeltaX);
        var detEBFD = ((withOffset ? (machine.PrizeX + 10000000000000) : machine.PrizeX) * machine.ButtonBDeltaY) - (machine.ButtonBDeltaX * (withOffset ? (machine.PrizeY + 10000000000000) : machine.PrizeY));
        var detAECF = (machine.ButtonADeltaX * (withOffset ? (machine.PrizeY + 10000000000000) : machine.PrizeY)) - ((withOffset ? (machine.PrizeX + 10000000000000) : machine.PrizeX) * machine.ButtonADeltaY);

        var (quotientX, remainderX) = Math.DivRem(detEBFD, detABCD);
        var (quotientY, remainderY) = Math.DivRem(detAECF, detABCD);

        // If either value gives a non-integer solution, then there is no way of getting the prize
        if(remainderX != 0 || remainderY != 0) continue;

        // Add the token count to the result
        result += (quotientX * 3) + quotientY;
    }

    return result;
}

void Part1And2()
{
    Console.WriteLine(SolveUsingCramersRule(false));
    Console.WriteLine(SolveUsingCramersRule(true));
}

Part1And2();

public record ClawMachine(long ButtonADeltaX, long ButtonADeltaY, long ButtonBDeltaX, long ButtonBDeltaY, long PrizeX, long PrizeY);
