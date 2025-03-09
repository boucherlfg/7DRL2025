using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    public static T Instance
    {
        get;
        private set;
    }
    protected virtual void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("More than one instance of MonoSingleton found!");
            Destroy(gameObject);
        }
        Instance = this as T;
    }
}