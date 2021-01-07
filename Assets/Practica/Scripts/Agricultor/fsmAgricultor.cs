using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class fsmAgricultor : MonoBehaviour {

    #region variables

    private StateMachineEngine fsmAgricultor_FSM;
    

    private PushPerception PlantarPerception;
    private PushPerception PararPlantarPerception;
    private PushPerception PararCosecharPerception;
    private PushPerception CosecharPerception;
    private PushPerception HayHambreYComidaPerception;
    private PushPerception HaySedYLechePerception;
    private PushPerception NecesidadSaciadaPerception;
    private State BuscarCampo;
    private State Plantando;
    private State Cosechando;
    private State Comiendo;
    private State Bebiendo;

    //Place your variables here
    public GameObject casa;
    private GameObject parcelaDestino;
    private NavMeshAgent navMesh;
    [SerializeField] private GameObject barraProgreso;
    public GameManagerScript gameManager;

    private bool plantando;
    private bool cosechando;

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
        fsmAgricultor_FSM = new StateMachineEngine(false);
        gameManager = FindObjectOfType<GameManagerScript>();
        hambre = 0;
        sed = 0;
        ratioGananciaHambre = Random.Range(0.1f, 0.3f);
        ratioGananciaSed = Random.Range(0.3f, 0.6f);

        navMesh = GetComponent<NavMeshAgent>();

        CreateStateMachine();

    }
    
    
    private void CreateStateMachine()
    {
        // Perceptions
        // Modify or add new Perceptions, see the guide for more
        PlantarPerception = fsmAgricultor_FSM.CreatePerception<PushPerception>();
        PararPlantarPerception = fsmAgricultor_FSM.CreatePerception<PushPerception>();
        PararCosecharPerception = fsmAgricultor_FSM.CreatePerception<PushPerception>();
        CosecharPerception = fsmAgricultor_FSM.CreatePerception<PushPerception>();
        HayHambreYComidaPerception = fsmAgricultor_FSM.CreatePerception<PushPerception>();
        HaySedYLechePerception = fsmAgricultor_FSM.CreatePerception<PushPerception>();
        NecesidadSaciadaPerception = fsmAgricultor_FSM.CreatePerception<PushPerception>();
        // States
        BuscarCampo = fsmAgricultor_FSM.CreateEntryState("BuscarCampo", BuscarCampoAction);
        Plantando = fsmAgricultor_FSM.CreateState("Plantando", PlantandoAction);
        Cosechando = fsmAgricultor_FSM.CreateState("Cosechando", CosechandoAction);
        Bebiendo = fsmAgricultor_FSM.CreateState("Bebiendo", BebiendoAction);
        Comiendo = fsmAgricultor_FSM.CreateState("Comiendo", ComiendoAction);
        // Transitions
        fsmAgricultor_FSM.CreateTransition("Plantar", BuscarCampo, PlantarPerception, Plantando);
        fsmAgricultor_FSM.CreateTransition("PararPlantar", Plantando, PararPlantarPerception, BuscarCampo);
        fsmAgricultor_FSM.CreateTransition("PararCosechar", Cosechando, PararCosecharPerception, BuscarCampo);
        fsmAgricultor_FSM.CreateTransition("Cosechar", BuscarCampo, CosecharPerception, Cosechando);
        fsmAgricultor_FSM.CreateTransition("HayHambreYComida", BuscarCampo, HayHambreYComidaPerception, Comiendo);
        fsmAgricultor_FSM.CreateTransition("HaySedYLeche", BuscarCampo, HaySedYLechePerception, Bebiendo);
        fsmAgricultor_FSM.CreateTransition("HambreSaciada", Comiendo, NecesidadSaciadaPerception, BuscarCampo);
        fsmAgricultor_FSM.CreateTransition("SedSaciada", Bebiendo, NecesidadSaciadaPerception, BuscarCampo);
        // ExitPerceptions

        // ExitTransitions

    }

    // Update is called once per frame
    private void Update()
    {
        if (fsmAgricultor_FSM.actualState != Comiendo && fsmAgricultor_FSM.actualState != Bebiendo)
        {
            sed += Time.deltaTime * ratioGananciaSed;
            hambre += Time.deltaTime * ratioGananciaHambre;
        }
        if (fsmAgricultor_FSM.actualState == Comiendo &&
            (int)transform.position.x == (int)gameManager.restaurantePlace.position.x &&
            (int)transform.position.z == (int)gameManager.restaurantePlace.position.z && !estaComiendo && gameManager.hay_comida)
        {
            estaComiendo = true;
            StartCoroutine(ComerTimer());
        }
        if (fsmAgricultor_FSM.actualState == Bebiendo &&
            (int)transform.position.x == (int)gameManager.restaurantePlace.position.x &&
            (int)transform.position.z == (int)gameManager.restaurantePlace.position.z && !estaBebiendo && gameManager.hay_leche)
        {
            estaBebiendo = true;
            StartCoroutine(BeberTimer());
        }

        if (plantando && (int)navMesh.destination.x == (int)transform.position.x && (int)navMesh.destination.z == (int)transform.position.z)
        {
            barraProgreso.GetComponent<Slider>().value += Time.deltaTime * 0.5f;
            if (barraProgreso.GetComponent<Slider>().value == barraProgreso.GetComponent<Slider>().maxValue)
            {
                //Debug.Log(owner.GetComponent<fsmLeñador>().fsmLeñador_FSM.actualState.Name);
                barraProgreso.GetComponent<Slider>().value = 0;
                barraProgreso.SetActive(false);
                parcelaDestino.GetComponent<ParcelaScript>().libre = false;
                parcelaDestino.GetComponent<ParcelaScript>().plantada = true;
                parcelaDestino.GetComponent<MeshRenderer>().material = parcelaDestino.GetComponent<ParcelaScript>().meshPlantado;
                fsmAgricultor_FSM.Fire("PararPlantar");
                //Debug.Log(fsmAgricultor_FSM.actualState.Name);
                plantando = false;
            }
        }

        if (cosechando && (int)navMesh.destination.x == (int)transform.position.x && (int)navMesh.destination.z == (int)transform.position.z)
        {
            barraProgreso.GetComponent<Slider>().value += Time.deltaTime * 0.5f;
            if (barraProgreso.GetComponent<Slider>().value == barraProgreso.GetComponent<Slider>().maxValue)
            {
                //Debug.Log("Termine de cosechar");
                gameManager.pasto += 2;
                barraProgreso.GetComponent<Slider>().value = 0;
                barraProgreso.SetActive(false);
                parcelaDestino.GetComponent<ParcelaScript>().cosechable = false;
                parcelaDestino.GetComponent<ParcelaScript>().libre = true;
                parcelaDestino.GetComponent<MeshRenderer>().material = parcelaDestino.GetComponent<ParcelaScript>().meshLibre;
                fsmAgricultor_FSM.Fire("PararCosechar");
                //Debug.Log(fsmAgricultor_FSM.actualState.Name);
                cosechando = false;
            }
        }
        if (fsmAgricultor_FSM.actualState.Name == "BuscarCampo")
        {
            BuscarCampoAction();
        }
        fsmAgricultor_FSM.Update();
    }

    protected void LateUpdate()
    {
        transform.localEulerAngles = new Vector3(-90, transform.localEulerAngles.y, 0);
    }
    // Create your desired actions

    private void BuscarCampoAction()
    {
        if (gameManager.hay_comida && hambre > 70)
        {
            fsmAgricultor_FSM.Fire("HayHambreYComida");
        }
        else if (gameManager.hay_leche && sed > 70)
        {
            fsmAgricultor_FSM.Fire("HaySedYLeche");
        }
        else
        {
            bool todoPlantado = true;
            foreach (GameObject parcela in casa.GetComponent<casaAgricultorController>().parcelas)
            {
                if (parcela.GetComponent<ParcelaScript>().libre)
                {
                    parcelaDestino = parcela;
                    navMesh.destination = parcelaDestino.transform.GetChild(0).position;
                    todoPlantado = false;
                    StartCoroutine("timer", "Plantar");
                    break;
                }
            }
            if (todoPlantado)
            {
                foreach (GameObject parcela in casa.GetComponent<casaAgricultorController>().parcelas)
                {
                    if (parcela.GetComponent<ParcelaScript>().cosechable)
                    {
                        parcelaDestino = parcela;
                        navMesh.destination = parcelaDestino.transform.GetChild(0).position;
                        //StartCoroutine("timer", "Cosechar");
                        fsmAgricultor_FSM.Fire("Cosechar");
                        break;
                    }
                }
            }
        }
        
        
    }
    
    private void PlantandoAction()
    {
        //Debug.Log("plantando");
        plantando = true;
    }
    
    private void CosechandoAction()
    {
        //Debug.Log("cosechando");
        cosechando = true;
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

    public IEnumerator timer(string estado)
    {
        //Debug.Log("Timer iniciado");
        yield return new WaitForSeconds(0.5f);

        fsmAgricultor_FSM.Fire(estado);
    }

    public IEnumerator ComerTimer()
    {
        yield return new WaitForSeconds(2);
        hambre = 0;
        gameManager.comida--;
        fsmAgricultor_FSM.Fire("HambreSaciada");
    }

    public IEnumerator BeberTimer()
    {
        yield return new WaitForSeconds(2);
        sed = 0;
        gameManager.leche--;
        fsmAgricultor_FSM.Fire("SedSaciada");
    }
}