using System.Reflection;

public static class Singleton<T>
  where T : class
{
    static Singleton()
    {
    }

    public static readonly T Instance =
      typeof(T).InvokeMember(typeof(T).Name,
                             BindingFlags.CreateInstance |
                             BindingFlags.Instance |
                             BindingFlags.NonPublic,
                             null, null, null) as T;
}