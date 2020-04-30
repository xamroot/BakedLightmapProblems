﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorLock : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Lock()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void Unlock()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}
