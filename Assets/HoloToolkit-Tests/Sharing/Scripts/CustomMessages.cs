// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using HoloToolkit.Unity;
using UnityEngine;

namespace HoloToolkit.Sharing.Tests
{
    /// <summary>
    /// Test class for demonstrating how to send custom messages between clients.
    /// </summary>
    public class CustomMessages : Singleton<CustomMessages>
    {
        /// <summary>
        /// Message enum containing our information bytes to share.
        /// The first message type has to start with UserMessageIDStart
        /// so as not to conflict with HoloToolkit internal messages.
        /// </summary>
        public enum TestMessageID : byte
        {
            HeadTransform = MessageID.UserMessageIDStart,
            SharedSticky = MessageID.UserMessageIDStart + 1,
            Draw3d = MessageID.UserMessageIDStart + 2,
            MovePos = MessageID.UserMessageIDStart + 3,
            VoiceLine = MessageID.UserMessageIDStart + 4,
            OverheadText = MessageID.UserMessageIDStart + 5,
            WallPos = MessageID.UserMessageIDStart + 6,
            DeleteShareSticky = MessageID.UserMessageIDStart + 7,
            DeleteAllShareSticky = MessageID.UserMessageIDStart + 8,
            DeleteShareVoiceLine = MessageID.UserMessageIDStart + 9,
            ResetShareLine = MessageID.UserMessageIDStart + 10,
            Max
        }

        public enum UserMessageChannels
        {
            Anchors = MessageChannel.UserMessageChannelStart
        }

        /// <summary>
        /// Cache the local user's ID to use when sending messages
        /// </summary>
        public long LocalUserID
        {
            get; set;
        }

        public delegate void MessageCallback(NetworkInMessage msg);
        private Dictionary<TestMessageID, MessageCallback> messageHandlers = new Dictionary<TestMessageID, MessageCallback>();
        public Dictionary<TestMessageID, MessageCallback> MessageHandlers
        {
            get
            {
                return messageHandlers;
            }
        }

        /// <summary>
        /// Helper object that we use to route incoming message callbacks to the member
        /// functions of this class
        /// </summary>
        private NetworkConnectionAdapter connectionAdapter;

        /// <summary>
        /// Cache the connection object for the sharing service
        /// </summary>
        private NetworkConnection serverConnection;

        private void Start()
        {
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
            InitializeMessageHandlers();
        }

        private void InitializeMessageHandlers()
        {
            SharingStage sharingStage = SharingStage.Instance;

            if (sharingStage == null)
            {
                Debug.Log("Cannot Initialize CustomMessages. No SharingStage instance found.");
                return;
            }

            serverConnection = sharingStage.Manager.GetServerConnection();
            if (serverConnection == null)
            {
                Debug.Log("Cannot initialize CustomMessages. Cannot get a server connection.");
                return;
            }

            connectionAdapter = new NetworkConnectionAdapter();
            connectionAdapter.MessageReceivedCallback += OnMessageReceived;

            // Cache the local user ID
            LocalUserID = SharingStage.Instance.Manager.GetLocalUser().GetID();

            for (byte index = (byte)TestMessageID.HeadTransform; index < (byte)TestMessageID.Max; index++)
            {
                if (MessageHandlers.ContainsKey((TestMessageID)index) == false)
                {
                    MessageHandlers.Add((TestMessageID)index, null);
                }

                serverConnection.AddListener(index, connectionAdapter);
            }
        }

        private NetworkOutMessage CreateMessage(byte messageType)
        {
            NetworkOutMessage msg = serverConnection.CreateMessage(messageType);
            msg.Write(messageType);
            // Add the local userID so that the remote clients know whose message they are receiving
            msg.Write(LocalUserID);
            return msg;
        }

        public void SendHeadTransform(Vector3 position, Quaternion rotation)
        {
            // If we are connected to a session, broadcast our head info
            if (serverConnection != null && serverConnection.IsConnected())
            {
                // Create an outgoing network message to contain all the info we want to send
                NetworkOutMessage msg = CreateMessage((byte)TestMessageID.HeadTransform);

                AppendTransform(msg, position, rotation);

                // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.
                serverConnection.Broadcast(
                    msg,
                    MessagePriority.Immediate,
                    MessageReliability.UnreliableSequenced,
                    MessageChannel.Avatar);
            }
        }

