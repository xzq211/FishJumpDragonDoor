using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    GameObject mplayer;
    GameObject mCamera;
    bool lost = false;
    public static GameManager Instance = null;
	// Use this for initialization
	void Start () {
        Instance = this;
        mplayer = GameObject.FindGameObjectWithTag("Player");
        mCamera = GameObject.FindGameObjectWithTag("MainCamera");
	}
	
	// Update is called once per frame
	void Update () {
        if (mCamera.transform.position.y - mplayer.transform.position.y >= 6.5f && !lost)
        {
            lost = true;
            Lost();
        }
	}
    public void Lost()
    {
        MessageBoxWnd.Instance.OnClickedCancle = new MessageBoxWnd.OnClickCancle(go =>
        {
            Application.Quit();
        });
        MessageBoxWnd.Instance.OnClickedOk = new MessageBoxWnd.OnClickOK(go =>
        {
            Time.timeScale = 1;
            Application.LoadLevel("ads");
        });
        MessageBoxWnd.Instance.Show("失败了，是否重来？", MessageBoxWnd.Style.OK_CANCLE);
        Time.timeScale = 0;
    }
}
