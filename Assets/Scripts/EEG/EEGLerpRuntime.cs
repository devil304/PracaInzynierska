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
        if (EEGDataExchange.Attension != cachedA)
        {
            aTime = 0;
            cachedA = EEGDataExchange.Attension;
        }
        if (EEGDataExchange.Meditation != cachedM)
        {
            mTime = 0;
            cachedM = EEGDataExchange.Meditation;
        }
        if(Math.Abs(EEGDataExchange.Attension-EEGDataExchange.GameAttension)>1)
        {
            aTime += Time.deltaTime;
            aTime = aTime > 1f ? 1f : aTime;
            EEGDataExchange.GameAttension = Mathf.Lerp(EEGDataExchange.GameAttension, EEGDataExchange.Attension, aTime);
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
            EEGDataForPlot.Add(new EEGPlotData(EEGDataExchange.RawAttension, EEGDataExchange.RawMeditation, EEGDataExchange.GameAttension,EEGDataExchange.GameMeditation,Time.realtimeSinceStartup));
        }
    }

    private void OnDestroy()
    {
        if(EEGDataForPlot.Count>0)
            SaveSystem.SaveEEGPlotData(EEGDataForPlot.ToArray());
    }
}