        public void SendShareSticky(Vector3 position, byte[] bytesData,int colorIndex)
        {
            // If we are connected to a session, broadcast our head info
            if (serverConnection != null && serverConnection.IsConnected())
            {
                // Create an outgoing network message to contain all the info we want to send
                NetworkOutMessage msg = CreateMessage((byte)TestMessageID.SharedSticky);

                AppendVector3(msg, position);
                msg.WriteArray(bytesData, 1024);
                //msg.Write(message);
                msg.Write(colorIndex);

                // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.
                serverConnection.Broadcast(
                    msg,
                    MessagePriority.Immediate,
                    MessageReliability.UnreliableSequenced,
                    MessageChannel.Avatar);
            }
        }

        public void Send3dDraw(Vector3 position, int isDragShare)
        {
            // If we are connected to a session, broadcast our head info
            if (serverConnection != null && serverConnection.IsConnected())
            {
                // Create an outgoing network message to contain all the info we want to send
                NetworkOutMessage msg = CreateMessage((byte)TestMessageID.Draw3d);

                AppendVector3(msg, position);
                msg.Write(isDragShare);

                // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.
                serverConnection.Broadcast(
                    msg,
                    MessagePriority.Immediate,
                    MessageReliability.UnreliableSequenced,
                    MessageChannel.Avatar);
            }
        }

        public void SendMovePos(Vector3 position, Quaternion rotation, int index, int isDrag)
        {
            // If we are connected to a session, broadcast our head info
            if (serverConnection != null && serverConnection.IsConnected())
            {
                // Create an outgoing network message to contain all the info we want to send
                NetworkOutMessage msg = CreateMessage((byte)TestMessageID.MovePos);

                AppendVector3(msg, position);
                AppendQuaternion(msg, rotation);
                msg.Write(index);
                msg.Write(isDrag);

                // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.
                serverConnection.Broadcast(
                    msg,
                    MessagePriority.Immediate,
                    MessageReliability.UnreliableSequenced,
                    MessageChannel.Avatar);
            }
        }

        public void SendVoiceLine(Vector3 position, Quaternion rotation, byte[] bytesData)
        {
            // If we are connected to a session, broadcast our head info
            if (serverConnection != null && serverConnection.IsConnected())
            {
                // Create an outgoing network message to contain all the info we want to send
                NetworkOutMessage msg = CreateMessage((byte)TestMessageID.VoiceLine);

                AppendVector3(msg, position);
                AppendQuaternion(msg, rotation);
                msg.WriteArray(bytesData, 1024);

                // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.
                serverConnection.Broadcast(
                    msg,
                    MessagePriority.Immediate,
                    MessageReliability.UnreliableSequenced,
                    MessageChannel.Avatar);
            }
        }

        public void SendOverheadText(Vector3 position, String overheadText)
        {
            // If we are connected to a session, broadcast our head info
            if (serverConnection != null && serverConnection.IsConnected())
            {
                // Create an outgoing network message to contain all the info we want to send
                NetworkOutMessage msg = CreateMessage((byte)TestMessageID.OverheadText);

                AppendVector3(msg, position);
                msg.Write(overheadText);

                // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.
                serverConnection.Broadcast(
                    msg,
                    MessagePriority.Immediate,
                    MessageReliability.UnreliableSequenced,
                    MessageChannel.Avatar);
            }
        }

        public void SendWallPos(Vector3 position, Quaternion rotation, int index, int isDrag)
        {
            // If we are connected to a session, broadcast our head info
            if (serverConnection != null && serverConnection.IsConnected())
            {
                // Create an outgoing network message to contain all the info we want to send
                NetworkOutMessage msg = CreateMessage((byte)TestMessageID.WallPos);

                AppendVector3(msg, position);
                AppendQuaternion(msg, rotation);
                msg.Write(index);
                msg.Write(isDrag);

                // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.
                serverConnection.Broadcast(
                    msg,
                    MessagePriority.Immediate,
                    MessageReliability.UnreliableSequenced,
                    MessageChannel.Avatar);
            }
        }

        public void DeleteShareSticky(int index)
        {
            // If we are connected to a session, broadcast our head info
            if (serverConnection != null && serverConnection.IsConnected())
            {
                // Create an outgoing network message to contain all the info we want to send
                NetworkOutMessage msg = CreateMessage((byte)TestMessageID.DeleteShareSticky);

                msg.Write(index);
                
                // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.
                serverConnection.Broadcast(
                    msg,
                    MessagePriority.Immediate,
                    MessageReliability.UnreliableSequenced,
                    MessageChannel.Avatar);
            }
        }

