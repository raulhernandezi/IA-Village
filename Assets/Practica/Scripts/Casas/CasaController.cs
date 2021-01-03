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
        propietario.GetComponent<fsmConstructor>().hogar = this;
    }
    
}
