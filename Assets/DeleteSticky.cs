using HoloToolkit.Sharing;
using HoloToolkit.Sharing.Tests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteSticky : MonoBehaviour {
    RaycastHit hitInfo;

    // Use this for initialization
    void Start () {
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.DeleteShareSticky] = ReceiveDelete;
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.DeleteAllShareSticky] = ReceiveAllDelete;

    }
	
	// Update is called once per frame
	void Update () {
        bool hit = Physics.Raycast(Camera.main.transform.position,
                                Camera.main.transform.forward,
                                out hitInfo,
                                20f);

        //Debug.Log(hitInfo.collider.gameObject.transform.parent.gameObject.tag);
    }

    private void ReceiveDelete(NetworkInMessage msg)
    {
        // Parse the message
        long userID = msg.ReadInt64();

        int ind = msg.ReadInt32();

        ReceiveDeleteSticky(ind);
    }

    private void ReceiveAllDelete(NetworkInMessage msg)
    {
        // Parse the message
        long userID = msg.ReadInt64();

        ReceiveAllDeleteSticky();
    }

    public void ReceiveDeleteSticky(int ind)
    {
        Destroy(Data.shareStickyList[ind]);
        //Data.shareStickyList[ind] = null;
        //Data.WallStickyList[ind] = 0;
    }

    public void ReceiveAllDeleteSticky()
    {
        Data.shareStickyList.Clear();
        Data.WallStickyList.Clear();
    }

    public void DeleteMySticky()
    {
        if (hitInfo.collider.gameObject.transform.parent.gameObject.tag == "Sticky")
        {
            Destroy(hitInfo.collider.gameObject.transform.parent.gameObject);
        }
    }

    public void DeleteShareSticky()
    {
        if (hitInfo.collider.gameObject.transform.parent.gameObject.tag == "Sticky")
        {
            for (int i = 0; i < Data.shareStickyList.Count; i++)
            {
                if (Data.shareStickyList[i] == hitInfo.collider.gameObject.transform.parent.gameObject)
                {
                    Destroy(Data.shareStickyList[i]);
                    CustomMessages.Instance.DeleteShareSticky(i);
                    Data.shareStickyList[i] = null;
                    Data.WallStickyList[i] = 0;
                }
            }
        }
    }

    public void ResetShareSticky()
    {
        for (int i = 0; i < Data.shareStickyList.Count; i++)
        {
            Destroy(Data.shareStickyList[i]);
            CustomMessages.Instance.DeleteShareSticky(i);
            Data.shareStickyList[i] = null;
            Data.WallStickyList[i] = 0;
        }
        Data.shareStickyList.Clear();
        Data.WallStickyList.Clear();
        CustomMessages.Instance.DeleteAllShareSticky();
    }
}
