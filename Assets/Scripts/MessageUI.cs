using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//работа с ui

public class MessageUI : MonoBehaviour
{

    [SerializeField]
    private GameObject getPattern;
    [SerializeField] private Animator animr;
    private static Animator animator1;
    private static GameObject patternMessage;
    private static MessageUI instatnce;
    // Start is called before the first frame update
    private void Start()
    {
        instatnce = this;
        patternMessage = getPattern;
        animator1 = animr;
    }
    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.K))
        {
            View("Hello World!",0);
        }
    }

    public static void View(string message,int type)
    {
        switch(type)
        {
            case 0:
            patternMessage.GetComponentInChildren<Text>().text = message;
            instatnce.StartCoroutine(MassagePlay(4));
            break;
            case 1:
            patternMessage.GetComponentInChildren<Text>().text = message;
            break;
            case 2:
            patternMessage.GetComponentInChildren<Text>().text = message;
            break;
        }
    }
    private static IEnumerator MassagePlay(int timedelay)
    {

        animator1.Play("Base Layer.on",0,1f);
        yield return new WaitForSeconds(timedelay);


    }
}
