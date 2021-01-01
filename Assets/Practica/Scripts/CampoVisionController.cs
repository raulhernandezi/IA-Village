using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampoVisionController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Jabali")
        {
            
            this.transform.GetComponentInParent<fsmCazador>().AnimalEnRango(collision.gameObject);
        }
    }
}
