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

    public struct Vector2 {

        public int x, y;

        public Vector2(int x, int y) => (this.x, this.y) = (x, y);
        public Vector2(Vector2 v) => (x, y) = (v.x, v.y);

        public static Vector2 operator +(Vector2 a, Vector2 b) => new(a.x + b.x, a.y + b.y);
        public static Vector2 operator -(Vector2 a, Vector2 b) => new(a.x - b.x, a.y - b.y);
        public static Vector2 operator *(Vector2 a, Vector2 b) => new(a.x * b.x, a.y * b.y);
        public static Vector2 operator /(Vector2 a, Vector2 b) => new(a.x / b.x, a.y / b.y);
        public static Vector2 operator -(Vector2 v) => new(-v.x, -v.y);

        public override readonly string ToString() => $"({x}, {y})";
    }

    public static class Program {

        public static void print(object? msg = null) => Console.Write(msg);
        public static void println(object? msg = null) => Console.WriteLine(msg);

        public static bool read(out string oot) => read(null, out oot);
        public static bool read(string? msg, out string oot) {
            print(msg);
            oot = Console.ReadLine() ?? "";
            return oot != "";
        }

        public static void Main() {
            println("Start\n");
            ChooseProgram();
            println("\nEnd");
        }

        private static void ChooseProgram() {

            while (read("Select day: ", out var num)) {

                var day = typeof(Program).GetMethod($"Day{num}");

                if (day == null) println("Invalid Day");
                else {
                    string inputPath = $"Day{num}_input.txt";

                    if (File.Exists(inputPath)) day.Invoke(null, new[] { File.ReadAllText(inputPath) });
                    else {
                        File.Create(inputPath);
                        println($"Input file not found. Generated {inputPath}");
                    }
                }

                println();
            }
        }

        #region --- Day 1: Trebuchet?! ---

        public static void Day1(string input) {

            string[] lines = input.Split('\n');

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

        public static void Day2(string input) {

            string[] lines = input.Split('\n');

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

        #region --- Day 3: Gear Ratios ---

        private readonly struct Number {

            public Number(Vector2[] positions, int value) => (this.positions, this.value) = (positions, value);

            public readonly Vector2[] positions;
            public readonly int value;

            public override readonly string ToString() => $"{value} : {{ {string.Join(", ", positions)} }}";
        }

        public static void Day3(string input) {

            string[] lines = input.Split("\n");

            var validDeltas = new Vector2[] {
                new( 0,  1),
                new( 1,  1),
                new( 1,  0),
                new( 1, -1),
                new( 0, -1),
                new(-1, -1),
                new(-1,  0),
                new(-1,  1),
            };

            int height = lines.Length,
                width = lines[0].Length;

            List<Number> numbers = new();

            List<Vector2> activeNumberPositions = null;
            int activeNumberValue = 0;

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)

                    if (int.TryParse(lines[y][x].ToString(), out int value))
                        if (activeNumberPositions != null) {
                            activeNumberPositions.Add(new(x, y));
                            activeNumberValue *= 10;
                            activeNumberValue += value;
                        } else {
                            activeNumberPositions = new() { new(x, y) };
                            activeNumberValue = value;
                        }

                    else if (activeNumberPositions != null) {
                        numbers.Add(new(activeNumberPositions.ToArray(), activeNumberValue));
                        activeNumberPositions = null;
                    }

            List<Number> notMissing = new();
            Dictionary<Vector2, List<int>> gears = new();

            foreach (var number in numbers) {

                foreach (var point in number.positions) {
                    foreach (var delta in validDeltas) {

                        Vector2 pos = point + delta;
                        if (pos.x >= width || pos.x < 0 || pos.y >= height || pos.y < 0) continue;

                        char check = lines[pos.y][pos.x];
                        if (check != '.' && !char.IsNumber(check)) {

                            notMissing.Add(number);

                            if (check == '*')
                                if (gears.TryGetValue(pos, out var values)) values.Add(number.value);
                                else gears.Add(pos, new() { number.value });

                            goto notMissing;
                        }
                    }
                }

                notMissing: continue;
            }

            int sum = 0;
            foreach (var num in notMissing) sum += num.value;

            int gearRatioSum = 0;
            foreach (var gear in gears.Values)
                if (gear.Count == 2)
                    gearRatioSum += gear[0] * gear[1];

            println($"part sum: {sum}\ngear ratio sum: {gearRatioSum}");
        }

        #endregion

        #region

        public static void Day4(string input) {

        }

        #endregion
    }
}
