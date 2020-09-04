using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easytl
{
    public class Semaphore
    {
        System.Threading.Semaphore Sem;
        /// <summary>
        /// 剩余信号量
        /// </summary>
        public int SurplusCount { get; set; } = 0;
        /// <summary>
        /// 最大信号量
        /// </summary>
        public int MaximumCount { get; set; } = 0;

        public Semaphore(int initialCount, int maximumCount)
        {
            SurplusCount = initialCount;
            MaximumCount = maximumCount;
            Sem = new System.Threading.Semaphore(initialCount, maximumCount);
        }

        public bool WaitOne(int millisecondsTimeout)
        {
            bool recive = Sem.WaitOne(millisecondsTimeout);
            if (recive)
                SurplusCount--;
            return recive;
        }

        public int Release()
        {
            if (SurplusCount < MaximumCount)
            {
                SurplusCount++;
                return Sem.Release();
            }
            else
                return MaximumCount;
        }

        public void Close()
        {
            Sem.Close();
        }

        public void Dispose()
        {
            Sem.Dispose();
        }
    }
}
