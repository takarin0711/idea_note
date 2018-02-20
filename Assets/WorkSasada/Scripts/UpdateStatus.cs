using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateStatus : MonoBehaviour {

    /*
	// Use this for initialization
	void Start () {
        m_DictationRecognizer = new DictationRecognizer();
        m_DictationRecognizer.InitialSilenceTimeoutSeconds = 30f;
        m_DictationRecognizer.DictationResult += (text, confidence) =>
        {
            if (canInput)
            {
                UpdateDiscussionStates(text);
            }
        };
    }
	
    */
	// Update is called once per frame
	void Update () {
		
	}

    public void UpdateDiscussionStates(string text)
    {
        RotateStatus stat = new RotateStatus();

        if (text.Equals("start session")) {
            stat.chngMsg(1);
        }
        else if (text.Equals("join session"))
        {
            stat.chngMsg(2);
        }
        else if (text.Equals("think idea"))
        {
            stat.chngMsg(3);
        }
        else if (text.Equals("share idea"))
        {
            stat.chngMsg(4);
        }
        else if (text.Equals("reset"))
        {
            stat.chngMsg(0);
        }
    }
}
