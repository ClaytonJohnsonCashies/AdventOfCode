<Query Kind="Program" />

void Main()
{
	var patternGroups = Inputs.Input1.Split(Environment.NewLine + Environment.NewLine)
		.Select(e => new Pattern(e.Split(Environment.NewLine)))
		.ToArray();

	foreach (var pattern in patternGroups)
	{
		pattern.FindReflectionsPart2();
		//pattern.Dump();
	}
		
	var sumScore = patternGroups.Sum(x => x.Score);
	sumScore.DumpTell();
}

public class Pattern
{
	private readonly string[] _pattern;
	
	public long Score { get; private set; }
	
	public Pattern(string[] pattern)
	{
		_pattern = pattern;
	}

	public void FindReflections()
	{
		var score = FindHorizontalReflection(_pattern) * 100;
		if (score == 0)
			score = FindVerticalReflection(_pattern);
		
		Score = score;
	}
	
	public void FindReflectionsPart2()
	{
		var variations = GetPatternVariations(_pattern);
		foreach (var variation in variations)
		{
			var score = FindHorizontalReflection(variation.Item1, variation.Item2.Item1) * 100;
			if (score > 0)
			{
				Score = score;
				return;
			}
		}

		foreach (var variation in variations)
		{
			var score = FindVerticalReflection(variation.Item1, variation.Item2.Item2);
			if (score > 0)
			{
				Score = score;
				return;
			}
		}
	}

	private int FindVerticalReflection(string[] pattern, int variationColumn = -1)
	{
		var halfPlusOne = pattern[0].Length / 2 + 1;
		
		var reflectionColumn = 0;
		for (int fold = 1; fold < pattern[0].Length; fold++)
		{	
			var leftOfLine = pattern.Select(p => new string(p.Substring(0, fold).Reverse().ToArray())).ToArray();
			var rightOfLine = pattern.Select(p => p.Substring(fold)).ToArray();
			
			var isReflection = true;
			for (int ii = 0; ii < pattern.Length; ii++)
			{
				var left = leftOfLine[ii];
				var right = rightOfLine[ii];
				
				var minLength = Math.Min(left.Length, right.Length);
				if (!left.Substring(0, minLength).Equals(right.Substring(0, minLength)))
				{
					isReflection = false;
					break;
				}
			}

			if (isReflection)
			{
				if (variationColumn != -1 && (fold == halfPlusOne + variationColumn || fold * 2 < variationColumn + 1))
				{
					isReflection = false;
					break;
				}

				reflectionColumn = fold;
				break;
			}
		}
		
		//reflectionColumn.DumpTell();
		return reflectionColumn;
	}

	private int FindHorizontalReflection(string[] pattern, int variationRow = -1)
	{
		var halfPlusOne = pattern.Length / 2 + 1;
		
		var reflectionRow = 0;
		for (int fold = 1; fold < pattern.Length; fold++)
		{
			var aboveTheLine = pattern.Take(fold).Reverse().ToArray();
			var belowTheLine = pattern.Skip(fold).ToArray();
			
			var isReflection = true;
			for (int ii = 0; ii < aboveTheLine.Length; ii++)
			{
				if (ii < belowTheLine.Length)
				{
					if (!belowTheLine[ii].Equals(aboveTheLine[ii]))
					{
						isReflection = false;
						break;
					}
				}
			}
			
			if (isReflection)
			{
				if (variationRow != -1 && (fold == halfPlusOne + variationRow || fold * 2 < variationRow + 1))
				{
					isReflection = false;
					break;
				}

				reflectionRow = fold;
				break;
			}
		}
		
		//reflectionRow.DumpTell();
		return reflectionRow;
	}

	private List<(string[], (int, int))> GetPatternVariations(string[] pattern)
	{
		var variations = new List<(string[], (int, int))>();
		var flipablePositions = FlipablePositions(pattern);
		//flipablePositions.Dump();
		
		foreach (var flip in flipablePositions)
		{
			variations.Add((pattern.Select((p, index) =>
			{
				if (index == flip.Item1)
				{
					if (p[flip.Item2] == '.')
						return p.ReplaceAt(flip.Item2, '#');
					else
						return p.ReplaceAt(flip.Item2, '.');
				}
				else
				{
					return p;
				}
			}).ToArray(), flip));
		}
		
		//variations.Dump();
		return variations;
	}

