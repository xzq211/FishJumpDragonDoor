using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {
    bool isStart = false;
	// Use this for initialization
	void Start () {
        var t = TableceshiManager.Instance.GetItem(1);
        Debug.Log(t.Id);
        Debug.Log(t.HH);
        Debug.Log(t.Name);
        Time.timeScale = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.A))
        {
            Debug.Log("oooo");
            MessageBoxWnd.Instance.Show("meiemie");
        }
	}
    void OnGUI()
    {
        if (!isStart)
        {
            if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2, 100, 50), "开始游戏"))
            {
                Time.timeScale = 1;
                isStart = true;
            }
        }
    }
}
