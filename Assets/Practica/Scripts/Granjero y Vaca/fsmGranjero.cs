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
    private PushPerception HayHambreYComidaPerception;
    private PushPerception HaySedYLechePerception;
    private PushPerception NecesidadSaciadaPerception;
    private State Comiendo;
    private State Bebiendo;
    private State Esperando;
    private State RellenarComedero;
    private State OrdeñarVaca;

    //Place your variables here

    public CorralController corralSuyo;
    [SerializeField] private NavMeshAgent navMesh;
    private GameObject vacaAOrdeñar;
    public GameManagerScript gameManager;
    public int pastoRecogido;
    public bool vacaLista;
    public bool pastoTomado;

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
        fsmGranjero_FSM = new StateMachineEngine(false);
        gameManager = FindObjectOfType<GameManagerScript>();

        hambre = 0;
        sed = 0;
        ratioGananciaHambre = Random.Range(0.1f, 0.3f);
        ratioGananciaSed = Random.Range(0.3f, 0.6f);

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
        HayHambreYComidaPerception = fsmGranjero_FSM.CreatePerception<PushPerception>();
        HaySedYLechePerception = fsmGranjero_FSM.CreatePerception<PushPerception>();
        NecesidadSaciadaPerception = fsmGranjero_FSM.CreatePerception<PushPerception>();

        // States
        Esperando = fsmGranjero_FSM.CreateEntryState("Esperando", EsperandoAction);
        RellenarComedero = fsmGranjero_FSM.CreateState("Rellenar Comedero", RellenarComederoAction);
        OrdeñarVaca = fsmGranjero_FSM.CreateState("Ordeñar Vaca", OrdeñarVacaAction);
        Bebiendo = fsmGranjero_FSM.CreateState("Bebiendo", BebiendoAction);
        Comiendo = fsmGranjero_FSM.CreateState("Comiendo", ComiendoAction);

        // Transitions
        fsmGranjero_FSM.CreateTransition("ComederoVacio", Esperando, ComederoVacioPerception, RellenarComedero);
        fsmGranjero_FSM.CreateTransition("ComederoHaSidoLlenado", RellenarComedero, ComederoHaSidoLlenadoPerception, Esperando);
        fsmGranjero_FSM.CreateTransition("VacaHaSidoOrdeñada", OrdeñarVaca, VacaHaSidoOrdeñadaPerception, Esperando);
        fsmGranjero_FSM.CreateTransition("VacaOrdeñable", Esperando, VacaOrdeñablePerception, OrdeñarVaca);
        fsmGranjero_FSM.CreateTransition("HayHambreYComida", Esperando, HayHambreYComidaPerception, Comiendo);
        fsmGranjero_FSM.CreateTransition("HaySedYLeche", Esperando, HaySedYLechePerception, Bebiendo);
        fsmGranjero_FSM.CreateTransition("HambreSaciada", Comiendo, NecesidadSaciadaPerception, Esperando);
        fsmGranjero_FSM.CreateTransition("SedSaciada", Bebiendo, NecesidadSaciadaPerception, Esperando);
    }
    
    private void Update()
    {
        if (fsmGranjero_FSM.actualState != Comiendo && fsmGranjero_FSM.actualState != Bebiendo)
        {
            sed += Time.deltaTime * ratioGananciaSed;
            hambre += Time.deltaTime * ratioGananciaHambre;
        }
        if (fsmGranjero_FSM.actualState == Comiendo &&
            (int)transform.position.x == (int)gameManager.restaurantePlace.position.x &&
            (int)transform.position.z == (int)gameManager.restaurantePlace.position.z && !estaComiendo && gameManager.hay_comida)
        {
            estaComiendo = true;
            StartCoroutine(ComerTimer());
        }
        if (fsmGranjero_FSM.actualState == Bebiendo &&
            (int)transform.position.x == (int)gameManager.restaurantePlace.position.x &&
            (int)transform.position.z == (int)gameManager.restaurantePlace.position.z && !estaBebiendo && gameManager.hay_leche)
        {
            estaBebiendo = true;
            StartCoroutine(BeberTimer());
        }
        if (fsmGranjero_FSM.actualState == Esperando)
        {
            if (gameManager.hay_comida && hambre > 70)
            {
                fsmGranjero_FSM.Fire("HayHambreYComida");
            }
            else if (gameManager.hay_leche && sed > 70)
            {
                fsmGranjero_FSM.Fire("HaySedYLeche");
            }
            else if (corralSuyo.pasto <= 0)
            {
                fsmGranjero_FSM.Fire("ComederoVacio");
            }
            else
            {
                foreach (GameObject vaca in corralSuyo.vacas)
                {
                    if (vaca.GetComponent<fsmVaca>().puedeSerOrdeñada)
                    {
                        //Debug.Log("Hay una vaca para ordeñar");
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
        }
        if(fsmGranjero_FSM.actualState == RellenarComedero)
        {
            if((int)transform.position.x == (int)gameManager.almacenDropPlace.position.x && (int)transform.position.z == (int)gameManager.almacenDropPlace.position.z && !pastoTomado)
            {
                pastoTomado = true;
                pastoRecogido = gameManager.pasto / 2;
                gameManager.pasto -= pastoRecogido;
                navMesh.destination = corralSuyo.lugarComer.position;
            }
            if((int)transform.position.x == (int)corralSuyo.lugarComer.position.x && (int)transform.position.z == (int)corralSuyo.lugarComer.position.z)
            {
                corralSuyo.AddPasto(pastoRecogido);
                fsmGranjero_FSM.Fire("ComederoHaSidoLlenado");
            }
        }
        if(fsmGranjero_FSM.actualState == OrdeñarVaca)
        {
            if((int)corralSuyo.lugarOrdeñoGranjero.position.x == (int)transform.position.x && (int)corralSuyo.lugarOrdeñoGranjero.position.z == (int)transform.position.z)
            {
                if (vacaLista)
                {
                    vacaAOrdeñar.GetComponent<fsmVaca>().siendoOrdeñada = true;
                    vacaLista = false;
                    vacaAOrdeñar.GetComponent<fsmVaca>().fsmVaca_FSM.Fire("GranjeroOrdeñando");
                    StartCoroutine(OrdeñarTimer());
                }
            }
        }
        fsmGranjero_FSM.Update();
    }

    protected void LateUpdate()
    {
        transform.localEulerAngles = new Vector3(-90, transform.localEulerAngles.y, 0);
    }

    // Create your desired actions

    private void EsperandoAction()
    {
        //Debug.Log("Entro a esperar");
        navMesh.destination = corralSuyo.lugarEsperaGranjero.position;
    }
    
    private void RellenarComederoAction()
    {
        navMesh.destination = gameManager.almacenDropPlace.position;
        pastoTomado = false;
    }
    
    private void OrdeñarVacaAction()
    {
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

    public IEnumerator OrdeñarTimer()
    {
        yield return new WaitForSeconds(5);
        gameManager.leche += 2;
        fsmGranjero_FSM.Fire("VacaHaSidoOrdeñada");
    }

    public IEnumerator ComerTimer()
    {
        yield return new WaitForSeconds(2);
        hambre = 0;
        gameManager.comida--;
        fsmGranjero_FSM.Fire("HambreSaciada");
    }

    public IEnumerator BeberTimer()
    {
        yield return new WaitForSeconds(2);
        sed = 0;
        gameManager.leche--;
        fsmGranjero_FSM.Fire("SedSaciada");
    }
}