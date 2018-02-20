using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

public class OnClick : MonoBehaviour ,IInputClickHandler{

    void IInputClickHandler.OnInputClicked(InputClickedEventData eventData)
    {
        //MakeStickyManager.Instance.MakeSticky("test");

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
