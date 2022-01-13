using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(CharacterController))]
public class Move : MonoBehaviour
{
    [SerializeField] SteamVR_Input_Sources sourceMovement, sourceTurning;
    [SerializeField] Transform leftHand;
    [SerializeField] float speed = 5, rotationSpeed = 2;
    CharacterController controller;
    [SerializeField] LayerMask groundCheckLayerMask;
    bool isGrounded = false;

    SteamVR_Action_Vector2 actionMovement, actionTurning;

    // Start is called before the first frame update
    void Start()
    {
        actionMovement = SteamVR_Actions._default.Movement;
        actionTurning = SteamVR_Actions._default.Turning;
        controller = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        float distanceFromFloor = Vector3.Dot(Player.instance.hmdTransform.localPosition, Vector3.up);
        controller.height = Mathf.Max(controller.radius, distanceFromFloor);
        var newCenter = transform.InverseTransformPoint(Player.instance.hmdTransform.position);
        newCenter.y = (controller.height / 2f);
        controller.center = newCenter;
        Vector2 moveVal = actionMovement.GetAxis(sourceMovement);
        Vector3 forward = Vector3.ProjectOnPlane(leftHand.forward, Vector3.up);
        Vector3 right = Vector3.ProjectOnPlane(leftHand.right, Vector3.up);
        right = right.sqrMagnitude < 0.3f * 0.3f ? Vector3.zero : right.normalized;
        forward = forward.sqrMagnitude < 0.3f * 0.3f ? Vector3.zero : forward.normalized;
        if (moveVal.sqrMagnitude > 0.25f * 0.25f)
        {
            controller.Move((forward * moveVal.y * speed) + (right * moveVal.x * speed));
        }
        

        Vector2 turnVal = actionTurning.GetAxis(sourceTurning);
        transform.RotateAround(Player.instance.hmdTransform.position, Vector3.up, rotationSpeed * turnVal.x);
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.BoxCast(transform.TransformPoint(controller.center) + Vector3.up * 0.1f, Vector3.one * 0.025f, Vector3.down, Quaternion.identity, 0.15f, groundCheckLayerMask))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
            controller.Move(Vector3.down * 11f);
        }
    }
}
