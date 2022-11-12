using FileParser;
using SheepTools.Model;
using System.Text;

namespace CatalystContest
{
    public class Contest_lvl3
    {
        public class Ghost
        {
            public Point Position { get; set; }
            public List<Direction> Direction { get; set; }

            public Ghost(Point position, List<Direction> directions)
            {
                Position = position;
                Direction = directions;
            }
        }

        private List<Direction> _directions = new();
        private Point _pacmanPosition;
        private List<Ghost> _ghostList;
        private List<List<Point>> _map = new();

        public void Run(string level)
        {
            ParseInput(level);

            string survived = "YES";
            var _pacmanPositionList = new List<Point>();

            for (int t = 0; t < _directions.Count; ++t)
            {
                var ghostPositionList = new List<Point>();

                var direction = _directions[t];
                _pacmanPosition = _pacmanPosition.Move(direction);

                foreach (var ghost in _ghostList)
                {
                    var ghostDirection = ghost.Direction[t];
                    ghost.Position = ghost.Position.Move(ghostDirection);
                    ghostPositionList.Add(ghost.Position);
                }

                if (ghostPositionList.Contains(_pacmanPosition) || _map[(int)_pacmanPosition.Y][(int)_pacmanPosition.X].Id == "W")
                {
                    survived = "NO";
                    break;
                }
                _pacmanPositionList.Add(_pacmanPosition);
            }

            var distinct = _pacmanPositionList.Distinct().ToList();
            var coins = _pacmanPositionList.Distinct().Count(p => _map[(int)p.Y][(int)p.X].Id == "C");

            using var sw = new StreamWriter($"Output/{level}.out");

            sw.WriteLine($"{coins} {survived}");
        }

        private void ParseInput(string level)
        {
            _ghostList = new();
            _directions = new();
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
            _directions = ParseMoves(file);

            var ghostCount = file.NextLine().NextElement<int>();
            for (int i = 0; i < ghostCount; ++i)
            {
                _ghostList.Add(new(ParsePosition(file), ParseMoves(file)));
            }

            if (!file.Empty)
            {
                throw new ParsingException("Error parsing file");
            }
        }

        private Point ParsePosition(ParsedFile file)
        {
            var line = file.NextLine();
            var yy = line.NextElement<int>() - 1;
            var xx = line.NextElement<int>() - 1;
            return new Point(_map[xx][yy].Id, xx, yy);
        }

        private static List<Direction> ParseMoves(ParsedFile file)
        {
            var numberOfMoves = file.NextLine().NextElement<int>();
            var result = new List<Direction>(numberOfMoves);
            var movesLine = file.NextLine();

            for (int i = 0; i < numberOfMoves; ++i)
            {
                result.Add(movesLine.NextElement<char>() switch
                {
                    'D' => Direction.Up,
                    'U' => Direction.Down,
                    'R' => Direction.Right,
                    'L' => Direction.Left,
                });
            }

            return result;
        }
    }
}
