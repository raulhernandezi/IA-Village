using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class fsmLeñador : MonoBehaviour {

    #region variables

    public StateMachineEngine fsmLeñador_FSM;
    

    private PushPerception HachaRotaPerception;
    private PushPerception TaladoPerception;
    private TimerPerception HachaReparadaPerception;
    private PushPerception ArbolEncontradoPerception;
    private State BuscarArbol;
    private State Talar;
    private State RepararHacha;

    //Place your variables here
    [SerializeField] private GameObject barraProgreso;
    private NavMeshAgent nmesh;
    private GameObject arbolPadre;
    public int arbolDestino;
    private GameObject[] arboles;

    public CasaController hogar;

    private bool yendoTalar;
    private bool talando;
    private int estadoHacha;
    #endregion variables

    // Start is called before the first frame update
    private void Start()
    {
        fsmLeñador_FSM = new StateMachineEngine(false);

        estadoHacha = 0;
        nmesh = GetComponent<NavMeshAgent>();
        arbolPadre = GameObject.FindGameObjectWithTag("ArbolPadre");
        arboles = new GameObject[arbolPadre.transform.childCount];

        for (int i = 0; i < arbolPadre.transform.childCount; i++)
        {
            arboles[i] = arbolPadre.transform.GetChild(i).gameObject;
        }
        arbolDestino = 0;

        CreateStateMachine();
    }
    
    
    private void CreateStateMachine()
    {
        // Perceptions
        // Modify or add new Perceptions, see the guide for more
        HachaRotaPerception = fsmLeñador_FSM.CreatePerception<PushPerception>();
        TaladoPerception = fsmLeñador_FSM.CreatePerception<PushPerception>();
        HachaReparadaPerception = fsmLeñador_FSM.CreatePerception<TimerPerception>(2f);
        ArbolEncontradoPerception = fsmLeñador_FSM.CreatePerception<PushPerception>();
        
        // States
        BuscarArbol = fsmLeñador_FSM.CreateEntryState("BuscarArbol", BuscarArbolAction);
        Talar = fsmLeñador_FSM.CreateState("Talar", TalarAction);
        RepararHacha = fsmLeñador_FSM.CreateState("RepararHacha", RepararHachaAction);
        
        // Transitions
        fsmLeñador_FSM.CreateTransition("HachaRota", Talar, HachaRotaPerception, RepararHacha);
        fsmLeñador_FSM.CreateTransition("Talado", Talar, TaladoPerception, BuscarArbol);
        fsmLeñador_FSM.CreateTransition("HachaReparada", RepararHacha, HachaReparadaPerception, BuscarArbol);
        fsmLeñador_FSM.CreateTransition("ArbolEncontrado", BuscarArbol, ArbolEncontradoPerception, Talar);
        
        // ExitPerceptions
        
        // ExitTransitions
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (yendoTalar && (int)nmesh.destination.x == (int)transform.position.x && (int)nmesh.destination.z == (int)transform.position.z)
        {
            Debug.Log("He llegado al arbol para talar");
            fsmLeñador_FSM.Fire("ArbolEncontrado");
            yendoTalar = false;
        }

        if (talando)
        {
            barraProgreso.GetComponent<Slider>().value += Time.deltaTime * 0.5f;
            if (barraProgreso.GetComponent<Slider>().value == barraProgreso.GetComponent<Slider>().maxValue)
            {
                //Debug.Log(owner.GetComponent<fsmLeñador>().fsmLeñador_FSM.actualState.Name);
                Destroy(arboles[arbolDestino].gameObject);
                barraProgreso.GetComponent<Slider>().value = 0;
                barraProgreso.SetActive(false);
                fsmLeñador_FSM.Fire("Talado");
                Debug.Log(fsmLeñador_FSM.actualState.Name);
                talando = false;
            }
        }

        if (estadoHacha == 5)
        {
            estadoHacha = 0;
            fsmLeñador_FSM.Fire("HachaRota");
        }

        fsmLeñador_FSM.Update();
    }

    // Create your desired actions
    
    private void BuscarArbolAction()
    {
        Debug.Log("Buscando arbol");
        for (int i = 0; i < arbolPadre.transform.childCount; i++)
        {
            arboles[i] = arbolPadre.transform.GetChild(i).gameObject;
        }
        arbolDestino = 0;

        for (int i = 0; i < arbolPadre.transform.childCount; i++)
        {
            if (Vector3.Distance(transform.position, arboles[i].transform.position) < Vector3.Distance(transform.position, arboles[arbolDestino].transform.position))
            {
                arbolDestino = i;
            }
        }
        Debug.Log("arbol: " + arbolDestino);
        nmesh.destination = arboles[arbolDestino].transform.GetChild(0).position;
        yendoTalar = true;
    }
    
    private void TalarAction()
    {
        Debug.Log("Talando");
        talando = true;
    }
    
    private void RepararHachaAction()
    {
        Debug.Log("Reparando hacha");
    }

   /* private GameObject[] obtenerArboles()
    {
        GameObject[] arrayArb = new GameObject[arbolPadre.transform.childCount];
        for (int i = 0; i < arbolPadre.transform.childCount; i++)
        {
            arrayArb[i] = arbolPadre.transform.GetChild(i).gameObject;
        }
        arbolDestino = arboles[0];
        return arrayArb;

    }*/
    
}