using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasaController : MonoBehaviour
{
    [SerializeField] public Transform spawnPlace;
    [SerializeField] public GameObject CharacterPrefab;
    [SerializeField] public GameManagerScript gameManager;
    private GameObject propietario;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManagerScript>();
        propietario = Instantiate(CharacterPrefab, spawnPlace.position, spawnPlace.rotation);
        if (propietario.GetComponent<fsmLeñador>() != null) {
            propietario.GetComponent<fsmLeñador>().hogar = this;
            gameManager.contadorOficios[2]++;
        }
        else if(propietario.GetComponent<fsmCazador>() != null)
        {
            propietario.GetComponent<fsmCazador>().hogar = this;
            gameManager.contadorOficios[1]++;
        }
        else
        {
            propietario.GetComponent<fsmConstructor>().hogar = this;
        }
    }
    
}
