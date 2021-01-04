using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasaController : MonoBehaviour
{
    [SerializeField] public Transform spawnPlace;
    [SerializeField] public GameObject CharacterPrefab;
    private GameObject propietario;
    
    void Start()
    {
        propietario = Instantiate(CharacterPrefab, spawnPlace.position, spawnPlace.rotation);
        if (propietario.GetComponent<fsmLeñador>() != null) {
            propietario.GetComponent<fsmLeñador>().hogar = this;
        }
        else if(propietario.GetComponent<fsmCazador>() != null)
        {
            propietario.GetComponent<fsmCazador>().hogar = this;
        }
        else
        {
            propietario.GetComponent<fsmConstructor>().hogar = this;
        }
    }
    
}
