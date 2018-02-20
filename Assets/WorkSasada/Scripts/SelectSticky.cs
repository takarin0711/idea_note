using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectSticky : MonoBehaviour {

    int cnt = 0;
    float rate = 1.5f;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        cnt++;

        if (cnt % 60 == 30)
        {
            selectStickyOn(rate);
        }
        else if (cnt % 60 == 0)
        {
            selectStickyOff(rate);
        }
    }

    //選択された付箋を強調（拡大・透過）する
    void selectStickyOn(float expandRate)
    {
        // 1. expandRate（拡大率）

        this.transform.localScale = this.transform.localScale * expandRate;
        MeshRenderer meshrender = this.GetComponent<MeshRenderer>();
        this.GetComponent<MeshRenderer>().material.color = new Color(meshrender.material.color.r, meshrender.material.color.g, meshrender.material.color.b, 0.5f);
        //     Debug.Log(meshrender.material.color.ToString());
    }

    //選択された付箋の強調を解除（縮小拡大・非透過）する
    void selectStickyOff(float expandRate)
    {
        // 1. expandRate（拡大率）

        this.transform.localScale = this.transform.localScale / expandRate;
        MeshRenderer meshrender = this.GetComponent<MeshRenderer>();
        this.GetComponent<MeshRenderer>().material.color = new Color(meshrender.material.color.r, meshrender.material.color.g, meshrender.material.color.b, 1.0f);
    }
}
