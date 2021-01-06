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
    private PushPerception HachaReparadaPerception;
    private PushPerception ArbolEncontradoPerception;
    private PushPerception HayHambreYComidaPerception;
    private PushPerception HaySedYLechePerception;
    private PushPerception NecesidadSaciadaPerception;
    private State BuscarArbol;
    private State Talar;
    private State RepararHacha;
    private State Comiendo;
    private State Bebiendo;

    //Place your variables here
    [SerializeField] private GameObject barraProgreso;
    private NavMeshAgent navMesh;
    private GameObject arbolPadre;
    public GameObject arbolDestino;
    private bool arbolEncontrado;
    private GameObject[] arboles;

    public CasaController hogar;
    private GameManagerScript gameManager;
    private Transform herreria;

    [SerializeField] private bool yendoTalar;
    [SerializeField] private bool talando;
    [SerializeField] private bool reparando;
    [SerializeField] private int estadoHacha;

    public float hambre;
    public float sed;
    private float ratioGananciaHambre;
    private float ratioGananciaSed;
    private bool estaComiendo;
    private bool estaBebiendo;
    #endregion variables

    // Start is called before the first frame update
    private void Start()
    {
        fsmLeñador_FSM = new StateMachineEngine(false);

        gameManager = FindObjectOfType<GameManagerScript>();
        herreria = gameManager.herreriaPlace;
        estadoHacha = 0;
        hambre = 0;
        sed = 0;
        ratioGananciaHambre = Random.Range(0.1f, 0.3f);
        ratioGananciaSed = Random.Range(0.3f, 0.6f);

        navMesh = GetComponent<NavMeshAgent>();
        arbolPadre = GameObject.FindGameObjectWithTag("ArbolPadre");
        arboles = new GameObject[arbolPadre.transform.childCount];

        for (int i = 0; i < arbolPadre.transform.childCount; i++)
        {
            arboles[i] = arbolPadre.transform.GetChild(i).gameObject;
        }
        arbolDestino = arboles[0];

        CreateStateMachine();
    }
    
    
    private void CreateStateMachine()
    {
        // Perceptions
        HachaRotaPerception = fsmLeñador_FSM.CreatePerception<PushPerception>();
        TaladoPerception = fsmLeñador_FSM.CreatePerception<PushPerception>();
        HachaReparadaPerception = fsmLeñador_FSM.CreatePerception<PushPerception>();
        ArbolEncontradoPerception = fsmLeñador_FSM.CreatePerception<PushPerception>();
        HayHambreYComidaPerception = fsmLeñador_FSM.CreatePerception<PushPerception>();
        HaySedYLechePerception = fsmLeñador_FSM.CreatePerception<PushPerception>();
        NecesidadSaciadaPerception = fsmLeñador_FSM.CreatePerception<PushPerception>();

        // States
        BuscarArbol = fsmLeñador_FSM.CreateEntryState("BuscarArbol", BuscarArbolAction);
        Talar = fsmLeñador_FSM.CreateState("Talar", TalarAction);
        RepararHacha = fsmLeñador_FSM.CreateState("RepararHacha", RepararHachaAction);
        Bebiendo = fsmLeñador_FSM.CreateState("Bebiendo", BebiendoAction);
        Comiendo = fsmLeñador_FSM.CreateState("Comiendo", ComiendoAction);

        // Transitions
        fsmLeñador_FSM.CreateTransition("HachaRota", Talar, HachaRotaPerception, RepararHacha);
        fsmLeñador_FSM.CreateTransition("HachaParaReparar", BuscarArbol, HachaRotaPerception, RepararHacha);
        fsmLeñador_FSM.CreateTransition("Talado", Talar, TaladoPerception, BuscarArbol);
        fsmLeñador_FSM.CreateTransition("HachaReparada", RepararHacha, HachaReparadaPerception, BuscarArbol);
        fsmLeñador_FSM.CreateTransition("ArbolEncontrado", BuscarArbol, ArbolEncontradoPerception, Talar);
        fsmLeñador_FSM.CreateTransition("HayHambreYComida", BuscarArbol, HayHambreYComidaPerception, Comiendo);
        fsmLeñador_FSM.CreateTransition("HaySedYLeche", BuscarArbol, HaySedYLechePerception, Bebiendo);
        fsmLeñador_FSM.CreateTransition("HambreSaciada", Comiendo, NecesidadSaciadaPerception, BuscarArbol);
        fsmLeñador_FSM.CreateTransition("SedSaciada", Bebiendo, NecesidadSaciadaPerception, BuscarArbol);

    }

    // Update is called once per frame
    private void Update()
    {
        if (fsmLeñador_FSM.actualState != Comiendo && fsmLeñador_FSM.actualState != Bebiendo)
        {
            sed += Time.deltaTime * ratioGananciaSed;
            hambre += Time.deltaTime * ratioGananciaHambre;
        }
        if (fsmLeñador_FSM.actualState == Comiendo &&
            (int)transform.position.x == (int)gameManager.restaurantePlace.position.x &&
            (int)transform.position.z == (int)gameManager.restaurantePlace.position.z && !estaComiendo && gameManager.hay_comida)
        {
            estaComiendo = true;
            StartCoroutine(ComerTimer());
        }
        if (fsmLeñador_FSM.actualState == Bebiendo &&
            (int)transform.position.x == (int)gameManager.restaurantePlace.position.x &&
            (int)transform.position.z == (int)gameManager.restaurantePlace.position.z && !estaBebiendo && gameManager.hay_leche)
        {
            estaBebiendo = true;
            StartCoroutine(BeberTimer());
        }

        if (fsmLeñador_FSM.actualState == BuscarArbol && arbolDestino != null &&
            (int)arbolDestino.GetComponent<ArbolController>().zonaDeTalado.position.x == (int)transform.position.x && 
            (int)arbolDestino.GetComponent<ArbolController>().zonaDeTalado.position.z == (int)transform.position.z)
        {
            Debug.Log("He llegado al arbol para talar");
            fsmLeñador_FSM.Fire("ArbolEncontrado");
            
        }

        if (fsmLeñador_FSM.actualState == Talar)
        {
            barraProgreso.GetComponent<Slider>().value += Time.deltaTime * 0.5f;
            if (barraProgreso.GetComponent<Slider>().value == barraProgreso.GetComponent<Slider>().maxValue)
            {
                talando = false;
                Destroy(arbolDestino.gameObject);
                barraProgreso.GetComponent<Slider>().value = 0;
                barraProgreso.SetActive(false);
                
                estadoHacha++;
                if (estadoHacha >= 6)
                {
                    Debug.Log("a reparar");
                    gameManager.madera++;
                    fsmLeñador_FSM.Fire("HachaRota");
                }
                else
                {
                    Debug.Log("ya he talado bieeeeeeeeeeeen");
                    StartCoroutine("timerTalado");
                    
                }
            }
        }

        if (fsmLeñador_FSM.actualState == RepararHacha && (int)transform.position.x == (int)herreria.position.x && (int)transform.position.z == (int)herreria.position.z)
        {
            Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            barraProgreso.GetComponent<Slider>().value += Time.deltaTime * 0.5f;
            if (barraProgreso.GetComponent<Slider>().value == barraProgreso.GetComponent<Slider>().maxValue)
            {
                estadoHacha = 0;
                fsmLeñador_FSM.Fire("HachaReparada");
                barraProgreso.GetComponent<Slider>().value = 0;
                barraProgreso.SetActive(false);
                
                Debug.Log(fsmLeñador_FSM.actualState.Name);
                reparando = false;

            }
        }
        
        fsmLeñador_FSM.Update();
    }

    protected void LateUpdate()
    {
        transform.localEulerAngles = new Vector3(-90, 0, 0);
    }

    // Create your desired actions

    private void BuscarArbolAction()
    {
        if (gameManager.hay_comida && hambre > 70)
        {
            fsmLeñador_FSM.Fire("HayHambreYComida");
        }
        else if (gameManager.hay_leche && sed > 70)
        {
            fsmLeñador_FSM.Fire("HaySedYLeche");
        }
        else if(estadoHacha >= 6)
        {
            fsmLeñador_FSM.Fire("HachaParaReparar");
        }
        else
        {
            arboles = new GameObject[arbolPadre.transform.childCount];
            for (int i = 0; i < arbolPadre.transform.childCount; i++)
            {
                arboles[i] = arbolPadre.transform.GetChild(i).gameObject;
            }
            arbolDestino = arboles[0];
            for (int i = 0; i < arbolPadre.transform.childCount; i++)
            {
                if (Vector3.Distance(transform.position, arboles[i].transform.position) < Vector3.Distance(transform.position, arbolDestino.transform.position) &&
                    !arboles[i].GetComponent<ArbolController>().siendoTalado)
                {
                    arbolDestino = arboles[i];
                }
            }
            
            arbolDestino.GetComponent<ArbolController>().siendoTalado = true;
            navMesh.destination = arbolDestino.GetComponent<ArbolController>().zonaDeTalado.position;
            yendoTalar = true;
            
        }
    }
    
    private void TalarAction()
    {
        talando = true;
    }
    
    private void RepararHachaAction()
    {
        navMesh.destination = herreria.position;
        reparando = true;
    }

    private void BebiendoAction()
    {
        navMesh.destination = gameManager.restaurantePlace.position;
        estaBebiendo = false;
    }

    private void ComiendoAction()
    {
        navMesh.destination = gameManager.restaurantePlace.position;
        estaComiendo = false;
    }

    IEnumerator timerTalado()
    {
        yield return new WaitForSeconds(0.1f);
        gameManager.madera++;
        fsmLeñador_FSM.Fire("Talado");
        
    }

    public IEnumerator ComerTimer()
    {
        yield return new WaitForSeconds(2);
        hambre = 0;
        gameManager.comida--;
        fsmLeñador_FSM.Fire("HambreSaciada");
    }

    public IEnumerator BeberTimer()
    {
        yield return new WaitForSeconds(2);
        sed = 0;
        gameManager.leche--;
        fsmLeñador_FSM.Fire("SedSaciada");
    }

}