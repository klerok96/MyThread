using System;
using System.Threading;

namespace MyThread
{
    class Worker
    {
        int _workId;

        public Worker(int workId)
        {
            _workId = workId;
        }        

        public void Work(dynamic obj)
        {
            int delay = obj.Delay;
            CancellationToken token = obj.Token;
            ManualResetEvent mre = obj.Mre;

            for (int i = 0; i < 100; i++)
            {
                mre.WaitOne();
                token.ThrowIfCancellationRequested();

                Thread.Sleep(delay);

                ProcessChanged(i, _workId);
            }

            ProcessChanged(0, _workId);
            // return obj;
        }

        public event Action<int, int> ProcessChanged;
    }
}
