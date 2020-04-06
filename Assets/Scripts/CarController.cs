using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public enum states { moving = 1, turn, getPassenger, decide, parking, stoped };
    public enum signs {none, forward, turnRight, turnLeft, max20, min30, d, p, np };

    // Car dynamics variables

    public WheelCollider w_frontDriver;
    public WheelCollider w_frontPassenger;
    public WheelCollider w_backDriver;
    public WheelCollider w_backPassenger;

    public Transform t_frontDriver;
    public Transform t_frontPassenger;
    public Transform t_backDriver;
    public Transform t_backPassenger;

    // Drive variables
    private float horizontalInput;
    private float verticalInput;
    private bool handBrake;
    private float steeringAngle;
    public float maxSteeringAngle = 30.0f;
    public float torque = 50.0f;
    private float currentSpeed;
    private bool speedChanged = false;
    public float oldMaxSpeed;
    public float maxSpeed;

    // Autonomous variables
    public bool autonomousMode = false;
    private GameObject sensors;
    private RadarSensorController radars;

    public bool crossProcess = false;
    public int carState = (int)states.stoped;

    public bool signProcessed = false;
        // Turn Variables
    public float turnTimer = 0.0f;
    public bool willTurn = false;
    public bool turnComplete = false;
    public int turnDirection = (int)signs.none;
        // Passenger Variables
    public float passengerTimer = 0.0f;
    public bool stopForPassenger = false;
    public bool gotPassenger = false;
    public float distanceToPassenger;

        // Traffic Light variables (true = green, false = red)
    public bool lightStatus = true;
    public float lightDistance;

    // Park Variables
    public bool parking;
    public GameObject parkSpot;
    public float parkDistance;
    
    public void Start()
    {
        oldMaxSpeed = maxSpeed;
        carState = (int)states.moving;
        sensors = GameObject.Find("Sensors");
        radars = GameObject.Find("Radars").GetComponent<RadarSensorController>();
    }

    public void UpdateWheelPose (WheelCollider _wCollider, Transform _wTransform)
    {
        Vector3 _pos = _wTransform.position;
        Quaternion _quat = _wTransform.rotation;

        _wCollider.GetWorldPose(out _pos, out _quat);

        _wTransform.position = _pos;
        _wTransform.rotation = _quat;
    }

    private void FixedUpdate()
    {
        if (autonomousMode != true)
            Drive();
        else
            AutonomousDrive();
    }

    public void Drive()
    {
        // Get the input from the user such as left, right, gas, brake and hand brake.
        // Input.GetAxis returns a float value between 0 and 1.0f, Input.GetKey returns a bool.
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        handBrake = Input.GetKey(KeyCode.Space);
        currentSpeed = 2 * Mathf.PI * w_frontDriver.radius * w_frontDriver.rpm * 60 / 1000;

        // Set the current steering angle with respect to max angle and user input.
        // Then update the WheelColliders' steering angles.
        steeringAngle = maxSteeringAngle * horizontalInput;
        w_frontDriver.steerAngle = steeringAngle;
        w_frontPassenger.steerAngle = steeringAngle;

        // If hand brake is not applied, apply the torque to the WheelColliders, else apply brake torque.
        if (handBrake != true)
        {
            if (verticalInput != 0 && currentSpeed < maxSpeed)
            {

                if (currentSpeed > 0 && verticalInput < 0)
                {
                    CarBrake();
                }
                else
                {
                    w_frontDriver.motorTorque = torque * verticalInput;
                    w_frontPassenger.motorTorque = torque * verticalInput;
                    w_frontDriver.brakeTorque = 0;
                    w_frontPassenger.brakeTorque = 0;
                    w_backDriver.brakeTorque = 0;
                    w_backPassenger.brakeTorque = 0;
                }
            }
            else
            {
                CarBrake();
            }
        }
        else
        {

            w_frontDriver.motorTorque = 0;
            w_frontPassenger.motorTorque = 0;
            w_frontDriver.brakeTorque = torque * 1000;
            w_frontPassenger.brakeTorque = torque * 1000;
            w_backDriver.brakeTorque = torque * 1000;
            w_backPassenger.brakeTorque = torque * 1000;
        }

        // Update all the wheel positions according to the WheelColliders' positions.
        UpdateWheelPose(w_frontDriver, t_frontDriver);
        UpdateWheelPose(w_frontPassenger, t_frontPassenger);
        UpdateWheelPose(w_backDriver, t_backDriver);
        UpdateWheelPose(w_backPassenger, t_backPassenger);

        // Calculate the Km/h.
        // WARNING! -THIS APPROACH MIGH BE WRONG AS WE DO NOT KNOW THE WHEEL RADIUS' UNIT!
        // IT IS ASSUMED TO BE IN MILLIMETER(MM) THEN CONVERTED TO CANTIMETER(CM) IN THE CALCULATION!.
        //Debug.Log("Km/h: " + (w_backDriver.rpm * w_backDriver.radius * 100 * 0.001885));
        //Debug.Log("RPM: " + w_backDriver.rpm);
        //Debug.Log("Steering Angle: " + steeringAngle);
    }

    public void AutonomousDrive()
    {
        
        currentSpeed = 2 * Mathf.PI * w_frontDriver.radius * w_frontDriver.rpm * 60 / 1000;

        if(carState == (int)states.stoped)
        {
            if (radars.isHitFront == true && radars.hitFront.distance < 1.0f)
                carState = (int)states.stoped;
            else
                carState = (int)states.moving;
        }
        /* SEQUENCE
         * 1- Check front left & right radars' distances, if >15, stop. There must be a turn.
         *  1.1- Check light if there is one - else canMove = true.
         *  1.2- Check turn sign if there is one - else direction = forward.
         *  1.3- Set turn and perform turn action.
         * 2- if < 15, try to centrize.
         * 3- Check for signs
         *  3.1- If there is any, perform sign action.
         * REPEAT
         **/
        
        if(carState != (int)states.decide)
        {
            checkLight();
            crossRoadCheck();
            checkForPassenger();
            checkParking();
        }

        switch (carState)
        {
            case (int)states.moving:
                
                if (willTurn)
                {
                    if(speedChanged == false)
                    {
                        oldMaxSpeed = maxSpeed;
                        speedChanged = true;
                    }
                    maxSpeed = 20f;
                }
                else
                {
                    maxSpeed = oldMaxSpeed;
                    speedChanged = false;
                }
                    

                steerCalculation();
                if (radars.isHitLeftFront && radars.isHitRightFront)
                {
                    verticalInput = 0.3f;
                    
                    if (verticalInput != 0 && currentSpeed < maxSpeed)
                        if (currentSpeed > 0 && verticalInput < 0)
                            CarBrake();
                        else
                            CarDrive();
                    else
                        CarBrake();
                }
                break;
            case (int)states.turn:

                Debug.Log("Turn! " + (signs)turnDirection);
                if (turnDirection == (int)signs.forward && turnTimer < 4.0f)
                {
                    verticalInput = 0.3f;
                    horizontalInput = 0.0f;
                    turnTimer += Time.deltaTime;
                    CarDrive();
                }
                else if (turnDirection == (int)signs.turnRight && turnTimer < 4.0f)
                {
                    Debug.Log("Turn Right");
                    turnTimer += Time.deltaTime;
                    if (turnTimer < 1.5f || turnTimer > 3.0f)
                    {
                        verticalInput = 0.3f;
                        horizontalInput = 0.0f;
                    }
                    else if (turnTimer >= 1.5f && turnTimer <= 3.0f)
                    {
                        verticalInput = 0.3f;
                        horizontalInput = 1.0f;
                    }
                    CarDrive();
                }
                else if (turnDirection == (int)signs.turnLeft && turnTimer < 4.0f)
                {
                    Debug.Log("Turn Left");
                    turnTimer += Time.deltaTime;
                    if (turnTimer < 1.5f || turnTimer > 3.0f)
                    {
                        verticalInput = 0.3f;
                        horizontalInput = 0.0f;
                    }
                    else if (turnTimer >= 1.5f && turnTimer <= 3.0f)
                    {
                        verticalInput = 0.3f;
                        horizontalInput = -1.0f;
                    }
                    CarDrive();
                }
                else
                {
                    carState = (int)states.moving;
                    turnComplete = true;
                    crossProcess = false;
                    willTurn = false;
                    turnTimer = 0.0f;
                }
                break;
            case (int)states.getPassenger:

                maxSpeed = 10f;
                if (distanceToPassenger < 8.0f)
                {
                    steerCalculationForPassenger();
                    verticalInput = 0.3f;
                }
                    
                if (radars.isHitRightFront && radars.isHitRight && passengerTimer < 35.0f)
                {
                    if(radars.hitRight.distance > 2.5f)
                    {
                        Debug.Log("STOP FOR PASSENGER");
                        if (verticalInput != 0 && currentSpeed < maxSpeed)
                        {
                                if (currentSpeed > 0 && verticalInput < 0)
                            {
                                CarBrake();
                            }
                            else
                            {
                                CarDrive();
                            }
                        }
                        else
                        {
                            CarBrake();
                        }
                    }
                    else
                    {
                        passengerTimer += Time.deltaTime;
                        CarBrake();
                    }
                }
                else
                {
                    gotPassenger = true;
                    stopForPassenger = false;
                    passengerTimer = 0.0f;
                    maxSpeed = 30f;
                    carState = (int)states.moving;
                }
                break;
            case (int)states.decide:

                if (willTurn)
                {
                    turnComplete = false;
                    willTurn = false;
                    carState = (int)states.turn;
                }
                break;
            case (int)states.parking:
                // radars.hitFront.distance
                //Debug.Log("Parking!");

                if(parkDistance > 50)
                {
                    oldMaxSpeed = 17.0f;
                    maxSpeed = 17.0f;
                }
                else if(parkDistance > 12 && parkDistance <= 50)
                {
                    oldMaxSpeed = 8.0f;
                    maxSpeed = 8.0f;
                }
                else if (parkDistance <= 12 && parkDistance > 0)
                {
                    oldMaxSpeed = 5.0f;
                    maxSpeed = 5.0f;
                }

                steerCalculationForParking();
                if (radars.hitFront.distance > 1.5f || !radars.isHitFront)
                {
                    verticalInput = 0.3f;

                    if (verticalInput != 0 && currentSpeed < maxSpeed)
                        if (currentSpeed > 0 && verticalInput < 0)
                            CarBrake();
                        else
                            CarDrive();
                    else
                        CarBrake();
                }
                else
                {
                    CarBrake();
                    carState = (int)states.stoped;
                }

                break;
            case (int)states.stoped:

                CarBrake();
                break;
            default:
                break;
        }

       
    }
    private void checkForPassenger()
    {
        if (passengerTimer < 15 && gotPassenger == true)
        {
            passengerTimer += Time.deltaTime;
        }
        else if(passengerTimer > 15 && gotPassenger == true)
        {
            passengerTimer = 0;
            gotPassenger = false;
            stopForPassenger = false;
        }
            
        if (stopForPassenger && gotPassenger == false)
        {
            carState = (int)states.getPassenger;
        }
    }

    private void crossRoadCheck()
    {
        if (radars.hitRightFront.distance > 10.0f && radars.hitLeftFront.distance > 10.0f && crossProcess == false && stopForPassenger == false)
        {
            // Crossing change state to decide!
            carState = (int)states.decide;
            crossProcess = true;
            Debug.Log("Cross Road Detected!");
            verticalInput = 0.0f;
            horizontalInput = 0.0f;
            CarBrake();
        }
        else if (radars.hitRightFront.distance >= radars.hitLeftFront.distance + 10.0f && crossProcess == false && stopForPassenger == false)
        {
            if (willTurn == false)
            {
                horizontalInput = 1.0f;
                verticalInput = 0.2f;
                carState = (int)states.moving;
                CarDrive();
            }
            else
            {
                carState = (int)states.decide;
                crossProcess = true;
                Debug.Log("Right T Road Detected!");
                verticalInput = 0.0f;
                horizontalInput = 0.0f;
                CarBrake();
            }
        }
        else if (radars.hitLeftFront.distance >= radars.hitRightFront.distance + 10.0f && crossProcess == false && stopForPassenger == false)
        {
            if(willTurn == false)
            {
                horizontalInput = -1.0f;
                verticalInput = 0.2f;
                carState = (int)states.moving;
                CarDrive();
            }
            else
            {
                carState = (int)states.decide;
                crossProcess = true;
                Debug.Log("Left T Road Detected!");
                verticalInput = 0.0f;
                horizontalInput = 0.0f;
                CarBrake();
            }
        }
    }

    private void steerCalculation()
    {
        if (radars.hitLeftFront.distance > (radars.hitRightFront.distance + 0.3f))
        {
            //turn left
            //Debug.Log("LEFT!");
            if (horizontalInput < -1)
                horizontalInput = -1.0f;
            else
                horizontalInput -= 0.1f;
        }
        else if (radars.hitRightFront.distance > (radars.hitLeftFront.distance + 0.3f))
        {
            //turn right
            //Debug.Log("RIGHT!");
            if (horizontalInput > 1)
                horizontalInput = 1.0f;
            else
                horizontalInput += 0.1f;
        }
        else
        {
            //go straight
            horizontalInput = 0;
        }
    }

    private void steerCalculationForPassenger()
    {
        Debug.Log("RF: " + radars.hitRightFront.distance + " R: " + radars.hitRight.distance);
        if (radars.hitRightFront.distance > 3.3f || radars.hitRight.distance < 2.2f)
        {
            //turn right
            if (horizontalInput > 1)
                horizontalInput = 1.0f;
            else
                horizontalInput += 0.1f;
        }
        else if(radars.hitRightFront.distance <= 3.5f || radars.hitRight.distance > 2.5f)
        {
            //turn left
            if (horizontalInput < -1)
                horizontalInput = -1.0f;
            else
                horizontalInput -= 0.1f;
        }
        else
        {
            //straight
            horizontalInput = 0;
        }
    }

    private void steerCalculationForParking()
    {
        if(radars.isHitFront == false)
        {
            horizontalInput = 0;
        }
        else
        {
            if (this.transform.position.x < parkSpot.transform.position.x + 0.1f)
            {
                //turn left
                Debug.Log("Turning left! Car Pos: " + this.transform.position.x + " Park Pos: " + parkSpot.transform.position.x);
                if (horizontalInput < -1)
                    horizontalInput = -1.0f;
                else
                    horizontalInput -= 0.1f;
            }
            else if (this.transform.position.x > parkSpot.transform.position.x - 0.1f)
            {
                //turn right
                Debug.Log("Turning right! Car Pos: " + this.transform.position.x + " Park Pos: " + parkSpot.transform.position.x);
                if (horizontalInput > 1)
                    horizontalInput = 1.0f;
                else
                    horizontalInput += 0.1f;
            }
            else
            {
                //go straight
                horizontalInput = 0;
            }
        }
    }

    private void checkLight()
    {
        if (lightStatus == false && lightDistance < 15.0f)
            carState = (int)states.stoped;
    }

    private void checkParking()
    {
        if (parking == true)
            carState = (int)states.parking;
    }

    public void CarBrake()
    {
        w_frontDriver.motorTorque = 0;
        w_frontPassenger.motorTorque = 0;
        w_frontDriver.brakeTorque = torque;
        w_frontPassenger.brakeTorque = torque;
        w_backDriver.brakeTorque = torque;
        w_backPassenger.brakeTorque = torque;
    }

    public void CarDrive()
    {
        steeringAngle = maxSteeringAngle * horizontalInput;
        w_frontDriver.steerAngle = steeringAngle;
        w_frontPassenger.steerAngle = steeringAngle;
        w_frontDriver.motorTorque = torque * verticalInput;
        w_frontPassenger.motorTorque = torque * verticalInput;
        w_frontDriver.brakeTorque = 0;
        w_frontPassenger.brakeTorque = 0;
        w_backDriver.brakeTorque = 0;
        w_backPassenger.brakeTorque = 0;
    }

    public float getSteeringAngle()
    {
        return steeringAngle;
    }

    public float getSpeed()
    {
        return currentSpeed;
    }

    public void AutonomousOnOff()
    {
        autonomousMode = !autonomousMode;
    }
}       
