using HoloToolkit.Unity.InputModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class InterjectionMode : MonoBehaviour {

    public static bool InterjectionModeFlag = false;
    public static String speechText;

    public KeywordManager keywordManager;
    public DictationRecognizer m_DictationRecognizer;

    //音声認識が動作してるか確認用
    public GameObject error;
    TextMesh errortex;
    String bbb;

    // 計測開始
    System.Diagnostics.Stopwatch sw;

    public float timeOut;
    private float timeElapsed;

    public static bool mugon = false;

    public static speech _speech;

    // Use this for initialization
    void Start () {
        InterjectionModeFlag = true;

        //m_DictationRecognizer = new DictationRecognizer();
        //m_DictationRecognizer.InitialSilenceTimeoutSeconds = 30f;
        //m_DictationRecognizer.DictationResult += (text, confidence) =>
        //{
        //    speechText = text;

        //};

        //_speech = GameObject.Find("InputManager").GetComponent<speech>();

        //if (PhraseRecognitionSystem.Status == SpeechSystemStatus.Running)
        //{
        //    PhraseRecognitionSystem.Shutdown();
        //}
        //m_DictationRecognizer.Start();

        //m_DictationRecognizer.DictationComplete += (completionCause) =>
        //{
        //    if (completionCause == DictationCompletionCause.TimeoutExceeded)
        //    {
        //        //音声認識を起動
        //        m_DictationRecognizer.Start();
        //    }
        //    else
        //    {
        //        //その他止まった原因に応じてハンドリング
        //    }
        //};

        //GameObject aaa = GameObject.Instantiate(error);
        //errortex = aaa.GetComponent<TextMesh>();

        // 計測開始
        //sw = new System.Diagnostics.Stopwatch();
        //sw.Start();
    }
	
	// Update is called once per frame
	void Update () {
        //音声認識が動作してるか確認
        //if (m_DictationRecognizer.Status == SpeechSystemStatus.Running)
        //{
        //    bbb = "Running";
        //    errortex.text = bbb;
        //    errortex.color = Color.green;
        //    //Debug.Log("Running");
        //}
        //else
        //{
        //    bbb = "Missing";
        //    errortex.text = bbb;
        //    errortex.color = Color.red;
        //}

        //if (_speech.Status == speech.State.Stop)
        //{
        //    _speech.StartRecordButtonOnClickHandler();
        //    //Debug.Log("Running");
        //}

        ////音声認識が動作してるか確認
        //if (_speech.Status == speech.State.Stop || _speech.Status == speech.State.Recording || _speech.Status == speech.State.Analyzing)
        //{ 
        //    bbb = "Running";
        //    errortex.text = bbb;
        //    errortex.color = Color.green;
        //    //Debug.Log("Running");
        //}
        //else
        //{
        //    bbb = "Missing";
        //    errortex.text = bbb;
        //    errortex.color = Color.red;
        //}

        ////一定間隔で音声を取得
        //timeElapsed += Time.deltaTime;
        ////timeOut[ms]毎に処理を実行
        //if (timeElapsed >= timeOut)
        //{
        //    // Do anything
        //    if (_speech.Status == speech.State.Recording)
        //    {
        //        _speech.StopRecordButtonOnClickHandler();
        //    }
        //    timeElapsed = 0.0f;
        //}

        //speechText = _speech.Str;

        //if (speechText != "")
        //{
        //    sw.Stop();
        //    speechText = "";
        //    mugon = false;
        //    sw.Reset();
        //    sw.Start();
        //}

        //if (sw.ElapsedMilliseconds > 10000)
        //{
        //    mugon = true;
        //}

        //Debug.Log(speechText);
    }

    private void OnDestroy()
    {
        InterjectionModeFlag = false;
        //Debug.Log("Destroy!");
    }
}
