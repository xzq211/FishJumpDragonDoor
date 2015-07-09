using System;
using System.Linq;
//using Command;
using UnityEngine;
using System.Collections;
using System.ComponentModel;
using Component = UnityEngine.Component;
using System.Collections.Generic;
using System.Reflection;


public enum WindowStyle {
    WS_Normal,
    WS_Ext,
    WS_CullingMask,
};

public class WindowList {
    public static Dictionary<string, object> ActiveWindowList = new Dictionary<string, object>();
    public static Dictionary<string, Type> ActiveWindowTypeList = new Dictionary<string, Type>();
}

public abstract class Window<T> where T : class, new() {
    static T msInstance = null;
    int mCullingMask = 0;
    public static T Instance { get { return msInstance ?? (msInstance = new T()); } }
    public static bool Exist = false;

    public static string[] FullWindows = new[] {
/* 
        "BossLevelWnd", "MapSelectWnd", "PvPWnd", "SkillWnd", "StrengthenWnd", "PetWnd", 
        "TowerLevelWnd","TransformWnd", "EquipStrengthenWnd"
*/
		"StrengthenWnd"
    };

   

    private static bool IsLoaded = false;

    GameObject mCamera = null;
    GameObject mRootUI = null;
    protected static GameObject mWndObject = null;
    GameObject mExtBackground = null;

    public AUiClass[] AUi;

    /// <summary>
    /// 挂在window 窗口上的MonoBehaviour脚本
    /// 用于和窗口生命周期一样的 StartCoroutine方法
    /// </summary>
    public WindowHelper WindowHelper = null;

    public virtual string PrefabName { get; set; }
    public GameObject WndObject { get { return mWndObject; } }
    protected virtual bool OnOpen() { return true; }

    protected virtual void BindUi() { }

    protected virtual IEnumerable OnAsyncOpen() {
        yield break;
    }

    protected virtual bool OnClose() { return true; }
    protected virtual bool OnShow() { return true; }
    protected virtual bool OnHide() { return true; }
    protected virtual void OnStart() { return; }

    public WindowStyle mWindowStyle = WindowStyle.WS_Normal;
    public WindowStyle WinStyle { get { return mWindowStyle; } set { mWindowStyle = value; } }

    public delegate void OnOpenCallBack();

    public GameObject FindGameObject(string path, params object[] args) {
        Transform transform = WndObject.transform.Find(string.Format(path, args));
        return transform == null ? null : transform.gameObject;
    }

    public Transform FindTransform(string path, params object[] args) {
        return WndObject.transform.Find(string.Format(path, args));
    }

    public UISprite FindUiSprite(string path, params object[] args) {
        Transform transform = WndObject.transform.Find(string.Format(path, args));

        return transform == null ? null : transform.GetComponent<UISprite>();
    }

    public UIToggle FindUiToggle(string path, params object[] args) {
        Transform transform = WndObject.transform.Find(string.Format(path, args));

        return transform == null ? null : transform.GetComponent<UIToggle>();
    }

    public UILabel FindUiLabel(string path, params object[] args) {
        Transform transform = WndObject.transform.Find(string.Format(path, args));

        return transform == null ? null : transform.GetComponent<UILabel>();
    }

    public UIButton FindUiButton(string path, params object[] args) {
        Transform transform = WndObject.transform.Find(string.Format(path, args));

        return transform == null ? null : transform.GetComponent<UIButton>();
    }

    public C FindComponent<C>(string path, params object[] args) where C : Component {
        Transform transform = WndObject.transform.Find(string.Format(path, args));

        return transform == null ? null : transform.GetComponent<C>();
    }

    public UISlider FindSlider(string path, params object[] args) {
        Transform transform = WndObject.transform.Find(string.Format(path, args));

        return transform == null ? null : transform.GetComponent<UISlider>();
    }

    public UITable FindTable(string path, params object[] args) {
        Transform transform = WndObject.transform.Find(string.Format(path, args));

        return transform == null ? null : transform.GetComponent<UITable>();
    }

    public UIGrid FindGrid(string path, params object[] args) {
        Transform transform = WndObject.transform.Find(string.Format(path, args));

        return transform == null ? null : transform.GetComponent<UIGrid>();
    }

