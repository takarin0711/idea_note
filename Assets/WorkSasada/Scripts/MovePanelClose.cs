using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePanelClose : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        GameObject ideaPanel01, ideaPanel02;
        ideaPanel01 = GameObject.Find("sampleIdea01");
        ideaPanel02 = GameObject.Find("sampleIdea02");

        float distance = 0.2f;
        float speed = 0.5f;

        moveClose(ideaPanel01, ideaPanel02, distance, speed);
    }

    void moveClose(GameObject movePanel, GameObject stablePanel, float nearestDistance, float moveSpeed)
    {
        // 1. 移動する付箋
        // 2. 移動しない付箋
        // 3. 近づいた場合の最短距離
        // 4. 移動速度に関するパラメータ（値が大きいほど早い）

        Vector3 fromPosition = movePanel.transform.position;
        Vector3 toPosition = stablePanel.transform.position;

        // 指定の距離までパネルを移動する
        if (Vector3.Magnitude(fromPosition - toPosition) > nearestDistance)
        {
            // 補完
            movePanel.transform.position = Vector3.Lerp(fromPosition, toPosition, Time.deltaTime * moveSpeed);

            // 等速
            // movePanel.transform.position = movePanel.transform.position + (toPosition - fromPosition) * moveSpeed;
        }
    }
}
