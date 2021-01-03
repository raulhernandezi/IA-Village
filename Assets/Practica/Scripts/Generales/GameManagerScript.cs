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
    public float hay_leche;
    public float hay_comida;

    [SerializeField] private TextMeshProUGUI txt_Comida;
    [SerializeField] private TextMeshProUGUI txt_Leche;
    [SerializeField] private TextMeshProUGUI txt_Madera;
    [SerializeField] private TextMeshProUGUI txt_Pasto;

    [SerializeField] public Transform almacenDropPlace;
    [SerializeField] public Transform herreriaPlace;
    [SerializeField] public Transform restaurantePlace;

    // Start is called before the first frame update
    void Start()
    {
        leche = 50;
        comida = 50;
        hay_leche = 0;
        hay_comida = 0;
        txt_Comida.text = "Comida: " + comida;
        txt_Leche.text = "Leche: " + leche;
        txt_Madera.text = "Madera: " + madera;
        txt_Pasto.text = "Pasto: " + pasto;
    }

    // Update is called once per frame
    void Update()
    {
        if(leche > 0)
        {
            hay_leche = 1f;
        }
        else
        {
            hay_leche = 0f;
        }

        if(comida > 0)
        {
            hay_comida = 1f;
        }
        else{
            hay_comida = 0f;
        }

        txt_Comida.text = "Comida: " + comida;
        txt_Leche.text = "Leche: " + leche;
        txt_Madera.text = "Madera: " + madera;
        txt_Pasto.text = "Pasto: " + pasto;
    }
}