    public void SetUiEventListenerClick(string path, UIEventListener.VoidDelegate callback, params object[] args) {
        UIEventListener.Get(WndObject.transform.Find(string.Format(path, args)).gameObject).onClick = callback;
    }

    public GameObject Control(string name) {
        if (mWndObject == null)
            return null;

        return Control(name, mWndObject);
    }

    public GameObject Control(string name, bool Inactive) {
        if (mWndObject == null)
            return null;

        return Control(name, mWndObject, Inactive);
    }

    public GameObject Control(string name, GameObject parent) {
        if (mWndObject == null)
            return null;

        return Control(name, parent, false);
    }

    public GameObject Control(string name, GameObject parent, bool Inactive) {
        if (mWndObject == null || parent == null)
            return null;
        Transform[] children = parent.GetComponentsInChildren<Transform>(Inactive);
        foreach (Transform child in children) {
            if (child.name == name)
                return child.gameObject;
        }
        return null;
    }

    private IEnumerator OpenAsyc(OnOpenCallBack callBack) {
        if (mWndObject) {
            GameObject.Destroy(mWndObject);
        }

        if (!WindowList.ActiveWindowList.ContainsKey(PrefabName)) {
            WindowList.ActiveWindowList.Add(PrefabName, this);
            WindowList.ActiveWindowTypeList.Add(PrefabName, GetType());
        }
        

        var req = Resources.LoadAsync<GameObject>(PrefabName);
        yield return req;

        var scale = (req.asset as GameObject).transform.localScale;
		var position = (req.asset as GameObject).transform.localPosition;
        mWndObject = GameObject.Instantiate(req.asset) as GameObject;

        if (mWndObject == null) {
            Debug.LogWarning("OpenAsyc win object is null");
            yield break;
        }

        WindowHelper = mWndObject.AddComponent<WindowHelper>();
        mWndObject.name = PrefabName;
        mWndObject.SetActive(true);
        IsLoaded = true;

        mRootUI = GameObject.Find("WindowsRoot/Camera");

        if (FullWindows.Contains(PrefabName)) {
            mCullingMask = Camera.main.cullingMask;
            Camera.main.cullingMask = 0;

            //var original = Resources.Load("BackgroundExtWnd") as GameObject;
            var original = Resources.Load("BackgroundExtWndTransparent") as GameObject;
            mExtBackground = NGUITools.AddChild(mRootUI, original);
        }

        WndObject.transform.parent = mRootUI.transform;
		WndObject.transform.localPosition = position;
        WndObject.transform.localScale = scale;

        BindUi();

        if (AUi != null) {
            foreach (var aUiClass in AUi) {
                if (aUiClass != null)
                    aUiClass.Bind(WndObject, this);
            }
        }

        OnOpen();

        var e = OnAsyncOpen().GetEnumerator();
        while (e.MoveNext()) {
            yield return e.Current;
        }

        OnStart();

        var peekaboo = WndObject.GetComponent<UiPeeKaBoo>() ?? WndObject.GetComponentInChildren<UiPeeKaBoo>();

        if (peekaboo != null) {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            if (peekaboo.Tweens.Length > 0) {
                foreach (var t in peekaboo.Tweens) {
                    foreach (var tw in t.GetComponents<UITweener>()) {
                        tw.PlayForward();
                    }
                }
            } else {
                foreach (var tw in WndObject.GetComponents<UITweener>()) {
                    tw.PlayForward();
                }
            }
        }

        if (callBack != null) {
            callBack();
        }
    }

    public void Open() {
        Exist = true;
        MainScript.Instance.StartCoroutine(OpenAsyc(null));



    }

    public void Open(OnOpenCallBack callBack) {
        Exist = true;

        MainScript.Instance.StartCoroutine(OpenAsyc(callBack));
    }

    public void CloseOther() {
        foreach (var w in WindowList.ActiveWindowList) {
            if (w.Key == "InGameMainWnd" || w.Key == "Front_LoadingWnd" || w.Key == "CreateRoleWnd") {
                continue;
            }

            var t = WindowList.ActiveWindowTypeList[w.Key];
            if (t != null) {
                var m = t.GetMethod("Close");
                m.Invoke(w.Value,null);
            }
        }
    }


