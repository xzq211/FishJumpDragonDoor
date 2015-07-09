using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
    GameObject mplayer;
    GameObject mCamera;
	// Use this for initialization
	void Start () {
        mplayer = GameObject.FindGameObjectWithTag("Player");
        mCamera = GameObject.FindGameObjectWithTag("MainCamera");
	}
	
	// Update is called once per frame
	void Update () {
      
	}
    void LateUpdate()
    {
        if (mplayer.transform.position.y + 1 > mCamera.transform.position.y)
        {
            mCamera.transform.position = new Vector3(mCamera.transform.position.x, Mathf.Lerp(mCamera.transform.position.y, mplayer.transform.position.y + 1, 0.1f), mCamera.transform.position.z);
        }
    }
}
