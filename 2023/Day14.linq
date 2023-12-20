<Query Kind="Program" />

void Main()
{
	int width = 0;
	int height = 0;
	
	var platform = Inputs.Example1.Split(Environment.NewLine);
	width = platform[0].Length;
	height = platform.Length;
	
	var columns = new List<char[]>();
	for (int col = 0; col < width; col++)
	{
		columns.Add(SlideRocks(platform.Select(p => p[col]).ToArray()));
	}
	
	//Render(columns, width, height);
	
	var totalLoad = CalculateTotalLoad(columns, width, height);
	totalLoad.DumpTell();
	
	long spinCount = 1000000L;
	var spunPlatform = SpinCycle(platform, width, height, spinCount);
	var spunTotalLoad = CalculateTotalLoad(spunPlatform, width, height);
	spunTotalLoad.DumpTell();
}

public List<char[]> SpinCycle(string[] platform, int width, int height, long spinCount)
{
	bool canDump = false;
	bool dumped = false;
	
	var history = new Dictionary<int, List<(List<char[]> Platform, long Spin)>>(); 
	
	List<char[]> result = platform.Select(p => p.ToArray()).ToList();
	long spin = 0;
	while (spin < spinCount)
	{
		//if (spin != 0)
		//{
		//	// compare for repitition
		//	if (history.TryGetValue(GetHashCode(result), out var matches))
		//	{
		//		(List<char[]> Platform, long Spin) m = matches.SingleOrDefault(ma => MyEquals(result, ma.Platform));
		//		if (m.Platform != null)
		//		{
		//			// repitition, inc spin count
		//			$"Repition Found at {spin} with position {m.Spin}".Dump();
		//			RenderAsRows(result, width, height, "result");
		//			RenderAsRows(m.Platform, width, height, "history");
		//			//return null;
		//		}
		//	}
		//	else
		//	{
		//		history.Add(GetHashCode(result), new List<(List<char[]> Platform, long Spin)>() { new (new List<char[]>(result), spin) });
		//	}
		//}
		
		// Up
		var upResult = new List<char[]>();
		for (int col = 0; col < width; col++)
			upResult.Add(SlideRocks(result.Select(p => p[col]).ToArray()));
		dumped = canDump ? Render(upResult, width, height, "upResult") : false;

		// Left
		var leftResult = new List<char[]>();
		for (int row = 0; row < height; row++)
			leftResult.Add(SlideRocks(upResult.Select(p => p[row]).ToArray()));
		dumped = canDump ? RenderAsRows(leftResult, width, height, "leftResult") : false;

		// Down
		var downResult = new List<char[]>();
		for (int col = 0; col < width; col++)
			downResult.Add(SlideRocks(leftResult.Select(p => p[col]).Reverse().ToArray()).Reverse().ToArray());
		dumped = canDump ? Render(downResult, width, height, "downResult") : false;

		// Right
		var rightResult = new List<char[]>();
		for (int row = 0; row < height; row++)
			rightResult.Add(SlideRocks(downResult.Select(p => p[row]).Reverse().ToArray()).Reverse().ToArray());
		dumped = canDump ? RenderAsRows(rightResult, width, height, "rightResult") : false;
		result = rightResult;
		
		spin++;
	}
	
	return result;
}

public int GetHashCode(List<char[]> platform)
{
	return (new string(platform[0]) + new string(platform[1]) + new string(platform[2])).GetHashCode();
}

public bool MyEquals(List<char[]> left, List<char[]> right)
{
	for (int ii = 0; ii < left.Count; ii++)
	{
		if (!left[ii].SequenceEqual(right[ii]))
		{
			return false;
		}
	}
	
	return true;
}

public char[] SlideRocks(char[] slice)
{
	Stack<char> newSlice = new Stack<char>();
	for (int ii = 0; ii < slice.Length; ii++)
	{
		if (slice[ii] == 'O')
			newSlice.Push('O');
		else if (slice[ii] == '#')
		{
			for (int jj = newSlice.Count; jj < ii; jj++)
			{
				newSlice.Push('.');
			}
			newSlice.Push('#');
		}
	}
	for (int ii = newSlice.Count; ii < slice.Length; ii++)
	{
		newSlice.Push('.');
	}

	return newSlice.ToArray().Reverse().ToArray();
}

