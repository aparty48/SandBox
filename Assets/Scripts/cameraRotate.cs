using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//работа с ui

public class cameraRotate : MonoBehaviour
{
    // Start is called before the first frame update

    private float sprotcam ;
    public bool PcD;
    public Camera manCam;
    public UIScript uis;
    void Start()
    {
      #if UNITY_STANDALONE || UNITY_EDITOR
          PcD=true;
      #endif
    }

    // Update is called once per frame
    void Update()
    {
        if(uis.nonemenu==true)
        {
          

        }
        //расщет скорости поворотакамери с учетом чуствительности
    }

    public void Swipe()
    {
        if (Input.touchCount > 0)
        {
            // GET TOUCH 0
            Touch touch0;
            if (true)
            {
                touch0 = Input.GetTouch(1);

                if (touch0.phase == TouchPhase.Moved)
                {
                    uis.player.transform.Rotate(0f, touch0.deltaPosition.x * sprotcam * Time.fixedDeltaTime, 0f);
                    manCam.transform.Rotate(-touch0.deltaPosition.y * sprotcam * Time.fixedDeltaTime, 0f, 0f);
                }
				// APPLY ROTATION
			}
			else
			{
                touch0 = Input.GetTouch(0);

                if (touch0.phase == TouchPhase.Moved)
                {
                    uis.player.transform.Rotate(0f, touch0.deltaPosition.x * sprotcam * Time.fixedDeltaTime, 0f);
                    manCam.transform.Rotate(-touch0.deltaPosition.y * sprotcam * Time.fixedDeltaTime, 0f, 0f);
                }
                // APPLY ROTATION
            }
        }
    }
    void OnMouseOver()
    {

    }


}
