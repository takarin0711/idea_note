using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HoloToolkit.Sharing.Tests;
using HoloToolkit.Sharing;
using HoloToolkit.Unity;
using UnityEngine.SceneManagement;

public class Receiver : MonoBehaviour, ISourceStateHandler, IInputHandler{

    IInputSource currentInputSource;
    private uint id;
    private bool isDrag = false;
    GameObject obj;
    GameObject shareobj;
    Vector3 pos0; // 線の開始の座標
    Vector3 dummypos;
    public GameObject original;
    public static List<GameObject> cubes = new List<GameObject>();
    List<GameObject> cubeList = new List<GameObject>(); // 共有した後のCube
    public GameObject Cube; // 共有用のCube
    int abc = 0;
    private List<Vector3> posList = new List<Vector3>();
    Vector3 pos;
    int isDragShare=0;

    public static int returnToMenuFlag = 0;

    string objectName;
    bool hit;

    public void OnInputDown(InputEventData eventData)
    {
        if (!eventData.InputSource.SupportsInputInfo(eventData.SourceId, SupportedInputInfo.Position))
        {
            return;
        }

        if (returnToMenuFlag == 1)
        {
            SceneManager.LoadScene("SceneLauncher");
            returnToMenuFlag = 0;
        }
        else
        {
            GameObject tmp = GameObject.Instantiate(original);
            obj = tmp;
            cubes.Add(tmp);

            currentInputSource = eventData.InputSource;
            id = eventData.SourceId;

            DontDestroyOnLoad(tmp);

            isDrag = true;
            isDragShare = 1;
            //posList.Clear();
            abc = 1;
        }

        Debug.Log("finger down");
    }

    public void OnInputUp(InputEventData eventData)
    {
        isDrag = false;
        isDragShare = 0;
        posList.Clear();
        Debug.Log("finger up");
    }

    public void OnSourceDetected(SourceStateEventData eventData)
    {
        Debug.Log("Detected");
    }

    public void OnSourceLost(SourceStateEventData eventData)
    {
        isDrag = false;
        isDragShare = 0;
        posList.Clear();
        Debug.Log("Lost");
    }

    // Use this for initialization
    void Start()
    {
        InputManager.Instance.PushFallbackInputHandler(gameObject);

        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.Draw3d] = Shared3dDraw;
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.ResetShareLine] = ReceiveResetShareLine;

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

    /*protected override void OnDestroy()
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
    }*/

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

    // Update is called once per frame
    void Update()
    {
        if (isDrag)
        {
            currentInputSource.TryGetPosition(id, out pos);

            //Debug.Log(pos);
            obj.transform.position = pos;

            //CustomMessages.Instance.Send3dDraw(pos);
        }

        CustomMessages.Instance.Send3dDraw(pos, isDragShare);


        RaycastHit hitInfo;

        hit = Physics.Raycast(Camera.main.transform.position,
                                Camera.main.transform.forward,
                                out hitInfo,
                                20f);

        if (hit && hitInfo.collider.tag == "DummyAnchor")
        {
            returnToMenuFlag = 1;
            Debug.Log(hitInfo.collider.tag);
        }

        //CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.Draw3d] = Shared3dDraw;
    }

    private void ReceiveResetShareLine(NetworkInMessage msg)
    {
        // Parse the message
        long userID = msg.ReadInt64();

        ResetShareLine();
    }

    public void ResetShareLine()
    {
        for (int i = 0; i < Data.shareLineList.Count; i++)
        {
            Destroy(Data.shareLineList[i]);
            Data.shareLineList[i] = null;
        }
        Data.shareLineList.Clear();
    }

    public void ResetLines()
    {
        for (int i = 0; i < cubes.Count; i++)
        {
            Destroy(cubes[i]);
            cubes[i] = null;
        }
        CustomMessages.Instance.ResetShareLine();
        cubes.Clear();

    }

    //public void ResetShareLines()
    //{
    //    for (int i = 0; i < Data.shareLineList.Count; i++)
    //    {
    //        Destroy(Data.shareLineList[i]);
    //        Data.shareLineList[i] = null;
    //    }
    //    Data.shareLineList.Clear();
    //}

    private void Shared3dDraw(NetworkInMessage msg)
    {
        //Debug.Log("Shared3dDraw");

        // Parse the message
        long userID = msg.ReadInt64();

        Vector3 Pos = CustomMessages.Instance.ReadVector3(msg);
        int DragFlag = msg.ReadInt32();
        //uint id = msg.ReadUint32();
        //Debug.Log("id =", + id);
        Make3dDraw(Pos, DragFlag);
    }

    public void Make3dDraw(Vector3 pos, int DragFlag)
    {
        //GameObject tmp = GameObject.Instantiate(Cube);
        //shareobj = tmp;
        //cubeList.Add(tmp);
        //currentInputSource = eventData.InputSource;
        //id = eventData.SourceId;

        //currentInputSource.TryGetPosition(id, out pos);

        //Debug.Log(pos);
        //shareobj.transform.position = pos;

        //DontDestroyOnLoad(tmp);

        if (DragFlag == 0)
        {
            posList.Clear();
        }

        if (DragFlag == 1)
        {
            posList.Add(pos);

            if (posList.Count > 1)
            {
                LineDraw(posList[posList.Count - 2], posList[posList.Count - 1]);
            }
        }

        // CustomMessages.Instance.Send3dDraw(pos);

        //Debug.Log(posList.Count);
    }

    public void LineDraw(Vector3 pos0, Vector3 pos1)
    {
        //Debug.Log("LineDraw!");
        // オブジェクトの作成
        GameObject newObj = new GameObject();
        // 親子関係設定
        newObj.transform.parent = transform;
        // ラインの作成
        LineRenderer newLine = new LineRenderer();

        // --- 重要 --- //
        newLine = newObj.AddComponent<LineRenderer>();
        // --- 重要 --- //

        // ラインの色
        newLine.SetColors(Color.blue, Color.blue);
        // ラインの幅
        newLine.SetWidth(0.01f, 0.01f);
        // ラインの頂点数？
        newLine.SetVertexCount(2);
        // 始点の設定
        newLine.SetPosition(0, pos0);
        // 終点の設定
        newLine.SetPosition(1, pos1);

        Data.shareLineList.Add(newLine);
        DontDestroyOnLoad(newLine);

    }

}