        public void DeleteAllShareSticky()
        {
            // If we are connected to a session, broadcast our head info
            if (serverConnection != null && serverConnection.IsConnected())
            {
                // Create an outgoing network message to contain all the info we want to send
                NetworkOutMessage msg = CreateMessage((byte)TestMessageID.DeleteAllShareSticky);

                // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.
                serverConnection.Broadcast(
                    msg,
                    MessagePriority.Immediate,
                    MessageReliability.UnreliableSequenced,
                    MessageChannel.Avatar);
            }
        }

        public void DeleteShareVoiceLine()
        {
            // If we are connected to a session, broadcast our head info
            if (serverConnection != null && serverConnection.IsConnected())
            {
                // Create an outgoing network message to contain all the info we want to send
                NetworkOutMessage msg = CreateMessage((byte)TestMessageID.DeleteShareVoiceLine);

                // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.
                serverConnection.Broadcast(
                    msg,
                    MessagePriority.Immediate,
                    MessageReliability.UnreliableSequenced,
                    MessageChannel.Avatar);
            }
        }

        //public void DeleteMyVoiceLine(int index)
        //{
        //    // If we are connected to a session, broadcast our head info
        //    if (serverConnection != null && serverConnection.IsConnected())
        //    {
        //        // Create an outgoing network message to contain all the info we want to send
        //        NetworkOutMessage msg = CreateMessage((byte)TestMessageID.DeleteMyVoiceLine);

        //        msg.Write(index);

        //        // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.
        //        serverConnection.Broadcast(
        //            msg,
        //            MessagePriority.Immediate,
        //            MessageReliability.UnreliableSequenced,
        //            MessageChannel.Avatar);
        //    }
        //}

        //public void ResetVoiceLine()
        //{
        //    // If we are connected to a session, broadcast our head info
        //    if (serverConnection != null && serverConnection.IsConnected())
        //    {
        //        // Create an outgoing network message to contain all the info we want to send
        //        NetworkOutMessage msg = CreateMessage((byte)TestMessageID.ResetVoiceLine);

        //        // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.
        //        serverConnection.Broadcast(
        //            msg,
        //            MessagePriority.Immediate,
        //            MessageReliability.UnreliableSequenced,
        //            MessageChannel.Avatar);
        //    }
        //}

        public void ResetShareLine()
        {
            // If we are connected to a session, broadcast our head info
            if (serverConnection != null && serverConnection.IsConnected())
            {
                // Create an outgoing network message to contain all the info we want to send
                NetworkOutMessage msg = CreateMessage((byte)TestMessageID.ResetShareLine);

                // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.
                serverConnection.Broadcast(
                    msg,
                    MessagePriority.Immediate,
                    MessageReliability.UnreliableSequenced,
                    MessageChannel.Avatar);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (serverConnection != null)
            {
                for (byte index = (byte)TestMessageID.HeadTransform; index < (byte)TestMessageID.Max; index++)
                {
                    serverConnection.RemoveListener(index, connectionAdapter);
                }
                connectionAdapter.MessageReceivedCallback -= OnMessageReceived;
            }
        }

        private void OnMessageReceived(NetworkConnection connection, NetworkInMessage msg)
        {
            byte messageType = msg.ReadByte();
            MessageCallback messageHandler = MessageHandlers[(TestMessageID)messageType];
            if (messageHandler != null)
            {
                messageHandler(msg);
            }
        }

        #region HelperFunctionsForWriting

        private void AppendTransform(NetworkOutMessage msg, Vector3 position, Quaternion rotation)
        {
            AppendVector3(msg, position);
            AppendQuaternion(msg, rotation);
        }

        private void AppendVector3(NetworkOutMessage msg, Vector3 vector)
        {
            msg.Write(vector.x);
            msg.Write(vector.y);
            msg.Write(vector.z);
        }

        private void AppendQuaternion(NetworkOutMessage msg, Quaternion rotation)
        {
            msg.Write(rotation.x);
            msg.Write(rotation.y);
            msg.Write(rotation.z);
            msg.Write(rotation.w);
        }

        #endregion

        #region HelperFunctionsForReading

        public Vector3 ReadVector3(NetworkInMessage msg)
        {
            return new Vector3(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
        }

        public Quaternion ReadQuaternion(NetworkInMessage msg)
        {
            return new Quaternion(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
        }

        #endregion
    }
}