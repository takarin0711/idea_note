using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall_Collision : MonoBehaviour {
    GameObject surface;
    private Rigidbody body;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

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
                offsetDistance = 0f;
            }
            //衝突判定のあったポイントを少し浮かせる
            Vector3 placementPosition = collision.contacts[0].point + (offsetDistance * collision.contacts[0].normal);

            GameObject obj=gameObject;

            // ぶつかった壁の方向を向かせる
            Quaternion rotation = Quaternion.LookRotation(surface.transform.forward, Vector3.up);
            obj.transform.position = placementPosition;
            obj.transform.rotation = rotation;

            body = obj.GetComponent<Rigidbody>();
            body.isKinematic = true;
            //Instantiate(gameObject, placementPosition, rotation);
            Debug.Log("Collision");
        }

    }

}
