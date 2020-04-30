using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayController : MonoBehaviour
{
    GameObject restartText;
    void Start()
    {
        restartText = transform.GetChild(0).gameObject;

        TurnOffAll();
    }

    public void TurnOffAll()
    {
        restartText.SetActive(false);
    }

    public void TurnOnRestartText()
    {
        restartText.SetActive(true);
    }
    public void TurnOffRestartText()
    {
        restartText.SetActive(false);
    }
}
