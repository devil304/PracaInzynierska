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

        public static EEGPlotData[] DeserializeXMLFileEEGPD(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(EEGPlotData[]));
            FileStream fs = new FileStream(path, FileMode.Open);
            return (EEGPlotData[])serializer.Deserialize(fs);
        }

        public static KinectPlotData[] DeserializeXMLFileKPD(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(KinectPlotData[]));
            FileStream fs = new FileStream(path, FileMode.Open);
            return (KinectPlotData[])serializer.Deserialize(fs);
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



    [Serializable]
    public class EEGPlotData
    {
        public float TimeS;
        public float RawAtt, RawMed;
        public float GameAtt, GameMed;

        public EEGPlotData() { }

        public EEGPlotData(float rawAtt, float rawMed, float gameAtt, float gameMed, float timeS)
        {
            RawAtt = rawAtt;
            RawMed = rawMed;
            GameAtt = gameAtt;
            GameMed = gameMed;
            TimeS = timeS;
        }
    }

    [Serializable]
    public class KinectPlotData
    {
        public float TimeS;
        public Vector3ToSave RawS1, RawS2, RawS3, GameS1, GameS2, GameS3;
        public KinectPlotData() { }

        public KinectPlotData(Vector3ToSave rawS1, Vector3ToSave rawS2, Vector3ToSave rawS3, Vector3ToSave gameS1, Vector3ToSave gameS2, Vector3ToSave gameS3, float timeS)
        {
            RawS1 = rawS1;
            RawS2 = rawS2;
            RawS3 = rawS3;
            GameS1 = gameS1;
            GameS2 = gameS2;
            GameS3 = gameS3;
            TimeS = timeS;
        }
    }

    [Serializable]
    public class Vector3ToSave
    {
        public float x, y, z;
        public Vector3ToSave() { }

        public Vector3ToSave(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}
