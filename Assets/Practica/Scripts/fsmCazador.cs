using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class fsmCazador : MonoBehaviour {

    #region variables

    private StateMachineEngine fsmCazador_FSM;
    

    private PushPerception AnimalEnRangoPerception;
    private PushPerception IrAPorComidaPerception;
    private PushPerception IrADejarComidaPerception;
    private PushPerception RondarDeNuevoPerception;
    private State Rondar;
    private State Disparar;
    private State CogerComida;
    private State DejarComida;
    private NavMeshAgent nmesh;
    public GameObject comedero;
    
    //Place your variables here

    #endregion variables

    // Start is called before the first frame update
    private void Start()
    {
        fsmCazador_FSM = new StateMachineEngine(false);
        nmesh = GetComponent<NavMeshAgent>();
        nmesh.destination = comedero.transform.position + new Vector3(0f, 0f, 0.5f);

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
        
        // States
        Rondar = fsmCazador_FSM.CreateEntryState("Rondar", RondarAction);
        Disparar = fsmCazador_FSM.CreateState("Disparar", DispararAction);
        CogerComida = fsmCazador_FSM.CreateState("CogerComida", CogerComidaAction);
        DejarComida = fsmCazador_FSM.CreateState("DejarComida", DejarComidaAction);
        
        // Transitions
        fsmCazador_FSM.CreateTransition("AnimalEnRango", Rondar, AnimalEnRangoPerception, Disparar);
        fsmCazador_FSM.CreateTransition("IrAPorComida", Disparar, IrAPorComidaPerception, CogerComida);
        fsmCazador_FSM.CreateTransition("IrADejarComida", CogerComida, IrADejarComidaPerception, DejarComida);
        fsmCazador_FSM.CreateTransition("RondarDeNuevo", DejarComida, RondarDeNuevoPerception, Rondar);
        
        // ExitPerceptions
        
        // ExitTransitions
        
    }

    // Update is called once per frame
    private void Update()
    {
        fsmCazador_FSM.Update();
    }

    // Create your desired actions
    
    private void RondarAction()
    {
        
    }
    
    private void DispararAction()
    {
        
    }
    
    private void CogerComidaAction()
    {
        
    }
    
    private void DejarComidaAction()
    {
        
    }
    
}