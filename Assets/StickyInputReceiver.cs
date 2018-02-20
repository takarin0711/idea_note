using HoloToolkit.Unity.InputModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class StickyInputReceiver : MonoBehaviour, ISourceStateHandler, IInputHandler
{

    IInputSource currentInputSource;
    private uint id;
    private bool isDrag = false;
    
    String txt = "";


    // Action Driver
    //public speech _speech;

    public void OnInputDown(InputEventData eventData)
    {
        if (!eventData.InputSource.SupportsInputInfo(eventData.SourceId, SupportedInputInfo.Position))
        {
            return;
        }

        currentInputSource = eventData.InputSource;
        id = eventData.SourceId;

        isDrag = true;

        if (InputStickyText.focusInput == true)
        {
            
            MakeStickyManager._speech.StartRecordButtonOnClickHandler();
            InputStickyText.tempStatusText = "Running";
        }

        Debug.Log("finger down");
    }

    public void OnInputUp(InputEventData eventData)
    {
        isDrag = false;

        if (MakeStickyManager._speech.pStatus == 1)
        {
            MakeStickyManager._speech.StopRecordButtonOnClickHandler();
            InputStickyText.tempStatusText = "Missing";
        }

        Debug.Log("finger up");
    }

    public void OnSourceDetected(SourceStateEventData eventData)
    {
        Debug.Log("Detected");
    }

    public void OnSourceLost(SourceStateEventData eventData)
    {
        isDrag = false;
        Debug.Log("Lost");
    }

    // Use this for initialization
    void Start()
    {
        InputManager.Instance.PushFallbackInputHandler(gameObject);

        //_speech = GameObject.Find("InputManager").GetComponent<speech>();

        //Debug.Log(_speech)

    }

    // Update is called once per frame
    void Update()
    {
        txt = MakeStickyManager._speech.Str;

        if (txt != "")
        {
            MakeStickyManager.Instance.MakeSticky(txt, Camera.main.transform.TransformPoint(new Vector3(0f, 0f, 1f)), 0);
        }

    }
    
}
