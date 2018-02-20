using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Windows.Speech;
using UnityEngine.UI;
using System.Linq;
using HoloToolkit.Sharing;
using HoloToolkit.Sharing.Tests;
using UnityEngine.SceneManagement;
using FrostweepGames.Plugins.GoogleCloud.SpeechRecognition;

public class Voice_receiver : MonoBehaviour, ISourceStateHandler, IInputHandler
{
    IInputSource currentInputSource;
    private uint id;
    private bool isDrag = false;
    GameObject obj;
    public GameObject original;
    private List<GameObject> cubes = new List<GameObject>();

    public KeywordManager keywordManager;
    public DictationRecognizer m_DictationRecognizer;

    GameObject abc;
    String txt = "";

    private List<Vector3> poss = new List<Vector3>();
    private List<Quaternion> rots = new List<Quaternion>();
    private List<Char> chars = new List<char>();

    private Vector3? tmppos;
    private Quaternion? tmprot;

    //タップしてる部分を表示する用
    public GameObject drawobj;
    GameObject drawtmp;

    //音声認識が動作してるか確認用
    public GameObject error;
    TextMesh errortex;
    String bbb;

    public static int returnToMenuFlag = 0;
    bool hit;

    // Action Driver
    private speech _speech;
    string tx;

    List<GameObject> voiceTextList = new List<GameObject>();

    public void OnInputDown(InputEventData eventData)
    {
        if (!eventData.InputSource.SupportsInputInfo(eventData.SourceId, SupportedInputInfo.Position))
        {
            return;
        }

        if (returnToMenuFlag == 1)
        {
            m_DictationRecognizer.Dispose();
            SceneManager.LoadScene("SceneLauncher");
            returnToMenuFlag = 0;
        }

        /*GameObject tmp = GameObject.Instantiate(original);
        obj = tmp;
        cubes.Add(tmp);*/

        drawtmp = GameObject.Instantiate(drawobj);

        currentInputSource = eventData.InputSource;
        id = eventData.SourceId;

        isDrag = true;
        Debug.Log("finger down");
    }

    public void OnInputUp(InputEventData eventData)
    {
        isDrag = false;
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

    IEnumerator StartDictation()
    {

        if (m_DictationRecognizer.Status == SpeechSystemStatus.Running)
        {

            yield return null;
        }
        else
        {

            while (m_DictationRecognizer.Status != SpeechSystemStatus.Running)
            {
                yield return new WaitForSeconds(0.5f);
            }


        }


    }

    // Use this for initialization
    void Start()
    {
        InputManager.Instance.PushFallbackInputHandler(gameObject);

        //m_DictationRecognizer = new DictationRecognizer();
        //m_DictationRecognizer.InitialSilenceTimeoutSeconds = 30f;
        //m_DictationRecognizer.DictationResult += (text, confidence) =>
        //{
        //    if (isDrag)
        //    {
        //        txt = text;
        //    }

        //};

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

        _speech = GameObject.Find("InputManager").GetComponent<speech>();

        //Debug.Log(_speech);

        GameObject aaa = GameObject.Instantiate(error);
        errortex = aaa.GetComponent<TextMesh>();

        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.VoiceLine] = SharedVoiceLine;
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.DeleteShareVoiceLine] = ReceiveDeleteShareVoice;
        //CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.DeleteMyVoiceLine] = ReceiveDeleteMyVoice;
        //CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.ResetVoiceLine] = ReceiveResetVoice;

        // SharingStage should be valid at this point, but we may not be connected.
        if (SharingStage.Instance.IsConnected)
        {
            Connected();
        }
        else
        {
            SharingStage.Instance.SharingManagerConnected += Connected;
        }

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(txt);
        txt = _speech.Str;
        
        //Debug.Log(txt);
        

