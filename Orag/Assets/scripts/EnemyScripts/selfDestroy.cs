using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selfDestroy : MonoBehaviour
{
    public float timer = 1;


    void Start()
    {
        Invoke("DoSomething", timer);
    }

    void DoSomething()
    {
        Destroy(gameObject);

    }
}
