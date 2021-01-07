using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorralController : MonoBehaviour
{
    public List<GameObject> vacas;
    [SerializeField] private GameObject vacaPrefab;
    [SerializeField] private GameObject granjeroPrefab;
    [SerializeField] public Transform lugarOrdeñoVaca;
    [SerializeField] public Transform lugarOrdeñoGranjero;
    [SerializeField] public Transform lugarEsperaGranjero;
    [SerializeField] public Transform lugarComer;
    [SerializeField] public Mesh comederoLleno;
    [SerializeField] public Mesh comederoVacio;
    [SerializeField] public GameObject comederoGO;
    public GameObject propietario;
    public int pasto;
    private GameManagerScript gameManager;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManagerScript>();
        GameObject vacaPrimera = Instantiate(vacaPrefab, transform.position + new Vector3(0,2f,0), Quaternion.identity);
        vacaPrimera.GetComponent<fsmVaca>().corral = this;
        AñadirVaca(vacaPrimera);
        propietario = Instantiate(granjeroPrefab, transform.position + new Vector3(5, 0.5f, 0), Quaternion.identity);
        propietario.GetComponent<fsmGranjero>().corralSuyo = this;
        pasto = 4;
        gameManager.contadorOficios[0]++;
    }
    

    public void AddPasto(int cantidad)
    {
        pasto += cantidad;
        comederoGO.GetComponent<MeshFilter>().sharedMesh = comederoLleno;
    }

    public void ComerPasto()
    {
        pasto--;
        if(pasto <= 0)
        {
            comederoGO.GetComponent<MeshFilter>().sharedMesh = comederoVacio;
        }
    }

    public void AñadirVaca(GameObject vaca)
    {
        vacas.Add(vaca);
    }

    public void CrearVaca()
    {
        GameObject vaca = Instantiate(vacaPrefab, transform.position + new Vector3(0, 2f, 0), Quaternion.identity);
        vaca.GetComponent<fsmVaca>().corral = this;
        AñadirVaca(vaca);
    }

    public Vector3 getRandomPointInside()
    {
        Vector3 punto = Random.insideUnitSphere;
        return punto;
    }
}
