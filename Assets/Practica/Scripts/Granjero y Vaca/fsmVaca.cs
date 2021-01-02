using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class fsmVaca : MonoBehaviour {

    #region variables

    public StateMachineEngine fsmVaca_FSM;
    

    private PushPerception TengoHambreYHayPastoPerception;
    private PushPerception PuedeReproducirsePerception;
    private PushPerception ReproduccionTerminadaPerception;
    private PushPerception HaComidoPerception;
    private PushPerception PuedeSerOrdeñadaPerception;
    private PushPerception GranjeroOrdeñandoPerception;
    private TimerPerception OrdeñoTerminadoPerception;
    private State Esperando;
    private State Comer;
    private State Reproducirse;
    private State Esperaraserordeñada;
    private State Ordeñando;

    //Place your variables here

    public CorralController corral;
    public NavMeshAgent navMesh;

    public bool comiendo;

    public float hambre;
    public int bienAlimentada;
    public int ordeñable;
    private float ratioPerdidaHambrePorSegundo = 10;

    public bool puedeSerOrdeñada;

    public Vector3 posicionRandom;

    #endregion variables

    // Start is called before the first frame update
    private void Start()
    {
        fsmVaca_FSM = new StateMachineEngine(false);
        navMesh = GetComponent<NavMeshAgent>();

        ordeñable = 0;
        hambre = 100;
        bienAlimentada = 0;

        CreateStateMachine();
    }
    
    
    private void CreateStateMachine()
    {
        // Perceptions
        // Modify or add new Perceptions, see the guide for more
        TengoHambreYHayPastoPerception = fsmVaca_FSM.CreatePerception<PushPerception>();
        PuedeReproducirsePerception = fsmVaca_FSM.CreatePerception<PushPerception>();
        ReproduccionTerminadaPerception = fsmVaca_FSM.CreatePerception<PushPerception>();
        HaComidoPerception = fsmVaca_FSM.CreatePerception<PushPerception>();
        PuedeSerOrdeñadaPerception = fsmVaca_FSM.CreatePerception<PushPerception>();
        GranjeroOrdeñandoPerception = fsmVaca_FSM.CreatePerception<PushPerception>();
        OrdeñoTerminadoPerception = fsmVaca_FSM.CreatePerception<TimerPerception>(5f);
        
        // States
        Esperando = fsmVaca_FSM.CreateEntryState("Esperando", EsperandoAction);
        Comer = fsmVaca_FSM.CreateState("Comer", ComerAction);
        Reproducirse = fsmVaca_FSM.CreateState("Reproducirse", ReproducirseAction);
        Esperaraserordeñada = fsmVaca_FSM.CreateState("Esperar a ser ordeñada", EsperaraserordeñadaAction);
        Ordeñando = fsmVaca_FSM.CreateState("Ordeñando", OrdeñandoAction);
        
        // Transitions
        fsmVaca_FSM.CreateTransition("TengoHambreYHayPasto", Esperando, TengoHambreYHayPastoPerception, Comer);
        fsmVaca_FSM.CreateTransition("PuedeReproducirse", Comer, PuedeReproducirsePerception, Reproducirse);
        fsmVaca_FSM.CreateTransition("ReproduccionTerminada", Reproducirse, ReproduccionTerminadaPerception, Esperando);
        fsmVaca_FSM.CreateTransition("HaComido", Comer, HaComidoPerception, Esperando);
        fsmVaca_FSM.CreateTransition("PuedeSerOrdeñada", Comer, PuedeSerOrdeñadaPerception, Esperaraserordeñada);
        fsmVaca_FSM.CreateTransition("GranjeroOrdeñando", Esperaraserordeñada, GranjeroOrdeñandoPerception, Ordeñando);
        fsmVaca_FSM.CreateTransition("OrdeñoTerminado", Ordeñando, OrdeñoTerminadoPerception, Esperando);
        
        // ExitPerceptions
        
        // ExitTransitions
        
    }

    // Update is called once per frame
    private void Update()
    {
        hambre -= ratioPerdidaHambrePorSegundo * Time.deltaTime;
        if(hambre <= 30 && fsmVaca_FSM.actualState == Esperando)
        {
            if(corral.pasto >= 1)
            {
                fsmVaca_FSM.Fire("TengoHambreYHayPasto");
            }
        }
        if(fsmVaca_FSM.actualState == Comer && navMesh.destination.x == transform.position.x && navMesh.destination.z == transform.position.z)
        {
            if (comiendo)
            {
                comiendo = false;
                StartCoroutine(ComerTimer());
            }
        }
        if(fsmVaca_FSM.actualState == Esperaraserordeñada && navMesh.destination.x == transform.position.x && navMesh.destination.z == transform.position.z)
        {
            corral.propietario.GetComponent<fsmGranjero>().vacaLista = true;
        }
        fsmVaca_FSM.Update();
    }

    // Create your desired actions
    
    private void EsperandoAction()
    {
        puedeSerOrdeñada = false;
        posicionRandom = Random.insideUnitSphere * 12;
        navMesh.destination = new Vector3(corral.transform.position.x + posicionRandom.x, transform.position.y, corral.transform.position.z + posicionRandom.z);
        StartCoroutine(MovimientoRandom());
    }
    
    private void ComerAction()
    {
        navMesh.destination = new Vector3(corral.comedero.transform.position.x, transform.position.y, corral.comedero.transform.position.z + 4);
        comiendo = true;
    }
    
    private void ReproducirseAction()
    {
        corral.CrearVaca();
        fsmVaca_FSM.Fire("ReproduccionTerminada");
    }
    
    private void EsperaraserordeñadaAction()
    {
        Debug.Log("Espero a ser ordeñada");
        puedeSerOrdeñada = true;
        posicionRandom = Random.insideUnitSphere * 12;
        navMesh.destination = new Vector3(corral.transform.position.x + posicionRandom.x, transform.position.y, corral.transform.position.z + posicionRandom.z);
    }
    
    private void OrdeñandoAction()
    {
        Debug.Log("Me ordeñan");
        StartCoroutine(OrdeñarTimer());
    }
    
    public IEnumerator MovimientoRandom()
    {
        yield return new WaitForSeconds(7);
        if (fsmVaca_FSM.actualState == Esperando)
        {
            posicionRandom = Random.insideUnitSphere * 3;
            navMesh.destination = new Vector3(transform.position.x + posicionRandom.x, transform.position.y, transform.position.z + posicionRandom.z);
            StartCoroutine(MovimientoRandom());
        }
    }

    public IEnumerator ComerTimer()
    {
        yield return new WaitForSeconds(3);
        hambre = 100;
        //ordeñable += Random.Range(10, 30);
        ordeñable += 100;
        bienAlimentada += 0; //Deberia ser 1
        if(ordeñable >= 100)
        {
            fsmVaca_FSM.Fire("PuedeSerOrdeñada");
            ordeñable = 0;
        }
        else if(bienAlimentada == 4)
        {
            if(corral.vacas.Count < 10)
            {
                fsmVaca_FSM.Fire("PuedeReproducirse");
            }
            else
            {
                fsmVaca_FSM.Fire("HaComido");
            }
            bienAlimentada = 0;
        }
        else
        {
            fsmVaca_FSM.Fire("HaComido");
        }
    }

    public IEnumerator OrdeñarTimer()
    {
        yield return new WaitForSeconds(5);
        fsmVaca_FSM.Fire("OrdeñoTerminado");
    }

    public void MoverASitioOrdeño()
    {
        navMesh.destination = new Vector3(corral.lugarOrdeñoVaca.position.x, transform.position.y, corral.lugarOrdeñoVaca.position.z);
    }

}