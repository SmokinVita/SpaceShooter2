﻿using UnityEngine;

public class Explosion : MonoBehaviour
{
    void Start()
    {
        Destroy(this.gameObject, 2.5f);
    }
}
