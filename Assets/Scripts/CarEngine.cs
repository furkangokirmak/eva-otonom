using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarEngine : MonoBehaviour {

    public Transform path;
    public float maxSteerAngle = 45f;
    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public float maxMotorTorque = 200f;
    public float currentSpeed;
    public float maxSpeed = 30f;
    public float newSteer;
    public GameObject Car;
    
    public WheelCollider FL;
    public WheelCollider FR;
    public WheelCollider BL;
    public WheelCollider BR;

    private int[] lightsState;

    private List<Transform> nodes;
    private int currentNode = 0;

    float timer = 0f;

    private void Start () {

        Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        for (int i = 0; i < pathTransforms.Length; i++) {
            if (pathTransforms[i] != path.transform) {
                nodes.Add(pathTransforms[i]);
            }
        }
    }
	
	private void FixedUpdate () {
        ApplySteer();
        Drive();
        CheckWaypointDistance();
        SlowDown();
        //Debug.Log(+currentNode);
        Stop();
        CheckLight();
    }

    private void ApplySteer() {
        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
        newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
        wheelFL.steerAngle = newSteer;
        wheelFR.steerAngle = newSteer;
    }

    private void Drive() {
        currentSpeed = 2 * Mathf.PI * wheelFL.radius * wheelFL.rpm * 60 / 1000;

        if (currentSpeed < maxSpeed) {
            wheelFL.motorTorque = maxMotorTorque;
            wheelFR.motorTorque = maxMotorTorque;
            BL.brakeTorque = 0;
            BR.brakeTorque = 0;
            FL.brakeTorque = 0;
            FR.brakeTorque = 0;
        } else {
            BL.brakeTorque = 300;
            BR.brakeTorque = 300;
            FL.brakeTorque = 300;
            FR.brakeTorque = 300;
            wheelFL.motorTorque = 0;
            wheelFR.motorTorque = 0;
        }
    }

    private void CheckWaypointDistance() {
        if(Vector3.Distance(transform.position, nodes[currentNode].position) < 0.8f) {
            if(currentNode == nodes.Count - 1) {
                currentNode = 0;
            } else {
                currentNode++;
            }
        }
    }

    private void SlowDown()
    {
        if (Vector3.Distance(Car.transform.position, nodes[currentNode].position) < 5f || Vector3.Distance(Car.transform.position, nodes[currentNode-1].position) < 2.5f)
        {
            if (currentNode != 3 || currentNode != 14 || currentNode != 25)
            {
                maxMotorTorque = 0;
            }
        }
        else
            maxMotorTorque = 200;

        if (currentNode == 4)
            maxSpeed = 20;
        if (currentNode == 17)
            maxSpeed = 30;
    }   

    private void Stop()
    {
        if (currentNode == 3 || currentNode == 14)
        {
            timer += Time.deltaTime;
            if (timer < 30)
            {
                BL.brakeTorque = 5000;
                BR.brakeTorque = 5000;
                FL.brakeTorque = 5000;
                FR.brakeTorque = 5000;
                //Debug.Log(timer);
            }
            else
            {
                BL.brakeTorque = 0;
                BR.brakeTorque = 0;
                FL.brakeTorque = 0;
                FR.brakeTorque = 0;
                maxMotorTorque = 200;
            }

        }
        if (currentNode == 4 || currentNode == 15)
            timer = 0;

        if (currentNode == 25)
        {
            BL.brakeTorque = 5000;
            BR.brakeTorque = 5000;
            FL.brakeTorque = 5000;
            FR.brakeTorque = 5000;
        }

    }

    public void CheckLight()
    {
        lightsState = GameObject.Find("IntersectionLight").GetComponent<TrafficLightController>().getState();
        // 1 19
        if (currentNode == 1)
        {
            if(lightsState[0]==0)
            {
                BL.brakeTorque = 5000;
                BR.brakeTorque = 5000;
                FL.brakeTorque = 5000;
                FR.brakeTorque = 5000;
            }
            else
            {
                BL.brakeTorque = 0;
                BR.brakeTorque = 0;
                FL.brakeTorque = 0;
                FR.brakeTorque = 0;
                maxMotorTorque = 200;
            }
        }
        if (currentNode == 19)
        {
            if (lightsState[1] == 0)
            {
                BL.brakeTorque = 5000;
                BR.brakeTorque = 5000;
                FL.brakeTorque = 5000;
                FR.brakeTorque = 5000;
            }
            else
            {
                BL.brakeTorque = 0;
                BR.brakeTorque = 0;
                FL.brakeTorque = 0;
                FR.brakeTorque = 0;
                maxMotorTorque = 200;
            }
        }
    }
}
