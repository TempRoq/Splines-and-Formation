using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T:MonoBehaviour
{

    public static T instance;

    protected virtual void Awake()
    {   
        if (instance == null)
        {
          //  print("making new instance");
            DontDestroyOnLoad(gameObject);           
            instance = gameObject.GetComponent<T>();
            OnFirstInstance();
        }
        else if (instance != this)
        {
          //  print("deleting instance, as instance already exists");
            OnAlreadyLoaded();
            Destroy(gameObject);
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {
       
    }

    public virtual void OnAlreadyLoaded()
    {



    }

    public virtual void OnFirstInstance()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }
}
