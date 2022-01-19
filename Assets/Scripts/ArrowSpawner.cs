using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSpawner : MonoBehaviour
{
    public GameObject arrowPrefab;
    
    private GameObject _arrow;

    // Start is called before the first frame update
    void Start()
    {
        arrowPrefab.transform.position = new Vector3(0,-0.7f,0);
        //marblePrefab.transform.SetParent(map.transform);
        CreateArrow();
    }

    private void Update()
    {
        if (IsNullOrDestroyed(_arrow))
            CreateArrow();
    }

    private void CreateArrow()
    {
        _arrow = Instantiate(arrowPrefab, transform);
    }

    public static bool IsNullOrDestroyed(GameObject obj)
    {
        if (object.ReferenceEquals(obj, null)) return true;

        if (obj is UnityEngine.Object) return (obj as UnityEngine.Object) == null;

        return false;
    }
}
