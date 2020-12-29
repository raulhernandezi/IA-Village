using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SISTEMADEUTILIDAD : MonoBehaviour {

    #region variables

    private UtilitySystemEngine SISTEMADEUTILIDAD_US;
    private StateMachineEngine TRABAJAR_SubFSM;
    

    private Factor FusionCOMER;
    private Factor FusionBEBER;
    private Factor TIENEHAMBRE;
    private Factor HAYCOMIDA;
    private Factor TIENESED;
    private Factor HAYLECHE;
    private Factor FusionTRABAJAR;
    private UtilityAction COMER;
    private UtilityAction BEBER;
    private UtilityAction TRABAJAR;
    private State NewState;
    
    //Place your variables here

    #endregion variables

    // Start is called before the first frame update
    private void Start()
    {
        SISTEMADEUTILIDAD_US = new UtilitySystemEngine(false);
        TRABAJAR_SubFSM = new StateMachineEngine(true);
        

        CreateTRABAJAR_SubFSM();
        CreateUtilitySystem();
    }
    
    private void CreateTRABAJAR_SubFSM()
    {
        // Perceptions
        // Modify or add new Perceptions, see the guide for more
        
        // States
        NewState = TRABAJAR_SubFSM.CreateEntryState("New State", NewStateAction);
        
        // Transitions
        
        // ExitPerceptions
        
        // ExitTransitions
        
    }
    
    private void CreateUtilitySystem()
    {
        // FACTORS
        List<Factor> FusionCOMERFactors = new List<Factor>
        {
            TIENEHAMBRE,
            HAYCOMIDA,
        };
        
        List<System.Single> FusionCOMERWeights = new List<System.Single>
        {
            0.6f,
            0.4f,
        };
        
        FusionCOMER = new WeightedSumFusion(FusionCOMERFactors, FusionCOMERWeights);
        List<Factor> FusionBEBERFactors = new List<Factor>
        {
            TIENESED,
            HAYLECHE,
        };
        
        List<System.Single> FusionBEBERWeights = new List<System.Single>
        {
            0.6f,
            0.4f,
        };
        
        FusionBEBER = new WeightedSumFusion(FusionBEBERFactors, FusionBEBERWeights);
        TIENEHAMBRE = new LeafVariable(() => /*Reference to desired variable*/0.0f, 1, 0);
        HAYCOMIDA = new LeafVariable(() => /*Reference to desired variable*/0.0f, 1, 0);
        TIENESED = new LeafVariable(() => /*Reference to desired variable*/0.0f, 1, 0);
        HAYLECHE = new LeafVariable(() => /*Reference to desired variable*/0.0f, 1, 0);
        List<Factor> FusionTRABAJARFactors = new List<Factor>
        {
            FusionCOMER,
            FusionBEBER,
        };
        
        FusionTRABAJAR = new MaxFusion(FusionTRABAJARFactors);
        
        // ACTIONS
        COMER = SISTEMADEUTILIDAD_US.CreateUtilityAction("COMER", COMERAction, FusionCOMER);
        BEBER = SISTEMADEUTILIDAD_US.CreateUtilityAction("BEBER", BEBERAction, FusionBEBER);
        TRABAJAR = SISTEMADEUTILIDAD_US.CreateSubBehaviour("TRABAJAR", FusionTRABAJAR, TRABAJAR_SubFSM);
        
        
        // ExitPerceptions
        
        // ExitTransitions
        TRABAJAR_SubFSM.CreateExitTransition("TRABAJAR_SubFSM Exit", null, null, SISTEMADEUTILIDAD_US);
        
    }

    // Update is called once per frame
    private void Update()
    {
        SISTEMADEUTILIDAD_US.Update();
        TRABAJAR_SubFSM.Update();
    }

    // Create your desired actions
    
    private void COMERAction()
    {
        
    }
    
    private void BEBERAction()
    {
        
    }
    
    private void NewStateAction()
    {
        
    }
    
}