using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class World : MonoBehaviour
{
	// Start is called before the first frame update
	public GameObject dl;
	private int Timetick = 32400;
	public int days = 0;
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}
	void FixedUpdate()
	{
		if (Timetick < 72000) Timetick++;
		else { Timetick = 0; days++; }
		dl.transform.rotation = Quaternion.Euler(Timetick / 360, days, 0.0f);
	}

}
