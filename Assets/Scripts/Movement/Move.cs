using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Move : MonoBehaviour
{
    [SerializeField] SteamVR_Input_Sources source;
    [SerializeField] Transform leftHand;
    [SerializeField] float speed = 5;
    Rigidbody body;

    SteamVR_Action_Vector2 action;

    // Start is called before the first frame update
    void Start()
    {
        action = SteamVR_Actions._default.Movement;
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        Vector2 moveVal = action.GetAxis(source);
        Vector3 forward = Vector3.ProjectOnPlane(leftHand.forward, Vector3.up);
        Vector3 right = Vector3.ProjectOnPlane(leftHand.right, Vector3.up);
        if (moveVal.sqrMagnitude > 0.25f * 0.25f)
        {
            body.MovePosition(body.position+(forward*moveVal.y*speed)+ (right * moveVal.x * speed));
        }
    }
}
