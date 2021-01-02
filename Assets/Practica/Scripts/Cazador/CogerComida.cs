using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CogerComida : MonoBehaviour
{

    [SerializeField] private GameObject barraProgreso;
    [SerializeField] private fsmCazador owner;

    private bool recogiendo;
    // Start is called before the first frame update
    void Start()
    {
        recogiendo = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (recogiendo)
        {
            barraProgreso.GetComponent<Slider>().value += Time.deltaTime * 0.5f;
            if (barraProgreso.GetComponent<Slider>().value == barraProgreso.GetComponent<Slider>().maxValue)
            {
                Destroy(owner.GetComponent<fsmCazador>().presa.gameObject);
                barraProgreso.GetComponent<Slider>().value = 0;
                barraProgreso.SetActive(false);
                owner.GetComponent<fsmCazador>().fsmCazador_FSM.Fire("IrADejarComida");
                recogiendo = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("He tocado algo");
        if (collision.gameObject.tag == "Jabali")
        {
            Debug.Log("He recogido el jabali");
            barraProgreso.SetActive(true);
            recogiendo = true;
        }
    }
}
