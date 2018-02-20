using HoloToolkit.Sharing;
using HoloToolkit.Sharing.Tests;
using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UserColorManager : Singleton<UserColorManager>
{
    public Dictionary<int, Color> colorMap = new Dictionary<int, Color>();
    bool isHost = false;
    private SessionUsersTracker usersTracker;
    public Color myColor = Color.white;
    public int mynumber = 0;
    // Use this for initialization
    void Start () {
        colorMap.Add(0, new Color(1f, 0f, 1f));
        colorMap.Add(1, new Color(1f, 0f, 1f));
        colorMap.Add(2, new Color(1f, 0f, 1f));
        colorMap.Add(3, new Color(1f, 0f, 1f));
        colorMap.Add(4, new Color(1f, 0f, 1f));
        colorMap.Add(5, new Color(1f, 0f, 1f));
        colorMap.Add(6, new Color(1f, 0f, 1f));
        

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


    /// <summary>
    /// Called when a new user is leaving the current session.
    /// </summary>
    /// <param name="user">User that left the current session.</param>
    private void UserLeftSession(User user)
    {
        int userId = user.GetID();
        if (userId != SharingStage.Instance.Manager.GetLocalUser().GetID())
        {
            
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
            
        }
        if (mynumber == 0)
        {
            usersTracker = SharingStage.Instance.SessionUsersTracker;
            mynumber = usersTracker.CurrentUsers.Count;
            myColor = colorMap[mynumber];
            Debug.Log("mynumber=" + mynumber);
        }


    }


    // Update is called once per frame
    void Update () {
		
	}
}
