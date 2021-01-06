using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampoVisionController : MonoBehaviour
{
    [SerializeField] fsmCazador cazador;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Jabali" && cazador.fsmCazador_FSM.actualState.Name == "Rondar")
        {
            if (collision.gameObject.GetComponent<JabaliController>().siendoCazado)
            {
                return;
            }
            cazador.AnimalEnRango(collision.gameObject);
            collision.gameObject.GetComponent<JabaliController>().siendoCazado = true;
        }
    }
}
