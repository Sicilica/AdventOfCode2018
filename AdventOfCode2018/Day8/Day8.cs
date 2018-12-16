using System.Collections.Generic;
using System.IO;

namespace AdventOfCode2018
{
    public class Day8 : Challenge
    {
        public override string Part1()
        {
            var root = GetTreeFromInput();

            return root.MetadataSum.ToString();
        }

        public override string Part2()
        {
            var root = GetTreeFromInput();

            return root.Value.ToString();
        }



        private Node GetTreeFromInput()
        {
            return ReadNode(GetInputAsInts());
        }



        private IEnumerator<int> GetInputAsInts()
        {
            using (var stream = GetResource("Day8/input.txt"))
            using (var reader = new StreamReader(stream))
            {
                int buffer = 0;

                while (true)
                {
                    var c = reader.EndOfStream ? ' ' : reader.Read();

                    if (c == ' ')
                    {
                        yield return buffer;
                        if (reader.EndOfStream) break;
                        else buffer = 0;
                    }
                    else buffer = (buffer * 10) + (c - '0');
                }
            }
        }

        private int ReadInt(IEnumerator<int> input)
        {
            input.MoveNext();
            return input.Current;
        }

        private Node ReadNode(IEnumerator<int> input)
        {
            var node = new Node(ReadInt(input), ReadInt(input));

            for (int i = 0; i < node.Children.Length; i++)
            {
                node.Children[i] = ReadNode(input);
            }

            for (int i = 0; i < node.Metadata.Length; i++)
            {
                node.Metadata[i] = ReadInt(input);
            }

            return node;
        }



        private class Node
        {
            public readonly Node[] Children;
            public readonly int[] Metadata;

            public int MetadataSum
            {
                get
                {
                    var sum = 0;
                    foreach (var child in Children) sum += child.MetadataSum;
                    foreach (var value in Metadata) sum += value;
                    return sum;
                }
            }

            public int Value
            {
                get
                {
                    if (Children.Length == 0) return MetadataSum;

                    var sum = 0;
                    foreach (var childIndex in Metadata)
                    {
                        if (childIndex <= Children.Length) sum += Children[childIndex - 1].Value;
                    }
                    return sum;
                }
            }

            public Node(int numChildren, int numMetadata)
            {
                Children = new Node[numChildren];
                Metadata = new int[numMetadata];
            }
        }
    }
}
