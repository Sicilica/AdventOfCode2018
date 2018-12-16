using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace AdventOfCode2018
{
    public class Day4 : Challenge
    {
        private static readonly Regex mainRegex = new Regex(@"^\[(\d{4}\-\d{2}\-\d{2}) (\d{2}):(\d{2})\] (.+)$", RegexOptions.Compiled);

        private static readonly Regex guardRegex = new Regex(@"^Guard #(\d+) begins shift$", RegexOptions.Compiled);
        private const string sleepText = "falls asleep";
        private const string wakeText = "wakes up";



        public override string Part1()
        {
            var guards = GetGuardsFromInput();

            var mostSleptGuardID = -1;
            var mostSleptDuration = -1;
            foreach (var guard in guards.Values)
            {
                var sleptDuration = guard.TotalMinutesAsleep;
                if (sleptDuration > mostSleptDuration)
                {
                    mostSleptDuration = sleptDuration;
                    mostSleptGuardID = guard.ID;
                }
            }

            return (mostSleptGuardID * guards[mostSleptGuardID].MostSleptMinute).ToString();
        }

        public override string Part2()
        {
            var guards = GetGuardsFromInput();

            var mostOverlapGuardID = -1;
            var mostOverlapMinute = -1;
            var mostOverlapTimes = -1;
            foreach (var guard in guards.Values)
            {
                var minute = guard.MostSleptMinute;
                var times = guard.GetTimesAsleepAt(minute);
                if (times > mostOverlapTimes)
                {
                    mostOverlapGuardID = guard.ID;
                    mostOverlapMinute = minute;
                    mostOverlapTimes = times;
                }
            }
            
            return (mostOverlapGuardID * mostOverlapMinute).ToString();
        }



        private Dictionary<int, Guard> GetGuardsFromInput()
        {
            var guards = new Dictionary<int, Guard>();
            var records = new Dictionary<string, Record>();

            using (var stream = GetResource("Day4/input.txt"))
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var match = mainRegex.Match(reader.ReadLine());
                    var date = DateTime.Parse(match.Groups[1].Value);
                    var hour = int.Parse(match.Groups[2].Value);
                    var minute = int.Parse(match.Groups[3].Value);
                    var action = match.Groups[4].Value;

                    if (hour == 23) date = date.AddDays(1);
                    else if (hour != 0) throw new Exception("Uh oh");

                    var recordID = date.ToShortDateString();

                    if (!records.ContainsKey(recordID)) records[recordID] = new Record();
                    var record = records[recordID];

                    if (action == sleepText)
                    {
                        record.SleepMinutes.Add(minute);
                    }
                    else if (action == wakeText)
                    {
                        record.WakeMinutes.Add(minute);
                    }
                    else
                    {
                        match = guardRegex.Match(action);
                        var guardID = int.Parse(match.Groups[1].Value);

                        if (!guards.ContainsKey(guardID)) guards[guardID] = new Guard(guardID);
                        var guard = guards[guardID];

                        guard.AddRecord(record);
                        record.GuardID = guardID;
                    }
                }
            }

            return guards;
        }



        private class Guard
        {
            private readonly List<Record> records = new List<Record>();

            public readonly int ID;


            
            public int MostSleptMinute
            {
                get
                {
                    var frequencies = new int[60];
                    int mostSleptIndex = 0;
                    foreach (var record in records)
                    {
                        foreach (var minute in record.GetSleepingMinutes())
                        {
                            frequencies[minute]++;
                            if (frequencies[minute] > frequencies[mostSleptIndex]) mostSleptIndex = minute;
                        }
                    }
                    return mostSleptIndex;
                }
            }

            public int TotalMinutesAsleep
            {
                get
                {
                    int total = 0;
                    foreach (var record in records)
                    {
                        total += record.SleepDuration;
                    }
                    return total;
                }
            }



            public Guard(int ID)
            {
                this.ID = ID;
            }



            public void AddRecord(Record record)
            {
                records.Add(record);
            }

            public int GetTimesAsleepAt(int minute)
            {
                int times = 0;
                foreach (var record in records)
                {
                    if (record.WasAsleepAt(minute)) times++;
                }
                return times;
            }
        }

        private class Record
        {
            public int GuardID;
            public readonly SortedSet<int> SleepMinutes = new SortedSet<int>();
            public readonly SortedSet<int> WakeMinutes = new SortedSet<int>();


            
            public int SleepDuration
            {
                get
                {
                    int value = 0;
                    foreach (var minute in WakeMinutes) value += minute;
                    foreach (var minute in SleepMinutes) value -= minute;
                    return value;
                }
            }



            public IEnumerable<int> GetSleepingMinutes()
            {
                var sleepMinEnum = SleepMinutes.GetEnumerator();
                var wakeMinEnum = WakeMinutes.GetEnumerator();
                while (sleepMinEnum.MoveNext() && wakeMinEnum.MoveNext())
                {
                    for (int minute = sleepMinEnum.Current; minute < wakeMinEnum.Current; minute++)
                    {
                        yield return minute;
                    }
                }
            }

            public bool WasAsleepAt(int minute)
            {
                var counter = 0;
                foreach (var sleepMinute in SleepMinutes)
                {
                    if (sleepMinute > minute) break;
                    counter++;
                }
                foreach (var wakeMinute in WakeMinutes)
                {
                    if (wakeMinute > minute) break;
                    counter--;
                }
                return counter > 0; // if counter isn't 0 or 1 then we've got some weird stuff
            }
        }
    }
}
