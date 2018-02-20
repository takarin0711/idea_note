using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System;
using UnityEngine.UI;
using HoloToolkit.Sharing.Tests;
using HoloToolkit.Sharing;

public class MakeStickyManager : Singleton<MakeStickyManager>
{
    public DictationRecognizer m_DictationRecognizer;
    public GameObject HoloSticky;
    public GameObject FukidashiUI;
    public Transform MyStickyHolder;
    private List<GameObject> stickyList = new List<GameObject>();
    public bool canInput = false;
    public bool isSelectingSticky = false;
    public GameObject SelectingSticky;

    public static Vector3 kariPos;

    public static speech _speech;
    string txt = "";
    // Use this for initialization
    void Start () {

        //m_DictationRecognizer = new DictationRecognizer();
        //m_DictationRecognizer.InitialSilenceTimeoutSeconds = 30f;
        //m_DictationRecognizer.DictationResult += (text, confidence) =>
        //{
        //    if (canInput)
        //    {
        //        MakeSticky(text, Camera.main.transform.TransformPoint(new Vector3(0f, 0f, 1f)),0);
        //    }

        //};
        _speech = GameObject.Find("InputManager").GetComponent<speech>();

        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.SharedSticky] = SharedSticky;

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

    private void Connected(object sender = null, EventArgs e = null)
    {
        SharingStage.Instance.SharingManagerConnected -= Connected;

        SharingStage.Instance.SessionUsersTracker.UserJoined += UserJoinedSession;
        SharingStage.Instance.SessionUsersTracker.UserLeft += UserLeftSession;
    }

    protected override void OnDestroy()
    {
        if (SharingStage.Instance != null)
        {
            if (SharingStage.Instance.SessionUsersTracker != null)
            {
                SharingStage.Instance.SessionUsersTracker.UserJoined -= UserJoinedSession;
                SharingStage.Instance.SessionUsersTracker.UserLeft -= UserLeftSession;
            }
        }

        base.OnDestroy();
    }

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
    private void SharedSticky(NetworkInMessage msg)
    {
        // Parse the message
        long userID = msg.ReadInt64();

        Vector3 StickyPos = CustomMessages.Instance.ReadVector3(msg);
        byte[] kariByte = new byte[1024];
        msg.ReadArray(kariByte, (uint)kariByte.Length);
        //string message = msg.ReadString();
        int colorIndex = msg.ReadInt32();

        string message = System.Text.Encoding.UTF8.GetString(kariByte);
        Debug.Log("message:"+ message);
        //Debug.Log("colorIndex=" + colorIndex);
        MakeShareSticky(message, StickyPos, colorIndex);
    }



    public void MakeSticky(string text,Vector3 pos,int colorIndex)
    {
        GameObject Sticky = GameObject.Instantiate(HoloSticky);
        Text tex = Sticky.transform.transform.Find("HoloStickyUI").transform.transform.Find("Image").transform.transform.Find("Text").GetComponent<Text>();
        tex.text = text;
        tex.color =  UserColorManager.Instance.colorMap[colorIndex];
        stickyList.Add(Sticky);
        Sticky.transform.position = pos; 
        Sticky.transform.rotation = Quaternion.LookRotation(Sticky.transform.position - Camera.main.transform.position);
        DontDestroyOnLoad(Sticky);

        kariPos = pos;
        txt = "";
        _speech.Str = "";
        //Sticky.transform.localScale = new Vector3(0.148f, 0.148f, 0.148f);


    }

    public void MakeShareSticky(string text, Vector3 pos, int colorIndex)
    {
        GameObject Sticky = GameObject.Instantiate(HoloSticky);
        Text tex = Sticky.transform.transform.Find("HoloStickyUI").transform.transform.Find("Image").transform.transform.Find("Text").GetComponent<Text>();
        tex.text = text;
        //tex.color = UserColorManager.Instance.colorMap[colorIndex];
        tex.color = Color.red;
        //stickyList.Add(Sticky);
        Sticky.transform.position = pos;
        Sticky.transform.rotation = Quaternion.LookRotation(Sticky.transform.position - Camera.main.transform.position);
        Data.shareStickyList.Add(Sticky);
        Data.WallStickyList.Add(0);
        DontDestroyOnLoad(Sticky);
        //Sticky.transform.localScale = new Vector3(0.148f, 0.148f, 0.148f);


    }

    // Update is called once per frame
    void Update () {
        //txt = _speech.Str;

        //if (txt != "")
        //{
        //    MakeSticky(txt, Camera.main.transform.TransformPoint(new Vector3(0f, 0f, 1f)), 0);
        //}
    }
}
