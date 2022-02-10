﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateReadableOnAwake : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        GetComponentInChildren<ReadableObjectBehaviour>().Read();
    }
}
