using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.Window {
    class SystemMsgWnd {
        public static string Prefab = "UiEffect/ui_xitongtishi";

        public static void Show(string msg) {

            MainScript.Instance.StartCoroutine(ShowMsg(msg));


        }

        private static IEnumerator ShowMsg(string msg) {
            var obj = Resources.LoadAsync<GameObject>(Prefab);
            yield return obj;

            var ui = NGUITools.AddChild(UICamera.mainCamera.gameObject, obj.asset as GameObject);
            ui.transform.localPosition = new Vector3(0,0,-700);

            ui.GetComponentInChildren<UILabel>().text = msg;


            var maxLen = 0f;
            foreach (var t in ui.GetComponentsInChildren<UITweener>()) {
                if (t.delay + t.duration > maxLen) {
                    maxLen = t.delay + t.duration;
                }
            }

            yield return new WaitForSeconds(maxLen);

            ui.SetActive(false);
            GameObject.Destroy(ui);
        }
    }
}
