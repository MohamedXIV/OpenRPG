using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideLoginBG : MonoBehaviour
{
    public GameObject signInBG;

    public void HideBG()
    {
        signInBG.SetActive(false);
    }
}
