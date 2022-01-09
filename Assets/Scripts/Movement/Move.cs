using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Move : MonoBehaviour
{
    [SerializeField] SteamVR_Input_Sources sourceMovement, sourceTurning;
    [SerializeField] Transform leftHand;
    [SerializeField] float speed = 5, rotationSpeed = 2;
    Rigidbody body;

    SteamVR_Action_Vector2 actionMovement, actionTurning;

    // Start is called before the first frame update
    void Start()
    {
        actionMovement = SteamVR_Actions._default.Movement;
        actionTurning = SteamVR_Actions._default.Turning;
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        Vector2 moveVal = actionMovement.GetAxis(sourceMovement);
        Vector3 forward = Vector3.ProjectOnPlane(leftHand.forward, Vector3.up);
        Vector3 right = Vector3.ProjectOnPlane(leftHand.right, Vector3.up);
        right = right.sqrMagnitude < 0.3f * 0.3f ? Vector3.zero : right;
        forward = forward.sqrMagnitude < 0.3f * 0.3f?Vector3.zero: forward;
        if (moveVal.sqrMagnitude > 0.25f * 0.25f)
        {
            body.MovePosition(body.position+(forward*moveVal.y*speed)+ (right * moveVal.x * speed));
        }

        Vector2 turnVal = actionTurning.GetAxis(sourceTurning);
        body.MoveRotation(body.rotation * Quaternion.AngleAxis(rotationSpeed*turnVal.x,Vector3.up));
    }
}
