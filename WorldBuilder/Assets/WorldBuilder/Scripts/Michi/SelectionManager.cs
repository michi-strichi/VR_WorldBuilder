// AUTHOR: MICHAEL HOCHREITER

using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectionManager : MonoBehaviour
{
    public InputActionProperty lTriggerPressed;
    public InputActionProperty lTriggerReleased;

    public InputActionProperty rTriggerPressed;
    public InputActionProperty rTriggerReleased;

    private StateMachine stateMachine;
    private Transform lHand;
    private Transform rHand;
    private bool doRaycast = false;
    private String currentHand = "";
    
    public Transform controllerHead;
    public GameObject selectionUI;

    [HideInInspector] public GameObject selectedBuildingBlock = null;


    private void Awake()
    {
        stateMachine = GameObject.FindWithTag("StateMachine").GetComponent<StateMachine>();
        lHand = GameObject.FindWithTag("LeftController").transform;
        rHand = GameObject.FindWithTag("RightController").transform;
    }

    public void OnEnable()
    {
        if (lTriggerPressed.action != null) lTriggerPressed.action.Enable();
        if (lTriggerPressed.action != null) lTriggerPressed.action.performed += LTriggerPressed;
        if (lTriggerReleased.action != null) lTriggerReleased.action.Enable();
        if (lTriggerReleased.action != null) lTriggerReleased.action.performed += LTriggerReleased;

        if (rTriggerPressed.action != null) rTriggerPressed.action.Enable();
        if (rTriggerPressed.action != null) rTriggerPressed.action.performed += RTriggerPressed;
        if (rTriggerReleased.action != null) rTriggerReleased.action.Enable();
        if (rTriggerReleased.action != null) rTriggerReleased.action.performed += RTriggerReleased;
    }
    
    void LTriggerPressed(InputAction.CallbackContext trigger)
    {
        TriggerPressed("left");
    }
    void RTriggerPressed(InputAction.CallbackContext trigger)
    {
        TriggerPressed("right");
    }
    void LTriggerReleased(InputAction.CallbackContext trigger)
    {
        TriggerReleased("left");
    }
    void RTriggerReleased(InputAction.CallbackContext trigger)
    {
        TriggerReleased("right");
    }

    private void TriggerPressed(string hand)
    {
        Debug.Log(hand + " trigger pressed");
        doRaycast = true;
        currentHand = hand;
        

        if (stateMachine.state == StateMachine.State.Idle)
        {
            RaycastHit hit;
            Ray ray = new Ray(
                hand == "left" ? lHand.position : rHand.position, 
                hand == "left" ? lHand.forward : rHand.forward);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.transform.CompareTag("BuildingBlock"))
                {
                    selectedBuildingBlock = hit.transform.gameObject;
                    selectedBuildingBlock.GetComponent<BuildingBlock>().EnableSelectionUI();
                }
            }
        }
    }

    private void TriggerReleased(string hand)
    {
        Debug.Log(hand + " trigger released");
        doRaycast = false;
        currentHand = "";
        
        if (stateMachine.state == StateMachine.State.Idle)
        {
            print("relased in idle mode");
            RaycastHit hit;
            Ray ray = new Ray(
                hand == "left" ? lHand.position : rHand.position, 
                hand == "left" ? lHand.forward : rHand.forward);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                switch (hit.transform.tag)
                {
                    case "rotate":
                        stateMachine.state = StateMachine.State.EditingRotation;
                        stateMachine.currentObject = selectedBuildingBlock;
                        break;
                    case "translate":
                        stateMachine.state = StateMachine.State.EditingTranslation;
                        stateMachine.currentObject = selectedBuildingBlock;
                        break;
                    case "scaleAll":
                        stateMachine.state = StateMachine.State.EditingScaleAllAxis;
                        stateMachine.currentObject = selectedBuildingBlock;
                        break;
                    case "scaleIndividual":
                        stateMachine.state = StateMachine.State.EditingScaleIndividualAxis;
                        stateMachine.currentObject = selectedBuildingBlock;
                        break;
                    default:
                        break;
                }

                // check if there is a buildingBlock selected (!= null), and the enable its selectionUI
                if (selectedBuildingBlock)
                {
                    selectedBuildingBlock.GetComponent<BuildingBlock>().DisableSelectionUI();
                    // MOVE BLOCK INTO TRANSFORMATION PARENT
                }
            }
            
            selectedBuildingBlock.GetComponent<BuildingBlock>().DisableSelectionUI();
            selectedBuildingBlock = null;
        }
    }

    private void Update()
    {
        if (doRaycast)
        {
            Ray ray = new Ray(
                currentHand == "left" ? lHand.position : rHand.position, 
                currentHand == "left" ? lHand.forward : rHand.forward);
            
            RaycastHit hit;
            Physics.Raycast(ray, out hit, Mathf.Infinity,0, QueryTriggerInteraction.UseGlobal);

        }
    }
}