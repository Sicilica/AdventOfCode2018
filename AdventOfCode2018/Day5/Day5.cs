using System;
using System.Collections.Generic;
using System.IO;

namespace AdventOfCode2018
{
    public class Day5 : Challenge
    {
        public override string Part1()
        {
            var polymer = GetPolymerFromInput();
            React(polymer);
            return polymer.Count.ToString();
        }

        public override string Part2()
        {
            var polymer = GetPolymerFromInput();

            var smallestSize = int.MaxValue;
            foreach (var unitType in GetUnitTypes(polymer))
            {
                var copy = new List<Unit>(polymer);
                RemoveUnitsOfType(copy, unitType);
                React(copy);
                smallestSize = Math.Min(smallestSize, copy.Count);
            }
            return smallestSize.ToString();
        }



        private List<Unit> GetPolymerFromInput()
        {
            using (var stream = GetResource("Day5/input.txt"))
            using (var reader = new StreamReader(stream))
            {
                var polymerString = reader.ReadToEnd();
                var polymer = new List<Unit>(polymerString.Length);
                foreach (var c in polymerString)
                {
                    polymer.Add(new Unit(c));
                }
                return polymer;
            }
        }

        private IEnumerable<char> GetUnitTypes(List<Unit> polymer)
        {
            var seenTypes = new HashSet<char>();
            foreach (var unit in polymer)
            {
                if (seenTypes.Contains(unit.Type)) continue;
                seenTypes.Add(unit.Type);
                yield return unit.Type;
            }
        }

        private void React(List<Unit> polymer)
        {
            int index = polymer.Count - 2;
            while (index >= 0)
            {
                var thisUnit = polymer[index];
                var prevUnit = polymer[index + 1];

                if (thisUnit.Type == prevUnit.Type && thisUnit.Polarity != prevUnit.Polarity)
                {
                    polymer.RemoveRange(index, 2);
                    while (index >= polymer.Count - 1) index--;
                }
                else index--;
            }
        }

        private void RemoveUnitsOfType(List<Unit> polymer, char unitType)
        {
            int index = polymer.Count - 1;
            while (index >= 0)
            {
                if (polymer[index].Type == unitType) polymer.RemoveAt(index);
                index--;
            }
        }


        
        private struct Unit
        {
            public readonly char Type;
            public readonly bool Polarity;



            public Unit(char raw)
            {
                Polarity = raw < 'a';
                Type = Polarity ? raw : (char)(raw - 'a' + 'A');
            }
        }
    }
}
