using System;

namespace PressureSensorCheck.Workflow
{
    internal class Dpi620StateMoq
    {
        internal static Dpi620StateMoq Instance { get; } = new Dpi620StateMoq();

        Random _rnd = new Random();
        private double slot1Max = 1000.0;
        private double slot2Max = 1000.0;
        private bool _isOpened = false;

        private Dpi620StateMoq() { }

        public double GetValue(int slot)
        {
            if (!_isOpened)
                return 0.0;
            var val = _rnd.NextDouble() * (slot == 1 ? slot1Max : slot2Max);
            return val;
        }

        public void SetUnit(int slot, string unit)
        {
        }

        public void Start()
        {
            _isOpened = true;
        }

        public void Stop()
        {
            _isOpened = false;
        }
    }
}