using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {
    Rigidbody rig;
	// Use this for initialization
	void Start () {
        rig = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            rig.velocity = rig.velocity + new Vector3(0, 5, 0);
        }
#else
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                rig.velocity = rig.velocity + new Vector3(0, 5, 0);
            }
        }
#endif
        //if (transform.position.y > 6)
        //    Debug.Log("win");
        //if (transform.position.y < -5)
        //    Debug.Log("lost");
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Obstacle")
        {
            GameManager.Instance.Lost();
        }
    }
}
