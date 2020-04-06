using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lamba : MonoBehaviour
{

    public GameObject kirmizi, sari, yesil;
    public float kirmizizaman, sarizaman, yesilzaman, zaman,uzaklik;
	public Camera cam;
	public CarController car;
	public bool durum;
    public Renderer[] light1;


    void Start()
    {
        light1 = GameObject.Find("Trafik1").GetComponentsInChildren<Renderer>();	
		car = GameObject.Find("Car").GetComponent<CarController>();
        cam = GameObject.Find("DashCam").GetComponent<Camera>();
		durum = false;
    }


    void Update()
    {
		
		
        if (zaman < yesilzaman)
        {
            zaman += Time.deltaTime;
        }
        else
        {
            zaman = 0;
        }
        if (zaman < kirmizizaman)
        {
            kirmizi.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
            sari.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
            yesil.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
			durum = false;
        }
        if (zaman > kirmizizaman && zaman < sarizaman)
        {
            kirmizi.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
            sari.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
            yesil.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
        }
        if (zaman > sarizaman)
        {
            kirmizi.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
            sari.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
            yesil.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
			durum = true;
        }

        if (light1[0].isVisible)
        {
            RaycastHit raycast;
            Vector3 direction = cam.transform.position - light1[0].transform.position;


            if (Physics.Raycast(light1[0].transform.position, direction, out raycast))
            {
                if (raycast.collider.name == "Car") //&& raycast.distance < 20.0f && Vector3.Angle(car.transform.position, direction) > (155.0f))
                {
                    Debug.DrawRay(light1[0].transform.position, direction, Color.yellow);
                   // Debug.Log("L1 ANGLE: " + Vector3.Angle(car.transform.position, direction));
                    car.lightStatus = durum;
                    car.lightDistance = raycast.distance;
                }
            }
        }

    }
}