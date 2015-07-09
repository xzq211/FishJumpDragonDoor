//using Command;
using UnityEngine;
using System.Collections;
using System.IO;
using System;
//using Assets.Script.Common;
//using UniExtensions.Resource;

public class MainScript : MonoBehaviour {
    static MainScript msInstance = null;
    public static MainScript Instance { get { return msInstance; } }

    #region WriteDebugLog
    public string FileName {
        get {
            DateTime Dt = DateTime.Now;
            string fileName = string.Format("LogFile-{0}-{1}-{2}.log", Dt.Year, Dt.Month, Dt.Day);
            return fileName;
        }
    }

    public string ResourcesPath {
        get {
            string path = "";
            if (Application.platform == RuntimePlatform.WindowsEditor) {
                path = Application.dataPath + "/StreamingAssets";
            } else if (Application.platform == RuntimePlatform.Android) {
                path = "jar:file://" + Application.dataPath + "!/assets/";
            } else if (Application.platform == RuntimePlatform.IPhonePlayer) {
                path = "file://" + Application.dataPath + "/Raw";
            }

            return path;
        }
    }
    #endregion WriteDebugLog

    void Awake() {
        msInstance = this;
    }

    void Start() {

    }

   

    void OnApplicationFocus() {
      
    }

    void OnApplicationPause(bool Pause) {

    }

   

    void InitIOSSDKObject() {
#if UNITY_IOS

#endif
    }

    void OnDestroy() {
      
    }

    void Update() {
   


#if UNITY_ANDROID
        if (Input.GetKeyDown(KeyCode.Escape)) {

            MessageBoxWnd.Instance.OnClickedOk = go => {
                Application.Quit();
            };

            MessageBoxWnd.Instance.Show("是否退出", MessageBoxWnd.Style.OK_CANCLE);
        }
#endif
    }



    private void OnSecond() {
        if (DateTime.Now.Second == 0) {
            OnMinute();
        }
    }

    private void OnMinute() {
        if (DateTime.Now.Minute == 0) {
            OnHour();
        }
    }
    private void OnHour() {
    }
    

    void OnApplicationQuit() {
        
    }


    void OnLocalize() {
     
    }

    public void Localize() {
       
    }

  

    public void ClosePlatform(string message) {
        Debug.Log("ClosePlatform=" + message);
        Application.Quit();
    }

}