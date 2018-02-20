using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateTitle : MonoBehaviour {

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
    void Update()
    {
    }

    public void updateDiscussionTitle(string text)
    {

        this.GetComponent<TextMesh>().text = text; 
    }
}
