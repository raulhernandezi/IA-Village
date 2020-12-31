using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class fsmCazador : MonoBehaviour {

    #region variables

    private StateMachineEngine fsmCazador_FSM;
    

    private PushPerception AnimalEnRangoPerception;
    private PushPerception IrAPorComidaPerception;
    private PushPerception IrADejarComidaPerception;
    private PushPerception RondarDeNuevoPerception;
    private State Rondar;
    private State Disparar;
    private State CogerComida;
    private State DejarComida;


    private NavMeshAgent nmesh;
    public GameObject comedero;
    private GameObject[] puntos;
    [SerializeField] private GameObject caminoCazador;
    [SerializeField] private GameObject flecha;
    [SerializeField] private GameObject barraProgreso;
    private GameObject presa;

    private int puntoActual;
    private bool rondar;
    private bool recogiendo;
    
    //Place your variables here

    #endregion variables

    // Start is called before the first frame update
    private void Start()
    {
        fsmCazador_FSM = new StateMachineEngine(false);
        nmesh = GetComponent<NavMeshAgent>();
        puntoActual = 0;
        rondar = false;
        recogiendo = false;

        puntos = new GameObject[caminoCazador.transform.childCount];
        for(int i = 0; i < caminoCazador.transform.childCount; i++)
        {
            puntos[i] = caminoCazador.transform.GetChild(i).gameObject;
        }

        CreateStateMachine();
    }
    
    
    private void CreateStateMachine()
    {
        // Perceptions
        // Modify or add new Perceptions, see the guide for more
        AnimalEnRangoPerception = fsmCazador_FSM.CreatePerception<PushPerception>();
        IrAPorComidaPerception = fsmCazador_FSM.CreatePerception<PushPerception>();
        IrADejarComidaPerception = fsmCazador_FSM.CreatePerception<PushPerception>();
        RondarDeNuevoPerception = fsmCazador_FSM.CreatePerception<PushPerception>();
        
        // States
        Rondar = fsmCazador_FSM.CreateEntryState("Rondar", RondarAction);
        Disparar = fsmCazador_FSM.CreateState("Disparar", DispararAction);
        CogerComida = fsmCazador_FSM.CreateState("CogerComida", CogerComidaAction);
        DejarComida = fsmCazador_FSM.CreateState("DejarComida", DejarComidaAction);
        
        // Transitions
        fsmCazador_FSM.CreateTransition("AnimalEnRango", Rondar, AnimalEnRangoPerception, Disparar);
        fsmCazador_FSM.CreateTransition("IrAPorComida", Disparar, IrAPorComidaPerception, CogerComida);
        fsmCazador_FSM.CreateTransition("IrADejarComida", CogerComida, IrADejarComidaPerception, DejarComida);
        fsmCazador_FSM.CreateTransition("RondarDeNuevo", DejarComida, RondarDeNuevoPerception, Rondar);
        
        // ExitPerceptions
        
        // ExitTransitions
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (rondar)
        {
            nmesh.destination = puntos[puntoActual].transform.position;
            if (nmesh.transform.position.x == puntos[puntoActual].transform.position.x && nmesh.transform.position.z == puntos[puntoActual].transform.position.z)
            {
                Debug.Log("Llegue");
                if (puntoActual == caminoCazador.transform.childCount - 1)
                {
                    Debug.Log("LLegue al ultimo punto, me vuelvo");
                    puntoActual = 0;
                }
                else
                {
                    Debug.Log("Paso al siguiente punto");
                    puntoActual++;
                }
            }
        }

        if (recogiendo)
        {
            barraProgreso.GetComponent<Slider>().value += Time.deltaTime * 0.5f;
            if(barraProgreso.GetComponent<Slider>().value == barraProgreso.GetComponent<Slider>().maxValue)
            {
                Destroy(presa.gameObject);
                barraProgreso.GetComponent<Slider>().value = 0;
                barraProgreso.SetActive(false);
                fsmCazador_FSM.Fire("IrADejarComida");
                recogiendo = false;
            }
        }

        fsmCazador_FSM.Update();
    }

    // Create your desired actions
    
    private void RondarAction()
    {
        Debug.Log("ESTADO RONDAR");
        rondar = true;
    }
    
    private void DispararAction()
    {
        rondar = false;
        nmesh.destination = transform.position;
        this.transform.LookAt(presa.transform);
        GameObject miFlecha = Instantiate(flecha, this.transform.position, this.transform.rotation);
        miFlecha.GetComponent<FlechaScript>().owner = this;
        miFlecha.GetComponent<FlechaScript>().jabali = presa;
        Debug.Log("PIUM PIUM!");
    }
    
    private void CogerComidaAction()
    {
        Debug.Log("Yendo a por la comida");
        nmesh.destination = presa.transform.position;
    }
    
    private void DejarComidaAction()
    {
        Debug.Log("Voy a dejar la comida");
    }

    public void AnimalEnRango(GameObject animal)
    {
        presa = animal;
        fsmCazador_FSM.Fire("AnimalEnRango");
    }

    public void AnimalMuerto()
    {
        fsmCazador_FSM.Fire("IrAPorComida");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Jabali")
        {
            barraProgreso.SetActive(true);
            recogiendo = true;
        }
    }
}