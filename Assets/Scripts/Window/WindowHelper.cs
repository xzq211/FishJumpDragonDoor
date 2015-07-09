using UnityEngine;
using System.Collections;

public class WindowHelper : MonoBehaviour {

    public delegate void OnEvent();

    public event OnEvent OnWindowUpdate;
    public event OnEvent OnWindowDestory;
    public event OnEvent OnWindowLateUpdate;
    public event OnEvent OnWindowDisable;
    public event OnEvent OnWindowEnable;

    public void LateUpdate() {
        if (OnWindowUpdate != null) {
            OnWindowUpdate();
        }
    }

    public void Update() {
        if (OnWindowLateUpdate != null) {
            OnWindowLateUpdate();
        }
    }

    public void OnDestroy() {
        if (OnWindowDestory != null) {
            OnWindowDestory();
        }
    }

    public void OnDisable() {
        if (OnWindowDisable != null) {
            OnWindowDisable();
        }
    }

    public void OnEnable() {
        if (OnWindowEnable != null) {
            OnWindowEnable();
        }
    }

    
}
