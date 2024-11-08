using System;
using System.Text;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{
    internal struct JobTimerElem : IComparable<JobTimerElem>
    {
        public int excTick;
        public Action action;
        public int CompareTo(JobTimerElem other)
        {
            return other.excTick - excTick;
        }
    }

    class JobTimer
    {
        public static JobTimer Instance { get; } = new JobTimer();
        PriorityQueue<JobTimerElem> _pq = new();
        object _lock = new();

        public void Push(Action action, int tickAfter = 0)
        {
            JobTimerElem job;
            job.excTick = tickAfter;
            job.action = action;

            lock(_lock)
            {
                _pq.Push(job);
            }
        }

        public void Flush()
        {
            while(true)
            {
                int now = System.Environment.TickCount;
                JobTimerElem job;
                lock(_lock)
                {
                    if(_pq.Count == 0)
                    {
                        break;
                    }

                    job = _pq.Peek();
                    if(job.excTick > now)
                    {
                        break;
                    }
                    _pq.Pop();
                }
                job.action.Invoke();
            }
        }
    }
}
