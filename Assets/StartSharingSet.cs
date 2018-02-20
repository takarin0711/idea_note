using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSharingSet : MonoBehaviour {
    public GameObject Sharing;
    public GameObject HologramCollection;
    GameObject StartSharing;
    GameObject StartHologramCollection;

    // Use this for initialization
    void Start () {
        GameObject exsistStartSharing = GameObject.Find("StartSharing(Clone)");
        GameObject exsistStartHologramCollection = GameObject.Find("StartHologramCollection(Clone)");
        if (exsistStartSharing == null) {
            StartSharing = Instantiate(Sharing);
            DontDestroyOnLoad(StartSharing);
        }
        if (exsistStartHologramCollection == null)
        {
            StartHologramCollection = Instantiate(HologramCollection);
            DontDestroyOnLoad(StartHologramCollection);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