    public void Close() {
        float delay = 0;

        if (WndObject != null) {
            var peekaboo = WndObject.GetComponent<UiPeeKaBoo>() ?? WndObject.GetComponentInChildren<UiPeeKaBoo>();

            if (peekaboo != null) {
                if (peekaboo.Tweens.Length > 0) {
                    foreach (var t in peekaboo.Tweens) {
                        foreach (var tw in t.GetComponents<UITweener>()) {
                            if (tw.delay + tw.duration > delay) {
                                delay = tw.delay + tw.duration;
                            }

                            tw.PlayReverse();
                        }
                    }
                } else {
                    foreach (var tw in WndObject.GetComponents<UITweener>()) {
                        if (tw.delay + tw.duration > delay) {
                            delay = tw.delay + tw.duration;
                        }

                        tw.PlayReverse();
                    }
                }
            }


        }

        MainScript.Instance.StartCoroutine(CloseInternal(delay));
    }

    private IEnumerator CloseInternal(float delay) {
        yield return new WaitForSeconds(delay);

        Exist = false;

        OnClose();
        if (mWndObject != null) {
            mWndObject.SetActive(false);
            GameObject.Destroy(mWndObject);
        }

        if (FullWindows.Contains(PrefabName)) {
            if (Camera.main) {
                Camera.main.cullingMask = mCullingMask;
            }

            GameObject.Destroy(mExtBackground);
        }

        WindowList.ActiveWindowList.Remove(PrefabName);
        WindowList.ActiveWindowTypeList.Remove(PrefabName);
        msInstance = null;
        IsLoaded = false;
    }

    public virtual void Show() {
        if (Exist) {
            WndObject.SetActive(true);
            if (WindowStyle.WS_Ext <= mWindowStyle)
                mExtBackground.SetActive(true);
        }

        OnShow();
    }

    public virtual void Hide() {
        if (Exist && WndObject != null) {
            WndObject.SetActive(false);
            if (WindowStyle.WS_Ext <= mWindowStyle)
                mExtBackground.SetActive(false);
        }

        OnHide();
    }

    //public void Request(ProtoBuf.IExtensible request, Client.OnResponse callback)
    //{
    //    MainScript.Instance.Request(request, delegate(string err, Response response)
    //    {
    //        if (!mWndObject)
    //            return;

    //        if (string.Compare(err, "Time out") == 0)
    //            Global.ShowLoadingEnd();

    //        callback(err, response);
    //    });
    //}

    public abstract class AUiClass {
        public Window<T> WinObj = null;
        public GameObject RootObj = null;

