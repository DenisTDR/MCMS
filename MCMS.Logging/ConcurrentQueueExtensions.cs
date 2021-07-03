using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MCMS.Logging
{
    public static class ConcurrentQueueExtensions
    {
        public static List<T> Dequeue<T>(this ConcurrentQueue<T> queue, int maxCount)
        {
            var list = new List<T>();

            while (list.Count < maxCount && queue.Count > 0)
            {
                if (!queue.TryDequeue(out var item))
                {
                    break;
                }

                list.Add(item);
            }

            return list;
        }
    }
}