using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgricultorBT : MonoBehaviour {

    #region variables

    private BehaviourTreeEngine AgricultorBT_BT;
    

    private SelectorNode PlantarOrCosechar;
    private SequenceNode SecuenciaSembrar;
    private LeafNode CampoVacio;
    private LeafNode Plantar;
    private SequenceNode SecuenciaCosechar;
    private LeafNode CampoCrecido;
    private LeafNode Recoger;
    private LeafNode Parar;
    private SequenceNode SecuenciaPlantar;
    private LeafNode EstaEnPlantacion;
    private SequenceNode SecuenciaRecoger;
    private LeafNode EstaEnRecogida;

    //Place your variables here
    private GameObject parcelasPadre;
    private GameObject parcelaDestino;
    private GameObject[] parcelas;

    private NavMeshAgent nmesh;
    #endregion variables

    // Start is called before the first frame update
    private void Start()
    {
        AgricultorBT_BT = new BehaviourTreeEngine(false);

        nmesh = GetComponent<NavMeshAgent>();
        parcelasPadre = GameObject.FindGameObjectWithTag("ParcelasPadre");
        parcelas = new GameObject[parcelasPadre.transform.childCount];
        for (int i = 0; i < parcelasPadre.transform.childCount; i++)
        {
            parcelas[i] = parcelasPadre.transform.GetChild(i).gameObject;
        }
        parcelaDestino = null;

        CreateBehaviourTree();
    }





    private void CreateBehaviourTree()
    {
        // Nodes
        PlantarOrCosechar = AgricultorBT_BT.CreateSelectorNode("PlantarOrCosechar");
        SecuenciaSembrar = AgricultorBT_BT.CreateSequenceNode("SecuenciaSembrar", false);
        CampoVacio = AgricultorBT_BT.CreateLeafNode("CampoVacio", CampoVacioAction, CampoVacioSuccessCheck);
        Plantar = AgricultorBT_BT.CreateLeafNode("Plantar", PlantarAction, PlantarSuccessCheck);
        SecuenciaCosechar = AgricultorBT_BT.CreateSequenceNode("SecuenciaCosechar", false);
        CampoCrecido = AgricultorBT_BT.CreateLeafNode("CampoCrecido", CampoCrecidoAction, CampoCrecidoSuccessCheck);
        Recoger = AgricultorBT_BT.CreateLeafNode("Recoger", RecogerAction, RecogerSuccessCheck);
        Parar = AgricultorBT_BT.CreateLeafNode("Parar", PararAction, PararSuccessCheck);
        SecuenciaPlantar = AgricultorBT_BT.CreateSequenceNode("SecuenciaPlantar", false);
        EstaEnPlantacion = AgricultorBT_BT.CreateLeafNode("EstaEnPlantacion", EstaEnPlantacionAction, EstaEnPlantacionSuccessCheck);
        SecuenciaRecoger = AgricultorBT_BT.CreateSequenceNode("SecuenciaRecoger", false);
        EstaEnRecogida = AgricultorBT_BT.CreateLeafNode("EstaEnRecogida", EstaEnRecogidaAction, EstaEnRecogidaSuccessCheck);
        
        // Child adding
        PlantarOrCosechar.AddChild(SecuenciaSembrar);
        PlantarOrCosechar.AddChild(SecuenciaCosechar);
        PlantarOrCosechar.AddChild(Parar);
        
        SecuenciaSembrar.AddChild(CampoVacio);
        SecuenciaSembrar.AddChild(SecuenciaPlantar);
        
        SecuenciaCosechar.AddChild(CampoCrecido);
        SecuenciaCosechar.AddChild(SecuenciaRecoger);
        
        SecuenciaPlantar.AddChild(EstaEnPlantacion);
        SecuenciaPlantar.AddChild(Plantar);
        
        SecuenciaRecoger.AddChild(Recoger);
        SecuenciaRecoger.AddChild(EstaEnRecogida);
        
        // SetRoot
        AgricultorBT_BT.SetRootNode(PlantarOrCosechar);
        
        // ExitPerceptions
        
        // ExitTransitions
        
    }

    // Update is called once per frame
    private void Update()
    {
        AgricultorBT_BT.Update();
    }

    // Create your desired actions
    
    private void CampoVacioAction()
    {
        foreach (GameObject parcela in parcelas)
        {
            if (parcela.GetComponent<ParcelaScript>().libre)
            {
                parcelaDestino = parcela;
                Debug.Log("Parcela libre: " + parcelaDestino.name);
                break;
            }
        }
    }
    
    private ReturnValues CampoVacioSuccessCheck()
    {
        //Write here the code for the success check for CampoVacio
        if (parcelaDestino != null)
        {
            Debug.Log("Exito Campo Vacio");
            return ReturnValues.Succeed;
        }
        else
        {
            Debug.Log("Fracaso Campo Vacio");
            return ReturnValues.Failed;
        }
    }
    
    private void PlantarAction()
    {
        
    }
    
    private ReturnValues PlantarSuccessCheck()
    {
        //Write here the code for the success check for Plantar
        return ReturnValues.Failed;
    }
    
    private void CampoCrecidoAction()
    {
        
    }
    
    private ReturnValues CampoCrecidoSuccessCheck()
    {
        //Write here the code for the success check for CampoCrecido
        return ReturnValues.Failed;
    }
    
    private void RecogerAction()
    {
        
    }
    
    private ReturnValues RecogerSuccessCheck()
    {
        //Write here the code for the success check for Recoger
        return ReturnValues.Failed;
    }
    
    private void PararAction()
    {
        
    }
    
    private ReturnValues PararSuccessCheck()
    {
        //Write here the code for the success check for Parar
        return ReturnValues.Failed;
    }
    
    private void EstaEnPlantacionAction()
    {
        nmesh.destination = new Vector3(parcelaDestino.transform.GetChild(0).position.x, transform.position.y, parcelaDestino.transform.GetChild(0).position.z);
    }
    
    private ReturnValues EstaEnPlantacionSuccessCheck()
    {
        if(transform.position.x == nmesh.destination.x && transform.position.z == nmesh.destination.z)
        {
            Debug.Log("Exito En Plantacion");
            return ReturnValues.Succeed;
        }
        else
        {
            Debug.Log("Fracaso En Plantacion");
            return ReturnValues.Failed;
        }
        
    }
    
    private void EstaEnRecogidaAction()
    {
        
    }
    
    private ReturnValues EstaEnRecogidaSuccessCheck()
    {
        //Write here the code for the success check for EstaEnRecogida
        return ReturnValues.Failed;
    }
    
}