using System;
using System.Threading;

namespace CompressThis.Services.Interfaces
{
    public interface IThreadPool
    {
        Thread RunDedicated(Action action);
        void Run(Action action);
    }
}