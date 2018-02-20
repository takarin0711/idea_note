// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

namespace HoloToolkit.Sharing.Tests
{
    /// <summary>
    /// Broadcasts the head transform of the local user to other users in the session,
    /// and adds and updates the head transforms of remote users.
    /// Head transforms are sent and received in the local coordinate space of the GameObject this component is on.
    /// </summary>
    public class RemoteHeadManager : Singleton<RemoteHeadManager>
    {
        public class RemoteHeadInfo
        {
            public long UserID;
            public GameObject HeadObject;
            public GameObject Interjection;
        }

        private int counter = 0;

        /// <summary>
        /// Keep a list of the remote heads, indexed by XTools userID
        /// </summary>
        private Dictionary<long, RemoteHeadInfo> remoteHeads = new Dictionary<long, RemoteHeadInfo>();

        public GameObject OverheadText;

        TextMesh overheadTex;

        Vector3 tmpHeadPosition;

        private HeadGesture gesture;
        private float cntUpTimer = 0.0f;

        string[] data = { "おぉ", "すごい", "へぇー", "なるほど" ,"Oh", "Wow","Really"};

        string[] data2 = { "うーん", "えー", "は", "えーっと", "umm", "Why", "What" };

        private void Start()
        {
            CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.HeadTransform] = UpdateHeadTransform;

            CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.OverheadText] = UpdateOverheadText;

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

        private void Update()
        {
            // Grab the current head transform and broadcast it to all the other users in the session
            Transform headTransform = Camera.main.transform;

            // Transform the head position and rotation from world space into local space
            Vector3 headPosition = transform.InverseTransformPoint(headTransform.position);
            Quaternion headRotation = Quaternion.Inverse(transform.rotation) * headTransform.rotation;

            //GameObject tmpHeadText = GameObject.Find("HeadText(Clone)");
            

            //tmpHeadText.transform.position = overheadPos;
            //Debug.Log(overheadTex.text);

            CustomMessages.Instance.SendHeadTransform(headPosition, headRotation);

            Vector3 overheadPos = headPosition;
            overheadPos.y += 0.3f;
            overheadTex = OverheadText.GetComponent<TextMesh>();
            //overheadTex.text = "ABC";


            if (InterjectionMode.InterjectionModeFlag==true)
            {
                gesture = GetComponent<HeadGesture>();

                cntUpTimer += Time.deltaTime;

                //if (InterjectionMode.speechText == "" && InterjectionMode.mugon == true)
                //{
                //    overheadTex.text = "Silence";
                //    Debug.Log("無言");
                //}
                //else if(InterjectionMode.speechText!="" && InterjectionMode.mugon==false)
                //{
                //    overheadTex.text = "";
                //    Debug.Log("");
                //}

                int ret = Array.IndexOf(data,InterjectionMode.speechText);
                if (ret >= 0 || gesture.isMovingDown)
                {
                    overheadTex.text = "Yes";
                    cntUpTimer = 0.0f;
                    Debug.Log("！！！");
                }

                int ret2 = Array.IndexOf(data2, InterjectionMode.speechText);
                if (ret2 >= 0 || gesture.isRightMove || gesture.isLeftMove)
                {
                    overheadTex.text = "No";
                    cntUpTimer = 0.0f;
                    Debug.Log("？？？");
                }

                if (cntUpTimer >= 5.0f)
                {
                    overheadTex.text = "";
                    cntUpTimer = 0.0f;
                }
            }

            CustomMessages.Instance.SendOverheadText(overheadPos, overheadTex.text);

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

            overheadTex.text = "";

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
                RemoveRemoteHead(remoteHeads[userId].HeadObject);
                RemoveRemoteHeadText(remoteHeads[userId].Interjection);
                remoteHeads.Remove(userId);
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
                GetRemoteHeadInfo(user.GetID());
            }
        }

        /// <summary>
        /// Gets the data structure for the remote users' head position.
        /// </summary>
        /// <param name="userId">User ID for which the remote head info should be obtained.</param>
        /// <returns>RemoteHeadInfo for the specified user.</returns>
        public RemoteHeadInfo GetRemoteHeadInfo(long userId)
        {
            RemoteHeadInfo headInfo;

            // Get the head info if its already in the list, otherwise add it
            if (!remoteHeads.TryGetValue(userId, out headInfo))
            {
                headInfo = new RemoteHeadInfo();
                headInfo.UserID = userId;
                headInfo.HeadObject = CreateRemoteHead();

                headInfo.Interjection = CreateInterjection();

                remoteHeads.Add(userId, headInfo);
            }

            return headInfo;
        }

        /// <summary>
        /// Called when a remote user sends a head transform.
        /// </summary>
        /// <param name="msg"></param>
        private void UpdateHeadTransform(NetworkInMessage msg)
        {
            // Parse the message
            long userID = msg.ReadInt64();

            Vector3 headPos = CustomMessages.Instance.ReadVector3(msg);

            Quaternion headRot = CustomMessages.Instance.ReadQuaternion(msg);

            RemoteHeadInfo headInfo = GetRemoteHeadInfo(userID);
            headInfo.HeadObject.transform.localPosition = headPos;
            headInfo.HeadObject.transform.localRotation = headRot;
        }

        /// <summary>
        /// Creates a new game object to represent the user's head.
        /// </summary>
        /// <returns></returns>
        private GameObject CreateRemoteHead()
        {
            GameObject newHeadObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            newHeadObj.transform.parent = gameObject.transform;
            newHeadObj.transform.localScale = Vector3.one * 0.2f;
            MeshRenderer meshRenderer = newHeadObj.GetComponent<MeshRenderer>();
            meshRenderer.material.color = new Color(0, 0, 0, 0.5f);
            return newHeadObj;
        }

        /// <summary>
        /// When a user has left the session this will cleanup their
        /// head data.
        /// </summary>
        /// <param name="remoteHeadObject"></param>
        private void RemoveRemoteHead(GameObject remoteHeadObject)
        {
            DestroyImmediate(remoteHeadObject);
        }

        private void RemoveRemoteHeadText(GameObject remoteHeadText)
        {
            DestroyImmediate(remoteHeadText);
        }

        private GameObject CreateInterjection()
        {
            GameObject newInterjection = GameObject.Instantiate(OverheadText);
            //GameObject newInterjection = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            newInterjection.transform.parent = gameObject.transform;
            newInterjection.transform.localScale = Vector3.one * 0.2f;
            return newInterjection;
        }

        private void UpdateOverheadText(NetworkInMessage msg)
        {
            // Parse the message
            long userID = msg.ReadInt64();

            Vector3 overheadPos = CustomMessages.Instance.ReadVector3(msg);

            String overheadStr = msg.ReadString();

            RemoteHeadInfo headInfo = GetRemoteHeadInfo(userID);
            headInfo.Interjection.transform.localPosition = overheadPos;

            TextMesh tmpTxt = headInfo.Interjection.transform.GetComponent<TextMesh>();
            tmpTxt.text = overheadStr;

            GameObject tmpPlayer = GameObject.Find("Cube");
            headInfo.Interjection.transform.rotation = Quaternion.LookRotation(headInfo.Interjection.transform.position - Camera.main.transform.position);
            //MakeOverheadText(overheadPos, overheadStr);

        }
    }
}