using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AdventOfCode2018
{
    public class Day16 : Challenge
    {
        private static readonly IEnumerable<Opcode> allOpcodes = (IEnumerable<Opcode>)Enum.GetValues(typeof(Opcode));



        public override string Part1()
        {
            ReadInput(out var examples, out var instructions);

            var ambiguousExampleCount = 0;
            foreach (var example in examples)
            {
                if (example.GetPossibleOpcodes().Count >= 3) ambiguousExampleCount++;
            }

            return ambiguousExampleCount.ToString();
        }

        public override string Part2()
        {
            ReadInput(out var examples, out var instructions);

            var opcodePossibilities = new Dictionary<int, HashSet<Opcode>>();
            var opcodeMap = new Dictionary<int, Opcode>();

            for (int i = 0; i < 16; i++)
            {
                opcodePossibilities[i] = new HashSet<Opcode>(allOpcodes);
            }

            var invalidOpcodes = new HashSet<Opcode>();
            foreach (var example in examples)
            {
                if (opcodeMap.ContainsKey(example.Instruction.Opcode)) continue;

                var possibleOpcodes = opcodePossibilities[example.Instruction.Opcode];
                var examplePossibleOpcodes = example.GetPossibleOpcodes();
                foreach (var opcode in possibleOpcodes)
                {
                    if (!examplePossibleOpcodes.Contains(opcode)) invalidOpcodes.Add(opcode);
                }
                foreach (var opcode in invalidOpcodes)
                {
                    possibleOpcodes.Remove(opcode);
                }
                invalidOpcodes.Clear();

                if (possibleOpcodes.Count == 1)
                {
                    Opcode opcode = 0;
                    foreach (var possibleOpcode in possibleOpcodes)
                    {
                        opcode = possibleOpcode;
                        break;
                    }
                    opcodeMap[example.Instruction.Opcode] = opcode;
                    foreach (var setOfPossibleOpcodes in opcodePossibilities.Values) setOfPossibleOpcodes.Remove(opcode);
                }
            }

            var state = new int[4];
            foreach (var instruction in instructions) instruction.Evaluate(state, opcodeMap);

            return state[0].ToString();
        }


        
        private void ReadInput(out List<Example> examples, out List<Instruction> instructions)
        {
            examples = new List<Example>();
            instructions = new List<Instruction>();

            using (var stream = GetResource("Day16/input.txt"))
            using (var reader = new System.IO.StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var firstLine = reader.ReadLine();
                    if (firstLine.Length == 0) break;

                    examples.Add(new Example(firstLine, reader.ReadLine(), reader.ReadLine()));

                    reader.ReadLine();
                }

                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    instructions.Add(new Instruction(reader.ReadLine()));
                }
            }
        }



        private struct Example
        {
            private static readonly Regex stateRegex = new Regex(@"^.{8}\[(\d+), (\d+), (\d+), (\d+)\]$", RegexOptions.Compiled);
            private static readonly Dictionary<int, Opcode> tmpOpcodeMap = new Dictionary<int, Opcode>();

            public readonly int[] InitialState;
            public readonly Instruction Instruction;
            public readonly int[] ResultState;

            public Example(string firstLine, string secondLine, string thirdLine)
            {
                InitialState = ParseState(firstLine);
                Instruction = new Instruction(secondLine);
                ResultState = ParseState(thirdLine);
            }



            public HashSet<Opcode> GetPossibleOpcodes()
            {
                var possibleOpcodes = new HashSet<Opcode>();

                var state = new int[4];
                foreach (var opcode in allOpcodes)
                {
                    InitialState.CopyTo(state, 0);
                    tmpOpcodeMap[Instruction.Opcode] = opcode;

                    Instruction.Evaluate(state, tmpOpcodeMap);

                    if (StatesAreEqual(state, ResultState)) possibleOpcodes.Add(opcode);
                }

                return possibleOpcodes;
            }



            private static bool StatesAreEqual(int[] a, int[] b)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (a[i] != b[i]) return false;
                }
                return true;
            }
            private static int[] ParseState(string line)
            {
                var match = stateRegex.Match(line);
                return new int[]
                {
                    int.Parse(match.Groups[1].Value),
                    int.Parse(match.Groups[2].Value),
                    int.Parse(match.Groups[3].Value),
                    int.Parse(match.Groups[4].Value)
                };
            }
        }

        private struct Instruction
        {
            public readonly int Opcode;
            public readonly int InputA;
            public readonly int InputB;
            public readonly int Output;

            public Instruction(string raw)
            {
                var parts = raw.Trim().Split(' ');
                Opcode = int.Parse(parts[0]);
                InputA = int.Parse(parts[1]);
                InputB = int.Parse(parts[2]);
                Output = int.Parse(parts[3]);
            }


            
            public void Evaluate(int[] state, Dictionary<int, Opcode> opcodeMap)
            {
                switch (opcodeMap[Opcode])
                {
                    case Day16.Opcode.addr:
                        state[Output] = state[InputA] + state[InputB];
                        break;
                    case Day16.Opcode.addi:
                        state[Output] = state[InputA] + InputB;
                        break;
                    case Day16.Opcode.mulr:
                        state[Output] = state[InputA] * state[InputB];
                        break;
                    case Day16.Opcode.muli:
                        state[Output] = state[InputA] * InputB;
                        break;
                    case Day16.Opcode.banr:
                        state[Output] = state[InputA] & state[InputB];
                        break;
                    case Day16.Opcode.bani:
                        state[Output] = state[InputA] & InputB;
                        break;
                    case Day16.Opcode.borr:
                        state[Output] = state[InputA] | state[InputB];
                        break;
                    case Day16.Opcode.bori:
                        state[Output] = state[InputA] | InputB;
                        break;
                    case Day16.Opcode.setr:
                        state[Output] = state[InputA];
                        break;
                    case Day16.Opcode.seti:
                        state[Output] = InputA;
                        break;
                    case Day16.Opcode.gtir:
                        state[Output] = InputA > state[InputB] ? 1 : 0;
                        break;
                    case Day16.Opcode.gtri:
                        state[Output] = state[InputA] > InputB ? 1 : 0;
                        break;
                    case Day16.Opcode.gtrr:
                        state[Output] = state[InputA] > state[InputB] ? 1 : 0;
                        break;
                    case Day16.Opcode.eqir:
                        state[Output] = InputA == state[InputB] ? 1 : 0;
                        break;
                    case Day16.Opcode.eqri:
                        state[Output] = state[InputA] == InputB ? 1 : 0;
                        break;
                    case Day16.Opcode.eqrr:
                        state[Output] = state[InputA] == state[InputB] ? 1 : 0;
                        break;
                }
            }
        }

        private enum Opcode
        {
            addr,
            addi,
            mulr,
            muli,
            banr,
            bani,
            borr,
            bori,
            setr,
            seti,
            gtir,
            gtri,
            gtrr,
            eqir,
            eqri,
            eqrr
        }
    }
}
