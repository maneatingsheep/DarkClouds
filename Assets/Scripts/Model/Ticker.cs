using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ticker : MonoBehaviour
{
    public Action<float> Ontick;

    // Update is called once per frame
    void Update()
    {
        Ontick(Time.deltaTime);
    }
}
