using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

public class StickyFaceMe : MonoBehaviour, IFocusable
{
    public GameObject Sticky;

    public void OnFocusEnter()
    {
        Sticky.transform.rotation = Quaternion.LookRotation(Sticky.transform.position - Camera.main.transform.position);
    }

    public void OnFocusExit()
    {
        
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