        public void Bind(GameObject root, Window<T> winClass) {
            RootObj = root;
            WinObj = winClass;

            var type = GetType();
            var typeOfWin = winClass.GetType();

            var rootPathAtt = type.GetCustomAttributes(typeof(NGuiPathRootAttrib), true);
            var rootPath = string.Empty;
            if (rootPathAtt.Length > 0) {
                var nGuiPathRootAttrib = rootPathAtt[0] as NGuiPathRootAttrib;
                if (nGuiPathRootAttrib != null)
                    rootPath = nGuiPathRootAttrib.Path;
            }


            var fields = type.GetFields();

            foreach (var f in fields) {

                var atts = f.GetCustomAttributes(typeof(NGuiPathAttrib), true);

                if (atts.Length == 0) {
                    continue;
                }

                var att = atts[0] as NGuiPathAttrib;
                if (att == null) {
                    continue;
                }

                if (string.IsNullOrEmpty(att.Path)) {
                    att.Path = f.Name;
                }

                if (!f.FieldType.IsArray) {
                    var trans = root.transform.Find(rootPath + att.Path);

                    if (trans == null) {
                        Debug.LogWarning("path not find: " + rootPath + att.Path);
                        continue;
                    }

                    if (att.UiElementType == NGuiPathAttrib.ElementType.SingleElement) {
                        if (f.FieldType.IsSubclassOf(typeof(Component))) {
                            f.SetValue(this, trans.GetComponent(f.FieldType));
                        }

                        if (f.FieldType == typeof(GameObject)) {
                            f.SetValue(this, trans.gameObject);
                        }
                    }

                    if (att.UiElementType == NGuiPathAttrib.ElementType.MutiElement) {
                        if (f.FieldType.IsSubclassOf(typeof(Component))) {
                            f.SetValue(this, trans.GetComponents(f.FieldType));
                        }
                    }

                    if (att.UiElementType == NGuiPathAttrib.ElementType.EventElementOnClick ||
                        att.UiElementType == NGuiPathAttrib.ElementType.EventElementOnPress
                        ) {

                        if (string.IsNullOrEmpty(att.EventCallBack)) {
                            continue;
                        }

                        var m = typeOfWin.GetMethod(att.EventCallBack);
                        if (m == null) {
                            Debug.LogWarning("method not found : " + att.EventCallBack);
                            continue;
                        }

                        f.SetValue(this, trans.gameObject);
                        switch (att.UiElementType) {
                            case NGuiPathAttrib.ElementType.EventElementOnClick:
                                UIEventListener.Get(trans.gameObject).onClick = Delegate.CreateDelegate(typeof(UIEventListener.VoidDelegate), winClass, m) as UIEventListener.VoidDelegate;
                                break;
                            case NGuiPathAttrib.ElementType.EventElementOnPress:
                                UIEventListener.Get(trans.gameObject).onPress = Delegate.CreateDelegate(typeof(UIEventListener.BoolDelegate), winClass, m) as UIEventListener.BoolDelegate;
                                break;
                        }
                    }

                    if (att.UiElementType == NGuiPathAttrib.ElementType.EventTabChange) {
                        if (string.IsNullOrEmpty(att.EventCallBack)) {
                            continue;
                        }

                        var m = typeOfWin.GetMethod(att.EventCallBack);
                        if (m == null) {
                            Debug.LogWarning("method not found : " + att.EventCallBack);
                            continue;
                        }

                        if (f.FieldType.IsSubclassOf(typeof(Component))) {
                            f.SetValue(this, trans.GetComponent(f.FieldType));
                        }

                        var t = trans.gameObject.GetComponent<UIToggle>();
                        if (t == null) {
                            Debug.LogWarning("toggle component not found");
                            continue;
                        }

                        t.onChange.Add(new EventDelegate(Delegate.CreateDelegate(typeof(EventDelegate.Callback), winClass, m) as EventDelegate.Callback));
                    }

                } else {

                    var objs = new List<GameObject>();
                    for (int i = att.MutiFormatStart; i <= att.MutiFormatEnd; i++) {
                        var p = string.Format(rootPath + att.Path, i);
                        var trans = root.transform.Find(p);

                        if (trans == null) {
                            Debug.LogWarning("path not find: " + p);
                            continue;
                        }

                        objs.Add(trans.gameObject);
                    }

                    if (att.UiElementType == NGuiPathAttrib.ElementType.SingleElement) {
                        var elementType = f.FieldType.GetElementType();
                        if (elementType != null && elementType.IsSubclassOf(typeof(Component))) {
                            var components = new ArrayList();
                            foreach (var obj in objs) {
                                components.Add(obj.GetComponent(elementType));
                            }

                            f.SetValue(this, components.ToArray(elementType));
                        }

                        if (elementType != null && elementType == typeof(GameObject)) {
                            f.SetValue(this, objs.ToArray());
                        }
                    }

                    if (att.UiElementType == NGuiPathAttrib.ElementType.EventElementOnPress ||
                        att.UiElementType == NGuiPathAttrib.ElementType.EventElementOnClick
                        ) {

                        if (string.IsNullOrEmpty(att.EventCallBack)) {
                            continue;
                        }

                        var m = typeOfWin.GetMethod(att.EventCallBack);
                        if (m == null) {
                            Debug.LogWarning("method not found : " + att.EventCallBack);
                            continue;
                        }

                        var elementType = f.FieldType.GetElementType();
                        if (elementType != null && elementType.IsSubclassOf(typeof(Component))) {
                            var components = new ArrayList();
                            foreach (var obj in objs) {
                                components.Add(obj.GetComponent(elementType));
                            }

                            f.SetValue(this, components.ToArray(elementType));
                        }

                        if (elementType != null && elementType == typeof(GameObject)) {
                            f.SetValue(this, objs.ToArray());
                        }

                        switch (att.UiElementType) {
                            case NGuiPathAttrib.ElementType.EventElementOnClick:
                                foreach (var obj in objs) {
                                    UIEventListener.Get(obj).onClick = Delegate.CreateDelegate(typeof(UIEventListener.VoidDelegate), winClass, m) as UIEventListener.VoidDelegate;
                                }
                                break;
                            case NGuiPathAttrib.ElementType.EventElementOnPress:
                                foreach (var obj in objs) {
                                    UIEventListener.Get(obj).onPress = Delegate.CreateDelegate(typeof(UIEventListener.BoolDelegate), winClass, m) as UIEventListener.BoolDelegate;
                                }
                                break;
                        }
                    }

                    if (att.UiElementType == NGuiPathAttrib.ElementType.EventTabChange) {
                        if (string.IsNullOrEmpty(att.EventCallBack)) {
                            continue;
                        }

                        var m = typeOfWin.GetMethod(att.EventCallBack);
                        if (m == null) {
                            Debug.LogWarning("method not found : " + att.EventCallBack);
                            continue;
                        }

                        var elementType = f.FieldType.GetElementType();
                        if (elementType != null && elementType.IsSubclassOf(typeof(Component))) {
                            var components = new ArrayList();
                            foreach (var obj in objs) {
                                components.Add(obj.GetComponent(elementType));
                            }

                            f.SetValue(this, components.ToArray(elementType));
                        }

                        foreach (var obj in objs) {
                            var t = obj.GetComponent<UIToggle>();
                            if (t == null) {
                                Debug.LogWarning("toggle component not found");
                                continue;
                            }

                            t.onChange.Add(new EventDelegate(Delegate.CreateDelegate(typeof(EventDelegate.Callback), winClass, m) as EventDelegate.Callback));
                        }
                    }
                }






            }
        }
    }





}

