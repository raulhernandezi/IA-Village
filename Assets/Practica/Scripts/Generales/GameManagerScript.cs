using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManagerScript : MonoBehaviour
{

    //RECURSOS
    public int leche;
    public int comida;
    public int madera;
    public int pasto;
    public bool hay_leche;
    public bool hay_comida;

    [SerializeField] private TextMeshProUGUI txt_Comida;
    [SerializeField] private TextMeshProUGUI txt_Leche;
    [SerializeField] private TextMeshProUGUI txt_Madera;
    [SerializeField] private TextMeshProUGUI txt_Pasto;
    [SerializeField] private GameObject arbolPrefab;
    [SerializeField] public GameObject arbolPadre;
    [SerializeField] public GameObject jabaliPrefab;

    [SerializeField] public Transform almacenDropPlace;
    [SerializeField] public Transform herreriaPlace;
    [SerializeField] public Transform restaurantePlace;
    [SerializeField] public Transform[] ZonasSpawnArboles;
    [SerializeField] public Vector3[] ZonasSpawnJabalis;

    public int[] contadorOficios = new int[4]; //0 Agricultor, 1 Cazador, 2 Leñador, 3 Granjero

    // Start is called before the first frame update
    void Start()
    {
        leche = 5;
        comida = 5;
        madera = 5;
        pasto = 5;
        hay_leche = false;
        hay_comida = false;
        txt_Comida.text = "Comida: " + comida;
        txt_Leche.text = "Leche: " + leche;
        txt_Madera.text = "Madera: " + madera;
        txt_Pasto.text = "Pasto: " + pasto;
        for(int i = 0; i < 20; i++)
        {
            SpawnearArbol();
            SpawnearJabali();
        }
        for(int i = 0; i < contadorOficios.Length; i++)
        {
            contadorOficios[i] = 1;
        }
        StartCoroutine(JabaliGeneratorAuto());
        //StartCoroutine(ArbolGeneratorAuto());
    }

    // Update is called once per frame
    void Update()
    {
        if(leche > 0)
        {
            hay_leche = true;
        }
        else
        {
            hay_leche = false;
        }

        if(comida > 0)
        {
            hay_comida = true;
        }
        else{
            hay_comida = false;
        }

        txt_Comida.text = "Comida: " + comida;
        txt_Leche.text = "Leche: " + leche;
        txt_Madera.text = "Madera: " + madera;
        txt_Pasto.text = "Pasto: " + pasto;
    }

    public void SpawnearArbol()
    {
        int zonaRandom = Random.Range(0, 3);
        Transform zona = ZonasSpawnArboles[zonaRandom];
        float posX = 0, posZ = 0;
        if(zonaRandom < 2)
        {
            posX = zona.position.x + Random.Range(-100, 100);
            posZ = zona.position.z + Random.Range(-200, 200);
        }
        else
        {
            posX = zona.position.x + Random.Range(-400, 400);
            posZ = zona.position.z + Random.Range(-100, 100);
        }
        //Debug.Log("Spawneo en el sector " + zonaRandom + " cuyo centro es " + zona.position.x + "," + zona.position.z);
        Vector3 spawnPos = new Vector3(posX, 0, posZ);
        Instantiate(arbolPrefab, spawnPos, Quaternion.identity, arbolPadre.transform);
    }

    public void SpawnearJabali()
    {
        int zonaRandom = Random.Range(0, 3);
        Vector3 zona = ZonasSpawnJabalis[zonaRandom];
        float posX = 0, posZ = 0;
        if (zonaRandom < 2)
        {
            posX = zona.x + Random.Range(-50, 50);
            posZ = zona.z + Random.Range(-300, 300);
        }
        else
        {
            posX = zona.x + Random.Range(-200, 200);
            posZ = zona.z + Random.Range(-50, 50);
        }
        //Debug.Log("Spawneo en el sector " + zonaRandom + " cuyo centro es " + zona.position.x + "," + zona.position.z);
        Vector3 spawnPos = new Vector3(posX, 1, posZ);
        Instantiate(jabaliPrefab, spawnPos, Quaternion.identity);
    }

    public IEnumerator JabaliGeneratorAuto()
    {
        yield return new WaitForSeconds(40f);
        SpawnearJabali();
        StartCoroutine(JabaliGeneratorAuto());
    }

    public IEnumerator ArbolGeneratorAuto()
    {
        yield return new WaitForSeconds(10f);
        SpawnearArbol();
        StartCoroutine(ArbolGeneratorAuto());
    }
}
