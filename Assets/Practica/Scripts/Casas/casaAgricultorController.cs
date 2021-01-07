using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class casaAgricultorController : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject spawnAgricultor;
    public GameObject[] parcelas = new GameObject[4];
    private GameManagerScript gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManagerScript>();
        GameObject agricultor = Instantiate(prefab, spawnAgricultor.transform.position, Quaternion.identity);
        agricultor.GetComponent<fsmAgricultor>().casa = this.gameObject;
        gameManager.contadorOficios[3]++;
    }

    
}
