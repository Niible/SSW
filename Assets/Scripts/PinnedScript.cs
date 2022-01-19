using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class PinnedScript : MonoBehaviour
{

    private Arrow _arrowParent;
    private void Start()
    {
        _arrowParent = this.transform.parent.GetComponent<Arrow>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("enter");
        _arrowParent.pinnedOnFloor = true;
    }
}
