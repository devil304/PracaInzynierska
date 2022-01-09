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

    public static Action OnEEGUpdate;

    public static void InitCloseEEG()
    {
        CloseEEG?.Invoke();
    }
}
