using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArbolController : MonoBehaviour
{
    public bool siendoTalado;
    [SerializeField] public Transform zonaDeTalado;

    void Start()
    {
        siendoTalado = false;
    }
    
}
