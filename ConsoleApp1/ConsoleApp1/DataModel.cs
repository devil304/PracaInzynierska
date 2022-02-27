using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace EEGDataAnalizer
{
    public static class DataModel
    {
        public static MeasuredDataConteiner[] DeserializeXMLFile(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MeasuredDataConteiner[]));
            FileStream fs = new FileStream(path,FileMode.Open);
            return (MeasuredDataConteiner[])serializer.Deserialize(fs);
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
        public float PoorSignal, Attention, Meditation;
        public float Delta, Theta, Alpha1, Alpha2, Beta1, Beta2, Gamma1, Gamma2;

        public MeasuredData() { }

        public MeasuredData(DateTime time, float poorSignal, float attention, float meditation, float delta, float theta, float alpha1, float alpha2, float beta1, float beta2, float gamma1, float gamma2)
        {
            Time = time;
            PoorSignal = poorSignal;
            Attention = attention;
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
}
