using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class fsmConstructor : MonoBehaviour {

    #region variables

    private StateMachineEngine fsmConstructor_FSM;


    private PushPerception ConstruccionFinalizadaPerception;
    private PushPerception HayMaderaYCimientosPerception;
    private PushPerception HayHambreYComidaPerception;
    private PushPerception HaySedYLechePerception;
    private PushPerception NecesidadSaciadaPerception;
    private State Comiendo;
    private State Bebiendo;
    private State Esperando;
    private State Construyendo;

    //Place your variables here

    [SerializeField] public NavMeshAgent navMesh;
    [SerializeField] public GameObject[] prefabsCasas;
    public CimientosControler cimientosController;
    public CasaController hogar;

    public GameManagerScript gameManager;

    private Transform lugarDondeConstruir;
    private bool llegando;

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
        fsmConstructor_FSM = new StateMachineEngine(false);
        gameManager = FindObjectOfType<GameManagerScript>();

        hambre = 0;
        sed = 0;
        ratioGananciaHambre = Random.Range(0.1f, 0.3f);
        ratioGananciaSed = Random.Range(0.3f, 0.6f);

        cimientosController = FindObjectOfType<CimientosControler>();

        CreateStateMachine();
    }


    private void CreateStateMachine()
    {
        // Perceptions
        // Modify or add new Perceptions, see the guide for more
        ConstruccionFinalizadaPerception = fsmConstructor_FSM.CreatePerception<PushPerception>();
        HayMaderaYCimientosPerception = fsmConstructor_FSM.CreatePerception<PushPerception>();
        HayHambreYComidaPerception = fsmConstructor_FSM.CreatePerception<PushPerception>();
        HaySedYLechePerception = fsmConstructor_FSM.CreatePerception<PushPerception>();
        NecesidadSaciadaPerception = fsmConstructor_FSM.CreatePerception<PushPerception>();

        // States
        Esperando = fsmConstructor_FSM.CreateEntryState("Esperando", EsperandoAction);
        Construyendo = fsmConstructor_FSM.CreateState("Construyendo", ConstruyendoAction);
        Bebiendo = fsmConstructor_FSM.CreateState("Bebiendo", BebiendoAction);
        Comiendo = fsmConstructor_FSM.CreateState("Comiendo", ComiendoAction);

        // Transitions
        fsmConstructor_FSM.CreateTransition("HayMaderaYCimientos", Esperando, HayMaderaYCimientosPerception, Construyendo);
        fsmConstructor_FSM.CreateTransition("ConstruccionFinalizada", Construyendo, ConstruccionFinalizadaPerception, Esperando);
        fsmConstructor_FSM.CreateTransition("HayHambreYComida", Esperando, HayHambreYComidaPerception, Comiendo);
        fsmConstructor_FSM.CreateTransition("HaySedYLeche", Esperando, HaySedYLechePerception, Bebiendo);
        fsmConstructor_FSM.CreateTransition("HambreSaciada", Comiendo, NecesidadSaciadaPerception, Esperando);
        fsmConstructor_FSM.CreateTransition("SedSaciada", Bebiendo, NecesidadSaciadaPerception, Esperando);
    }


    private void Update()
    {
        if(fsmConstructor_FSM.actualState != Comiendo && fsmConstructor_FSM.actualState != Bebiendo)
        {
            sed += Time.deltaTime * ratioGananciaSed;
            hambre += Time.deltaTime * ratioGananciaHambre;
        }
        if(fsmConstructor_FSM.actualState == Comiendo && 
            (int)transform.position.x == (int)gameManager.restaurantePlace.position.x && 
            (int)transform.position.z == (int)gameManager.restaurantePlace.position.z && !estaComiendo && gameManager.hay_comida)
        {
            estaComiendo = true;
            StartCoroutine(ComerTimer());
        }
        if (fsmConstructor_FSM.actualState == Bebiendo &&
            (int)transform.position.x == (int)gameManager.restaurantePlace.position.x &&
            (int)transform.position.z == (int)gameManager.restaurantePlace.position.z && !estaBebiendo && gameManager.hay_leche)
        {
            estaBebiendo = true;
            StartCoroutine(BeberTimer());
        }
        if (fsmConstructor_FSM.actualState == Esperando)
        {
            if(gameManager.hay_comida && hambre > 70)
            {
                fsmConstructor_FSM.Fire("HayHambreYComida");
            }
            else if (gameManager.hay_leche && sed > 70)
            {
                fsmConstructor_FSM.Fire("HaySedYLeche");
            }
            else if (gameManager.madera >= 20)
            {
                for (int i = 0; i < cimientosController.cimientos.Length; i++)
                {
                    if (!cimientosController.cimientos[i].ocupado)
                    {
                        cimientosController.OcuparCimiento(i);
                        lugarDondeConstruir = cimientosController.cimientos[i].gameObject.transform;
                        fsmConstructor_FSM.Fire("HayMaderaYCimientos");
                        break;
                    }
                }
            }
        }
        if (fsmConstructor_FSM.actualState == Construyendo)
        {
            if ((int)navMesh.destination.x == (int)transform.position.x && (int)navMesh.destination.z == (int)transform.position.z && llegando)
            {
                llegando = false;
                Debug.Log("Comienzo a construir");
                StartCoroutine(ConstruirTimer());
            }
        }
        fsmConstructor_FSM.Update();
    }

    protected void LateUpdate()
    {
        transform.localEulerAngles = new Vector3(-90, transform.localEulerAngles.y, 0);
    }

    // Create your desired actions

    private void EsperandoAction()
    {
        navMesh.destination = hogar.spawnPlace.position;
    }

    private void ConstruyendoAction()
    {
        navMesh.destination = lugarDondeConstruir.position - new Vector3(15, 0, 0);
        llegando = true;
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

    public IEnumerator ConstruirTimer()
    {
        yield return new WaitForSeconds(5);
        //Aqui deberia o bien decidir de que oficio hay menos y en ese caso construir esa casa concreta
        int contadorMasBajo = 100;
        int indice = 0;
        for(int i = 0; i < gameManager.contadorOficios.Length; i++)
        {
            if(gameManager.contadorOficios[i] < contadorMasBajo)
            {
                contadorMasBajo = gameManager.contadorOficios[i];
                indice = i;
            }
        }
        Instantiate(prefabsCasas[indice], lugarDondeConstruir.position, lugarDondeConstruir.rotation);
        gameManager.madera -= 20;
        fsmConstructor_FSM.Fire("ConstruccionFinalizada");
    }

    public IEnumerator ComerTimer()
    {
        yield return new WaitForSeconds(2);
        hambre = 0;
        gameManager.comida--;
        fsmConstructor_FSM.Fire("HambreSaciada");
    }

    public IEnumerator BeberTimer()
    {
        yield return new WaitForSeconds(2);
        sed = 0;
        gameManager.leche--;
        fsmConstructor_FSM.Fire("SedSaciada");
    }

}