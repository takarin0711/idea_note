using System.Collections;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using UnityEngine.Windows.Speech;
using UnityEngine.UI;

public class InputStickyText : MonoBehaviour ,IFocusable{

    public Text statusText;
    public GameObject XPic;
    public static string tempStatusText = "";

    public KeywordManager keywordManager;
    //public GameObject refObj;

    public enum State { Stop, Recording, Analyzing };

    public static bool focusInput=false;

    public void OnFocusEnter()
    {
        //keywordManager.StopKeywordRecognizer();
        //refObj.GetComponent<SpeechOnOff>().OnOff = 1;
        //if (MakeStickyManager.Instance.m_DictationRecognizer.Status == SpeechSystemStatus.Running)
        //{
        //    statusText.text = "Running";
        //}
        //else if (MakeStickyManager.Instance.m_DictationRecognizer.Status == SpeechSystemStatus.Stopped)
        //{
        //    statusText.text = "Stopped";
        //}
        //else
        //{
        //    statusText.text = "Failed";
        //}
        if (focusInput==false) {
            focusInput = true;
        }

        if (MakeStickyManager._speech.pStatus == 0)
        {
            tempStatusText = "Missing";
            XPic.SetActive(true);
        }
        else if(MakeStickyManager._speech.pStatus == 1)
        {
            tempStatusText = "Running";
            XPic.SetActive(false);
        }
        else if(MakeStickyManager._speech.pStatus == 2)
        {
            tempStatusText = "Analyzing";
            XPic.SetActive(true);
        }
        else
        {
            tempStatusText = "Failed";
            XPic.SetActive(true);
        }

        //StartCoroutine(StartDictation());

    }

    public void OnFocusExit()
    {
        //keywordManager.StartKeywordRecognizer();
        //refObj.GetComponent<SpeechOnOff>().OnOff = 0;
        statusText.text = "";
        tempStatusText = "";
        MakeStickyManager.Instance.canInput = false;
        XPic.SetActive(true);

        if (focusInput == true)
        {
            focusInput = false;
        }

        //if (MakeStickyManager.Instance._speech.pStatus == 1)
        //{
        //    MakeStickyManager.Instance._speech.StopRecordButtonOnClickHandler();
        //}
    }

    //IEnumerator StartDictation()
    //{
    //    DontDestroyOnLoad(XPic);
    //    //if (MakeStickyManager.Instance.m_DictationRecognizer.Status == SpeechSystemStatus.Running)
    //    //{
    //    //    MakeStickyManager.Instance.canInput = true;
    //    //    XPic.SetActive(false);
    //    //    yield return null;
    //    //}
    //    //else
    //    //{
    //    //    if (PhraseRecognitionSystem.Status == SpeechSystemStatus.Running)
    //    //    {
    //    //        PhraseRecognitionSystem.Shutdown();
    //    //    }
    //    //    MakeStickyManager.Instance.m_DictationRecognizer.Start();
    //    //    while (MakeStickyManager.Instance.m_DictationRecognizer.Status != SpeechSystemStatus.Running)
    //    //    {
    //    //        yield return new WaitForSeconds(0.5f);
    //    //    }

    //    //    statusText.text = "Running";
    //    //    MakeStickyManager.Instance.canInput = true;
    //    //    XPic.SetActive(false);
    //    //}

    //    //if (MakeStickyManager.Instance._speech.pStatus == 1)
    //    //{
    //    //    MakeStickyManager.Instance.canInput = true;
    //    //    XPic.SetActive(false);
    //    //    yield return null;
    //    //}
    //    //else
    //    //{
    //    //    MakeStickyManager.Instance._speech.StartRecordButtonOnClickHandler();
    //    //    while (MakeStickyManager.Instance._speech.pStatus != 1)
    //    //    {
    //    //        yield return new WaitForSeconds(0.5f);
    //    //    }

    //    //    statusText.text = "Running";
    //    //    MakeStickyManager.Instance.canInput = true;
    //    //   XPic.SetActive(false);
    //    //}

    //}

    // Use this for initialization
    void Start () {
        statusText.text = "";
        //refObj = GameObject.Find("SpeechOnOff");

    }
	
	// Update is called once per frame
	void Update () {
        if (tempStatusText != "")
        {
            statusText.text = tempStatusText;
        }

        if(statusText.text == "Running")
        {
            XPic.SetActive(false);
        }
        else if (statusText.text == "Missing")
        {
            XPic.SetActive(true);
        }
        else if (statusText.text == "Analyzing")
        {
            XPic.SetActive(true);
        }
        else if (statusText.text == "Failed")
        {
            XPic.SetActive(true);
        }
    }
}
