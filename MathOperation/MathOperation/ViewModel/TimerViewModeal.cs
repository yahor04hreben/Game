using MathOperation.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace MathOperation.ViewModel
{
    public class TimerViewModeal : AbstractViewModel
    {

        public Timer Timer { get; private set; }

        public TimerViewModeal()
        {
            Timer = new Timer();
            Timer.Start();
        }

        public TimerViewModeal(TimerViewModeal tv)
        {
            this.Timer = tv.Timer;
            this.Seconds = tv.Seconds;
            this.Minutes = tv.Minutes;
        }

        private int _Seconds;
        public int Seconds
        {
            get => _Seconds;
            set
            {
                _Seconds = value;
                if (_Seconds == 60)
                {
                    Minutes++;
                    _Seconds = 0;
                }
            }
        }

        public bool IsStoped { get; set; }

        private int _Minutes;
        public int Minutes
        {
            get => _Minutes;
            set
            {
                _Minutes = value;
            }
        }
        public void Start()
        {
            Seconds = 0;
            Minutes = 0;

            IsStoped = false;
            Timer.Enabled = true;
            Timer.Interval = 1000;
            Timer.Start();
        }

        public void Colapse()
        {
            Timer.Stop();
            Timer.Dispose();
        }

        public void Increment()
        {
            if(!IsStoped)
                Seconds++;
        }

        public void Stop()
        {
            IsStoped = true;
        }

        public void Resume()
        {
            IsStoped = false;
        }

        public string GetTime()
        {
            string secs = Seconds < 10 ? "0" + Seconds.ToString() : Seconds.ToString();
            string mins = Minutes < 10 ? "0" + Minutes.ToString() : Minutes.ToString();

            return mins + ':' + secs;
        }
    }
}
