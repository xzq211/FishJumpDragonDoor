using UnityEngine;
using System.Collections;

public class TestWnd : Window<TestWnd>
{
    public override string PrefabName
    {
        get
        {
            return "TestWnd";
        }
        
    }

    protected override bool OnOpen()
    {

        return base.OnOpen();
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
