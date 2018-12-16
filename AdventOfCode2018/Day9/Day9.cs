using System;
using System.Collections.Generic;

namespace AdventOfCode2018
{
    public class Day9 : Challenge
    {
        public override string Part1()
        {
            return CalculateHighScore(430, 71588).ToString();
        }

        public override string Part2()
        {
            return CalculateHighScore(430, 71588 * 100).ToString();
        }



        private long CalculateHighScore(int numPlayers, int lastMarble)
        {
            long highScore = 0;
            var playerScores = new long[numPlayers];
            var circle = new LinkedList<int>();
            circle.AddFirst(0);
            var cursor = circle.First;
            for (int marble = 1; marble <= lastMarble; marble++)
            {
                if (marble % 23 == 0)
                {
                    var activePlayer = marble % numPlayers;

                    playerScores[activePlayer] += marble;

                    for (int i = 0; i < 7; i++)
                    {
                        cursor = cursor == circle.First ?  circle.Last : cursor.Previous;
                    }
                    playerScores[activePlayer] += cursor.Value;

                    var newCursor = cursor == circle.Last ? circle.First : cursor.Next;
                    circle.Remove(cursor);
                    cursor = newCursor;

                    highScore = Math.Max(highScore, playerScores[activePlayer]);
                }
                else
                {
                    cursor = cursor == circle.Last ? circle.First : cursor.Next;

                    circle.AddAfter(cursor, marble);
                    cursor = cursor.Next;
                }
            }

            return highScore;
        }
    }
}