        if (isDrag)
        {
            Vector3 pos;
            currentInputSource.TryGetPosition(id, out pos);
            Vector3 directionToTarget = Camera.main.transform.position - pos;
            Quaternion qua;
            currentInputSource.TryGetOrientation(id, out qua);
            qua = Quaternion.LookRotation(-directionToTarget);

            drawtmp.transform.position = pos;

            //音声認識が動作してるか確認
            //if (m_DictationRecognizer.Status == SpeechSystemStatus.Running)
            //{
            //    bbb = "Running";
            //    errortex.text = bbb;
            //    errortex.color = Color.green;
            //    //Debug.Log("Running");
            //}

            //音声認識が動作してるか確認
            if (_speech.Status == speech.State.Stop)
            {
                bbb = "Running";
                errortex.text = bbb;
                errortex.color = Color.green;

                _speech.StartRecordButtonOnClickHandler();
                //Debug.Log("Running");
            }

            if (tmppos == null && tmprot == null)
            {
                poss.Add(pos);
                rots.Add(qua);
                tmppos = pos;
                tmprot = qua;
            } else if (((Vector3)tmppos - pos).sqrMagnitude > 0.001)
            {
                //Debug.Log("hoge");
                poss.Add(pos);
                rots.Add(qua);
                tmppos = pos;
                tmprot = qua;
            }

            

        }
        else
        {
            if (_speech.Status == speech.State.Recording)
            {
                _speech.StopRecordButtonOnClickHandler();
            }

            bbb = "Missing";
            errortex.text = bbb;
            errortex.color = Color.red;
            //Debug.Log(txt);
            //if (txt != "")
            //{
            //    foreach (char c in txt)
            //    {
            //        chars.Add(c);
            //    }
            //    txt = "";
            //}

            //// 話した内容を一文字ずつ表示
            //if (chars.Count != 0)
            //{
            //    int tmpcount = chars.Count;
            //    for (int i = 0; i < poss.Count; i++)
            //    {
            //        if (tmpcount > 0)
            //        {
            //            GameObject tmp = GameObject.Instantiate(original);
            //            TextMesh tex = tmp.GetComponent<TextMesh>();
            //            String tmptxt;
            //            tmptxt = chars[i].ToString();
            //            tex.text = tmptxt;
            //            tmp.transform.position = poss[i];
            //            tmp.transform.rotation = rots[i];
            //            DontDestroyOnLoad(tmp);

            //            tmpcount = tmpcount - 1;
            //            CustomMessages.Instance.SendVoiceLine(poss[i], rots[i], tmptxt);
            //        }
            //    }
            //}
            //ResetObj();

            // 音声が取得できなかった場合,一定時間後に保持した座標を消す
            if (_speech.Status == speech.State.Analyzing)
            {
                StartCoroutine("shokika");
            }

            ResetDrawObj();
        }

        if (txt != "")
        {
            foreach (char c in txt)
            {
                chars.Add(c);
            }

            // 話した内容を一文字ずつ表示
            if (chars.Count != 0)
            {
                int tmpcount = chars.Count;
                for (int i = 0; i < poss.Count; i++)
                {
                    if (tmpcount > 0)
                    {
                        GameObject tmp = GameObject.Instantiate(original);
                        TextMesh tex = tmp.GetComponent<TextMesh>();
                        String tmptxt;
                        tmptxt = chars[i].ToString();
                        tex.text = tmptxt;
                        tmp.transform.position = poss[i];
                        tmp.transform.rotation = rots[i];
                        voiceTextList.Add(tmp);
                        DontDestroyOnLoad(tmp);

                        byte[] bytesData = new byte[1024];
                        bytesData = System.Text.Encoding.UTF8.GetBytes(tmptxt);
                        tmpcount = tmpcount - 1;
                        CustomMessages.Instance.SendVoiceLine(poss[i], rots[i], bytesData);
                    }
                }
            }
            ResetObj();
        }


        //if (txt != null)
        //{
        //    bbb = "Success";
        //    errortex.text = bbb;
        //    errortex.color = Color.blue;
        //}

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

    public void ResetObj()
    {
        txt = "";
        _speech.Str = "";
        tmppos = null;
        tmprot = null;
        poss.Clear();
        rots.Clear();
        chars.Clear();
    }

    public void ResetDrawObj()
    {
        if (drawtmp != null)
        {
            Destroy(drawtmp);
            drawtmp = null;
        }
    }

    IEnumerator shokika()
    {
        yield return new WaitForSeconds(3); // 一定時間待つ
        if (txt == "") {
            ResetObj();
        }

    }

    private void ReceiveDeleteShareVoice(NetworkInMessage msg)
    {
        // Parse the message
        long userID = msg.ReadInt64();

        DeleteShareTextList();
    }

    //private void ReceiveDeleteMyVoice(NetworkInMessage msg)
    //{
    //    // Parse the message
    //    long userID = msg.ReadInt64();

    //    int ind = msg.ReadInt32();

    //    DeleteMyTextList(ind);
    //}