public long CalculateTotalLoad(List<char[]> platform, int width, int height)
{
	long load = 0L;

	for (int row = 0; row < height; row++)
	{
		load += platform.Select(p => p[row] == 'O' ? height - row : 0).Sum();
	}

	return load;
}

public bool Render(List<char[]> platform, int width, int height, string dumpName = null)
{
	if (dumpName != null)
		dumpName.Dump();
	
	for (int row = 0; row < height; row++)
	{
		var rowStr = new string(platform.Select(p => p[row]).ToArray());
		rowStr.DumpMonospace();
	}

	"".Dump();
	return true;
}

public bool RenderAsRows(List<char[]> platform, int width, int height, string dumpName = null)
{
	if (dumpName != null)
		dumpName.Dump();

	for (int row = 0; row < width; row++)
	{
		var colStr = new string(platform[row]);
		colStr.DumpMonospace();
	}

	"".Dump();
	return true;
}

public class Inputs
{
	public const string Example1 =
@"O....#....
O.OO#....#
.....##...
OO.#O....O
.O.....O#.
O.#..O.#.#
..O..#O..O
.......O..
#....###..
#OO..#....";

	public const string Input1 =
@"O.O#..O.#...#..O.O#..O#...O..O.#...O#.OO.O...#....#.O.##..####.OOO....OO......##..........#O.#O.O.#.
..O.#..#O...#..#.....O..O..OO..#.O..O#..#O#...#......O.O.O......#..OO##...O..#.O.O..#...#O....O.OO..
....O#O#.O#...#..O.O#..#.....O....#.....OOO#OO.#....#...#.O.OOOO..#O.O##....##..O.....##.OOO.#..##..
OO...OO...#..OOO..#....O.#.......##O....#..O.O#..#.O.#..O.#O.#.#..#....#...O.......O#O.....OOO###...
..#...#...O.O....#..O.....#...OO.##O...#O...#.#.O#O.OO.OO.O...#O#...#.#...#.....O..#..O##.##.OO..#.O
..#.#.O.#OO#...#.#O##O.OO..#......#OO....O........#OO#..O.O.....#.....O.O..O.#O#..O......O....O.##.#
.....O..O......O...#.#.#.....#OOO##O.#O...O...O.O..#...#O##.OO......O......OO..O#..##......#OO#..O..
...#...O.#O.......#.O....#..#.....#.O.#.....#..O..#.....O#..O#.O.O......O###O#..O...OO..##........##
.......#O..O..O........#...#....O..#...#.....#.....#.#O.#.O...O#.....OO.O........#O#O.O..#.....O#O..
OOO....OO..#O.O#....O#O....OO...O.O...O..O.#..O#........OO....#...#.....OO..O#....##OO#.O#O........O
#.#......O..O...O###O.OO..##......#........O........O......O..O..O.....O.##.O...O..O..OO.O##O.O..#O.
...O.............#.OO.O#.#.....O.O#..OO#.............O..OO.#.O.#O#.O....#..#........O.#.O.......O#..
.............OO#.....O.O..O..O...O.O..#..#.#..O....OO#..OO..#.O..O#.....#.#....O.O...#.#O....#...O.#
..O..#O.#.#OOO.....#O..OO.##....O..##.O.....O....O..#.#...O.O..O##...O...O..#..O.#O.##.......O...#..
...O#.....O#O...O....OOO...#.#....##O.....OOO.##...OO.........#.#.#..O....#O#.OO.O........O.........
.......#........#....OO........O...OO.##...O.#.....#....O..OO.O.#...OO..O...O..O....O.#....#.......#
..#....#...##O..#....O#..O.O....#....#..###.#.O.....###..OO....#...OO.OOO#O...##...O.#O#...O...O.O..
O#..O#...OO..#.......#....OOO...OO.O...OO...##.O..O##.O......#....#O.O..O#OOO.#.O#O.......OO...OO...
OO.#.#...#..O.#.#.OO..OO.#O.#..O....OOO.#.....O.OO....O.#.#..##.....O.#...O......#OO#..O.O..#.....#.
O.....O..O..........O....O..#..#...........#.#O.......#....O.#..##....#O#...#O.O...O#O..#O#..OO.#.O.
.#.##....O.#...OO##O.#..O..OO........O...O......O#OO..OO.O...#..#.O#O.......O#...O.O#..#OOO.O...OO#.
..O..#OO#O#.O..O..#......#.O..#...O..O..O...##O.O..##.....#O.O.OOO.O..O...O...O..O..O....#......#OO.
...OOOO.#.#.O.OO#......#.#.O..O.#...O..O..#......#....##O.#.O.#O#O....###.#..#.....O#.O........O....
..OO#.........OOO..........O...O..###...##...O........#....O.O#O....#...............#...O..O.O...#..
.O......#O.OO.....O..#........O#OO.....O....O.O#.O.O...O....#..O#O....#.....O...O#O.O..#.OO..##.#.#.
....O.O#..#...O#.O.O.#......OO..O#..#..#..##...O.#..###.O#.O..O#....O.#..O.O#..##O#....#O.OO#.#.....
O##..#.....##...O......#..O....#O..#O...O.........O..OO......#.#OO##...#.....#......O.....O#..O..#..
.##.O#O#O...O..#...........#OO....O.#.O.O.#.#O.#..#...O........O...#...#..OOO..##..O......O##O#.....
...O...O#.....#O..#..#..O.#.##O..........#.O..###.O..O#O....O...O#O....OO.O..#.#.O#O#.....#.#.O.OO#O
...O..#O.#O......O....O...O..O#......OO...#..O.OO.O.#..#..#......OOO..#.....OO....O..#.#O......O....
.....#...##.....O#..O.....#O.#..OO..#.#..##....#.O.#......#.#O.O..O.O.......O......O#.O#O#...OO...#.
......##.O#....O#.....O......#OO#...O....O#....#....O...#..#.#....OO...##OO..#.....##..O..O.O...O..O
OO##..#...OO.......O...#..#..O.....O..OO.OO.#..#.#O.#O..#O.##..............O...O.#O..#..#.O......##.
..O.#.....O#O......O#O#..O.O.....#...OO.#.O.#....#.###....#.#...........O#O.....OO.#..O...#..OO...#O
#..O..O..O#..O...#O.#...###..O..#.O#.#OO.......OO..........O..OOO...#....OO......O..OO#.OO#...O.O.O.
O.O..O..##..O...O.#O#.O.#.O..#O.OO#.OO..O..#...O.........#OO.......O...OO..####O...O..O...#OOO..O.O.
O..#.O.#.OOO...#O.....O#O.#O#.OO...#OO.#.##.........O..O#O#.#.O#...O..O....O..#O...O#O...O..O.O.....
O.O..O...#.O#.#...O..#.O.#.O##.O..#..#...OO#...##.O.#.##.OOO.#.........O.O....#..O.O.....#......OOO.
O..#O.#....OO#..O.##........O...#.#.....OO.....#O...OO........OO.#OO.....#.......O.#..OO#..O.#O#....
.OO..O.#..#...O.O.OOO#.O...O.O..OO.#O#..OO..#.....#.#..O....O.#O.##...O..O.....#...##...O.O#O...O..O
#....O.#......OO....#..O#.OOO..O.#...O.........###O.....##..O..O.O.O.O....##.....O...O.O..O.O.....O.
#...OO...O....##O........O.O..OO..#.....#.....OO...O...O.....#..O...#.....#.#...#...................
...O....O..OO.OO.......##...#OOO.....O..OO.O#.#........O..O.O....O.#.O#...#.#.........#......O....#.
O.........#..#..##..OO#....#..OO#O....O.O...O...................O.....O..OO.O.O.O..O.#...#...O.#.#O.
....#.O#.O..#..#..#...#OO.#.......O.#....O...O...#.##...O...O....O#.O.....O#....#.#......#.#..O.O.O.
#O##.#.....#.##O..#...O...###.OO......#......#O....#..#..O........#.O..O.....O#..O...OO#..OO.#.OO.OO
.O#.###O.O.#...####O....#.##O.O##.#..#.#..O........###.#....##O....O.O....OO.O...O.#O...OOO##...#.O.
....O..O....O............O.O......OO.##..#OO..#O...#.O.O..#..#O.O.........##.#..#..OO.....O.......#.
O....##.#O...O.OO..O#..O...#...O.OO.#..O.#......#.O..#.#..........O###...OOO.O..#.......O.O.O......O
.#......##...O.....#.O.....O.#.......O#O...O#.O.......O..#O..O#...O..O..OO#..#.O#...O.#.#.O...#.#.OO
...O.#.O.#......##..O..........OOO.....O.O......O.....O..#.O..#...#.OOO...O..#..#...#......O...#...O
..O..OO...#..#O....OO...#O...O#..O##....O#....#..#.#...#...........###...........#...OO...OO........
OO....O...#OO..O..O.O.O..#.O.....O.#...O........OO...#......O.#....O..O.#.##.........OO.O#...O.OO...
.#.....O.O#O..O.....#.#...O.O##O.......#..O#.#.#.OO#.O.O.#O#..OO..O.#O..O.OOOO......O....O...#..#...
#......O..#...O..#.O....#..OO...#.........O.O...#..O......O.O.O.#.O.......O..##..O#.#..O..#....OO.#.
.O..##O#O.O.O....#...#..O#.#.#.......O........#..#OO.OO..O..O.#...O..OOOO.....O#....O####.......#...
.#OO#.O.#..........OO..O.......O#....#.OO#...##......####...O...OOO...#....O.#..O.......#..O.O......
....O..#........#OO.......O.#..O...O..O..#.O....#.O.O.O..OO.O#O.O..O.###..O.#.#.......#.#...O#...O..
OO....O#O.O.#.#.##O#.#O#....#....O.O.......O.O.OO....O#.....O.O.OO#.##.O.#..##O..O..O#O......O...O#O
.#...#O..##.##.OO....##.....#O...#.O.O..#.....##......OO#OO.#...O...#..O....O#.#.O..O.#.......O.#.#O
O.#OO#OOO..O.#..OO..O......#O...O....##..#..#O...OO#.#.OO........O.#...O..O#OOO.O....##.O.O........#
.###..O.O...O#O..OO.O....#...OO.OO.O..OO..O.O.O.........#.O#...#...#..O..#..O..OO..#.O.....OO#......
.O#...O#..#.....O......O##..O.O.O..#..#.#.....#...#....#..#...O.O#..O...O..#...O......#.#...O.....O.
.O#..OO#.O.O...#O...O.....#....##..####.....O..O.....O...#O..#.......#OOO#.#....O.......O.#O.#..O#..
......O.O#....O.......O.O...O..O#..O...#....#O..##OO.#O......OOO.....#.#O.O#.O#.O....O#O#O..###.#...
#.O....#O......OOO......##.O.OO....#O...#OO...OO.##...O.O..#OO..OOO#..#.O.........O#...O........#...
.O....#.......O#.O..##...O##OO......#..O.O#O.O.#...........#OO........O...O....#.............#.#..O.
....#O.#..#.#..........OO...O..........O....O#...#OO.......O...O.#OO..OOO....O.O#..O.O.###..O.O#...O
O...#..OO....O.......#.OO...O........O.OO..#..#..#.#.O..O.#....O.O......OO..OO.O..#..OOO#..OOO......
.OO#.O.....O..#...O.....O....O#........#....#.....O..#..###...#.O.......OO......O..O.OO..O..#.O#O#..
O....O...##.#....#O..O.#......................O..O#OOO..##....#.#..O.O.....O#..O.O.O...#.#.OOO...O..
.......O..OO.O..O#.OO.#.O..#O....#.OOO.........##......#.O#.O..O....O....O.....#O.O...O.#.......O.#O
....O##.#...O.##...O#...OOOO...OO...#.O.#...O.#O.......#.O.....##....O.#O....#......##.O....O.OO#..O
OO...O#O#O#.#OO.#O.O..#.#.#.#.............#.O........#...O..O......O#.#OO.O...#.O..O...O#.O....O#..O
.#..O..#.OO.#O...O#.O.#.#..#.......#.......#O...#..#O#....O...#....OO.....O......O..#.....O..O..#.O#
...OO....O.#....#.O.#..#.#O.#.#O.....O..O##.......O..#O.#.O...OOO#O.#......O....#......#......O##...
...##.O.O.....##....O.....#O..O........#.#.O#O....O#..O...O#OO...O....O..O.O.#.#..#OO...O..#....OO..
O...#OO.O.OO#..O....##O...#....#...#.O.O..O.........#..##.#O.#.OO..OO...#...O.O#.OO..#.O.OO#.#..##OO
..O.O#..O..#O...O.#.O.#....#....#.#O...O..O#O....#...#..#..##......#.O..........OOO........O.#......
.....#....#....OO....#....O.#O..O...O#O..#...###....O.....O........##.#.#..#.O.OO.#O.O.O..#...O#O..O
.OO.....O.O.....#....O.O....O......O#O....O#.#..O....O.#............#..O.#.....OO....O...O....O.#.O.
.........O#OO.#O.....O......O...#..#O..O.#...O.O..OO.........#.O#OO.#.OO.........#.OO..O.O#O..O.O#..
#...O...O....#O.###.....O.OO..O..OO.O#O....#O..#O..OO..O..#........O...#.....#..#...OO..O....#.....O
......O.O.OO#.O.......O#O.OO.#..O....O...#.O#..#....O.#....#..#O...#.OO.O#........#......O....O.O...
..#...##...#.O.O..#...O#.....##.......OO..O.#....O.O#O#..O...O#..OO...##O...O..O.....#.#...O.O......
OOO.O.O#O......#..OO.O...OO...O............O..O#..##O...###.#......O#..#.O..O.OOO...#..O#......#OO.#
O###.......#.O.O.#.....#.OO..O.O...#..O#O..#O.O.#.O..O.O..O#.#....#..#...O#..###.#..O#.O.........OO.
.......#O#OO#...#.#...#.O...O.....O.O#.....O.O.O##.#...#O.#....OO...O.OO..O....O.O..#..#.....O...#..
.O..###..OO#O.#.#OO.O#...#..O#...O.....O#.O......O...OO..O..O.....O..##.....OO.....O......O........O
..O..#.O.#..OO.O#..O#..O.....OO#O#....OO.OO#O...O...O.O......OO.#O#....O..#O.#.OO##.#....#O#.#......
O.#O.O.O.O...OO.....O.#...##..#....O....O..#..#O#..O#.#.OO.#O..........#...O..##..O.##.O.O...#.#O..#
......O#...O.#..O#.O.........#.#....O.O....O.#..O..O##.#..O....#..#O#.OO.........#.O##O.....OO.OO.O.
#....#.#O.O.O............O.O.#.O..O.........O..O........O......O.....OO.OO..OO.#....OO.OO.#....#..##
.......#...O.O.#.O.....#.O#.#..O..O...O.O.#O..#...##.........O..#.OO..O........#.....#.......OO.....
...#....O...O...#O.OO...#....#..O.#.O.#......##.#.#.OO.O.O...#..#..O........O.##..O...O#O...OO.O....
.......OO...O..O...........O###......#....O.......O..O.....O#...#.O.#.#......O..O.....#.#OO.......O.
.#..#.....#.O..O..O.....##..O.........O#.##O....OO...O...#..O.O.##OO......O.#..O....O..O.#.O.O...O.O
.O....O...O....#.O......#..##O.#..O..O.....O...OO..###....#..#..#..........#.#.#.#.#........O...O.O.
.OOO.#.#O....O.#...OO#.##...#...O.#...OO..###..OO.#.O....O..OO#..O..O....O#.#.O####..O....O..O#O....
.OO.OO#......#....#..##O#O...OO....#....O......OO..OOO.....##.......O#.#.O.#O......O..#.#O..O.O..#..";
}