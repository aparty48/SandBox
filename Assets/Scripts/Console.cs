using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Console : MonoBehaviour
{
    public Pad pad;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string Send(string command)
	{
        string[] comm = { "sandbox", "survival" };
        int comn = 0;
        string parametrs = "";
        if(command != null)
		{
            return "Not input command";
		}
        for(int i = 0;i<comm.Length; i++)
		{
            if(command.IndexOf(comm[i]) == 0)
            {
                comn = i;
				for(int j = 0; j<command.Length; j++)
                {
                    if(command.Substring(j,j+1)==" ")
					{
                        parametrs = command.Remove(0, j);
                        break;
					}
                }
            }
		}
        switch(comn)
		{
            case 0:
                pad.prog(0);
                break;
            case 1:
                pad.prog(1);
                break;
		}
        return "";
	}
}
