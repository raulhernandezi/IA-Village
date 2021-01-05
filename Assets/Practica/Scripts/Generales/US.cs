using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class US : MonoBehaviour
{

    #region variables

    private UtilitySystemEngine US_US;
    private StateMachineEngine trabajarfsm_SubFSM;

    private PushPerception PlantarPerception;
    private PushPerception PararPlantarPerception;
    private PushPerception PararCosecharPerception;
    private PushPerception CosecharPerception;
    private State BuscarCampo;
    private State Plantando;
    private State Cosechando;

    private Factor fusionComer;
    private Factor fusionBeber;
    private Factor tienehambre;
    private Factor haycomida;
    private Factor hayleche;
    private Factor tienesed;
    private Factor trabajar;
    private Factor fusionTrabajar;
    private UtilityAction comer;
    private UtilityAction beber;
    private UtilityAction trabajarfsm;
    private State NewState;

    //Place your variables here
    private bool fsmCreada;
    private bool usCreado;


    public float hambre;
    public float sed;
    public float hay_hambre;
    public float hay_sed;
    public float seTrabaja;

    private bool comiendo;
    private bool bebiendo;
    private NavMeshAgent nmesh;

    private GameManagerScript gms;


    private Transform restaurante;
    [SerializeField] private GameObject barraProgreso;


    public GameObject casa;
    private GameObject parcelaDestino;

    private bool plantando;
    private bool cosechando;
    #endregion variables

    /*private void Awake()
    {
        trabajarfsm_SubFSM = GetComponent<fsmAgricultor>().fsmAgricultor_FSM;
    }*/
    // Start is called before the first frame update
    private void Start()
    {
        hambre = 50;
        sed = 0;
        hay_hambre = 0;
        hay_sed = 0;
        seTrabaja = 0.65f;



        gms = FindObjectOfType<GameManagerScript>();

        restaurante = gms.restaurantePlace;

        trabajarfsm_SubFSM = new StateMachineEngine(false);
        US_US = new UtilitySystemEngine(false);

        nmesh = GetComponent<NavMeshAgent>();



        Createtrabajarfsm_SubFSM();
        CreateUtilitySystem();

    }

    private void Createtrabajarfsm_SubFSM()
    {
        // Perceptions
        // Modify or add new Perceptions, see the guide for more
        PlantarPerception = trabajarfsm_SubFSM.CreatePerception<PushPerception>();
        PararPlantarPerception = trabajarfsm_SubFSM.CreatePerception<PushPerception>();
        PararCosecharPerception = trabajarfsm_SubFSM.CreatePerception<PushPerception>();
        CosecharPerception = trabajarfsm_SubFSM.CreatePerception<PushPerception>();
        // States
        BuscarCampo = trabajarfsm_SubFSM.CreateEntryState("BuscarCampo", BuscarCampoAction);
        Plantando = trabajarfsm_SubFSM.CreateState("Plantando", PlantandoAction);
        Cosechando = trabajarfsm_SubFSM.CreateState("Cosechando", CosechandoAction);
        // Transitions
        trabajarfsm_SubFSM.CreateTransition("Plantar", BuscarCampo, PlantarPerception, Plantando);
        trabajarfsm_SubFSM.CreateTransition("PararPlantar", Plantando, PararPlantarPerception, BuscarCampo);
        trabajarfsm_SubFSM.CreateTransition("PararCosechar", Cosechando, PararCosecharPerception, BuscarCampo);
        trabajarfsm_SubFSM.CreateTransition("Cosechar", BuscarCampo, CosecharPerception, Cosechando);
        // ExitPerceptions

        // ExitTransitions
        fsmCreada = true;
    }

    private void CreateUtilitySystem()
    {

        tienehambre = new LeafVariable(() => hay_hambre, 1, 0);
        haycomida = new LeafVariable(() => gms.hay_comida, 1, 0);
        hayleche = new LeafVariable(() => gms.hay_leche, 1, 0);
        tienesed = new LeafVariable(() => hay_sed, 1, 0);
        trabajar = new LeafVariable(() => seTrabaja, 0.65f, 0.65f);

        // FACTORS
        List<Factor> fusionComerFactors = new List<Factor>
        {
            tienehambre,
            haycomida,
        };

        List<System.Single> fusionComerWeights = new List<System.Single>
        {
            0.6f,
            0.4f,
        };

        fusionComer = new WeightedSumFusion(fusionComerFactors, fusionComerWeights);
        List<Factor> fusionBeberFactors = new List<Factor>
        {
            hayleche,
            tienesed,
        };

        List<System.Single> fusionBeberWeights = new List<System.Single>
        {
            0.4f,
            0.6f,
        };

        fusionBeber = new WeightedSumFusion(fusionBeberFactors, fusionBeberWeights);


        List<Factor> fusionTrabajarFactors = new List<Factor>
        {
            //fusionComer,
            //fusionBeber,
            trabajar
        };


        fusionTrabajar = new MaxFusion(fusionTrabajarFactors);

        // ACTIONS
        comer = US_US.CreateUtilityAction("comer", comerAction, fusionComer);
        beber = US_US.CreateUtilityAction("beber", beberAction, fusionBeber);
        trabajarfsm = US_US.CreateSubBehaviour("trabajar_fsm", fusionTrabajar, trabajarfsm_SubFSM);


        // ExitPerceptions

        // ExitTransitions
        //trabajarfsm_SubFSM.CreateExitTransition("trabajarfsm_SubFSM Exit", trabajarfsm_SubFSM.GetState("BuscarCampo"), null, US_US);
        usCreado = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (fsmCreada && usCreado)
        {
            if (hambre > 70)
            {
                hay_hambre = 1f;
            }
            else
            {
                hay_hambre = 0f;
            }

            if (sed > 70)
            {
                hay_sed = 1f;
            }
            else
            {
                hay_sed = 0f;
            }

            Debug.Log("Comiendo" + comiendo);
            Debug.Log("Bebiendo" + bebiendo);
            if (comiendo && (int)nmesh.destination.x == (int)transform.position.x && (int)nmesh.destination.z == (int)transform.position.z)
            {
                barraProgreso.GetComponent<Slider>().value += Time.deltaTime * 0.5f;
                if (barraProgreso.GetComponent<Slider>().value == barraProgreso.GetComponent<Slider>().maxValue)
                {
                    Debug.Log("Acabo de comer");
                    barraProgreso.GetComponent<Slider>().value = 0;
                    //barraProgreso.SetActive(false);
                    comiendo = false;
                    hambre = 0;
                    gms.comida--;
                }
            }

            if (bebiendo && (int)nmesh.destination.x == (int)transform.position.x && (int)nmesh.destination.z == (int)transform.position.z)
            {
                barraProgreso.GetComponent<Slider>().value += Time.deltaTime * 0.5f;
                if (barraProgreso.GetComponent<Slider>().value == barraProgreso.GetComponent<Slider>().maxValue)
                {
                    Debug.Log("Acabo de comer");
                    barraProgreso.GetComponent<Slider>().value = 0;
                    //barraProgreso.SetActive(false);
                    bebiendo = false;
                    sed = 0;
                    gms.leche--;
                }
            }

            if (plantando && (int)nmesh.destination.x == (int)transform.position.x && (int)nmesh.destination.z == (int)transform.position.z)
            {
                barraProgreso.GetComponent<Slider>().value += Time.deltaTime * 0.5f;
                if (barraProgreso.GetComponent<Slider>().value == barraProgreso.GetComponent<Slider>().maxValue)
                {
                    //Debug.Log(owner.GetComponent<fsmLeñador>().fsmLeñador_FSM.actualState.Name);
                    barraProgreso.GetComponent<Slider>().value = 0;
                    //barraProgreso.SetActive(false);
                    parcelaDestino.GetComponent<ParcelaScript>().libre = false;
                    parcelaDestino.GetComponent<ParcelaScript>().plantada = true;
                    parcelaDestino.GetComponent<MeshRenderer>().material = parcelaDestino.GetComponent<ParcelaScript>().meshPlantado;
                    trabajarfsm_SubFSM.Fire("PararPlantar");
                    //Debug.Log(trabajarfsm_SubFSM.actualState.Name);
                    plantando = false;
                }
            }

            if (cosechando && (int)nmesh.destination.x == (int)transform.position.x && (int)nmesh.destination.z == (int)transform.position.z)
            {
                barraProgreso.GetComponent<Slider>().value += Time.deltaTime * 0.5f;
                if (barraProgreso.GetComponent<Slider>().value == barraProgreso.GetComponent<Slider>().maxValue)
                {
                    //Debug.Log(owner.GetComponent<fsmLeñador>().fsmLeñador_FSM.actualState.Name);
                    barraProgreso.GetComponent<Slider>().value = 0;
                    //barraProgreso.SetActive(false);
                    parcelaDestino.GetComponent<ParcelaScript>().cosechable = false;
                    parcelaDestino.GetComponent<ParcelaScript>().libre = true;
                    parcelaDestino.GetComponent<MeshRenderer>().material = parcelaDestino.GetComponent<ParcelaScript>().meshLibre;
                    trabajarfsm_SubFSM.Fire("PararCosechar");
                    //Debug.Log(trabajarfsm_SubFSM.actualState.Name);
                    cosechando = false;
                    gms.pasto++;
                }
            }
            if (!comiendo && !bebiendo)
            {
                hambre += 0.5f * Time.deltaTime;
                sed += 0.5f * Time.deltaTime;
            }

            if (trabajarfsm_SubFSM.actualState.Name == "BuscarCampo")
            {
                BuscarCampoAction();
            }
            US_US.Update();
            trabajarfsm_SubFSM.Update();
        }

    }

    // Create your desired actions

    private void comerAction()
    {
        comiendo = true;
        nmesh.destination = restaurante.transform.position;
    }

    private void beberAction()
    {
        bebiendo = true;
        nmesh.destination = restaurante.transform.position;
    }

    private void BuscarCampoAction()
    {
        bool todoPlantado = true;
        foreach (GameObject parcela in casa.GetComponent<casaAgricultorController>().parcelas)
        {
            if (parcela.GetComponent<ParcelaScript>().libre)
            {
                parcelaDestino = parcela;
                nmesh.destination = parcelaDestino.transform.GetChild(0).position;
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
                    nmesh.destination = parcelaDestino.transform.GetChild(0).position;
                    trabajarfsm_SubFSM.Fire("Cosechar");
                    break;
                }
            }
        }

    }

    private void PlantandoAction()
    {
        Debug.Log("plantando");
        plantando = true;
    }

    private void CosechandoAction()
    {
        Debug.Log("cosechando");
        cosechando = true;
    }

    public IEnumerator timer(string estado)
    {
        //Debug.Log("Timer iniciado");
        yield return new WaitForSeconds(0.5f);

        trabajarfsm_SubFSM.Fire(estado);
    }

}