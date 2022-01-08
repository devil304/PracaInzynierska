using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Valve.VR;

public class InputsSensors : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI leftTMP, rightTMP;

    private void Update()
    {
        SteamVR_Action_Single action = SteamVR_Actions._default.Squeeze;
        float left = action.GetAxis(SteamVR_Input_Sources.LeftHand);
        float right = action.GetAxis(SteamVR_Input_Sources.RightHand);
        leftTMP.text = $"Left squeeze: {left}";
        rightTMP.text = $"Right squeeze: {right}";
    }
}
