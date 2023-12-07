/* Made by Oliver Beebe 2023 */

// https://adventofcode.com/2023

namespace AdventOfCode {

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

        #region Util

        public static void print(object? msg = null) => Console.Write(msg);
        public static void println(object? msg = null) => Console.WriteLine(msg);
        public static bool read(string? msg, out string oot) {
            print(msg);
            oot = Console.ReadLine() ?? "";
            return oot != "";
        }
        
        public static string Replace(this string s, int index, char c) {
            var sb = s.ToCharArray();
            sb[index] = c;
            return string.Concat(sb);
        }

        public static int Sign(this int i) => i > 0 ? 1 : i < 0 ? -1 : 0;

        #endregion

        #region Day Selection

        public static void Main() {

            println("Start\n");

            while (read("Select day: ", out var num)) {

                var day = typeof(Program).GetMethod($"Day{num}");

                if (day == null) {
                    println("Invalid Day");
                    continue;
                }

                string inputPath = $"Day{num}_input.txt";

                if (!File.Exists(inputPath)) {
                    println($"Input file not found. Generated {inputPath}");
                    File.Create(inputPath);
                    continue;
                }

                day.Invoke(null, new[] { File.ReadAllText(inputPath) });

                println();
            }

            println("\nEnd");
        }

        #endregion

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

        #region --- Day 4: Scratchcards ---

        public static void Day4(string input) {

            string[] lines = input.Split("\n");

            for (int i = 0; i < lines.Length; i++)
                lines[i] = lines[i][(lines[i].IndexOf(':') + 1)..];

            int[] numCards = new int[lines.Length];
            Array.Fill(numCards, 1);

            int pointSum = 0;
            for (int i = 0; i < lines.Length; i++) {

                var sides = lines[i].Split('|');
                List<int> GetNums(string sequence) {
                    var nums = new List<string>(sequence.Split(' ')).FindAll(n => n != "");
                    var ints = nums.ConvertAll(int.Parse);
                    return ints;
                }
                List<int> winning = GetNums(sides[0]),
                          mine = GetNums(sides[1]);

                int nums = 0;

                foreach (var num in winning)
                    if (mine.Contains(num)) nums++;

                for (int num = i + 1; num < i + nums + 1; num++)
                    numCards[num] += numCards[i];

                pointSum += (int)Math.Pow(2, nums - 1);
            }

            int cardSum = numCards.Sum();

            println($"Total points: {pointSum}\nTotal cards: {cardSum}");
        }

        #endregion

        #region --- Day 5: If You Give A Seed A Fertilizer ---

        private readonly struct Map {

            public static Map Build(string line) => new(line);

            private Map(string line) {
                var nums = Array.ConvertAll(line.Split(" "), long.Parse);
                end   = nums[0];
                start = nums[1];
                range = nums[2];
            }

            private readonly long end, start, range;

            public readonly bool Try(long input, out long output) {
                output = input + end - start;
                return input >= start && input <= start + range;
            }

            public readonly void ClampRange(ref long min, ref long max) {
                min = Math.Min(min, start);
                max = Math.Max(max, end);
            }
        }

        public static void Day5(string input) {

            var groups = input.Split("\n\n");

            long[] ranges = new List<string>(groups[0][7..].Split(" ")).ConvertAll(long.Parse).ToArray();

            var mapGroups =
                new List<string>(groups[1..]) // map groups
                .ConvertAll(l => new List<string>(l.Split("\n")[1..]) // map data
                .ConvertAll(Map.Build)); // map struct

            void GetRange(ref long start, ref long end) {

                foreach (var group in mapGroups)
                    foreach (var map in group)
                        map.ClampRange(ref start, ref end);
            }

            long DoMap(long value) {

                foreach (var map in mapGroups[0])
                    if (map.Try(value, out var o)) {
                        value = o;
                        break;
                    }

                return value;
            }

            long basicMin = Array.ConvertAll(ranges, DoMap).Min(),
                 rangeMin = long.MaxValue;

            for (long i = 0; i < ranges.Length; i += 2) {

                long start = 0,
                     end = long.MaxValue;

                print($"range: {start}, {end} -> ");

                GetRange(ref start, ref end);
                start = Math.Max(start, ranges[i]);
                end   = Math.Min(end, ranges[i] + ranges[i + 1]);

                println($"{start}, {end}");

                for (long n = start; n < end; n++)
                    rangeMin = Math.Min(rangeMin, DoMap(n));
            }

            println($"basic min: {basicMin}\nrange min: {rangeMin}");
        }

