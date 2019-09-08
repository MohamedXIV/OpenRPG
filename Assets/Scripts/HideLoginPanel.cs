using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideLoginPanel : MonoBehaviour
{
    public GameObject loginPanel;

    public void HidePanel()
    {
        loginPanel.SetActive(false);
    }

}
