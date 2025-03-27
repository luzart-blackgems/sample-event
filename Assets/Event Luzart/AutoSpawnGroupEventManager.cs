using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSpawnGroupEventManager 
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitGroupMethod()
    {
        var GroupEventManager = Resources.Load<GameObject>("GroupEventManager");
        GameObject obj2 = Object.Instantiate(GroupEventManager);
    }
}
