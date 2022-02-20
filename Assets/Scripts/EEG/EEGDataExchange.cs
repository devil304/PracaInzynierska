using EEGProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void BasicVoid();

public static class EEGDataExchange
{
    public static event BasicVoid CloseEEG;

    public static string DataInfo="";

    public volatile static float Raw, Poor, Attension, Meditation;
    public volatile static float RawAttension, RawMeditation;
    public static float GameAttension, GameMeditation;

    public static Action OnEEGUpdate;

    public static void InitCloseEEG()
    {
        CloseEEG?.Invoke();
    }

    public static void SetNewVal(SplitType ValType, float newVal)
    {
        if (ValType == SplitType.Attension)
        {
            Attension = newVal;
        }
        else
        {
            Meditation = newVal;
        }
    }
}