	private List<(int, int)> FlipablePositions(string[] pattern)
	{
		var flipablePositions = new List<(int, int)>();
		int patternWidth = pattern[0].Length;
		
		// find rows which differ by 1 position
		for (int ii = 0; ii < pattern.Length - 1; ii++)
		{
			for (int jj = ii + 1; jj < pattern.Length; jj++)
			{
				var diffIndex = -1;
				for (int index = 0; index < patternWidth; index++)
				{
					if (pattern[ii][index] != pattern[jj][index])
					{
						if (diffIndex != -1)
						{
							diffIndex = -1;
							break;
						}
						diffIndex = index;
					}
				}
				
				if (diffIndex != -1)
					flipablePositions.Add((ii, diffIndex));
			}
		}
		
		return flipablePositions;
	}
}


public class Inputs
{
	public const string Example1 =
@"#.##..##.
..#.##.#.
##......#
##......#
..#.##.#.
..##..##.
#.#.##.#.

#...##..#
#....#..#
..##..###
#####.##.
#####.##.
..##..###
#....#..#";

	public const string Example2 =
@"..##..##.
..#.##.#.
##......#
##......#
..#.##.#.
..##..##.
#.#.##.#.";

	public const string Input1 =
@"##...######...##.
......#..#.......
##.##########.###
###..........###.
...#..#..#..#...#
##..###..###..###
..##.#.##.#.##..#
..####.##.####..#
####........####.
..#.##.##.##.#..#
##.###.##.###.###
###.#......#.####
......####......#
#..#.##..##.#..##
..#.########.#...
##...######...##.
..#.##....##.#..#

...##......#.
...#.......#.
#......##.###
.##########.#
#####.###.##.
###.####.##..
###.####.##..
#####.###.##.
.##########.#

#.##..#
##.#.#.
##.#.##
#.##..#
...####
#.#..#.
#.#..#.

.##.#..
.##.###
..#####
#..####
.####..
.#...##
##...##

#.#.....#.##.##.#
#.#.....#.##.##.#
.....#...#####...
...#.###....#...#
.###..####.#..#.#
#.###.#.#..###..#
..####...##.#.##.
..####...##.#.##.
#.#.#.#.#..###..#
.###..####.#..#.#
...#.###....#...#
.....#...#####...
#.#.....#.##.##.#

###.####.#..#
#...##.##..#.
.#.##.#.###.#
.#..#.#.###.#
#.#...##.#.##
##.#..##.#..#
##.#...#.#..#
#.#...##.#.##
.#..#.#.###.#
.#.##.#.###.#
#...##.##..#.
###.####.#..#
#..#..#..#.##
.#...#.#..#.#
.#...#.#..#.#
#..#..#..#.##
###.####.#..#

.#.#..#..
..#.##.#.
..#.##.#.
##.#..#..
.#.#.#.##
...#...##
...#...##

.#......#..##.###
.###..###.##.####
.##....##.#####.#
#.######.##...##.
##.#..#.##.###..#
#...##...#.#.###.
..##..##.........
#..####.###....#.
.#.####.#.#####..
.##....##.##..##.
###.##.####....##
#..#..#..#..##...
#..####..#####...
#..####..#####...
#..#..#..#..##...

...#....#
###.####.
###....#.
.#..####.
..##.##.#
#####..##
#..######
#####..##
.###.##.#
..##....#
..##....#
..##....#
..##....#
.###.##.#
#####..##

#.##...##.#
##..#####..
##..##.#.#.
.####......
#.##.###...
..##....###
##..####...
##..####...
..##....###
#.##.###...
.####......
##..##.#.#.
##..#####..

..###...#
..###...#
.#..##.##
.###....#
...####.#
..###.#.#
#.#..#.#.
#.#..#.#.
..###.###
...####.#
.###....#
.#..##.##
..###...#

...#..#.###....
#.#....#..#....
..##......##.#.
##.#.####.#.###
##..##.#..###.#
#######...##..#
..#.#...###.#..
..#####..######
###.##..#..####
##.######..#...
##.######..#...
###.##..#..####
..#####..######
..#.#...###.#..
#######...##..#

#.##.##
#.##.##
..###.#
..###.#
..####.
......#
.....#.
.#..##.
.#..##.
.....#.
......#
..####.
..###.#
..###.#
#.#####

##..#..#.
..###..##
###.###..
###......
....#..#.
...#.##.#
.....##..
##...##..
##.#....#

.#..#.##..##.#..#
#.#...##..##...#.
#.#...##..##...#.
.#..#.##..##.#..#
.###....##....###
###..#..##..#..##
#.#...######...#.
...##...##..###..
#...#.######.#...

..#..#....#.###
#.####....#..##
###..###..#....
.#.##.#....##..
#......#...##..
.#.##.#..###.##
..#..#..#.#.#..
.######.#.#..##
###..####..####
##.##.###......
...##...###....

###..###..#####..
.#...#.....##....
.##............#.
####...#.####....
#....#.#.##.#..#.
##....#.#.#..#..#
#.#.###..#..#..##
#.#.###.##..#..##
##....#.#.#..#..#
##....#.#.#..#..#
#.#.###.##..#..##
#.#.###..#..#..##
##....#.#.#..#..#

##..#..#.#.##.#
##.#.#.#...####
####..##...#.#.
..##.#....#.#..
..##.#....#.#..
###...##...#.#.
##.#.#.#...####
##..#..#.#.##.#
#######.###...#

....#.##..#..#.
#..###.....##..
#..#.###.##..##
#######.##.##.#
.##..#....####.
....##..#......
####..#.#.#..#.
..#.####.##..##
.##...##.#.##.#
....#..#.##..##
.....#..#.####.
.##..##....##..
....#.##...##..

##....#....##
.....###.####
.####....##.#
.####....##.#
.....###.####
##....#....##
.....#.###...
###.#.#...###
###.#.#...###
.....#.###...
##....#.....#

############.##
#..######..#.##
#...####...####
...######......
#.###..###.#.##
#..##..##..####
.###....###.###
#.#......#.#.##
..########..##.

......#
####.##
.##.#..
#..###.
....##.
###.#..
#####..
#####..
###.#..
....##.
#..###.
.##.#..
####.##
......#
....#.#
....###
....#.#

.#.##.###.#..
.#...#.#...##
.##..#####...
.##..#####...
.#...#.#...##
##.##.###.#..
.##.##..#.###
###..#.#...##
.#.##....##..
##..##..###..
##..##.#.#.##
...#.#..#####
.##.#.#.###..
..##..#.#.###
..#.##....#..
#.#####.#..##
.####..###...

..#.##.##########
.##..#.#...#....#
..#.##...###....#
...#.#..#..##..##
.#..#........##..
.#..#........##..
#..#.#..#..##..##
..#.##...###....#
.##..#.#...#....#
..#.##.##########
#.#.####..#.####.
.##.###..##......
###.#..###.######

.##..#.#..#.#.#
.##..#.#..#.#.#
#.#.#.##..#.#..
#..#....##.##..
#.....##.#.#.#.
#..#.....#.###.
..##.##..#####.
###..#..#.###.#
..##...#.......
.#...###.#.##.#
.##..###.#.##.#
..##...#.......
###..#..#.###.#
..##.##..#####.
#..#.....#.###.

.########
....####.
####...##
####....#
#..#.##.#
##.##..##
##.##..##

#........##
.##.##.##..
...####....
##.####.###
###.##.####
#..#..#..##
.#......#..
..#.##.#...
.##.##.##..
.#####.##..
..######...
.#.#..#.#..
.##....##..
.##.##.##..
..#....#...

#########.#######
.#.##....##.#..#.
##.##.##.##.####.
.##.##..##.##..##
.#.########.#..#.
....##..##.......
..#........#....#
#.#.#.##.#.#.##.#
.###.####.###..##
##...#..#...####.
.#....##....#..#.
.#.#.####.#.#..#.
#..########..##..
####.#..#.#######
####.#..#.#######
...#.#..#.#......
..###.##.###....#

.#.####.....#####
.###.##.#.#...###
#.#.#######.#####
#..#...###..#....
#.#.##.#.#.####..
.#.#..#.#..#..#..
##.#..#.#..#..#..

.........##.#.#
..#######....##
.###...####..#.
....#.#.#.##...
.###.#.#....#.#
#.##...##...##.
.##...#.#.....#
.##...#.#.....#
#.##...##...##.
.###.#.#....#.#
....#.#.#.##...
.###...####....
..#######....##
.........##.#.#
.........##.#.#

....##...
......###
#..##..##
.##.#.#..
#..##....
.##.#...#
#######.#
#..#.##.#
#..####.#

##.####.#######
...####...#.##.
#.#.##.#.###..#
..######..#####
....##....#....
....##......##.
.#..##..#.###.#
#...##...#.....
##..##..#######
.##.##.##..#..#
#..####..#.....
...#..#....#..#
#.##..##.#.####

##.#.##
...####
####...
..#..##
...##..
###.#.#
.....##
##.####
..#....

.###.##.#...#..
.#.....#..###.#
.#.....#..###.#
.###..#.#...#..
.###.####..##..
.###.#....#.##.
#.#..##.......#
##..#.##.#...#.
...#..#....#...
##...##...#.##.
##...##...#.##.
...#..#....#...
##..#.##.#...#.
#.#..##.......#
.###.#....#.##.

#...#....##..
......###..##
.##..#...##..
####.##......
..##.##......
##.....######
..#.#..######
.#.#.#.######
.#..###.####.
#............
.####....##..
.##.#####..##
.##.###..##..
###..#.######
#...#.#######
..#.#........
##...##......

##.#.....
..#..#.#.
....#.#.#
....#..#.
....#..#.
....#.#.#
..#..#.#.
##.#.....
.####....
#######..
###.###..
.####....
##.#.....

#.###.###
.##.#..#.
#.#.####.
.#.######
#...#..#.
#.##....#
.#...##..
#.#.####.
###.#..#.
...#....#
#.#######
#.#######
...#....#
###.#..#.
#.#.####.
.#...##..
#.##....#

....##.
#..#..#
.##....
####..#
#..####
....##.
#......

#..##..#.
.##..##..
#..##..##
........#
.##..##..
########.
........#
###..####
#..##..#.
#..##..##
.##..##.#
.##..##..
#..##..#.

#...#.#..#.#.
####.......##
..###.#..#.##
.######..####
#######..####
....###..###.
#.##...##...#
.###...##...#
.##.##....##.
......#..#...
#.#..........
#.#.##.##.##.
#.#.##.##.##.
#.#..........
......#..#...
.##.##....##.
.###...##...#

.##....#....#
#####.##....#
.#.....#....#
##..#####..##
...###.##..##
#.#.####....#
###..........
####.#..####.
##.##.#......
...##########
......###..##
###.#.###..##
#.###...#..#.
#####...#..#.
###.#.###..##

####.#.#..#.#
#####..#..#..
#..###..##..#
####.########
.....########
####..######.
.##..##....##
.##.#........
.##.#..####..
.##....####..
#..#..#.##.#.
....##.####.#
######..##..#
#####..#..#..
##.###.####.#
#..##.######.
....#########

...#.....##
####..#.###
#########..
...##.....#
..#..#.....
......#.##.
##.#.......
###...##.##
....#####..
....#####..
###...##..#
##.#.......
......#.##.

##..###
..##...
#.##.##
..##...
#.##.##
.#..#..
#.##.##
#.##.#.
#....##
##..###
.#..#..

....##.##.##.
..#.###..###.
#.#.##....##.
##..#.####.##
#...#.#..#.#.
###..#....#..
#####.####.##
..#..######..
#...########.
#...########.
..#..######..
#####.####.##
###..#....#..

##....##..#
###..######
.#....#....
###..######
#.####.####
#......####
.######....
###..######
..####..##.
.######....
...#....##.
##.##.#####
..#..#.....

##..#.#####..##.#
.#..###........##
.#...#.###..#.#.#
.#...#.###..#.#.#
.#..###........##
##..#.##.##..##.#
.#.#..##.##....#.
####.####.#.##..#
.#####...###..#..
.###...##.#####..
#..##.#....####..
#.#.#.###..#.#.##
##..#..####..#.##
..#..##..#####.#.
..#.#.#..#.###..#
..#.#.#..#.###..#
..#..##..#####.#.

..#...########.
##.#..###..###.
...##..#.##.#..
.#...#..####..#
######...##...#
...#..#.####.#.
......##....##.

..#..#.
...####
...####
..#..#.
..#...#
####...
#..#...
##....#
#.#...#
####.##
..##.##
.#.#..#
...#.#.
#.####.
#.#.##.

.##...###
.##...###
#.##.##..
##.#..###
#####....
.#.......
#.#.#....
##.##.###
..###..#.
##.##.#..
##..#.#..
..###..#.
##.##.###
#.#.#....
.#.......
#####....
##.#..###

.#..###.#.#.##..#
###.#.#.#.#.#....
....#..##.#..####
#.....#...#.##..#
#..##..###.##....
..#.#.##.###.....
##.#....#.#######
...###.#.#.######
####.##....#..##.
..####.....#.###.
#####..###.#.####
...##....#.######
#...##....#.#.##.
##.#...##.#.#####
.##.##.#.###.....
.########..##....
.########..##....

....#.##...##
##.....#.#...
..##..##..#..
##.#.##....##
..##.....#.##
.#..#.##.#...
.#...#.######
.#...#.######
.#..#.##.#...
..##.....#.##
##.#.##....##
..##..##..#..
##....##.#...

.#..#...#.#...#
..##..#..##.###
.#..#.##.#.#...
..##..##.#.###.
#....###.....##
#....###.....##
..##..##.#.###.
.#..#..#.#.#...
..##..#..##.###
.#..#...#.#...#
##..##...####.#
.#..#..###.##..
#....##.#..##.#
#.##.######....
..##..#..#.####

.#.#.##...####...
.#.#.###...######
####..#.#........
####..#.#.....#..
.#.#.###...######
.#.#.##...####...
######.#......##.
##.#...#.....#.#.
#.#.#.#.#.#.#.#.#
#..####.###...###
..#.###.#.##.#..#
...####.#.#.#..##
...####.#.#.#..##

....#.#.#.##..#
##...#..#..#.##
####.##..##..##
#.###.#.##.#.#.
..#.##.#...####
..#.##.#...####
#.###.#.##...#.
#.#..#..###...#
#.#..#..###...#
#.###.#.##...#.
..#.##.#...####
..#.##.#...####
#.###.#.##.#.#.

##....##...
#..##..#.#.
#..##..###.
##....##...
.##..##..##
.#.##.#.#.#
..#..#..##.
##....##...
..#..#.....

..###.#...##.####
.##....#...#..##.
.#####...########
.#####...########
.##....#...#..##.
..###.#...##.####
#.#.##.#.##.##..#
.#....##...##....
....##..#.#.##..#
..####.#.###.#..#
.#####....##.....
..#..#...........
#.####..........#

#.#..#.##
.#.##.#..
##....###
.##..##..
...##....
...##....
.##..##..
##....###
.#.##.#..
#.#..#.#.
##....##.
..####...
.#....#.#

....##...##..
.##.#..#.#.##
#..##....###.
#..##....###.
.##.#..###.##
....##...##..
#..#.##.#.##.
.##.###..##.#
#######.##...
#####..####..
#..###.....#.

#.####.....
.###...#..#
###...##..#
#.##..#####
#.##..#####
###...##..#
.###...#..#
#.####.....
..#..##.##.
...#.######
.....#.####
#.##.##.##.
#####.##.##

#######.##...####
#######.##...####
.#....##....###..
.......#...#..#..
.###...#.#######.
#.#######.#####..
.##.#.###...#..##
...#..####..##...
..#.##.#.##..#..#
..#.##.#.##..#..#
...#..##.#..##...
.##.#.###...#..##
#.#######.#####..
.###...#.#######.
.......#...#..#..
.#....##....###..
#######.##...####

##..#.#...#
...########
...#.##....
####.###..#
...#.#..##.
....##.....
...##...##.
##...#..##.
...##...##.
###.#..####
..###.##..#
....####..#
..##...####
..#.#.#....
##..#.##..#
..###.#####
####..#....

..###.##..#....##
...##.#.#..#...#.
##..##....###.#.#
##..##....###.#.#
...##.#.#..##..#.
..###.##..#....##
##....###..####..
###...####.....##
#####..###...##..
..#..#.###...#...
....####.##..#.##

...#....##....#
...##.#....#.##
#.#.##..##..#..
.######.##.####
.##.###....###.
#..###.#..#.###
#.###.######.##
.##..###..###..
.##..###..###..
#.###.######.##
#..###.#..#.###

###.###
##..#..
###.#..
###.#..
##...##
..##...
##.##..
###.#..
..#####
##.##.#
..#####
..#....
##..###

#..#.#.#..#.#
.##.#.##..##.
#####..#..#..
#..###.#..#.#
####..##..##.
......#.##.#.
#..##...##...
.##....####..
#..####....##
..#..#......#
#..#.#.####.#

.......#.##
##..##.#..#
##..##.###.
..##..#....
.#..#.#..##
......#.#..
........#..
########.##
......#..##
######..###
########..#
......##.#.
##########.

.#.#.####.#.#..
##...####...##.
..#.######.#..#
####.#..#.#####
####......####.
##..#.##.#..###
##..##..##..###
....#....#....#
###.######.###.
..............#
##.########.##.

.#..#..
...###.
...###.
.#..#..
.##....
##..###
...#.##
..#.#..
..#...#
#.##..#
#.#...#

...#...
#..##..
...##..
.#.##..
.#.##..
...##..
#..##..
#..#...
#.#####
#...#..
#.#.#..
#.#....
....#..
....###
..###..
.#..#..
..#..##

...#...###.....
.#.##...#.##.##
#.#.###....#.##
..#.##...#.##..
#.####.###...##
...##.#.#.##...
####..##.#..###
.##.#...#..####
.#....###.##...
#.#....##..####
##.###..#####..
.#......##.#.##
##...###.......
#..#..#.#.#..##
#..#..#.#.#..##
##...###...#...
.#......##.#.##

.##..##
####.##
.##.#..
#####..
#####.#
.##.###
#..####
#..#...
####.##
#..#.##
....#..

####.#..#..#####.
#...#......#.#.##
#...#....#.#.#.##
####.#..#..#####.
##..#...#...#....
#..#...#.#...###.
....#...#####.#.#
#...##..###...#.#
.#.#.####.###.#..
#.....##..#.##.##
##..#####..#.#.#.
##..#####..#.#.#.
#.....##..#.##.##

#....##..##..#.
..##...###.#...
..##...###.#...
#....##..##..#.
.#..#.#####.##.
#.##.##...#.#.#
#.##.##...###..
#....#....#.#..
#.........#...#

#####..####..##
#..##.#....#.##
#..##.#####..##
.##..##....##..
....#..#..#..#.
#..#..######..#
.##.#.##..##.#.
....#........#.
#####........##

##....#####.##.
.#.##.#.##..##.
.######.....##.
........##.#..#
###..######.##.
..#..#...#.....
........##.####
#......#.##....
##....##..#.##.
.#.##.#..#..##.
..####....#####
#..#...#..#####
.##..##..#.#..#
#......#.##.##.
#..##..#..##..#

...####..#.#.####
..#..#.####.##..#
.....###.###..##.
.....#######..##.
..#..#.####.##..#
...####..#.#.####
.#.###..##.#.#..#
#.##.######..#..#
.#..#.##..#.##..#
.#.#....###.#.##.
.#..###..#.##.##.
..##..###...#####
##...#...#.......
#..#.###..#..####
##..#...#....####

.##.#........##..
#####.#.#....#.##
#####.#.#....#.##
.##.#.#......##..
...###.#####...#.
####.######.#.#.#
..#.#.#...#.#.###
##.#.###...#.#.#.
##.#.###...#.#.#.

..##.###.
.##..##.#
###.#.#..
##..#.#.#
#.#.##.##
#.#..#.##
##..#.#.#
###.#.#..
.##..##.#
..##.###.
..##.###.

#.#.#..#.#.#....#
#.########.#.##.#
####....####.##.#
#....##....######
...######...####.
#.##.##.##.###.##
.#.######.#..##..
#####..######..##
....####....####.

##.#..#.###
..##..##...
##.#..#.###
....##.....
#.#....#.##
.###..###..
#.#....#.##
#.##..##.##
#.######.##
.#......#..
###....####
.###..###..
#.#.##.#.##
#.#....#.##
.####.###..

.#..#..###...##
..##.....#.#...
.####.####.....
.#..#.#.##.##..
######.###.####
#.##.#..#.....#
######.#..##.#.
######.#..##.#.
#.##.#..#.....#
######.###.####
.#..#.#.##.##..
.####.#.##.....
..##.....#.#...

##..##...####..
.####..###..###
#....##.##..##.
#.##.##.#.##.#.
......#.######.
#.##.###.#..#.#
..##..####..###
#....#.#.#..#..
........#....#.
......###....##
.......##.##.##
#.##.#..#.##.#.
..##...##.##.##
#.##.###......#
.......########
.####.##..##..#
.####.#.##..##.

#..####..#.###.
##......##.#..#
###.##.###...##
#.#....#.#.##.#
#.#....#.#.##.#
###.##.###...##
##......##.#..#
#..####..#.###.
##..##..###..##
##########.#...
.##....##.##...
.#.#..#.#.##.#.
#.#...##.##.#.#
.##.##.##.##.##
#############.#

##.#..#######.###
#....##..#..#.#.#
#...###..#..#.#.#
##.#..#######.###
##.#...#.##...###
##.#...#.##...###
##.#..#######.###
#...###..#..#.#.#
#....##..#..#.#.#
##.#..#######.###
#..#......###...#
.#..###..#.......
......#..##.##...
.#.#...##.#..####
..#..##..#.#.###.

.###..##.##
.###..##.##
##...#....#
.#.##.#..##
#...#.##...
##.#...####
####.#.##..

..#...#..#...
##....####...
##..#.#..#.#.
##.####..####
###..##..##..
..##.#....#.#
##...........
....#.####.#.
..##.##..##.#
####..#..#..#
.....######..
..##.#.##.#.#
..#.#..##....

..#..#...
###....##
##.##...#
##..#.#..
##.#...#.
##..#..#.
..#####.#
..##..##.
########.
#######..
..##..##.
..#####.#
##..#..#.

####..###.##.#.
####..###.##...
.##..####.####.
#..##...##.###.
.##..###.......
#..#.##..#.#...
#..#####.##.#.#
.##....#..#.#.#
.##.#####.##.#.
.##..#...###..#
#..##..#.#...#.
#..#.##....#...
#####...####..#

#..##....##..####
#....####....#..#
#..#.####.#..####
###.##..##.###..#
#.#.#.##.#.#.#..#
##..#.##.#..#####
...##.##.##...##.
#..#..##.##..####
#.#.#.##.#.#.#..#
.##.#....#.##.##.
...#.#..#.#...##.
..##..##..##..##.
..##......##.....

.....#..##..#
##...########
.#.#....##...
#.#.#........
.#...########
...##########
######..##..#

.#..#..#.
.#..#..#.
##..#####
#.##.##..
.#..#.###
##...#.#.
#....#.##

...#.##.#
..######.
##...#..#
##...##.#
..######.
...#.##.#
###.#..#.
###..#...
###.#.##.
...###.##
..##.#..#

#.#########
#..........
####.####.#
###.##..##.
.###.#..#.#
##...#####.
....#....#.
.#.#..##..#
.#.#..##..#

#########.#..
##.##.###.#..
##....##...##
###..####....
.#.##.#.###.#
##.##.####.##
.#.##.#.#..##
.##..##.#.#..
#.#..#.#..###
#.#..#.######
..####..#..##
##.##.####...
##....#######
###..#####...
.#.##.#.#.###
.#.##.#.##.##
##....##.....

#####.#...#
.##...####.
.###...##.#
.....#.....
.....#..#..
##..#..#..#
..###..##..
#####.####.
##...#..##.
..#.#.....#
..#.#.....#
##...#..##.
#####.####.
..###..##..
##..#..#..#
.....#..#..
.....#.....

.##.....####..#
.....#........#
#..#.####..####
####...#.##.#..
####.#.######.#
....##...##...#
####.#.#.##.#.#
......#.#..#.#.
#..####......##
####...######..
#..####..##..##
#..##.#.####.#.
......##.##.##.

##.########
##.########
.#..##.##.#
##.#.######
....##.##.#
#.###..##..
..##...##..
..#####..##
#.#.#######
..##.#.##.#
....#......
.#####....#
#.#.#..##..
.#.##.#..#.
#.#....##..
.#.####...#
#..##......

.#....#.#####.#.#
#.#...##..###.###
#.##.#..##..##..#
.....#.###.#...##
.##..#....###.#..
.#..#...#..#.#.#.
.#..#.#..#.##.###
.#..#.#..#.##.###
.#..#...#..#.#...
.##..#....###.#..
.....#.###.#...##
#.##.#..##..##..#
#.#...##..###.###
.#....#.#####.#.#
.#....#.#####.#.#

##..##...
###.##...
#.###..#.
#.###..#.
###.##...
##..##...
#....#...
#####..#.
##..#####
#.##..#.#
.###.#.#.
###.###.#
...###...";
}