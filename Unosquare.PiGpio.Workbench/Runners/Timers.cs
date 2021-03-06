﻿namespace Unosquare.PiGpio.Workbench.Runners
{
    using Swan.Logging;
    using Swan.Threading;
    using System.Threading;

#pragma warning disable CA1001 // Types that own disposable fields should be disposable
    internal class Timers : RunnerBase
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
    {
        private Timer _currentTimer;
        private ManualResetEvent _timerTicked;
        private int _remainingTicks = 30;

        public Timers(bool isEnabled)
            : base(isEnabled) { }

        protected override void OnSetup()
        {
            _remainingTicks = 30;
            _timerTicked = new ManualResetEvent(false);
            _currentTimer = Board.Timing.StartTimer(500, () =>
            {
                _remainingTicks--;
                _timerTicked.Set();
            });
        }

        protected override void DoBackgroundWork(CancellationToken ct)
        {
            while (ct.IsCancellationRequested == false)
            {
                if (_timerTicked.WaitOne(50))
                {
                    $"Timer Ticked. Remaining Ticks: {_remainingTicks}".Info(Name);
                    _timerTicked.Reset();
                }

                if (_remainingTicks <= 0)
                    break;
            }
        }

        protected override void Cleanup()
        {
            _currentTimer.Dispose();
            _timerTicked.Dispose();
        }
    }
}
