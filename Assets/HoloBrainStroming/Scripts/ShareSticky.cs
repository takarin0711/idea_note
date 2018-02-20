using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;
using UnityEngine.UI;
using HoloToolkit.Sharing.Tests;
using HoloToolkit.Sharing;

public class ShareSticky : MonoBehaviour, IInputClickHandler
{
    public GameObject Sticky;
    public bool isShared;


    //public List<GameObject> shareStickyList = new List<GameObject>();

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (!isShared)
        {
            isShared = true;
            float randamX = UnityEngine.Random.Range(-0.1f, 0.1f);
            float randamY = UnityEngine.Random.Range(0f, 0.1f);
            float randamZ = UnityEngine.Random.Range(-0.1f, 0.1f);
            //Vector3 MoveToPos = new Vector3(MakeStickyManager.Instance.MyStickyHolder.position.x + randomX, MakeStickyManager.Instance.MyStickyHolder.position.y + randomY, MakeStickyManager.Instance.MyStickyHolder.position.z + randamZ);
            Vector3 MoveToPos = new Vector3(MakeStickyManager.kariPos.x+randamX, MakeStickyManager.kariPos.y+randamY, MakeStickyManager.kariPos.z+randamZ);
            iTween.MoveTo(Sticky, MoveToPos, 7f);
            Text tex = Sticky.transform.transform.Find("HoloStickyUI").transform.transform.Find("Image").transform.transform.Find("Text").GetComponent<Text>();
            //tex.color = UserColorManager.Instance.myColor;
            tex.color = Color.red;

            Data.shareStickyList.Add(Sticky);
            Data.WallStickyList.Add(0);

            DontDestroyOnLoad(Sticky);

            byte[] bytesData = new byte[1024];
            bytesData = System.Text.Encoding.UTF8.GetBytes(tex.text);
            CustomMessages.Instance.SendShareSticky(MoveToPos, bytesData, UserColorManager.Instance.mynumber);
            //CustomMessages.Instance.SendShareSticky(MoveToPos, tex.text, UserColorManager.Instance.mynumber);
        }
        else
        {
            MakeStickyManager.Instance.SelectingSticky = Sticky;
        }
        

    }

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {


		
	}
}
