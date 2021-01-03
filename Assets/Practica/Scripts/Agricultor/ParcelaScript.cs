using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParcelaScript : MonoBehaviour
{
    public bool libre;
    public bool plantada;
    public bool cosechable;
    public bool cosechado;
    [SerializeField] public Material meshLibre;
    [SerializeField] public Material meshPlantado;
    [SerializeField] public Material meshCrecido;
    


    // Start is called before the first frame update
    void Start()
    {
        libre = true;
        plantada = false;
        cosechable = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (plantada)
        {
            StartCoroutine("creciendo");
            plantada = false;
        }

        if (cosechable)
        {
            GetComponent<MeshRenderer>().material = meshCrecido;
        }

        if (cosechado)
        {
            GetComponent<MeshRenderer>().material = meshLibre;
            libre = true;
        }
    }

    IEnumerator creciendo()
    {
        yield return new WaitForSeconds(Random.Range(20f, 40f));

        cosechable = true;
    }

    
}
