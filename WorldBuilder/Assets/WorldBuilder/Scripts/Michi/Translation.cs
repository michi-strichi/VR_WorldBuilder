// AUTHOR: MICHAEL HOCHREITER

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.XR.Interaction.Toolkit.AR;
using UnityEngine;

public class Translation : MonoBehaviour
{
    public bool debug = true;
    public Transform debugBlock;
    public float farPositionClip = 10f;
    public float nearPositionClip = 0.1f;
    
    private  Transform zoomObject;
    private  Transform leftHand;
    private  Transform rightHand;

    private float oldT1 = 0f;
    private float oldT2 = 0f;

    private StateMachine stateMachine;
    private Transform modificationParent;

    private void Awake()
    {
        modificationParent = GameObject.FindWithTag("ModificationParent").transform;
        leftHand = GameObject.FindWithTag("LeftController").transform;
        rightHand = GameObject.FindWithTag("RightController").transform;
        
        stateMachine = GameObject.FindWithTag("StateMachine").GetComponent<StateMachine>();
    }
    
    void Update()
    {
        if (stateMachine.state == StateMachine.State.EditingTranslation) {
            var newPosition = CalculateBlockPosition();
            
            // FIX THIS: !!!
            if (!ObjectBelowGround(stateMachine.currentObject.transform)) 
            {
                modificationParent.position = newPosition;  
            }
        } else if (debug)
        {
            if (!float.IsNaN(CalculateBlockPosition().x) && 
                !float.IsNaN(CalculateBlockPosition().y) &&
                !float.IsNaN(CalculateBlockPosition().z))
            {
                debugBlock.position = CalculateBlockPosition();
            }
        }
    }

    private Vector3 CalculateBlockPosition()
    {
        // https://math.stackexchange.com/questions/2213165/find-shortest-distance-between-lines-in-3d

        // hand positions
        Vector3 r1 = leftHand.position;
        Vector3 r2 = rightHand.position;
        
        // direction vectors of hands
        Vector3 e1 = leftHand.forward;  
        Vector3 e2 = rightHand.forward;

        Vector3 n = Vector3.Cross(e1, e2); // direction vector of closest connecting line
        
        // scalars for first line and second line that lead to endpoints of closest connecting line
        float t1 = Vector3.Dot(Vector3.Cross(e2, n), r2 - r1) / Vector3.Dot(n, n);
        float t2 = Vector3.Dot(Vector3.Cross(e1, n), r2 - r1) / Vector3.Dot(n, n);

        // clamp distance where the block can be placed so it cannot be placed inifitely far away as lines start to get more parallel
        t1 = Mathf.Clamp(t1, nearPositionClip, farPositionClip);
        t2 = Mathf.Clamp(t2, nearPositionClip, farPositionClip);
        
        // clip values in case of intersection point being BEHIND player 
        // also smooth values
        var velocityT1 = 0.0f;
        var velocityT2 = 0.0f;
        
        if (t1 < 0)
        {
            t1 = farPositionClip;
        }
        else
        {
            Mathf.SmoothDamp(oldT1, t1, ref velocityT1, 0.05f);
        }
        
        if (t2 < 0)
        {
            t2 = farPositionClip;
        }
        else
        {
            Mathf.SmoothDamp(oldT2, t2, ref velocityT2, 0.05f);
        }
        
        oldT1 = t1;
        oldT2 = t2;

        // endpoints of closest connecting line
        Vector3 endpoint1 = r1 + t1 * e1;
        Vector3 endpoint2 = r2 + t2 * e2;

        Vector3 midpoint = Vector3.Lerp(endpoint1, endpoint2, 0.5f); // point where the block will be places

        return midpoint;
    }

    private bool ObjectBelowGround(Transform t)
    {
        
        
        // FIX THIS!!!
        
        
        
        return false;
    }
}