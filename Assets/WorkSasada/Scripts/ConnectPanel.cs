using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectPanel : MonoBehaviour {

    public GameObject objLine;

    // Use this for initialization
    void Start() {
        GameObject ideaPanel02, ideaPanel03;
        ideaPanel02 = GameObject.Find("sampleIdea02");
        ideaPanel03 = GameObject.Find("sampleIdea03");

        Color c1 = Color.yellow;

        Connect2Panels(ideaPanel02, ideaPanel03, c1);

    }

    // Update is called once per frame
    void Update() {
    }

    void Connect2Panels(GameObject fromPanel, GameObject toPanel, Color lineColor) {

        //オブジェクト生成
        GameObject returnLine = Instantiate(objLine, new Vector3(-0.5f, 0.0f, 1.0f), Quaternion.identity) as GameObject;
        LineRenderer lineRenderer = returnLine.GetComponent<LineRenderer>();

        // 線の始点と終点を設定
        lineRenderer.SetPosition(0, fromPanel.transform.position);
        lineRenderer.SetPosition(1, toPanel.transform.position);

        // 色の指定
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
    }
}