    //private void ReceiveResetVoice(NetworkInMessage msg)
    //{
    //    // Parse the message
    //    long userID = msg.ReadInt64();

    //    ResetVoiceLine();
    //}

    public void DeleteShareTextList()
    {
        for (int i = 0; i < Data.shareVoiceTextList.Count; i++)
        {
            Destroy(Data.shareVoiceTextList[i]);
            Data.shareVoiceTextList[i] = null;
        }
        Data.shareVoiceTextList.Clear();
    }

    //public void DeleteMyTextList(int ind)
    //{
    //    Destroy(voiceTextList[ind]);
    //}

    //public void ResetVoiceLine()
    //{
    //    Data.shareVoiceTextList.Clear();
    //    //voiceTextList.Clear();
    //}

    //public void DeleteVoiceLine()
    //{
    //    for (int i = 0; i < voiceTextList.Count; i++)
    //    {
    //        Destroy(voiceTextList[i]);
    //        CustomMessages.Instance.DeleteShareVoiceLine(i);
    //        voiceTextList[i] = null;
    //    }
    //    voiceTextList.Clear();
    //    CustomMessages.Instance.ResetVoiceLine();
    //}

    public void DeleteShareVoiceLine()
    {
        for (int i = 0; i < voiceTextList.Count; i++)
        {
            Destroy(voiceTextList[i]);
            voiceTextList[i] = null;
        }
        voiceTextList.Clear();
        CustomMessages.Instance.DeleteShareVoiceLine();
        //CustomMessages.Instance.ResetVoiceLine();
    }


    public void ResetLines()
    {
        for (int i = 0; i < cubes.Count; i++)
        {
            Destroy(cubes[i]);
            cubes[i] = null;
        }
        cubes.Clear();

    }

    private void Connected(object sender = null, EventArgs e = null)
    {
        SharingStage.Instance.SharingManagerConnected -= Connected;

        SharingStage.Instance.SessionUsersTracker.UserJoined += UserJoinedSession;
        SharingStage.Instance.SessionUsersTracker.UserLeft += UserLeftSession;
    }

    //protected override void OnDestroy()
    //{
    //    if (SharingStage.Instance != null)
    //    {
    //        if (SharingStage.Instance.SessionUsersTracker != null)
    //        {
    //            SharingStage.Instance.SessionUsersTracker.UserJoined -= UserJoinedSession;
    //            SharingStage.Instance.SessionUsersTracker.UserLeft -= UserLeftSession;
    //        }
    //    }

    //    base.OnDestroy();
    //}

    /// <summary>
    /// Called when a new user is leaving the current session.
    /// </summary>
    /// <param name="user">User that left the current session.</param>
    private void UserLeftSession(User user)
    {
        int userId = user.GetID();
        if (userId != SharingStage.Instance.Manager.GetLocalUser().GetID())
        {
            //いなくなった時の処理
        }
    }

    /// <summary>
    /// Called when a user is joining the current session.
    /// </summary>
    /// <param name="user">User that joined the current session.</param>
    private void UserJoinedSession(User user)
    {
        if (user.GetID() != SharingStage.Instance.Manager.GetLocalUser().GetID())
        {
            //GetRemoteHeadInfo(user.GetID());
        }
    }

    /// <summary>
    /// Called when a remote user sends a head transform.
    /// </summary>
    /// <param name="msg"></param>
    private void SharedVoiceLine(NetworkInMessage msg)
    { 
        // Parse the message
        long userID = msg.ReadInt64();

        Vector3 Vpos = CustomMessages.Instance.ReadVector3(msg);
        Quaternion Vrot = CustomMessages.Instance.ReadQuaternion(msg);
        byte[] kariByte = new byte[1024];
        msg.ReadArray(kariByte, (uint)kariByte.Length);
        string Vstr = System.Text.Encoding.UTF8.GetString(kariByte);
        //string Vstr = msg.ReadString();
        MakeVoiceLine(Vpos, Vrot, Vstr);
    }

    public void MakeVoiceLine(Vector3 Vpos, Quaternion Vrot, String Vstr)
    {
        GameObject Stmp = GameObject.Instantiate(original);
        TextMesh Stex = Stmp.GetComponent<TextMesh>();
        
        Stex.text = Vstr;
        Stmp.transform.position = Vpos;
        Stmp.transform.rotation = Vrot;
        Data.shareVoiceTextList.Add(Stmp);
        DontDestroyOnLoad(Stmp);
    }
}