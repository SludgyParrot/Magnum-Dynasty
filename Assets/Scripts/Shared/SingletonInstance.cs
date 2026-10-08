using UnityEngine;

public abstract class SingletonInstance<T> : MonoBehaviour where T : MonoBehaviour
{
   private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(T)) as T;

                if(instance == null)
                    instance = new GameObject(nameof(T)).AddComponent<T>();
            }
            return instance;
        }
    }

    [SerializeField]
    protected bool doNotDestroyOnLoad;

    private void Awake()
    {
        if(doNotDestroyOnLoad)
        {
            if (instance != null && instance != this)
                Destroy(this);
            else
                DontDestroyOnLoad(this);
        }
    }
}
