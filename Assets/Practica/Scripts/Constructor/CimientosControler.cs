using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CimientosControler : MonoBehaviour
{
    [SerializeField] public Cimiento[] cimientos;
    
    public void OcuparCimiento(int index)
    {
        cimientos[index].ocupado = true;
    }
}
