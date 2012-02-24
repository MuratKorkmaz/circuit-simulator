using System.Collections.Generic;

namespace JavaToSharp
{
    class StringTokenizer
    {
        private readonly Queue<string> _tokens; 

        public StringTokenizer(string line)
        {
            _tokens = new Queue<string>();
            var items = line.Split(' ');
            foreach (var item in items)
            {
                _tokens.Enqueue(item);
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
