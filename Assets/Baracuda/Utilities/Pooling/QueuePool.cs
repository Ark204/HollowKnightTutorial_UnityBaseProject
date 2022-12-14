// Copyright (c) 2022 Jonathan Lang

using System.Collections.Generic;
using Baracuda.Utilities.Pooling.Source;

namespace Baracuda.Utilities.Pooling
{
    public class QueuePool<T>
    {
        private static readonly ObjectPoolT<Queue<T>> poolTBase 
            = new ObjectPoolT<Queue<T>>(() => new Queue<T>(), actionOnRelease: l => l.Clear());

        public static Queue<T> Get()
        {
            return poolTBase.Get();
        }
        
        public static void Release(Queue<T> toRelease)
        {
            poolTBase.Release(toRelease);
        }
        
        public static PooledObject<Queue<T>> GetDisposable()
        {
            return poolTBase.GetDisposable();
        }
    }
}