using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class clickZoneScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField]private UIScript uiscript;
    [SerializeField]private Build bld;

    private float rotSpeed;
    private float timer,milisec;
    // Start is called before the first frame update
    void Start()
    {
        rotSpeed = (uiscript.chustvitelnost / 480) * 160;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerDown(PointerEventData ped)
    {
        timer = Time.deltaTime;
        milisec = (timer % 1) * 1000;
    }

    public void OnPointerUp(PointerEventData ped)
    {
        if(milisec<100f)
        {
          milisec = 0f;
          bld.time();
        }
    }

    public void OnDrag(PointerEventData ped)
    {
        uiscript.player.transform.Rotate(0f,ped.delta.x * rotSpeed * Time.fixedDeltaTime,0f);
        uiscript.manCam.transform.Rotate(-ped.delta.y * rotSpeed * Time.fixedDeltaTime,0f,0f);
    }
}
