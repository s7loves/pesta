using System;

namespace Pesta.Utilities
{
    public class Provider<T>
    {
        public T get()
        {
            return (T)Activator.CreateInstance(typeof(T));
        }
    }
}
