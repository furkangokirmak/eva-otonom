using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    public Renderer[] light1, light2;
    public Camera cam;
    public GameObject [] trafficLightR1, trafficLightR2;
    public CarController car;
    private int [] state = new int[2];
    private float timer = 0f;

    private void Start()
    {
        light1 = GameObject.Find("TrafficLightUp1").GetComponentsInChildren<Renderer>();
        light2 = GameObject.Find("TrafficLightUp2").GetComponentsInChildren<Renderer>();
        car = GameObject.Find("Car").GetComponent<CarController>();
        cam = GameObject.Find("DashCam").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 0 && timer < 35)
        {
            if(timer < 15)
            {
                turnGreen(trafficLightR1);
                turnRed(trafficLightR2);
                state[0] = 1;
                state[1] = 0;

            }
            else if (timer < 20)
            {
                turnYellow(trafficLightR1);
                turnYellow(trafficLightR2);
            }
            else
            {
                turnRed(trafficLightR1);
                turnGreen(trafficLightR2);
                state[0] = 0;
                state[1] = 1;
            }
                
        }
        else if (timer >= 35)
        {
            timer = 0;
        }


        if (light1[0].isVisible)
        {
            RaycastHit raycast;
            Vector3 direction = cam.transform.position - light1[0].transform.position;


            if (Physics.Raycast(light1[0].transform.position, direction, out raycast))
            {
                if (raycast.collider.name == "Car" && raycast.distance < 20.0f && Vector3.Angle(car.transform.position, direction) > (155.0f))
                {
                    Debug.DrawRay(light1[0].transform.position, direction, Color.yellow);
                    //Debug.Log("L1 ANGLE: " + Vector3.Angle(car.transform.position, direction));
                    car.lightStatus = state[0] == 1 ? true : false;
                    car.lightDistance = raycast.distance;
                }
            }
        }

        if (light2[0].isVisible)
        {
            RaycastHit raycast;
            Vector3 direction = cam.transform.position - light2[0].transform.position;


            if (Physics.Raycast(light2[0].transform.position, direction, out raycast))
            {
                if (raycast.collider.name == "Car" && raycast.distance < 20.0f && Vector3.Angle(car.transform.position, direction) > (155.0f))
                {
                    Debug.DrawRay(light2[0].transform.position, direction, Color.yellow);
                    //Debug.Log("L2 ANGLE: " + Vector3.Angle(car.transform.position, direction));
                    car.lightStatus = state[1] == 1? true : false;
                    car.lightDistance = raycast.distance;
                }
            }
        }
    }

    private void turnRed(GameObject[] lights)
    {
        MeshRenderer [] lightBaseMesh;
        Light [] lightBaseLights;

        for(int i=0; i<lights.Length; i++)
        {
            lightBaseMesh = lights[i].GetComponentsInChildren<MeshRenderer>();
            lightBaseMesh[1].material.mainTextureOffset = new Vector2(1, 0);
            lightBaseLights = lights[i].GetComponentsInChildren<Light>(true);
            foreach(Light l in lightBaseLights)
            {
                if (l.color == Color.red)
                    l.enabled = true;
                else
                    l.enabled = false;
            }
        }
    }
    private void turnYellow(GameObject[] lights)
    {
        MeshRenderer [] lightBaseMesh;
        Light[] lightBaseLights;

        for (int i = 0; i < lights.Length; i++)
        {
            lightBaseMesh = lights[i].GetComponentsInChildren<MeshRenderer>();
            lightBaseMesh[1].material.mainTextureOffset = new Vector2(1.33f, 0);
            lightBaseLights = lights[i].GetComponentsInChildren<Light>(true);
            foreach (Light l in lightBaseLights)
            {
                if (l.color == Color.yellow)
                    l.enabled = true;
                else
                    l.enabled = false;
            }
        }
    }

    private void turnGreen(GameObject[] lights)
    {
        MeshRenderer [] lightBaseMesh;
        Light [] lightBaseLights;

        for (int i = 0; i < lights.Length; i++)
        {
            lightBaseMesh = lights[i].GetComponentsInChildren<MeshRenderer>();
            lightBaseMesh[1].material.mainTextureOffset = new Vector2(1.66f, 0);
            lightBaseLights = lights[i].GetComponentsInChildren<Light>(true);
            foreach (Light l in lightBaseLights)
            {
                if (l.color == Color.green)
                    l.enabled = true;
                else
                    l.enabled = false;
            }
        }
    }

    public int[] getState()
    {
        return state;
    }
}
