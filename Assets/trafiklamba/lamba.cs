using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lamba : MonoBehaviour
{

    public GameObject kirmizi, sari, yesil;
    public float kirmizizaman, sarizaman, yesilzaman, zaman;

    void Start()
    {

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
        }

    }
}