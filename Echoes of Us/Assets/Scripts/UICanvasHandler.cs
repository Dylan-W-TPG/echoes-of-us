using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvasHandler : MonoBehaviour
{
    public static UICanvasHandler Instance;
    //Creation of Singleton
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else if (Instance != this)
        {
            //Destroys itself
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
