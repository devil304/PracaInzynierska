using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void BasicVoid();

public static class EEGDataExchange
{
    public static event BasicVoid CloseEEG;

    public static void InitCloseEEG()
    {
        CloseEEG.Invoke();
    }
}
