using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenu : MonoBehaviour, ISourceStateHandler, IInputHandler
{
    public static int returnToMenuFlag = 0;

    string objectName;
    bool hit;

    public void OnInputDown(InputEventData eventData)
    {
        if (!eventData.InputSource.SupportsInputInfo(eventData.SourceId, SupportedInputInfo.Position))
        {
            return;
        }

        //Debug.Log(returnToMenuFlag);

        if (returnToMenuFlag == 1)
        {
            SceneManager.LoadScene("SceneLauncher");
            returnToMenuFlag = 0;
        }
        
        Debug.Log("finger down");
    }

    public void OnInputUp(InputEventData eventData)
    {
        Debug.Log("finger up");
    }

    public void OnSourceDetected(SourceStateEventData eventData)
    {
        Debug.Log("Detected");
    }

    public void OnSourceLost(SourceStateEventData eventData)
    {
        Debug.Log("Lost");
    }

    // Use this for initialization
    void Start () {
        InputManager.Instance.PushFallbackInputHandler(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
        RaycastHit hitInfo;

        hit = Physics.Raycast(Camera.main.transform.position,
                                Camera.main.transform.forward,
                                out hitInfo,
                                20f);

        if (hit && hitInfo.collider.tag == "DummyAnchor")
        {
            returnToMenuFlag = 1;
            //Debug.Log(hitInfo.collider.tag);
        }
    }
}
