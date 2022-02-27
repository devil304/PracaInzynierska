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

    public volatile static float Raw, Poor, Attention, Meditation;
    public volatile static float RawAttention, RawMeditation;
    public static float GameAttention, GameMeditation;

    public static Action OnEEGUpdate;

    public static void InitCloseEEG()
    {
        CloseEEG?.Invoke();
    }

    public static void SetNewVal(SplitType ValType, float newVal)
    {
        if (ValType == SplitType.Attention)
        {
            Attention = newVal;
        }
        else
        {
            Meditation = newVal;
        }
    }
}
