using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SignController : MonoBehaviour
{
    public Camera cam;
    public Renderer[] renderer;
    public CarController car;
    public string type;

    public void Start()
    {
        car = GameObject.Find("Car").GetComponent<CarController>();
        cam = GameObject.Find("DashCam").GetComponent<Camera>();
        renderer = this.gameObject.GetComponentsInChildren<Renderer>();
    }

    public void Update()
    {
        if(renderer[1].isVisible)
        {
            RaycastHit raycast;
            Vector3 direction = cam.transform.position - this.transform.position;
            
            if (Physics.Raycast(transform.position, direction, out raycast))
            {
                if (raycast.collider.name == "Car" && raycast.distance < 5.5f  && Vector3.Angle(car.transform.position, direction) > (50.0f))
                {
                    Debug.DrawRay(transform.position, direction, Color.red);
                    Debug.Log("SIGN IS VISIBLE! Type: " + type + " DISTANCE: " + raycast.distance);

                    // Speed signs
                    if (type == "min30")
                    {
                        car.maxSpeed = 30;
                        car.oldMaxSpeed = 30;
                    }
                    if (type == "max20")
                    {
                        car.maxSpeed = 20;
                        car.oldMaxSpeed = 20;
                    }

                    // Turns
                    if(car.willTurn == false)
                    {
                        if(type == "turnRight")
                        {
                            car.willTurn = true;
                            car.turnDirection = (int)CarController.signs.turnRight;
                        }
                        if (type == "turnLeft")
                        {
                            car.willTurn = true;
                            car.turnDirection = (int)CarController.signs.turnLeft;
                        }
                        if (type == "forward")
                        {
                            car.willTurn = true;
                            car.turnDirection = (int)CarController.signs.forward;
                        }
                    }

                    // Passenger
                    car.distanceToPassenger = raycast.distance;
                    if (car.stopForPassenger == false)
                        if (type == "d")
                        {
                            car.stopForPassenger = true;
                        }

                    // Parking
                    if(car.parking == false)
                    {
                        if (type == "finish")
                            car.parking = true;
                    }
                    
                }
                else if(car.parking == true && raycast.collider.name == "Car" && raycast.distance < 60.0f)
                {
                    if (type == "p")
                    {
                        Debug.DrawRay(transform.position, direction, Color.green);
                        car.parkSpot = this.gameObject;
                        car.parkDistance = raycast.distance;
                    }
                }
            }
        }
    }
}
