using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Move : MonoBehaviour
{
    [SerializeField] SteamVR_Input_Sources sourceMovement, sourceTurning;
    [SerializeField] Transform leftHand;
    [SerializeField] Transform head;
    [SerializeField] float speed = 5, rotationSpeed = 2;
    [SerializeField] Rigidbody body;

    SteamVR_Action_Vector2 actionMovement, actionTurning;

    // Start is called before the first frame update
    void Start()
    {
        actionMovement = SteamVR_Actions._default.Movement;
        actionTurning = SteamVR_Actions._default.Turning;
        body = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        Vector3 actPos = transform.position;
        actPos.y = body.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveVal = actionMovement.GetAxis(sourceMovement);
        Vector3 forward = Vector3.ProjectOnPlane(leftHand.forward, Vector3.up);
        Vector3 right = Vector3.ProjectOnPlane(leftHand.right, Vector3.up);
        right = right.sqrMagnitude < 0.3f * 0.3f ? Vector3.zero : right.normalized;
        forward = forward.sqrMagnitude < 0.3f * 0.3f?Vector3.zero: forward.normalized;
        if (moveVal.sqrMagnitude > 0.25f * 0.25f)
        {
            body.velocity = ((forward * moveVal.y * speed) + (right * moveVal.x * speed));
        }
        else if (body.velocity.x != 0 || body.velocity.z != 0)
            body.velocity = new Vector3(0,body.velocity.y,0);


        Vector2 turnVal = actionTurning.GetAxis(sourceTurning);
        body.MoveRotation(body.rotation * Quaternion.AngleAxis(rotationSpeed * turnVal.x, Vector3.up));
    }
}
