using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall_receiver : MonoBehaviour, ISourceStateHandler, IInputHandler
{
    public GameObject SomePrefab;

    GameObject surface;

    GameObject getObject;

    string objectName;

    RaycastHit hitInfo;
    bool hit;

    public void OnInputDown(InputEventData eventData)
    {
        if (!eventData.InputSource.SupportsInputInfo(eventData.SourceId, SupportedInputInfo.Position))
        {
            return;
        }

        //RaycastHit hitInfo;

        //bool hit = Physics.Raycast(Camera.main.transform.position,
        //                        Camera.main.transform.forward, out hitInfo, 20f);

        //objectName = hitInfo.collider.tag;

        if (hit && objectName != "Sticky")
        {
            //ヒットした表面の取得
            surface = hitInfo.transform.gameObject;

            Debug.Log(objectName);

            //表面からどれから浮かすかの値
            float hoverDistance = 0.15f;
            float offsetDistance = hoverDistance;

            //設定したhoverDistanceよりGazeと衝突判定の発生した平面との距離が少ない場合0fにする。
            if (hitInfo.distance <= hoverDistance)
            {
                offsetDistance = 0f;
            }
            //衝突判定のあったポイントを少し浮かせる
            Vector3 placementPosition = hitInfo.point + (offsetDistance * hitInfo.normal);

            // ぶつかった壁の方向を向かせる
            Quaternion rotation = Quaternion.LookRotation(surface.transform.forward, Vector3.up);
            Instantiate(SomePrefab, placementPosition, rotation);
        }
        else if (hit && objectName == "Sticky" && getObject == null)
        {
            getObject = hitInfo.transform.gameObject;
            Destroy(getObject);
            Debug.Log(objectName);
            Debug.Log("aaa");
        }

        //Debug.Log(objectName);

        Debug.Log("finger down");
    }

    public void OnInputUp(InputEventData eventData)
    {
        Debug.Log("finger up");
    }

    public void OnSourceDetected(SourceStateEventData eventData)
    {
        Debug.Log("Detected");
    }

    public void OnSourceLost(SourceStateEventData eventData)
    {
        Debug.Log("Lost");
    }

    // Use this for initialization
    void Start () {
        InputManager.Instance.PushFallbackInputHandler(gameObject);
    }
	
	// Update is called once per frame
	void Update () {

        hit = Physics.Raycast(Camera.main.transform.position,
                                Camera.main.transform.forward, out hitInfo, 20f);

        objectName = hitInfo.collider.tag;
        //Debug.Log(objectName);
    }
}
