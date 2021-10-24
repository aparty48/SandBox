using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAmeraNI : MonoBehaviour
{
    public GameObject playr;
    public Camera cam;
    public float rots;
    public bool enabl;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(enabl)
		{
            cam.gameObject.SetActive(true);
            transform.Rotate(0.0f, rots, 0.0f);
            transform.RotateAround(playr.transform.position, Vector3.up,rots *Time.deltaTime);

		}
        else
		{
            cam.gameObject.SetActive(false);
		}
    }
}
