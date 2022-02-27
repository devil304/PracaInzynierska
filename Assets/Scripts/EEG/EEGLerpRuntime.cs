using EEGProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EEGLerpRuntime : MonoBehaviour
{
    float aTime=0,mTime=0;
    float cachedA=0,cachedM=0;
    List<EEGPlotData> EEGDataForPlot = new List<EEGPlotData>(); 

    private void Update()
    {
        bool _updated = false;
        if (EEGDataExchange.Attention != cachedA)
        {
            aTime = 0;
            cachedA = EEGDataExchange.Attention;
        }
        if (EEGDataExchange.Meditation != cachedM)
        {
            mTime = 0;
            cachedM = EEGDataExchange.Meditation;
        }
        if(Math.Abs(EEGDataExchange.Attention-EEGDataExchange.GameAttention)>1)
        {
            aTime += Time.deltaTime;
            aTime = aTime > 1f ? 1f : aTime;
            EEGDataExchange.GameAttention = Mathf.Lerp(EEGDataExchange.GameAttention, EEGDataExchange.Attention, aTime);
            _updated = true;
        }
        if (Math.Abs(EEGDataExchange.Meditation - EEGDataExchange.GameMeditation) > 1)
        {
            mTime += Time.deltaTime;
            mTime = mTime > 1f ? 1f : mTime;
            EEGDataExchange.GameMeditation = Mathf.Lerp(EEGDataExchange.GameMeditation, EEGDataExchange.Meditation, mTime);
            _updated=true;
        }
        if(_updated)
            EEGDataExchange.OnEEGUpdate?.Invoke();
        if (cachedA!=0 || cachedM != 0)
        {
            EEGDataForPlot.Add(new EEGPlotData(EEGDataExchange.RawAttention, EEGDataExchange.RawMeditation, EEGDataExchange.GameAttention,EEGDataExchange.GameMeditation,Time.realtimeSinceStartup));
        }
    }

    private void OnDestroy()
    {
        if(EEGDataForPlot.Count>0)
            SaveSystem.SaveEEGPlotData(EEGDataForPlot.ToArray());
    }
}
