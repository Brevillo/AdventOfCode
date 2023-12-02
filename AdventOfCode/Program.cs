/* Made by Oliver Beebe 2023 */

// https://adventofcode.com/2023

using System;
using System.Text;

namespace AdventOfCode {

    public static class Util {
        
        public static string Replace(this string s, int index, char c) {
            var sb = new StringBuilder(s);
            sb[index] = c;
            return sb.ToString();
        }
    }

    public static class Program {

        public static void print(object? msg = null) => Console.Write(msg);
        public static void println(object? msg = null) => Console.WriteLine(msg);
        public static string read() => Console.ReadLine() ?? "";

        public static void Main() {
            println("Start");
            ChooseProgram();
            println("End");
        }

        private static void ChooseProgram() {

            Action? program = read() switch {

                "1" => Day1,
                "2" => Day2,
                "3" => Day3,

                _   => null,
            };

            program?.Invoke();
        }

        #region --- Day 1: Trebuchet?! ---

        private const string day1_input = "Day1_input.txt";

        public static void Day1() {

            string[] lines = File.ReadAllText(day1_input).Split('\n');

            var namedNums = new List<string>() {
                "zero",
                "one",
                "two",
                "three",
                "four",
                "five",
                "six",
                "seven",
                "eight",
                "nine",
            };

            int sum = 0;

            for (int l = 0; l < lines.Length; l++) {

                var line = lines[l];

                int? n1 = null,
                     n2 = null;

                List<(int index, char found)> found = new();

                for (int i = 0; i < namedNums.Count; i++) {

                    int index = 0;
                    string edited = new(line);

                    while (index != -1) {
                        index = edited.IndexOf(namedNums[i]);

                        if (index != -1) {
                            char c = i.ToString()[0];
                            found.Add((index, c));
                            edited = edited.Replace(index, c);
                        }
                    }
                }

                foreach (var f in found)
                    line = line.Replace(f.index, f.found);

                for (int i = 0; i < line.Length; i++)
                    if (int.TryParse(line[i].ToString(), out int num))
                        if (n1 == null) n1 = num;
                        else n2 = num;

                if (n1 == null) {
                    println($"No number was found in line: {line}");
                    return;
                }

                int num1 = (int)n1,
                    lineNum = num1 * 10 + (n2 ?? num1);

                sum += lineNum;
            }

            println(sum);
        }

        #endregion

        #region --- Day 2: Cube Conundrum ---

        private const string day2_input = "Day2_input.txt";

        private readonly struct Game {

            public Game(string line) {

                var data = line.Split(':', ';');

                ID = int.Parse(data[0].Remove(0, 4));

                sets = new();

                foreach (var point in data[1..]) {

                    Set set = new(0);

                    var groups = point.Split(',');

                    foreach (var group in groups) {

                        var info = group.Split(" ");
                        int count = int.Parse(info[1]);

                        foreach (var color in set.colors)
                            if (group.Contains(color)) {
                                set[color] += count;
                                break;
                            }
                    }

                    sets.Add(set);
                }
            }

            public readonly int ID;
            public readonly List<Set> sets;
        }

        private readonly struct Set {

            public Set(int init) => counts = new() {
                { "red", init },
                { "green", init },
                { "blue", init },
            };

            public int this[string color] {
                get => counts[color];
                set => counts[color] = value;
            }

            public string[] colors => counts.Keys.ToArray();

            private readonly Dictionary<string, int> counts;

            public override string ToString() => $"Set: {string.Join(", ", counts.Values)}";
        }

        public static void Day2() {

            string[] lines = File.ReadAllText(day2_input).Split('\n');

            var games = new List<string>(lines).ConvertAll(line => new Game(line));

            Dictionary<string, int> goals = new() {
                { "red", 12 },
                { "green", 13 },
                { "blue", 14 },
            };

            int possibilitySum = 0;

            foreach (var game in games) {

                foreach (var set in game.sets)
                    foreach (var goal in goals)
                        if (set[goal.Key] > goal.Value)
                            goto fail;

                possibilitySum += game.ID;

                fail: continue;
            }

            println($"possiblities: {possibilitySum}");

            int minimumSum = 0;

            foreach (var game in games) {

                Set min = new(0);

                foreach (var set in game.sets) {
                    foreach (var color in min.colors)
                        min[color] = (int)MathF.Max(min[color], set[color]);
                }

                int pow = 1;
                foreach (var color in min.colors)
                    pow *= min[color];

                minimumSum += pow;
            }

            println($"minimum: {minimumSum}");
        }

        #endregion

        #region --- Day 3: ?????? ---

        private const string day3_input = "Day3_input.txt";

        public static void Day3() {

        }

        #endregion
    }
}
