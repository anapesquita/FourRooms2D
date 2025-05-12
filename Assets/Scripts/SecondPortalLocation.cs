using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondPortalLocation : MonoBehaviour
{
    private Vector3 secondPortalPosition = new Vector3(-3f, -3f, 0f);

    void Start()
    {
        transform.position = secondPortalPosition;
    }
}