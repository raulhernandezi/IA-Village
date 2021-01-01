using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class fsmGranjero : MonoBehaviour {

    #region variables

    private StateMachineEngine fsmGranjero_FSM;
    

    private PushPerception ComederoVacioPerception;
    private PushPerception ComederoHaSidoLlenadoPerception;
    private PushPerception VacaHaSidoOrdeñadaPerception;
    private PushPerception VacaOrdeñablePerception;
    private State Esperando;
    private State RellenarComedero;
    private State OrdeñarVaca;

    //Place your variables here

    public CorralController corralSuyo;
    [SerializeField] private NavMeshAgent navMesh;
    private GameObject vacaAOrdeñar;

    public bool vacaLista;

    #endregion variables

    // Start is called before the first frame update
    private void Start()
    {
        fsmGranjero_FSM = new StateMachineEngine(false);

        CreateStateMachine();
    }
    
    
    private void CreateStateMachine()
    {
        // Perceptions
        // Modify or add new Perceptions, see the guide for more
        ComederoVacioPerception = fsmGranjero_FSM.CreatePerception<PushPerception>();
        ComederoHaSidoLlenadoPerception = fsmGranjero_FSM.CreatePerception<PushPerception>();
        VacaHaSidoOrdeñadaPerception = fsmGranjero_FSM.CreatePerception<PushPerception>();
        VacaOrdeñablePerception = fsmGranjero_FSM.CreatePerception<PushPerception>();
        
        // States
        Esperando = fsmGranjero_FSM.CreateEntryState("Esperando", EsperandoAction);
        RellenarComedero = fsmGranjero_FSM.CreateState("Rellenar Comedero", RellenarComederoAction);
        OrdeñarVaca = fsmGranjero_FSM.CreateState("Ordeñar Vaca", OrdeñarVacaAction);
        
        // Transitions
        fsmGranjero_FSM.CreateTransition("ComederoVacio", Esperando, ComederoVacioPerception, RellenarComedero);
        fsmGranjero_FSM.CreateTransition("ComederoHaSidoLlenado", RellenarComedero, ComederoHaSidoLlenadoPerception, Esperando);
        fsmGranjero_FSM.CreateTransition("VacaHaSidoOrdeñada", OrdeñarVaca, VacaHaSidoOrdeñadaPerception, Esperando);
        fsmGranjero_FSM.CreateTransition("VacaOrdeñable", Esperando, VacaOrdeñablePerception, OrdeñarVaca);
    }


    private void Update()
    {
        if(fsmGranjero_FSM.actualState == Esperando)
        {
            foreach(GameObject vaca in corralSuyo.vacas)
            {
                if (vaca.GetComponent<fsmVaca>().puedeSerOrdeñada)
                {
                    Debug.Log("Hay una vaca para ordeñar");
                    vacaAOrdeñar = vaca;
                    fsmGranjero_FSM.Fire("VacaOrdeñable");
                    vacaAOrdeñar.GetComponent<fsmVaca>().MoverASitioOrdeño();
                    navMesh.destination = new Vector3(
                        corralSuyo.lugarOrdeñoGranjero.position.x, 
                        transform.position.y, 
                        corralSuyo.lugarOrdeñoGranjero.position.z);
                    break;
                }
            }
        }
        if(fsmGranjero_FSM.actualState == OrdeñarVaca)
        {
            if(navMesh.destination.x == transform.position.x && navMesh.destination.z == transform.position.z)
            {
                if (vacaLista)
                {
                    vacaLista = false;
                    vacaAOrdeñar.GetComponent<fsmVaca>().fsmVaca_FSM.Fire("GranjeroOrdeñando");
                    StartCoroutine(OrdeñarTimer());
                }
            }
        }
        fsmGranjero_FSM.Update();
    }

    // Create your desired actions
    
    private void EsperandoAction()
    {
        Debug.Log("Entro a esperar");
        navMesh.destination = corralSuyo.lugarEsperaGranjero.position;
    }
    
    private void RellenarComederoAction()
    {
        //Aqui falta hacer que detecte en el Update cuando hay poca comida en el comedero y que vaya al almacen a recoger todo el PASTO
        //que haya para traerlo al comedero
    }
    
    private void OrdeñarVacaAction()
    {
    }
    
    public IEnumerator OrdeñarTimer()
    {
        yield return new WaitForSeconds(5);
        //
        //Aqui hay que añadir que sume 1 a la leche del pueblo
        //
        Debug.Log("Añado 1 a la LECHE rica");
        fsmGranjero_FSM.Fire("VacaHaSidoOrdeñada");
    }
}