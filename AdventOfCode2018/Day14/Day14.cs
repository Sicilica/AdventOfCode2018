using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode2018
{
    public class Day14 : Challenge
    {
        private const int ELF_COUNT = 2;
        private static readonly int[] INITIAL_RECIPES = new int[] { 3, 7 };

        private const int PUZZLE_INPUT = 652601;

        public override string Part1()
        {
            const int SKIP_RECIPES = PUZZLE_INPUT;
            const int RETURN_RECIPES = 10;
            
            var elves = new int[ELF_COUNT];
            var recipes = new int[SKIP_RECIPES + RETURN_RECIPES + (int)Math.Log10(ELF_COUNT * 9) + 1];
            var recipeCount = INITIAL_RECIPES.Length;
            for (int i = 0; i < ELF_COUNT; i++) elves[i] = i;
            INITIAL_RECIPES.CopyTo(recipes, 0);

            while (recipeCount < SKIP_RECIPES + RETURN_RECIPES)
            {
                var sum = 0;
                foreach (var elf in elves) sum += recipes[elf];

                var newRecipeCount = sum == 0 ? 1 : (int)Math.Log10(sum) + 1;
                var divisor = (int)Math.Pow(10, newRecipeCount - 1);
                for (int i = 0; i < newRecipeCount; i++)
                {
                    recipes[recipeCount + i] = (sum / divisor) % 10;
                    divisor /= 10;
                }
                recipeCount += newRecipeCount;

                for (int i = 0; i < elves.Length; i++) elves[i] = (elves[i] + 1 + recipes[elves[i]]) % recipeCount;
            }

            var sb = new StringBuilder();
            for (int i = 0; i < RETURN_RECIPES; i++)
            {
                sb.Append(recipes[SKIP_RECIPES + i].ToString());
            }
            return sb.ToString();
        }

        public override string Part2()
        {
            var puzzleInputLength = (int)Math.Log10(PUZZLE_INPUT) + 1;

            var elves = new int[ELF_COUNT];
            var recipes = new List<int>(INITIAL_RECIPES);
            for (int i = 0; i < ELF_COUNT; i++) elves[i] = i;

            while (true)
            {
                var sum = 0;
                foreach (var elf in elves) sum += recipes[elf];

                var newRecipeCount = sum == 0 ? 1 : (int)Math.Log10(sum) + 1;
                var divisor = (int)Math.Pow(10, newRecipeCount - 1);
                for (int i = 0; i < newRecipeCount; i++)
                {
                    recipes.Add((sum / divisor) % 10);
                    divisor /= 10;
                }

                for (int i = newRecipeCount - 1; i >= 0; i--)
                {
                    if (recipes.Count - i < puzzleInputLength) continue;

                    sum = 0;
                    for (int j = puzzleInputLength - 1; j >= 0; j--)
                    {
                        sum *= 10;
                        sum += recipes[recipes.Count - i - j - 1];
                    }
                    if (sum == PUZZLE_INPUT)
                    {
                        return (recipes.Count - i - puzzleInputLength).ToString();
                    }
                }

                for (int i = 0; i < elves.Length; i++) elves[i] = (elves[i] + 1 + recipes[elves[i]]) % recipes.Count;
            }
        }
    }
}
