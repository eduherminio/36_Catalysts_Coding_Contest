using FileParser;
using System.Text;

namespace CatalystContest
{
    public class Contest_lvl1
    {
        public void Run(string level)
        {
            var input = ParseInput(level).ToList();

            var output = input.Sum(line => line.Count(c => c == 'C'));

            using var sw = new StreamWriter($"Output/{level}.out");

            sw.WriteLine(output);
        }

        private IEnumerable<string> ParseInput(string level)
        {
            var file = new ParsedFile($"Inputs/{level}");
            var numberOfLines = file.NextLine().NextElement<int>();
            for (int i = 0; i < numberOfLines; ++i)
            {
                var line = file.NextLine();
                while (!line.Empty)
                {
                    yield return line.NextElement<string>();
                }
            }
            if (!file.Empty)
            {
                throw new ParsingException("Error parsing file");
            }
        }
    }
}
