using System.Collections.Generic;

namespace JavaToSharp
{
    class StringTokenizer
    {
        private readonly Queue<string> _tokens; 

        public StringTokenizer(string line)
        {
            _tokens = new Queue<string>();
            ConstructFrom(line);
        }

        public StringTokenizer(IList<string> lines)
        {
            _tokens = new Queue<string>();
            int len = lines.Count;
            for (int i = 0; i < len; i++)
            {
                ConstructFrom(lines[i]);
            }
        }

        private void ConstructFrom(string line)
        {
            var items = line.Split(' ');
            int len = items.Length;
            for (int i = 0; i < len; i++)
            {
                _tokens.Enqueue(items[i]);
            }
        }

        public string nextToken()
        {
            return _tokens.Count == 0 ? string.Empty : _tokens.Dequeue();
        }

        public bool hasMoreTokens()
        {
            return _tokens.Count > 0;
        }
    }
}
