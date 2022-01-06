using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    private bool SelectmodeButon;
    [SerializeField]
    private GameObject panelSelectMode;
    private int Mode;
    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {

    }

    public void SelectMode()
    {
        SelectmodeButon = !SelectmodeButon;
        panelSelectMode.SetActive(SelectmodeButon);
    }
    public void SelectModeInt(int a)
    {
        Mode = a;
    }
}
