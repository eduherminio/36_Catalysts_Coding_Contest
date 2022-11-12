using FileParser;
using SheepTools.Model;
using System.Text;

namespace CatalystContest
{

    public class Contest_lvl4_incomplete
    {
        public record PointWithParent(List<List<Point>> Map, string Id, double X, double Y) : Point(Id, X, Y)
        {
            public List<(PointWithParent PreviousStep, int Coints)> PreviousSteps { get; set; } = new();
            public Direction? PreviousDirection { get; init; }
            public PointWithParent? Parent { get; init; }
            public int Hops { get; init; }
            public int Coins { get; internal set; }

            public new PointWithParent Move(Direction direction, double distance = 1)
            {
                var result = direction switch
                {
                    Direction.Right => this with { X = X + distance, Parent = this, PreviousDirection = direction, Hops = Hops + 1, Map = Map.Select(list => list.ToList()).ToList() },
                    Direction.Left => this with { X = X - distance, Parent = this, PreviousDirection = direction, Hops = Hops + 1, Map = Map.Select(list => list.ToList()).ToList() },
                    Direction.Up => this with { Y = Y + distance, Parent = this, PreviousDirection = direction, Hops = Hops + 1, Map = Map.Select(list => list.ToList()).ToList() },
                    Direction.Down => this with { Y = Y - distance, Parent = this, PreviousDirection = direction, Hops = Hops + 1, Map = Map.Select(list => list.ToList()).ToList() },
                    _ => throw new NotSupportedException($"Direction {direction} isn't supported yet")
                };

                var resultPointInMap = result.Map[(int)result.Y][(int)result.X];
                if (resultPointInMap.Id == "C")
                {
                    resultPointInMap.Id = "-";
                    result.Coins += 1;
                }

                return result;
            }

            #region Equals override

            public virtual bool Equals(PointWithParent? other) => base.Equals(other);

            public override int GetHashCode() => base.GetHashCode();

            #endregion
        }

        private PointWithParent _pacmanPosition;
        private List<List<Point>> _map = new();
        private long _maxMoves;

        public void Run(string level)
        {
            ParseInput(level);

            var _pacmanPositionList = new List<PointWithParent>();
            var totalCoins = _map.Sum(line => line.Count(p => p.Id == "C"));

            Stack<PointWithParent> stack = new(
            new[]
            {
                _pacmanPosition.Move(Direction.Up),
                _pacmanPosition.Move(Direction.Down),
                _pacmanPosition.Move(Direction.Right),
                _pacmanPosition.Move(Direction.Left)
            });

            while (stack.TryPop(out var newPacmanPosition))
            {
                if (newPacmanPosition.Coins == totalCoins)
                {
                    RecreateSolution(level, newPacmanPosition, totalCoins);
                    return;
                }

                var pointInMap = newPacmanPosition.Map[(int)newPacmanPosition.Y][(int)newPacmanPosition.X];
                if (pointInMap.Id == "W" || pointInMap.Id == "G" || newPacmanPosition.Hops > _maxMoves)
                {
                    continue;
                }

                var directions = new Direction[] { Direction.Up, Direction.Down, Direction.Right, Direction.Left };
                foreach (var dir in directions)
                {
                    var next = newPacmanPosition.Move(dir);
                    Console.WriteLine(next.Coins);
                    if (next.Coins == totalCoins)
                    {
                        ;
                    }
                    if (!next.PreviousSteps.Any(s => s.Coints == next.Coins && s.PreviousStep == next))
                    {
                        next.PreviousSteps = new(newPacmanPosition.PreviousSteps)
                        {
                            (newPacmanPosition, newPacmanPosition.Coins)
                        };
                        stack.Push(next);
                    }
                }
            }

            throw new();
        }

        private bool RecreateSolution(string level, PointWithParent end, int totalCoins)
        {
            var sb = new StringBuilder();
            var parentList = new HashSet<PointWithParent>(new[] { end });

            while (end.Parent is not null)
            {
                sb.Append(end.PreviousDirection switch
                {
                    Direction.Up => 'D',
                    Direction.Down => 'U',
                    Direction.Right => 'R',
                    Direction.Left => 'L',
                });
                end = end.Parent;
                parentList.Add(end);
            }

            if (parentList.Count(p => _map[(int)p.Y][(int)p.X].Id == "C") == totalCoins)
            {
                using var sw = new StreamWriter($"Output/{level}.out");
                sw.WriteLine(string.Join("", sb.ToString().Reverse()));

                return true;
            }

            return false;
        }

        private void ParseInput(string level)
        {
            _map = new();

            var file = new ParsedFile($"Inputs/{level}");
            var numberOfLines = file.NextLine().NextElement<int>();
            for (int i = 0; i < numberOfLines; ++i)
            {
                var line = file.NextLine();
                int x = 0;
                _map.Add(new());
                while (!line.Empty)
                {
                    _map.Last().Add(new Point(line.NextElement<char>().ToString(), x++, i));
                }
            }
            _pacmanPosition = ParsePosition(file);
            _maxMoves = file.NextLine().NextElement<long>();

            if (!file.Empty)
            {
                throw new ParsingException("Error parsing file");
            }
        }

        private PointWithParent ParsePosition(ParsedFile file)
        {
            var line = file.NextLine();
            var yy = line.NextElement<int>() - 1;
            var xx = line.NextElement<int>() - 1;
            return new PointWithParent(_map, _map[xx][yy].Id, xx, yy);
        }
    }
}
