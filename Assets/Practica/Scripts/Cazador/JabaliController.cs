using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JabaliController : MonoBehaviour
{
    public bool siendoCazado;

    void Start()
    {
        siendoCazado = false;
    }

    protected void LateUpdate()
    {
        transform.localEulerAngles = new Vector3(-90, transform.localEulerAngles.y, 0);
    }
}
