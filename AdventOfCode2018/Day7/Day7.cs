using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace AdventOfCode2018
{
    public class Day7 : Challenge
    {
        private static readonly Regex rx = new Regex(@"^Step ([A-Z]) must be finished before step ([A-Z]) can begin.$", RegexOptions.Compiled);



        public override string Part1()
        {
            var steps = GetStepsFromInput();

            var availableSteps = new SortedSet<char>();
            var incompletePrerequisitesForStep = new Dictionary<char, int>(steps.Count);
            foreach (var stepID in steps.Keys)
            {
                incompletePrerequisitesForStep[stepID] = steps[stepID].Prerequisites.Count;
                if (incompletePrerequisitesForStep[stepID] == 0) availableSteps.Add(stepID);
            }

            var solution = new char[steps.Count];
            for (int index = 0; index < solution.Length; index++)
            {
                char step = '?';
                foreach (var availableStep in availableSteps)
                {
                    step = availableStep;
                    break;
                }

                availableSteps.Remove(step);
                solution[index] = step;

                foreach (var dependent in steps[step].Dependents)
                {
                    if (--incompletePrerequisitesForStep[dependent] <= 0) availableSteps.Add(dependent);
                }
            }

            return new string(solution);
        }

        public override string Part2()
        {
            const int WORKER_COUNT = 5;
            const int BASE_STEP_TIME = 60;

            var steps = GetStepsFromInput();

            var availableSteps = new SortedSet<char>();
            var incompletePrerequisitesForStep = new Dictionary<char, int>(steps.Count);
            foreach (var stepID in steps.Keys)
            {
                incompletePrerequisitesForStep[stepID] = steps[stepID].Prerequisites.Count;
                if (incompletePrerequisitesForStep[stepID] == 0) availableSteps.Add(stepID);
            }

            var elapsedTime = -1;   // differences in implementations give an off by one error, sigh
            var workers = new WorkerState[WORKER_COUNT];
            while (true)
            {
                bool allIdle = true;

                for (int i = 0; i < WORKER_COUNT; i++)
                {
                    if (workers[i].Timer > 0)
                    {
                        allIdle = false;

                        if (--workers[i].Timer == 0)
                        {
                            foreach (var dependent in steps[workers[i].Step].Dependents)
                            {
                                if (--incompletePrerequisitesForStep[dependent] <= 0) availableSteps.Add(dependent);
                            }
                        }
                    }

                    if (workers[i].Timer == 0)
                    {
                        char step = '?';
                        foreach (var availableStep in availableSteps)
                        {
                            step = availableStep;
                            break;
                        }
                        if (step != '?')
                        {
                            allIdle = false;
                            availableSteps.Remove(step);
                            workers[i].Step = step;
                            workers[i].Timer = BASE_STEP_TIME + (step - 'A' + 1);
                        }
                    }
                }

                if (allIdle) return elapsedTime.ToString();
                elapsedTime++;
            }
        }



        private Dictionary<char, Step> GetStepsFromInput()
        {
            var steps = new Dictionary<char, Step>();

            using (var stream = GetResource("Day7/input.txt"))
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var match = rx.Match(reader.ReadLine());
                    var parent = match.Groups[1].Value[0];
                    var child = match.Groups[2].Value[0];

                    if (!steps.ContainsKey(parent)) steps[parent] = new Step();
                    steps[parent].Dependents.Add(child);

                    if (!steps.ContainsKey(child)) steps[child] = new Step();
                    steps[child].Prerequisites.Add(parent);
                }
            }

            return steps;
        }



        private class Step
        {
            public readonly HashSet<char> Prerequisites = new HashSet<char>();
            public readonly HashSet<char> Dependents = new HashSet<char>();
        }

        private struct WorkerState
        {
            public char Step;
            public int Timer;
        }
    }
}
