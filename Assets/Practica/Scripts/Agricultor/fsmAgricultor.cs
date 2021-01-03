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
    private State BuscarCampo;
    private State Plantando;
    private State Cosechando;

    //Place your variables here
    public GameObject casa;
    private GameObject parcelaDestino;
    private NavMeshAgent nmesh;
    [SerializeField] private GameObject barraProgreso;

    private bool plantando;
    private bool cosechando;
    #endregion variables

    
    // Start is called before the first frame update
    private void Start()
    {
        fsmAgricultor_FSM = new StateMachineEngine(false);

        nmesh = GetComponent<NavMeshAgent>();

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
        // States
        BuscarCampo = fsmAgricultor_FSM.CreateEntryState("BuscarCampo", BuscarCampoAction);
        Plantando = fsmAgricultor_FSM.CreateState("Plantando", PlantandoAction);
        Cosechando = fsmAgricultor_FSM.CreateState("Cosechando", CosechandoAction);
        // Transitions
        fsmAgricultor_FSM.CreateTransition("Plantar", BuscarCampo, PlantarPerception, Plantando);
        fsmAgricultor_FSM.CreateTransition("PararPlantar", Plantando, PararPlantarPerception, BuscarCampo);
        fsmAgricultor_FSM.CreateTransition("PararCosechar", Cosechando, PararCosecharPerception, BuscarCampo);
        fsmAgricultor_FSM.CreateTransition("Cosechar", BuscarCampo, CosecharPerception, Cosechando);
        // ExitPerceptions

        // ExitTransitions

    }

    // Update is called once per frame
    private void Update()
    {
        if (plantando && nmesh.destination.x == transform.position.x && nmesh.destination.z == transform.position.z)
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
                Debug.Log(fsmAgricultor_FSM.actualState.Name);
                plantando = false;
            }
        }

        if (cosechando && nmesh.destination.x == transform.position.x && nmesh.destination.z == transform.position.z)
        {
            barraProgreso.GetComponent<Slider>().value += Time.deltaTime * 0.5f;
            if (barraProgreso.GetComponent<Slider>().value == barraProgreso.GetComponent<Slider>().maxValue)
            {
                //Debug.Log(owner.GetComponent<fsmLeñador>().fsmLeñador_FSM.actualState.Name);
                barraProgreso.GetComponent<Slider>().value = 0;
                barraProgreso.SetActive(false);
                parcelaDestino.GetComponent<ParcelaScript>().cosechable = false;
                parcelaDestino.GetComponent<ParcelaScript>().libre = true;
                parcelaDestino.GetComponent<MeshRenderer>().material = parcelaDestino.GetComponent<ParcelaScript>().meshLibre;
                fsmAgricultor_FSM.Fire("PararCosechar");
                Debug.Log(fsmAgricultor_FSM.actualState.Name);
                cosechando = false;
            }
        }
        if (fsmAgricultor_FSM.actualState.Name == "BuscarCampo")
        {
            BuscarCampoAction();
        }
        fsmAgricultor_FSM.Update();
    }

    // Create your desired actions
    
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
                    //StartCoroutine("timer", "Cosechar");
                    fsmAgricultor_FSM.Fire("Cosechar");
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

        fsmAgricultor_FSM.Fire(estado);
    }
}