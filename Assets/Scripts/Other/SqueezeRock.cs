using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class SqueezeRock : MonoBehaviour
{
    Interactable interactable;
    SkinnedMeshRenderer skinnedRenderer;
    SteamVR_Action_Single action;

    // Start is called before the first frame update
    void Start()
    {
        interactable = GetComponent<Interactable>();
        skinnedRenderer = GetComponent<SkinnedMeshRenderer>();
        action = SteamVR_Actions._default.Squeeze;
    }

    // Update is called once per frame
    void Update()
    {
        float grip = 0;

        if (interactable.attachedToHand)
        {
            grip = action.GetAxis(interactable.attachedToHand.handType);
        }

        skinnedRenderer.SetBlendShapeWeight(0, Mathf.Lerp(skinnedRenderer.GetBlendShapeWeight(0), 100-(grip * 100), Time.deltaTime * 10));
    }
}
