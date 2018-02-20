using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Data {

    public static List<GameObject> shareStickyList = new List<GameObject>(); // 共有した付箋

    public static List<int> WallStickyList = new List<int>(); // 壁に貼りついてるかどうか

    public static List<LineRenderer> shareLineList = new List<LineRenderer>(); // 共有した3D Drawの線

    public static List<GameObject> shareVoiceTextList = new List<GameObject>(); // 共有したVoiceLineの文字

	//// Use this for initialization
	//void Start () {
		
	//}
	
	//// Update is called once per frame
	//void Update () {
		
	//}

    public static int SerchShareSticky(GameObject Sticky)
    {
        return shareStickyList.IndexOf(Sticky);
    }

    public static GameObject GetShareSticky(int index)
    {
        return shareStickyList[index];
    }

    public static void SetShareSticky(GameObject Sticky)
    {
        shareStickyList.Add(Sticky);
    }

    public static void DebugSticky()
    {
        Debug.Log(shareStickyList[0]);
    }
}
