using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pad : MonoBehaviour
{
    public UIScript uisc;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void prog(int comm)
	{
        switch(comm)
		{
            case 0:
                uisc.survivalMode = true;
                uisc.sandboxMode = false;
                break;
            case 1:
                uisc.sandboxMode = true;
                uisc.survivalMode = false;
                break;
		}

	}
}
