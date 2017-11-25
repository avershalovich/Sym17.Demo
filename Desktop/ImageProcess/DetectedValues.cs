namespace ImageProcess
{
    using System.Collections.Generic;
    using System.Linq;

    public class DetectedValues
    {
        private float _minValue = 0;
        private byte _minCount = 5, _delta =0;
        private DetectResult _bestValue;
        private List<DetectResult> _results = new List<DetectResult>();

        public DetectedValues(byte minCount, float minValue)
        {
            _minCount = minCount;
            _minValue = minValue;
        }

        public int Count
        {
            get { return _results.Count; }
        }

        public float BestValue
        {
            get { return _results.Count==0 ? _minValue : _results.Max(x => x.SharpnessValue); }
        }

        public float WorstValue
        {
            get { return _results.Count == 0 ? _minValue : _results.Min(x => x.SharpnessValue); }
        }
        public bool Add(DetectResult value)
        {
            if (value.SharpnessValue <= _minValue)
            {
                return false;
            }

            if (_results.Count < _minCount)
            {
                _results.Add(value);
                _results = _results.OrderByDescending(x => x.SharpnessValue).ToList();
                _bestValue = _results.OrderByDescending(x => x.SharpnessValue).FirstOrDefault();
                return true;
            }

            if (_results.Count < _minCount + _delta)
            {
                var min = _results.Last();
                if (min.SharpnessValue < value.SharpnessValue)
                {
                    _results.Add(value);
                    _results = _results.OrderByDescending(x => x.SharpnessValue).ToList();
                    _bestValue = _results.OrderByDescending(x => x.SharpnessValue).FirstOrDefault();
                    return true;
                }
            }
            return false;
        }

        public DetectResult GetBestValue()
        {
            return _bestValue;
        }

        public void Clear()
        {
            _results = new List<DetectResult>();
            _minValue = 0;
        }

        public bool ReadyForRecognize
        {
            get { return _results.Count == _minCount + _delta; }
        }

        public List<DetectResult> GetBestValues(byte count)
        {
            return _results.OrderByDescending(x => x.SharpnessValue).Take(count).ToList();
        }
    }
}