        #endregion

        #region --- Day 6: Wait For It ---

        public static void Day6(string input) {

            var lines = Array.ConvertAll(input.Split("\n"), line => line[11..].Split(" ", StringSplitOptions.RemoveEmptyEntries));

            long Dist(long time, long dist) {

                long Bound(long sign) {
                    long result = (long)Math.Floor((-sign * Math.Sqrt(time * time - 4 * dist) - time) / -2d);
                    if (result * (time - result) - dist == 0 && sign > 0) result++;
                    return result;
                }

                return Bound(1) - Bound(-1);
            }

            long[] GetLongs(int line) => Array.ConvertAll(lines[line], long.Parse);
            long[] times = GetLongs(0),
                   dists = GetLongs(1);

            long separate = 1;
            for (int i = 0; i < times.Length; i++)
                separate *= Dist(times[i], dists[i]);

            long Total(int line) => long.Parse(string.Concat(lines[line]));
            long single = Dist(Total(0), Total(1));

            println($"separate product: {separate}\nsingle product: {single}");
        }

        #endregion

        #region --- Day 7: Camel Cards ---

        public static void Day7(string input) {

            var handBids = Array.ConvertAll(input.Split("\n"), line => {
                var nums = line.Split(" ");
                return (hand: nums[0], bid: int.Parse(nums[1]));
            });

            List<char> cards = new() { '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A' };

            List<Func<(int[] counts, int jokers), bool>> types = new() {
                d => d.counts.Contains(5 - d.jokers) || d.jokers == 5,          // five of a kind
                d => d.counts.Contains(4 - d.jokers),                           // four of a kind
                d => d.counts.Length == 2 && d.counts.Contains(3 - d.jokers),   // full house
                d => d.counts.Contains(3 - d.jokers),                           // three of a kind
                d => d.counts.Count(c => c == 2) == 2,                          // two pair
                d => d.counts.Contains(2 - d.jokers),                           // one pair
                d => true,                                                      // high card
            };

            int GetSum(bool consideringJokers) {

                int Type(string hand) {

                    Dictionary<char, int> charCounts = new();
                    int jokers = 0;

                    foreach (var c in hand)
                        if (consideringJokers && c == 'J') jokers++;
                        else if (!charCounts.ContainsKey(c)) charCounts.Add(c, 1);
                        else charCounts[c]++;

                    var counts = charCounts.Values.ToArray();

                    return types.FindIndex(type => type.Invoke((counts, jokers)));
                }

                int CompareHands((string hand, int bid) hb1, (string hand, int bid) hb2) {

                    int t1 = Type(hb1.hand),
                        t2 = Type(hb2.hand);

                    if (t1 != t2) return (t1 - t2).Sign();

                    for (int i = 0; i < 5; i++) {

                        char c1 = hb1.hand[i],
                             c2 = hb2.hand[i];

                        if (c1 == c2) continue;

                        int i1 = consideringJokers && c1 == 'J' ? -1 : cards.IndexOf(c1),
                            i2 = consideringJokers && c2 == 'J' ? -1 : cards.IndexOf(c2);

                        return (i2 - i1).Sign();
                    }

                    return 0;
                }

                Array.Sort(handBids, CompareHands);

                int sum = 0;
                for (int i = 0; i < handBids.Length; i++)
                    sum += handBids[i].bid * (handBids.Length - i);

                return sum;
            }

            println($"no jokers: {GetSum(false)}\nwith jokers: {GetSum(true)}");
        }

        #endregion

        #region

        public static void Day8(string input) {

        }

        #endregion
    }
}
