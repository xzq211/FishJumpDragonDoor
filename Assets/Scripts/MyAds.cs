using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;
public class MyAds : MonoBehaviour {
    public GameObject obj;
    string str = "";
	// Use this for initialization
    void Awake() {
        if (Advertisement.isSupported) {
            Advertisement.Initialize("48614");
            str = "Platform Initialize";
        } else {
            str = "Platform not supported";
        }
    }
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
	}
    void OnGUI() {
        if (GUILayout.Button(Advertisement.IsReady(null) ? "Show Ad" : "Waiting...",GUILayout.Width(100),GUILayout.Height(100)))
        {
            // Show with default zone, pause engine and print result to debug log
            Advertisement.Show(null, new ShowOptions
            {
                resultCallback = result => {
                    str = result.ToString();
                }
            });
        }
        GUILayout.Label(str,GUILayout.Width(100),GUILayout.Height(100));
        if(GUILayout.Button("OpenTestScene",GUILayout.Width(100),GUILayout.Height(100)))
        {
            TestWnd.Instance.Open();
        }
    }
}
