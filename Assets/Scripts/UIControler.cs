using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControler : MonoBehaviour
{
    private CarController car;
    private Text steeringAngleField;
    private Text speedField;
    private float steeringAngle;
    private float speed;

    private void Start()
    {
        steeringAngleField = GameObject.Find("steeringAngleField").GetComponent<Text>();
        speedField = GameObject.Find("speedField").GetComponent<Text>();
        car = GameObject.FindGameObjectWithTag("Player").GetComponent<CarController>();
    }
    void Update()
    {
        speed = car.getSpeed();
        steeringAngle = car.getSteeringAngle();
        steeringAngleField.text = steeringAngle.ToString();
        speedField.text = speed.ToString();
    }
}
