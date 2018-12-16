using System;
using System.Collections.Generic;
using System.IO;

namespace AdventOfCode2018
{
    public class Day12 : Challenge
    {
        private const int RELEVANT_NEIGHBORS = 2;
        const int CODE_WIDTH = RELEVANT_NEIGHBORS * 2 + 1;

        public override string Part1()
        {
            const long GENERATIONS = 20;

            ReadInput(out var initialPlants, out var rules);

            var resultPlants = Simulate(initialPlants, rules, GENERATIONS);

            long sum = 0;
            foreach (var plant in resultPlants)
            {
                sum += plant;
            }
            return sum.ToString();
        }

        public override string Part2()
        {
            const long GENERATIONS = 50000000000;

            ReadInput(out var plants, out var rules);

            long currentDelta = 0;
            long currentSpree = 0;

            long prevSum = 0;
            foreach (var plant in plants) prevSum += plant;

            long iteration;
            for (iteration = 0; iteration < GENERATIONS; iteration++)
            {
                plants = Simulate(plants, rules, 1);

                long sum = 0;
                foreach (var plant in plants) sum += plant;

                var newDelta = sum - prevSum;
                if (newDelta == currentDelta)
                {
                    if (++currentSpree >= 20) break;
                }
                else
                {
                    currentDelta = newDelta;
                    currentSpree = 1;
                }

                prevSum = sum;
            }

            return (prevSum + (GENERATIONS - iteration) * currentDelta).ToString();
        }


        
        private void ReadInput(out HashSet<long> initialPlants, out HashSet<long> rules)
        {
            initialPlants = new HashSet<long>();
            rules = new HashSet<long>();

            using (var stream = GetResource("Day12/input.txt"))
            using (var reader = new StreamReader(stream))
            {
                var initialStateText = reader.ReadLine().Substring("initial state: ".Length);
                for (int i = 0; i < initialStateText.Length; i++)
                {
                    if (initialStateText[i] == '#') initialPlants.Add(i);
                }

                reader.ReadLine();  // empty line

                while (!reader.EndOfStream)
                {
                    var ruleLine = reader.ReadLine();
                    if (!ruleLine.EndsWith("#")) continue;
                    var ruleText = ruleLine.Substring(0, CODE_WIDTH);

                    var ruleCode = 0;
                    for (int i = 0; i < CODE_WIDTH; i++)
                    {
                        if (ruleText[i] == '#') ruleCode += 1 << i;
                    }
                    rules.Add(ruleCode);
                }
            }
        }

        private HashSet<long> Simulate(HashSet<long> initialPlants, HashSet<long> rules, long generations)
        {
            var currentPlants = new HashSet<long>(initialPlants);
            var nextPlants = new HashSet<long>();
            var potsToCheck = new HashSet<long>();

            for (long generation = 0; generation < generations; generation++)
            {
                foreach (var plant in currentPlants)
                {
                    for (var delta = -RELEVANT_NEIGHBORS; delta <= RELEVANT_NEIGHBORS; delta++)
                    {
                        potsToCheck.Add(plant + delta);
                    }
                }

                foreach (var pot in potsToCheck)
                {
                    var code = 0;
                    for (var delta = -RELEVANT_NEIGHBORS; delta <= RELEVANT_NEIGHBORS; delta++)
                    {
                        if (currentPlants.Contains(pot + delta)) code += 1 << (delta + RELEVANT_NEIGHBORS);
                    }

                    if (rules.Contains(code)) nextPlants.Add(pot);
                }

                Swap(ref currentPlants, ref nextPlants);
                nextPlants.Clear();
                potsToCheck.Clear();
            }

            return currentPlants;
        }

        private void Swap<T>(ref T a, ref T b)
        {
            var tmp = a;
            a = b;
            b = tmp;
        }
    }
}
