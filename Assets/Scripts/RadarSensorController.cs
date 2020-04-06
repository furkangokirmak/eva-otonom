using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarSensorController : MonoBehaviour
{
    public GameObject frontRadar, backRadar, frontLeftRadar, frontRightRadar, leftRadar, rightRadar;
    public RaycastHit hitFront, hitBack, hitLeftFront, hitRightFront, hitLeft, hitRight;
    public bool isHitFront, isHitBack, isHitLeftFront, isHitRightFront, isHitLeft, isHitRight;
    public float maxRange = 30f;

    void Update()
    {
        if (Physics.Raycast(leftRadar.transform.position, leftRadar.transform.TransformDirection(Vector3.up), out hitLeft, maxRange))
        {
            Debug.DrawRay(leftRadar.transform.position, leftRadar.transform.TransformDirection(Vector3.up) * hitLeft.distance, Color.yellow);
            //Debug.Log("Hit LEFT: " + hitLeft.distance);
            isHitLeft = true;
        }
        else
            isHitLeft = false;

        if (Physics.Raycast(rightRadar.transform.position, rightRadar.transform.TransformDirection(Vector3.up), out hitRight, maxRange))
        {
            Debug.DrawRay(rightRadar.transform.position, rightRadar.transform.TransformDirection(Vector3.up) * hitRight.distance, Color.blue);
            //Debug.Log("Hit Right: " + hitRight.distance);
            isHitRight = true;
        }
        else
            isHitRight = false;

        if (Physics.Raycast(frontRadar.transform.position, frontRadar.transform.TransformDirection(Vector3.up), out hitFront, maxRange))
        {
            Debug.DrawRay(frontRadar.transform.position, frontRadar.transform.TransformDirection(Vector3.up) * hitRight.distance, Color.red);
            //Debug.Log("Hit FRONT: " + hitFront.distance);
            isHitFront = true;
        }
        else
            isHitFront = false;

        if (Physics.Raycast(backRadar.transform.position, backRadar.transform.TransformDirection(Vector3.up), out hitBack, maxRange))
        {
            Debug.DrawRay(backRadar.transform.position, backRadar.transform.TransformDirection(Vector3.up) * hitBack.distance, Color.green);
            //Debug.Log("Hit BACK: " + hitBack.distance);
            isHitBack = true;
        }
        else
            isHitBack = false;

        if (Physics.Raycast(frontLeftRadar.transform.position, frontLeftRadar.transform.TransformDirection(Vector3.up), out hitLeftFront, maxRange))
        {
            Debug.DrawRay(frontLeftRadar.transform.position, frontLeftRadar.transform.TransformDirection(Vector3.up) * hitLeftFront.distance, Color.magenta);
            //Debug.Log("Hit FRONT-LEFT: " + hitLeftFront.distance);
            isHitLeftFront = true;
        }
        else
            isHitLeftFront = false;

        if (Physics.Raycast(frontRightRadar.transform.position, frontRightRadar.transform.TransformDirection(Vector3.up), out hitRightFront, maxRange))
        {
            Debug.DrawRay(frontRightRadar.transform.position, frontRightRadar.transform.TransformDirection(Vector3.up) * hitRightFront.distance, Color.black);
            //Debug.Log("Hit FRONT-RIGHT: " + hitRightFront.distance);
            isHitRightFront = true;
        }
        else
            isHitRightFront = false;
    }
}
