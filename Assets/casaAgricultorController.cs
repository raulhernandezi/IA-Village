using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class casaAgricultorController : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject spawnAgricultor;
    public GameObject[] parcelas = new GameObject[4];
    // Start is called before the first frame update
    void Start()
    {
        GameObject agricultor = Instantiate(prefab, spawnAgricultor.transform.position, Quaternion.identity);
        agricultor.GetComponent<US>().casa = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
