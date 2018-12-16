namespace AdventOfCode2018
{
    public static class Program
    {
        private static readonly Challenge[] challenges = new Challenge[]
        {
            new Day1(),
            new Day2(),
            new Day3(),
            new Day4(),
            new Day5(),
            new Day6(),
            new Day7(),
            new Day8(),
            new Day9(),
            new Day10(),
            new Day11(),
            new Day12(),
            new Day13(),
            new Day14(),
            new Day15(),
            new Day16(),
            new Day17(),
            new Day18(),
            new Day19(),
            new Day20(),
            new Day21(),
            new Day22(),
            new Day23(),
            new Day24(),
            new Day25()
        };

        public static void Main()
        {
            while (true)
            {
                Write($"Which challenge would you like to run (1-{challenges.Length})?");
                var challenge = challenges[ReadInt(1, challenges.Length) - 1];
                Write("Which part would you like to run (1-2)?");
                var part = ReadInt(1, 2);

                Write("Calculating...");
                var stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();
                var answer = part == 1 ? challenge.Part1() : challenge.Part2();
                stopwatch.Stop();
                Write($"Answer: {answer}");
                Write($"Took {stopwatch.ElapsedMilliseconds} ms");
                Write("\n");
            }
        }



        private static int ReadInt(int min, int max)
        {
            while (true)
            {
                var line = System.Console.ReadLine();
                if (!int.TryParse(line, out var val)) continue;
                if (val >= min && val <= max) return val;
            }
        }

        private static void Write(string msg)
        {
            System.Console.WriteLine(msg);
        }
    }
}
