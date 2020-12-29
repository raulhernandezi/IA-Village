using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class US : MonoBehaviour {

    #region variables

    private UtilitySystemEngine US_US;
    private StateMachineEngine trabajarfsm_SubFSM;
    

    private Factor fusionComer;
    private Factor fusionBeber;
    private Factor tienehambre;
    private Factor haycomida;
    private Factor hayleche;
    private Factor tienesed;
    private Factor fusionTrabajar;
    private UtilityAction comer;
    private UtilityAction beber;
    private UtilityAction trabajarfsm;
    private State NewState;

    //Place your variables here
    private float hambre;
    private float sed;
    private float hay_hambre;
    private float hay_sed;

    private GameManagerScript gms;

    [Header("Lugares")]
    [SerializeField] private GameObject comedero;
    [SerializeField] private GameObject bebedero;

    #endregion variables

    // Start is called before the first frame update
    private void Start()
    {
        hambre = 0;
        sed = 100;
        hay_hambre = 0;
        hay_sed = 0;

        gms = FindObjectOfType<GameManagerScript>();
        US_US = new UtilitySystemEngine(false);
        //trabajarfsm_SubFSM = new StateMachineEngine(true);
        

        //Createtrabajarfsm_SubFSM();
        CreateUtilitySystem();
    }
    
    private void Createtrabajarfsm_SubFSM()
    {
        // Perceptions
        // Modify or add new Perceptions, see the guide for more
        
        // States
        //NewState = trabajarfsm_SubFSM.CreateEntryState("New State ", NewStateAction);
        
        // Transitions
        
        // ExitPerceptions
        
        // ExitTransitions
        
    }
    
    private void CreateUtilitySystem()
    {
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
        tienehambre = new LeafVariable(() => hay_hambre, 1, 0);
        haycomida = new LeafVariable(() => gms.hay_comida, 1, 0);
        hayleche = new LeafVariable(() => gms.hay_leche, 1, 0);
        tienesed = new LeafVariable(() => hay_sed, 1, 0);
        List<Factor> fusionTrabajarFactors = new List<Factor>
        {
            fusionComer,
            fusionBeber,
        };
        
        //fusionTrabajar = new MaxFusion(fusionTrabajarFactors);
        
        // ACTIONS
        comer = US_US.CreateUtilityAction("comer", comerAction, fusionComer);
        beber = US_US.CreateUtilityAction("beber", beberAction, fusionBeber);
       // trabajarfsm = US_US.CreateSubBehaviour("trabajar_fsm", fusionTrabajar, trabajarfsm_SubFSM);
        
        
        // ExitPerceptions
        
        // ExitTransitions
        //trabajarfsm_SubFSM.CreateExitTransition("trabajarfsm_SubFSM Exit", null, null, US_US);
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (hambre > 0)
        {
            hay_hambre = 1f;
        }else
        {
            hay_hambre = 0f;
        }

        if (sed > 0)
        {
            hay_sed = 1f;
        } else
        {
            hay_sed = 0f;
        }

        US_US.Update();
        //trabajarfsm_SubFSM.Update();
    }

    // Create your desired actions
    
    private void comerAction()
    {
        this.transform.Translate(comedero.transform.position);
    }
    
    private void beberAction()
    {
        this.transform.Translate(bebedero.transform.position);
    }
    
    private void NewStateAction()
    {
        
    }
    
}