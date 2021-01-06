using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class fsmCazador : MonoBehaviour {

    #region variables

    public StateMachineEngine fsmCazador_FSM;
    

    private PushPerception AnimalEnRangoPerception;
    private PushPerception IrAPorComidaPerception;
    private PushPerception IrADejarComidaPerception;
    private PushPerception RondarDeNuevoPerception;
    private PushPerception HayHambreYComidaPerception;
    private PushPerception HaySedYLechePerception;
    private PushPerception NecesidadSaciadaPerception;
    private State Comiendo;
    private State Bebiendo;
    private State Rondar;
    private State Disparar;
    private State CogerComida;
    private State DejarComida;


    private NavMeshAgent nmesh;
    public GameObject comedero;
    private GameObject[] puntos;
    private GameObject caminoCazador;
    [SerializeField] private GameObject flecha;
    [SerializeField] private GameObject barraProgreso;
    [SerializeField] private GameObject spawnFlecha;
    [SerializeField] private Mesh meshCarne;
    [SerializeField] private Material materialCarne;
    [SerializeField] private GameObject carne;
    [HideInInspector] public GameObject presa;

    public CasaController hogar;

    private int puntoActual;
    private bool rondar;
    private bool recogiendo;
    private bool entregando;

    //Place your variables here

    public float hambre;
    public float sed;
    private float ratioGananciaHambre;
    private float ratioGananciaSed;
    private bool estaComiendo;
    private bool estaBebiendo;

    public GameManagerScript gameManager;

    #endregion variables

    // Start is called before the first frame update
    private void Start()
    {
        fsmCazador_FSM = new StateMachineEngine(false);
        nmesh = GetComponent<NavMeshAgent>();
        puntoActual = 0;
        rondar = false;
        recogiendo = false;
        entregando = false;
        gameManager = FindObjectOfType<GameManagerScript>();

        hambre = 0;
        sed = 0;
        ratioGananciaHambre = Random.Range(0.1f, 0.3f);
        ratioGananciaSed = Random.Range(0.3f, 0.6f);

        caminoCazador = GameObject.FindGameObjectWithTag("CaminoCazador");
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
        HayHambreYComidaPerception = fsmCazador_FSM.CreatePerception<PushPerception>();
        HaySedYLechePerception = fsmCazador_FSM.CreatePerception<PushPerception>();
        NecesidadSaciadaPerception = fsmCazador_FSM.CreatePerception<PushPerception>();

        // States
        Rondar = fsmCazador_FSM.CreateEntryState("Rondar", RondarAction);
        Disparar = fsmCazador_FSM.CreateState("Disparar", DispararAction);
        CogerComida = fsmCazador_FSM.CreateState("CogerComida", CogerComidaAction);
        DejarComida = fsmCazador_FSM.CreateState("DejarComida", DejarComidaAction);
        Bebiendo = fsmCazador_FSM.CreateState("Bebiendo", BebiendoAction);
        Comiendo = fsmCazador_FSM.CreateState("Comiendo", ComiendoAction);

        // Transitions
        fsmCazador_FSM.CreateTransition("AnimalEnRango", Rondar, AnimalEnRangoPerception, Disparar);
        fsmCazador_FSM.CreateTransition("IrAPorComida", Disparar, IrAPorComidaPerception, CogerComida);
        fsmCazador_FSM.CreateTransition("IrADejarComida", CogerComida, IrADejarComidaPerception, DejarComida);
        fsmCazador_FSM.CreateTransition("RondarDeNuevo", DejarComida, RondarDeNuevoPerception, Rondar);
        fsmCazador_FSM.CreateTransition("HayHambreYComida", Rondar, HayHambreYComidaPerception, Comiendo);
        fsmCazador_FSM.CreateTransition("HaySedYLeche", Rondar, HaySedYLechePerception, Bebiendo);
        fsmCazador_FSM.CreateTransition("HambreSaciada", Comiendo, NecesidadSaciadaPerception, Rondar);
        fsmCazador_FSM.CreateTransition("SedSaciada", Bebiendo, NecesidadSaciadaPerception, Rondar);

        // ExitPerceptions

        // ExitTransitions

    }

    // Update is called once per frame
    private void Update()
    {
        if (fsmCazador_FSM.actualState != Comiendo && fsmCazador_FSM.actualState != Bebiendo)
        {
            sed += Time.deltaTime * ratioGananciaSed;
            hambre += Time.deltaTime * ratioGananciaHambre;
        }
        if (fsmCazador_FSM.actualState == Comiendo &&
            (int)transform.position.x == (int)gameManager.restaurantePlace.position.x &&
            (int)transform.position.z == (int)gameManager.restaurantePlace.position.z && !estaComiendo && gameManager.hay_comida)
        {
            estaComiendo = true;
            StartCoroutine(ComerTimer());
        }
        if (fsmCazador_FSM.actualState == Bebiendo &&
            (int)transform.position.x == (int)gameManager.restaurantePlace.position.x &&
            (int)transform.position.z == (int)gameManager.restaurantePlace.position.z && !estaBebiendo && gameManager.hay_leche)
        {
            estaBebiendo = true;
            StartCoroutine(BeberTimer());
        }
        if (rondar)
        {
            if (gameManager.hay_comida && hambre > 70)
            {
                fsmCazador_FSM.Fire("HayHambreYComida");
            }
            else if (gameManager.hay_leche && sed > 70)
            {
                fsmCazador_FSM.Fire("HaySedYLeche");
            }
            else if ((int)transform.position.x == (int)puntos[puntoActual].transform.position.x && (int)transform.position.z == (int)puntos[puntoActual].transform.position.z)
            {
                if (puntoActual == caminoCazador.transform.childCount - 1)
                {
                    puntoActual = 0;
                }
                else
                {
                    puntoActual++;
                }
            }
        }

        if ((int)nmesh.destination.x == (int)transform.position.x && (int)nmesh.destination.z == (int)transform.position.z  && entregando)
        {
            entregando = false;
            gameManager.comida += 2;
            fsmCazador_FSM.Fire("RondarDeNuevo");
        }

        fsmCazador_FSM.Update();
    }

    protected void LateUpdate()
    {
        transform.localEulerAngles = new Vector3(-90, transform.localEulerAngles.y, 0);
    }

    // Create your desired actions
    
    private void RondarAction()
    {
        rondar = true;
        nmesh.destination = puntos[puntoActual].transform.position;
    }
    
    private void DispararAction()
    {
        rondar = false;
        nmesh.destination = transform.position;
        this.transform.LookAt(presa.transform);
        StartCoroutine("flechaTimer");
    }
    
    private void CogerComidaAction()
    {
        nmesh.destination = presa.transform.position;
    }
    
    private void DejarComidaAction()
    {
        nmesh.destination = gameManager.almacenDropPlace.position;
        entregando = true;
    }

    private void BebiendoAction()
    {
        nmesh.destination = gameManager.restaurantePlace.position;
        estaBebiendo = false;
    }

    private void ComiendoAction()
    {
        nmesh.destination = gameManager.restaurantePlace.position;
        estaComiendo = false;
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

    public IEnumerator flechaTimer()
    {
        yield return new WaitForSeconds(2f);

        presa.gameObject.GetComponent<MeshFilter>().mesh = meshCarne;
        presa.gameObject.GetComponent<MeshRenderer>().material = materialCarne;
        AnimalMuerto();
    }

    public IEnumerator ComerTimer()
    {
        yield return new WaitForSeconds(2);
        hambre = 0;
        gameManager.comida--;
        fsmCazador_FSM.Fire("HambreSaciada");
    }

    public IEnumerator BeberTimer()
    {
        yield return new WaitForSeconds(2);
        sed = 0;
        gameManager.leche--;
        fsmCazador_FSM.Fire("SedSaciada");
    }
}