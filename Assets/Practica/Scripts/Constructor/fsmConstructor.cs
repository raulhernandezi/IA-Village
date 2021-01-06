using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class fsmConstructor : MonoBehaviour {

    #region variables

    private StateMachineEngine fsmConstructor_FSM;
    

    private PushPerception ConstruccionFinalizadaPerception;
    private PushPerception HayMaderaYCimientosPerception;
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

    //Variable temporal
    public float madera;
    public int ratioCrecimientoMadera = 1;

    #endregion variables

    // Start is called before the first frame update
    private void Start()
    {
        fsmConstructor_FSM = new StateMachineEngine(false);
        gameManager = FindObjectOfType<GameManagerScript>();
        madera = 0;
        cimientosController = FindObjectOfType<CimientosControler>();

        CreateStateMachine();
    }
    
    
    private void CreateStateMachine()
    {
        // Perceptions
        // Modify or add new Perceptions, see the guide for more
        ConstruccionFinalizadaPerception = fsmConstructor_FSM.CreatePerception<PushPerception>();
        HayMaderaYCimientosPerception = fsmConstructor_FSM.CreatePerception<PushPerception>();
        
        // States
        Esperando = fsmConstructor_FSM.CreateEntryState("Esperando", EsperandoAction);
        Construyendo = fsmConstructor_FSM.CreateState("Construyendo", ConstruyendoAction);
        
        // Transitions
        fsmConstructor_FSM.CreateTransition("HayMaderaYCimientos", Esperando, HayMaderaYCimientosPerception, Construyendo);
        fsmConstructor_FSM.CreateTransition("ConstruccionFinalizada", Construyendo, ConstruccionFinalizadaPerception, Esperando);
        
    }


    private void Update()
    {
        if(fsmConstructor_FSM.actualState == Esperando)
        {
            //Aqui deberia estar comprobando si hay mas de 20 de madera en el gamemanager, pero como de momento no hay, voy a simularlo
            //haciendo una variable madera que vaya creciendo poco a poco con el tiempo
            //madera += ratioCrecimientoMadera * Time.deltaTime;
            if(madera >= 20)
            {
                for(int i = 0; i < cimientosController.cimientos.Length; i++)
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
        if(fsmConstructor_FSM.actualState == Construyendo)
        {
            if((int)navMesh.destination.x == (int)transform.position.x && (int)navMesh.destination.z == (int)transform.position.z && llegando)
            {
                llegando = false;
                Debug.Log("Comienzo a construir");
                StartCoroutine(ConstruirTimer());
            }
        }
        fsmConstructor_FSM.Update();
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

    public IEnumerator ConstruirTimer()
    {
        yield return new WaitForSeconds(5);
        //Aqui deberia o bien decidir de que oficio hay menos y en ese caso construir esa casa concreta
        int random = Random.Range(0, 3);
        Debug.Log("Casa construida");
        Instantiate(prefabsCasas[random], lugarDondeConstruir.position, lugarDondeConstruir.rotation);
        gameManager.madera -= 20;
        fsmConstructor_FSM.Fire("ConstruccionFinalizada");
    }
    
}