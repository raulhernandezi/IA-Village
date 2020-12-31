using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlechaScript : MonoBehaviour
{

    private float speed;
    public GameObject jabali;
    public fsmCazador owner;
    [SerializeField] private Mesh meshCarne;

    // Start is called before the first frame update
    void Start()
    {
        speed = 2;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * 0.1f, Space.Self);
        //transform.position = Vector3.MoveTowards(transform.position, jabali.transform.position, 0.1f * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Jabali")
        {
            collision.gameObject.GetComponent<MeshFilter>().mesh = meshCarne;
            owner.AnimalMuerto();
            Destroy(this.gameObject);
        }
    }
}
