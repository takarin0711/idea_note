// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System;
using HoloToolkit.Sharing;
using HoloToolkit.Sharing.Tests;
using UnityEngine.UI;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Component that allows dragging an object with your hand on HoloLens.
    /// Dragging is done by calculating the angular delta and z-delta between the current and previous hand positions,
    /// and then repositioning the object based on that.
    /// </summary>
    public class Space_receiver : MonoBehaviour,
                                 IFocusable,
                                 IInputHandler,
                                 ISourceStateHandler
    {
        /// <summary>
        /// Event triggered when dragging starts.
        /// </summary>
        public event Action StartedDragging;

        /// <summary>
        /// Event triggered when dragging stops.
        /// </summary>
        public event Action StoppedDragging;

        [Tooltip("Transform that will be dragged. Defaults to the object of the component.")]
        public Transform HostTransform;

        [Tooltip("Scale by which hand movement in z is multipled to move the dragged object.")]
        public float DistanceScale = 2f;

        public enum RotationModeEnum
        {
            Default,
            LockObjectRotation,
            OrientTowardUser,
            OrientTowardUserAndKeepUpright
        }

        public RotationModeEnum RotationMode = RotationModeEnum.Default;

        [Tooltip("Controls the speed at which the object will interpolate toward the desired position")]
        [Range(0.01f, 1.0f)]
        public float PositionLerpSpeed = 0.2f;

        [Tooltip("Controls the speed at which the object will interpolate toward the desired rotation")]
        [Range(0.01f, 1.0f)]
        public float RotationLerpSpeed = 0.2f;

        public bool IsDraggingEnabled = true;

        private Camera mainCamera;
        private bool isDragging;
        private bool isGazed;
        private Vector3 objRefForward;
        private Vector3 objRefUp;
        private float objRefDistance;
        private Quaternion gazeAngularOffset;
        private float handRefDistance;
        private Vector3 objRefGrabPoint;

        private Vector3 draggingPosition;
        private Quaternion draggingRotation;

        private IInputSource currentInputSource = null;
        private uint currentInputSourceId;

        Vector3 pos;
        Quaternion rot;
        int index = -1;
        GameObject HostGameObject;

        //壁の衝突関連
        GameObject surface;
        GameObject obj;
        private Rigidbody body;
        Vector3 placementPosition;
        Quaternion rotation;
        bool shoutotu = false;
        int isShareDrag = 0;
        int ind = -1;
        //int isWall;

        int sendFlag = 0;
        Transform shareTransform;

        RaycastHit hitInfo;
        public GameObject HoloSticky;
        public GameObject Sticky;
        public Text displayText;
        bool displayTextExist = false;

        private void Start()
        {
            if (HostTransform == null)
            {
                HostTransform = transform;
                HostGameObject = gameObject;
            }

            //mainCamera = Camera.main;

            CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.MovePos] = SharedMovePos;
            CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.WallPos] = SharedWallPos;

            // SharingStage should be valid at this point, but we may not be connected.
            if (SharingStage.Instance.IsConnected)
            {
                Connected();
            }
            else
            {
                SharingStage.Instance.SharingManagerConnected += Connected;
            }

            //付箋の衝突処理を無視する対象をここに記述
            int layer1 = LayerMask.NameToLayer("Sticky");
            int layer2 = LayerMask.NameToLayer("Sticky");
            int layer3 = LayerMask.NameToLayer("InputUI");
            //Physics.IgnoreLayerCollision(layer1, layer2);
            Physics.IgnoreLayerCollision(layer1, layer3);
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

        private void OnDestroy()
        {
            if (isDragging)
            {
                StopDragging();
            }

            if (isGazed)
            {
                OnFocusExit();
            }
        }

        private void Update()
        {
            //RaycastHit hitInfo;

            String objectName;

            bool hit = Physics.Raycast(Camera.main.transform.position,
                                    Camera.main.transform.forward,
                                    out hitInfo,
                                    20f);

            objectName = hitInfo.collider.gameObject.name;

            // 付箋が視線上にあるとき
            if (hit && objectName == "BackFace")
            {
                // ある程度横または裏側から見たとき
                if (Vector3.Angle(Camera.main.transform.forward, hitInfo.normal) < 130)
                {
                    displayTextExist = true;
                }
                else
                {
                    displayTextExist = false;
                }

            }
            else
            {
                if (displayTextExist == true)
                {
                    displayTextExist = false;
                }
            }

            // 目の前にテキストを表示させるかどうか
            if (displayTextExist == true)
            {
                if (Sticky == null)
                {
                    MakeDisplayText();
                }
            }
            else
            {
                if (Sticky != null)
                {
                    Destroy(Sticky);
                    Sticky = null;
                }
            }

            //以下、付箋の動きに関する処理
            index = Data.shareStickyList.IndexOf(HostGameObject);

            if (isDragging == false && sendFlag == 1)
            {
                if (ind != -1)
                {
                    isShareDrag = 0;
                    //Data.WallStickyList[index] = 1;
                    //CustomMessages.Instance.SendMovePos(pos, rot, ind, isShareDrag);

                    ind = -1;
                    sendFlag = 1;
                }
            }

            if (IsDraggingEnabled && isDragging)
            {
                UpdateDragging();

                if (index != -1 && sendFlag == 0)
                {
                    CustomMessages.Instance.SendMovePos(pos, rot, index, isShareDrag);
                }

            }

        }

        /// <summary>
        /// Starts dragging the object.
        /// </summary>
        public void StartDragging()
        {
            if (!IsDraggingEnabled)
            {
                return;
            }

            if (isDragging)
            {
                return;
            }

            // Add self as a modal input handler, to get all inputs during the manipulation
            InputManager.Instance.PushModalInputHandler(gameObject);

            isDragging = true;
            isShareDrag = 1;
            sendFlag = 0;
            if (index != -1)
            {
                Data.WallStickyList[index] = 0;
            }
            //GazeCursor.Instance.SetState(GazeCursor.State.Move);
            //GazeCursor.Instance.SetTargetObject(HostTransform);

            Vector3 gazeHitPosition = GazeManager.Instance.HitInfo.point;
            Vector3 handPosition;
            currentInputSource.TryGetPosition(currentInputSourceId, out handPosition);

            Vector3 pivotPosition = GetHandPivotPosition();
            handRefDistance = Vector3.Magnitude(handPosition - pivotPosition);
            objRefDistance = Vector3.Magnitude(gazeHitPosition - pivotPosition);

            Vector3 objForward = HostTransform.forward;
            Vector3 objUp = HostTransform.up;

            // Store where the object was grabbed from
            objRefGrabPoint = Camera.main.transform.InverseTransformDirection(HostTransform.position - gazeHitPosition);

            Vector3 objDirection = Vector3.Normalize(gazeHitPosition - pivotPosition);
            Vector3 handDirection = Vector3.Normalize(handPosition - pivotPosition);

            objForward = Camera.main.transform.InverseTransformDirection(objForward);       // in camera space
            objUp = Camera.main.transform.InverseTransformDirection(objUp);       		   // in camera space
            objDirection = Camera.main.transform.InverseTransformDirection(objDirection);   // in camera space
            handDirection = Camera.main.transform.InverseTransformDirection(handDirection); // in camera space

            objRefForward = objForward;
            objRefUp = objUp;

            // Store the initial offset between the hand and the object, so that we can consider it when dragging
            gazeAngularOffset = Quaternion.FromToRotation(handDirection, objDirection);
            draggingPosition = gazeHitPosition;

            StartedDragging.RaiseEvent();
        }

        /// <summary>
        /// Gets the pivot position for the hand, which is approximated to the base of the neck.
        /// </summary>
        /// <returns>Pivot position for the hand.</returns>
        private Vector3 GetHandPivotPosition()
        {
            Vector3 pivot = Camera.main.transform.position + new Vector3(0, -0.2f, 0) - Camera.main.transform.forward * 0.2f; // a bit lower and behind
            return pivot;
        }

        /// <summary>
        /// Enables or disables dragging.
        /// </summary>
        /// <param name="isEnabled">Indicates whether dragging shoudl be enabled or disabled.</param>
        public void SetDragging(bool isEnabled)
        {
            if (IsDraggingEnabled == isEnabled)
            {
                return;
            }

            IsDraggingEnabled = isEnabled;

            if (isDragging)
            {
                StopDragging();
            }
        }

        /// <summary>
        /// Update the position of the object being dragged.
        /// </summary>
        private void UpdateDragging()
        {
            Vector3 newHandPosition;
            currentInputSource.TryGetPosition(currentInputSourceId, out newHandPosition);

            Vector3 pivotPosition = GetHandPivotPosition();

            Vector3 newHandDirection = Vector3.Normalize(newHandPosition - pivotPosition);

            newHandDirection = Camera.main.transform.InverseTransformDirection(newHandDirection); // in camera space
            Vector3 targetDirection = Vector3.Normalize(gazeAngularOffset * newHandDirection);
            targetDirection = Camera.main.transform.TransformDirection(targetDirection); // back to world space

            float currenthandDistance = Vector3.Magnitude(newHandPosition - pivotPosition);

            float distanceRatio = currenthandDistance / handRefDistance;
            float distanceOffset = distanceRatio > 0 ? (distanceRatio - 1f) * DistanceScale : 0;
            float targetDistance = objRefDistance + distanceOffset;

            draggingPosition = pivotPosition + (targetDirection * targetDistance);

            if (RotationMode == RotationModeEnum.OrientTowardUser || RotationMode == RotationModeEnum.OrientTowardUserAndKeepUpright)
            {
                draggingRotation = Quaternion.LookRotation(HostTransform.position - pivotPosition);
            }
            else if (RotationMode == RotationModeEnum.LockObjectRotation)
            {
                draggingRotation = HostTransform.rotation;
            }
            else // RotationModeEnum.Default
            {
                Vector3 objForward = Camera.main.transform.TransformDirection(objRefForward); // in world space
                Vector3 objUp = Camera.main.transform.TransformDirection(objRefUp);   // in world space
                draggingRotation = Quaternion.LookRotation(objForward, objUp);
            }

            // Apply Final Position
            HostTransform.position = Vector3.Lerp(HostTransform.position, draggingPosition + Camera.main.transform.TransformDirection(objRefGrabPoint), PositionLerpSpeed);
            // Apply Final Rotation
            HostTransform.rotation = Quaternion.Lerp(HostTransform.rotation, draggingRotation, RotationLerpSpeed);

            if (RotationMode == RotationModeEnum.OrientTowardUserAndKeepUpright)
            {
                Quaternion upRotation = Quaternion.FromToRotation(HostTransform.up, Vector3.up);
                HostTransform.rotation = upRotation * HostTransform.rotation;
            }

            body = HostGameObject.GetComponent<Rigidbody>();
            body.isKinematic = true; //一度物理演算を止める

            if (body.isKinematic == true)
            {
                body.isKinematic = false;
            }

            HostGameObject.transform.transform.Find("HoloStickyUI").transform.transform.Find("Image").GetComponent<StickyFaceMe>().enabled = true;
            HostGameObject.transform.transform.Find("BackFace").GetComponent<StickyFaceMe>().enabled = true;

            pos = HostTransform.position;
            rot = HostTransform.rotation;

            //index = Data.shareStickyList.IndexOf(HostGameObject);
            //if (index != -1)
            //{
            //    Data.WallStickyList[index] = 0;
            //}

            //isShareDrag = 1;

            //sendFlag = 0;

            Debug.Log(index);
            //Debug.Log(Data.WallStickyList[index]);

        }

        /// <summary>
        /// Stops dragging the object.
        /// </summary>
        public void StopDragging()
        {
            if (!isDragging)
            {
                return;
            }

            // Remove self as a modal input handler
            InputManager.Instance.PopModalInputHandler();

            isDragging = false;

            isShareDrag = 0;

            //放したら壁に貼り付ける
            //if (shoutotu == true)
            //{
            //    obj.transform.position = placementPosition;
            //    obj.transform.rotation = rotation;

            //    pos = placementPosition;
            //    rot = rotation;

            //    body = obj.GetComponent<Rigidbody>();
            //    body.isKinematic = true;

            //    obj.transform.transform.Find("HoloStickyUI").transform.transform.Find("Image").GetComponent<StickyFaceMe>().enabled = false;
            //    obj.transform.transform.Find("BackFace").GetComponent<StickyFaceMe>().enabled = false;

            //    sendFlag = 1;

            //    int ind = Data.shareStickyList.IndexOf(obj);
            //    if (ind != -1)
            //    {
            //        Data.WallStickyList[ind] = 1;

            //        //CustomMessages.Instance.SendMovePos(pos, rot, ind, isShareDrag);   
            //    }


            //    ind = -1;
            //    obj = null;
            //    shoutotu = false;

            //}

            index = -1;
            //isDragging = false;
            currentInputSource = null;
            StoppedDragging.RaiseEvent();
        }

        public void OnFocusEnter()
        {
            if (!IsDraggingEnabled)
            {
                return;
            }

            if (isGazed)
            {
                return;
            }

            isGazed = true;
        }

        public void OnFocusExit()
        {
            if (!IsDraggingEnabled)
            {
                return;
            }

            if (!isGazed)
            {
                return;
            }

            isGazed = false;
        }

        public void OnInputUp(InputEventData eventData)
        {
            if (currentInputSource != null &&
                eventData.SourceId == currentInputSourceId)
            {
                isShareDrag = 0;
                //sendFlag = 1;
                StopDragging();
            }
        }

        public void OnInputDown(InputEventData eventData)
        {
            if (isDragging)
            {
                // We're already handling drag input, so we can't start a new drag operation.
                return;
            }

            if (!eventData.InputSource.SupportsInputInfo(eventData.SourceId, SupportedInputInfo.Position))
            {
                // The input source must provide positional data for this script to be usable
                return;
            }

            currentInputSource = eventData.InputSource;
            currentInputSourceId = eventData.SourceId;
            StartDragging();
        }

        public void OnSourceDetected(SourceStateEventData eventData)
        {
            // Nothing to do
        }

        public void OnSourceLost(SourceStateEventData eventData)
        {
            if (currentInputSource != null && eventData.SourceId == currentInputSourceId)
            {
                isShareDrag = 0;
                //sendFlag = 1;
                StopDragging();
            }
        }

        //壁に衝突したときの処理
        void OnCollisionEnter(Collision collision)
        {
            //Destroy(collision.gameObject);
            //Destroy(this.gameObject);
            if (collision.gameObject.tag == "Wall")
            {
                //ヒットした表面の取得
                surface = collision.transform.gameObject;

                //表面からどれから浮かすかの値
                float hoverDistance = 0.15f;
                float offsetDistance = hoverDistance;

                //設定したhoverDistanceよりGazeと衝突判定の発生した平面との距離が少ない場合0fにする。
                if (Vector3.Distance(gameObject.transform.position, collision.contacts[0].point) <= hoverDistance)
                {
                    offsetDistance = 0.03f;
                }
                //衝突判定のあったポイントを少し浮かせる
                placementPosition = collision.contacts[0].point + (offsetDistance * collision.contacts[0].normal);

                obj = HostGameObject;

                // ぶつかった壁の方向を向かせる
                rotation = Quaternion.LookRotation(surface.transform.forward, Vector3.up);

                obj.transform.position = placementPosition;
                obj.transform.rotation = rotation;

                pos = placementPosition;
                rot = rotation;

                body = obj.GetComponent<Rigidbody>();
                body.isKinematic = true;

                obj.transform.transform.Find("HoloStickyUI").transform.transform.Find("Image").GetComponent<StickyFaceMe>().enabled = false;
                obj.transform.transform.Find("BackFace").GetComponent<StickyFaceMe>().enabled = false;

                ind = Data.shareStickyList.IndexOf(obj);
                if (ind != -1)
                {
                    Data.WallStickyList[ind] = 1;

                    sendFlag = 1;

                    //CustomMessages.Instance.SendMovePos(pos, rot, ind, isShareDrag);   
                }

                obj = null;

                StopDragging();

                sendFlag = 1;

                CustomMessages.Instance.SendWallPos(pos, rot, ind, isShareDrag);
                //shoutotu = false;

                //index = Data.shareStickyList.IndexOf(obj);
                //if (index != -1)
                //{
                //    Data.WallStickyList[index] = 1;
                //}

                //shoutotu = true;

                //obj.transform.position = placementPosition;
                //obj.transform.rotation = rotation;

                //body = obj.GetComponent<Rigidbody>();
                //body.isKinematic = true;
                //Instantiate(gameObject, placementPosition, rotation);
                Debug.Log("Collision");
            }

        }

        public void MakeDisplayText()
        {
            //GameObject gameObject;
            Text tex;
            String temp;
            Sticky = GameObject.Instantiate(HoloSticky);

            tex = hitInfo.collider.gameObject.transform.parent.gameObject.transform.transform.Find("HoloStickyUI").transform.transform.Find("Image").transform.transform.Find("Text").GetComponent<Text>();
            temp = tex.text;

            Sticky.transform.position = Camera.main.transform.TransformPoint(new Vector3(0f, 0f, 0.8f));
            Sticky.transform.rotation = Quaternion.LookRotation(Sticky.transform.position - Camera.main.transform.position);
            displayText = Sticky.transform.transform.Find("HoloStickyUI").transform.transform.Find("Image").transform.transform.Find("Text").GetComponent<Text>();
            displayText.text = temp;

            //Debug.Log("MakeDisplayText!");
        }

        private void SharedMovePos(NetworkInMessage msg)
        {
            // Parse the message
            long userID = msg.ReadInt64();

            Vector3 MovePos = CustomMessages.Instance.ReadVector3(msg);
            Quaternion rt = CustomMessages.Instance.ReadQuaternion(msg);
            int id = msg.ReadInt32();
            int isD = msg.ReadInt32();
            MovePosition(MovePos, rt, id, isD);
            //MakeSticky(message, StickyPos, colorIndex);
        }

        private void SharedWallPos(NetworkInMessage msg)
        {
            // Parse the message
            long userID = msg.ReadInt64();

            Vector3 MovePos = CustomMessages.Instance.ReadVector3(msg);
            Quaternion rt = CustomMessages.Instance.ReadQuaternion(msg);
            int id = msg.ReadInt32();
            int isD = msg.ReadInt32();
            WallPosition(MovePos, rt, id, isD);
            //MakeSticky(message, StickyPos, colorIndex);
        }

        public void MovePosition(Vector3 mp, Quaternion rt, int id, int isD)
        {
            Debug.Log(id);

            int isWall = Data.WallStickyList[id];

            //if (isD == 0 && isWall == 1) //壁にあるとき
            //{
            //    GameObject tmp = Data.shareStickyList[id];
            //    Rigidbody bd = tmp.GetComponent<Rigidbody>();
            //    bd.isKinematic = true;

            //    tmp.transform.position = mp;
            //    tmp.transform.rotation = rt;
            //    tmp.transform.transform.Find("HoloStickyUI").transform.transform.Find("Image").GetComponent<StickyFaceMe>().enabled = false;
            //    tmp.transform.transform.Find("BackFace").GetComponent<StickyFaceMe>().enabled = false;
            //    Debug.Log("壁");
            //}

            if (isD == 1) //空間上にあるとき
            {
                GameObject tmp = Data.shareStickyList[id];
                Rigidbody bd = tmp.GetComponent<Rigidbody>();
                bd.isKinematic = true; //一度物理演算を止める
                bd.isKinematic = false;

                tmp.transform.transform.Find("HoloStickyUI").transform.transform.Find("Image").GetComponent<StickyFaceMe>().enabled = true;
                tmp.transform.transform.Find("BackFace").GetComponent<StickyFaceMe>().enabled = true;
                tmp.transform.position = mp;
                Debug.Log("空間");
            }

            //shareTransform.position = mp;
        }

        public void WallPosition(Vector3 mp, Quaternion rt, int id, int isD)
        {
            Debug.Log(id);

            int isWall = Data.WallStickyList[id];

            GameObject tmp = Data.shareStickyList[id];
            Rigidbody bd = tmp.GetComponent<Rigidbody>();
            bd.isKinematic = true;

            tmp.transform.position = mp;
            tmp.transform.rotation = rt;
            tmp.transform.transform.Find("HoloStickyUI").transform.transform.Find("Image").GetComponent<StickyFaceMe>().enabled = false;
            tmp.transform.transform.Find("BackFace").GetComponent<StickyFaceMe>().enabled = false;
            Debug.Log("壁");

            //if (isD == 1) //空間上にあるとき
            //{
            //    GameObject tmp = Data.shareStickyList[id];
            //    Rigidbody bd = tmp.GetComponent<Rigidbody>();
            //    bd.isKinematic = true; //一度物理演算を止める
            //    bd.isKinematic = false;

            //    tmp.transform.transform.Find("HoloStickyUI").transform.transform.Find("Image").GetComponent<StickyFaceMe>().enabled = true;
            //    tmp.transform.transform.Find("BackFace").GetComponent<StickyFaceMe>().enabled = true;
            //    tmp.transform.position = mp;
            //    Debug.Log("空間");
            //}
        }

        public void ResetShareSticky()
        {
            for (int i = 0; i < Data.shareStickyList.Count; i++)
            {
                Destroy(Data.shareStickyList[i]);
                Data.shareStickyList[i] = null;
            }
            Data.shareStickyList.Clear();

        }
    }
}
