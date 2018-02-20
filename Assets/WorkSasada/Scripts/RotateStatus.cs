using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateStatus : MonoBehaviour
{
    //　カウンタ
    float cnt = 0.0f;

    // 公転半径の設定
    float tmpRadius = 1f;
    // 公転の中心の設定
    public Transform pos;


    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        rotateStatMsg(tmpRadius, pos.position);

        //if (cnt % 120 == 0) {
        //    chngMsg((int)((cnt % 30) % 4)+1);
        //}


    }

    // 引数を元に、会議のステータスを示すメッセージを自転と公転させる
    void rotateStatMsg(float radius, Vector3 centerPosition)
    {
        // 1. 公転半径の設定
        // 2. 公転の中心座標

        cnt += 1.0f;
        // 速度（値が大きいほど高速）
        float speed = 0.25f;
        this.transform.Rotate(new Vector3(0.0f, speed * 360.0f / 60.0f , 0.0f));
        this.transform.position = radius * new Vector3(-1.0f * Mathf.Sin(speed * 2.0f * Mathf.PI * cnt / 60.0f),1f , -1.0f * Mathf.Cos(speed * 2.0f * Mathf.PI * cnt / 60.0f)) + centerPosition;
    }

    // 引数を元に、会議のステータスを示すメッセージを変更する
    public void chngMsg(int statNum)
    {
        // 1. ステータスのID

        if (statNum == 1)
        {
            this.GetComponent<TextMesh>().text = "Start";
        }
        else if (statNum == 2)
        {
            this.GetComponent<TextMesh>().text = "Join";
        }
        else if (statNum == 3)
        {
            this.GetComponent<TextMesh>().text = "Share";
        }
        else if (statNum == 4)
        {
            this.GetComponent<TextMesh>().text = "Finish";
        }
        else {
            this.GetComponent<TextMesh>().text = "Not Started";
        }
        
    }

}
