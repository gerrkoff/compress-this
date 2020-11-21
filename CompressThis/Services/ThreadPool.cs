using System;
using System.Collections.Generic;
using System.Threading;
using CompressThis.Services.Interfaces;

namespace CompressThis.Services
{
    public class ThreadPool : IThreadPool
    {
        private readonly Queue<Action> _runQueue = new Queue<Action>();

        public ThreadPool()
        {
            var threadCount = Environment.ProcessorCount * 30;
            for (int i = 0; i < threadCount; i++)
            {
                new Thread(RunQueueConsume) {IsBackground = true}.Start();
            }
        }
        
        public Thread RunDedicated(Action action)
        {
            var thread = new Thread(() => action()) {IsBackground = true};
            thread.Start();
            return thread;
        }

        public void Run(Action action)
        {
            lock (_runQueue)
            {
                _runQueue.Enqueue(action);
                Monitor.Pulse(_runQueue);
            }
        }

        private void RunQueueConsume()
        {
            while (true)
            {
                Action action;
                lock (_runQueue)
                {
                    while (_runQueue.Count == 0)
                        Monitor.Wait(_runQueue);

                    action = _runQueue.Dequeue();
                }

                action();
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}