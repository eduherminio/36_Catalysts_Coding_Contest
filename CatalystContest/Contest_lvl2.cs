using FileParser;
using SheepTools.Model;
using System.Text;

namespace CatalystContest
{
    public class Contest_lvl2
    {
        private List<Direction> _directions = new();
        private Point _pacmanPosition;
        private List<List<Point>> _map = new();

        public void Run(string level)
        {
            ParseInput(level);

            var _pacmanPositionList = new List<Point>();
            foreach (var direction in _directions)
            {
                _pacmanPosition = _pacmanPosition.Move(direction);
                _pacmanPositionList.Add(_pacmanPosition);
            }

            var coins = _pacmanPositionList.Distinct().Count(p => _map[(int)p.Y][(int)p.X].Id == "C");

            using var sw = new StreamWriter($"Output/{level}.out");

            sw.WriteLine(coins);
        }

        private void ParseInput(string level)
        {
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
            var pacmanLine = file.NextLine();
            var yy = pacmanLine.NextElement<int>() - 1;
            var xx = pacmanLine.NextElement<int>() - 1;
            _pacmanPosition = new Point("P", xx, yy);

            var numberOfMoves = file.NextLine().NextElement<int>();
            var movesLine = file.NextLine();
            for (int i = 0; i < numberOfMoves; ++i)
            {
                _directions.Add(movesLine.NextElement<char>() switch
                {
                    'D' => Direction.Up,
                    'U' => Direction.Down,
                    'R' => Direction.Right,
                    'L' => Direction.Left,
                });
            }

            if (!file.Empty)
            {
                throw new ParsingException("Error parsing file");
            }
        }
    }
}
