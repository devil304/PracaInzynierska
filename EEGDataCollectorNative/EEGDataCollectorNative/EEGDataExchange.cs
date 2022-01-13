using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

public delegate void BasicVoid();

public static class EEGDataExchange
{
    public static event BasicVoid CloseEEG;

    /*public static Action StartMeasurement = delegate { };

    public static Action StopMeasurement = delegate { };*/

    public volatile static bool EEGInited = false;

    public volatile static float Raw, Poor, Attension, Meditation;

    public volatile static bool CollectData, Collecting = false;

    public volatile static List<MeasuredDataConteiner> DataList;

    static EEGDataExchange()
    {
        DataList = new List<MeasuredDataConteiner>();
    }

    public static void StartMeasurement(string name)
    {
        if (!Collecting)
        {
            DataList.Add(new MeasuredDataConteiner(name));
            CollectData = true;
            Trace.WriteLine("StartedMeasurment");
        }
        else
        {
            Trace.WriteLine("WARNING: Data are still collected");
        }
    }

    public static void StopMeasurement(Action whenStopped = null)
    {
        CollectData = false;
        while (Collecting) { }
        whenStopped?.Invoke();
    }

    public static void SaveFile(string name)
    {  
        if (!File.Exists(@".\" + name+".eegm"))
        {
            TextWriter writer = new StreamWriter(name + ".eegm");
            XmlSerializer serializer = new XmlSerializer(typeof(MeasuredDataConteiner[]));
            serializer.Serialize(writer, DataList.ToArray());
            writer.Close();
        }
        else
            Trace.WriteLine("ERROR: File already exist");
    }

    public static void InitCloseEEG()
    {
        CloseEEG?.Invoke();
    }
}

[Serializable]
public class MeasuredDataConteiner
{
    public string Name;
    public List<MeasuredData> Data;
    public List<MeasuredDataRaw> DataRaw;

    public MeasuredDataConteiner() { }

    public MeasuredDataConteiner(string name)
    {
        Name = name;
        Data = new List<MeasuredData>();
        DataRaw = new List<MeasuredDataRaw>();
    }
}

[Serializable]
public class MeasuredData
{
    public DateTime Time;
    public float PoorSignal, Attension, Meditation;
    public float Delta, Theta, Alpha1, Alpha2, Beta1, Beta2, Gamma1, Gamma2;

    public MeasuredData() { }

    public MeasuredData(DateTime time, float poorSignal, float attension, float meditation, float delta, float theta, float alpha1, float alpha2, float beta1, float beta2, float gamma1, float gamma2)
    {
        Time = time;
        PoorSignal = poorSignal;
        Attension = attension;
        Meditation = meditation;
        Delta = delta;
        Theta = theta;
        Alpha1 = alpha1;
        Alpha2 = alpha2;
        Beta1 = beta1;
        Beta2 = beta2;
        Gamma1 = gamma1;
        Gamma2 = gamma2;
    }
}

[Serializable]
public class MeasuredDataRaw
{
    public DateTime Time;
    public float Raw;

    public MeasuredDataRaw() { }

    public MeasuredDataRaw(DateTime time, float raw)
    {
        Time = time;
        Raw = raw;
    }
}
