public class Singleton<T> where T : new()
{
    public static T instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
            }
            return _instance;
        }
    }
    static T _instance;
}