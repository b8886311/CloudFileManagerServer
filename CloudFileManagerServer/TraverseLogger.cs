using System.Diagnostics;

namespace CloudFileManagerServer
{
    public class TraverseLogger
    {
        private List<string> _records { get; set; }

        private Stopwatch _stopwatch { get; set; }

        public TraverseLogger()
        {
            _records = new List<string>();
            _stopwatch = new Stopwatch();
        }

        public void Initialize()
        {
            _records.Clear();
            _stopwatch.Reset();
        }

        public void Start()
        {
            _stopwatch.Start();
        }   

        public void Stop()
        {
            _stopwatch.Stop();
            Log($"Traversal completed in {_stopwatch.ElapsedMilliseconds} ms.");
        }

        public void Log(string message)
        {
            _records.Add(message);
        }

        public List<string> GetTraverseLog()
        {
            return _records;
        }
    }
}
