using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public interface IJob
    {
        void Push(Action job);

    }
   
    public class Job : IJob
    {
        Queue<Action> _jobs = new();
        object _lock = new();
        bool _flush = false;

        public void Push(Action job)
        {
            bool flush = false;
            lock (_lock)
            { 
                _jobs.Enqueue(job); 
                if(_flush == false)
                {
                    flush = _flush = true;
                }
            }

            if(flush)
            {
                Flush();
            }
        }

        void Flush()
        {
            while(true)
            {
                Action action = Pop();
                if(action == null)
                {
                    return;
                }
                action.Invoke();
            }
        }

        Action Pop()
        {
            lock(_lock)
            {
                if(_jobs.Count == 0)
                {
                    _flush = false;
                    return null; 
                }
                return _jobs.Dequeue();
            }
        }
    }
}
