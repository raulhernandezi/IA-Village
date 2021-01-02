using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasaConstructor : MonoBehaviour
{
    [SerializeField] public Transform spawnConstructor;
    [SerializeField] public GameObject ConstructorPrefab;
    private GameObject propietario;
    
    void Start()
    {
        propietario = Instantiate(ConstructorPrefab, spawnConstructor.position, spawnConstructor.rotation);
        propietario.GetComponent<fsmConstructor>().hogar = this;
    }
    
}
