<Query Kind="Program" />

void Main()
{
	(int Time, long Distance)[] exampleRaces = [(7, 9), (15, 40), (30, 200)];
	(int Time, long Distance)[] inputRaces = [(52, 426), (94, 1374), (75, 1279), (94, 1216)];

	(int Time, long Distance)[] exampleRacesPart2 = [(71530, 940200)];
	(int Time, long Distance)[] inputRacesPart2 = [(52947594, 426137412791216L)];

	var races = inputRacesPart2;
	
	List<long> winCounts = new List<long>();
	foreach (var race in races)
	{
		var possibleDistances = Enumerable.Range(1, race.Time - 1)
			.Select(t => (long)t * (race.Time - t))
			.ToArray();
		//possibleDistances.DumpTell();
		
		var countPossibleWins = possibleDistances.Count(pd => pd > race.Distance);
		winCounts.Add(countPossibleWins);
	}
	
	var totalWinOptions = winCounts.Aggregate((product, value) => product * value);
	totalWinOptions.Dump();
}

public class Inputs
{
	public const string Example1 =
@"Time:      7  15   30
Distance:  9  40  200";

	public const string Input1 =
@"Time:        52     94     75     94
Distance:   426   1374   1279   1216";
}