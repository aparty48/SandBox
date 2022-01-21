using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour
{
    public Pad pad;
    public Text textConsole;
    public GameObject textGameobject;
    public GameObject contentGM;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public string SendCommand(string command)
	{
        string[] commands = { "sandbox", "survival" };
        int comnumber = 0;
        string parametrs = "";
        if(command != null)
		{
            return "Not input command";
		}
        for(int i = 0;i<commands.Length; i++)
		{
            if(command.IndexOf(commands[i]) == 0)
            {
                comnumber = i;
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
        switch(comnumber)
		{
            case 0:
                pad.prog(0);
                break;
            case 1:
                pad.prog(1);
                break;
		}
    Debug.Log(command);
        return "";
	}
  public void CommandStart(InputField input)
  {
    SendCommand(input.text);
    //Instantiate(textConsole,new Vector3(0, 0, 0),new Quaternion(0, 0, 0, 0),contentGM);
    textConsole.text += input.text + "\n";
    input.text = "";
  }
}