/// <summary>
/// ui 根路径描述
/// </summary>
public class NGuiPathRootAttrib : Attribute {
    public NGuiPathRootAttrib(string path) {
        Path = path;
    }

    public NGuiPathRootAttrib() {

    }

    public string Path { get; set; }
}

/// <summary>
/// ui 元素描述
/// </summary>
public class NGuiPathAttrib : System.Attribute {
    public NGuiPathAttrib() {

    }

    public NGuiPathAttrib(string path) {
        Path = path;
    }

    public NGuiPathAttrib(string path, ElementType uiElementType, string eventCallBack) {
        Path = path;
        UiElementType = uiElementType;
        EventCallBack = eventCallBack;
    }

    public NGuiPathAttrib(string path, ElementType uiElementType, string eventCallBack, int mutiFormatStart, int mutiFormatEnd) {
        Path = path;
        UiElementType = uiElementType;
        EventCallBack = eventCallBack;
        MutiFormatStart = mutiFormatStart;
        MutiFormatEnd = mutiFormatEnd;
    }

    /// <summary>
    /// 页面元素类型
    /// </summary>
    public enum ElementType {
        /// <summary>
        /// GameObject 下只绑定一个组件
        /// </summary>
        SingleElement = 0,
        /// <summary>
        /// GameObject 下绑定多个组件
        /// </summary>
        MutiElement = 1,
        /// <summary>
        /// Click事件元素
        /// </summary>
        EventElementOnClick = 2,
        /// <summary>
        /// 按下事件元素
        /// </summary>
        EventElementOnPress = 4,

        EventTabChange = 8,

    }

    /// <summary>
    /// 页面元素类型
    /// </summary>
    /// <value>
    /// The type of the UI element.
    /// </value>
    public ElementType UiElementType { get; set; }

    /// <summary>
    /// 元素路径
    /// 
    /// 可以使用 format {0}
    /// </summary>
    /// <value>
    /// The path.
    /// </value>
    public string Path { get; set; }

    /// <summary>
    /// 事件函数字符串,必须是public 描述类型
    /// </summary>
    /// <value>
    /// The event call back.
    /// </value>
    public string EventCallBack { get; set; }

    /// <summary>
    /// 路径开始id
    /// </summary>
    /// <value>
    /// The muti format start.
    /// </value>
    public int MutiFormatStart { get; set; }

    /// <summary>
    /// 路径结束id
    /// </summary>
    /// <value>
    /// The muti format end.
    /// </value>
    public int MutiFormatEnd { get; set; }


}