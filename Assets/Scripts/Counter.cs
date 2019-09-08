using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    // Start is called before the first frame update
    public Text textCounter;
    public int incrementBy;

    public void IncrementOnClick()
    {
        textCounter.text = (int.Parse(textCounter.text) + incrementBy).ToString();
    }
}
