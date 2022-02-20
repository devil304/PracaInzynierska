using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;
using Random = System.Random;

namespace EEGProcessing
{
    static class EEGFilters
    {
        static Node[] Roots;

        public static float Threshold = 0f;

        public static void StartEEGFilters()
        {
            Roots = SaveSystem.LoadFile("TestTrees");
            
            var trees = Roots.Shuffle().ToList();
        }
        
        public static float Score(float[] data)
        {
            double scoresums = 0f;
            for (int i = 0; i < Roots.Count(); i++)
            {
                scoresums += Evaluate(data, Roots[i], 0);
            }
            scoresums /= Roots.Count();
            return (float)Math.Pow(2f, -(scoresums / C(254)));
        }

        static float Evaluate(float[] data, Node root, int currentDepth)
        {
            if (root.Left == null && root.Right == null)
            {
                return currentDepth + C(root.SampleCount);
            }
            if (data[(int)root.SplitType] < root.DataVal)
            {
                return Evaluate(data, root.Left, currentDepth + 1);
            }
            else if (data[(int)root.SplitType] >= root.DataVal)
            {
                return Evaluate(data, root.Right, currentDepth + 1);
            }
            return float.NegativeInfinity;
        }

        static float C(int n)
        {
            if (n <= 1) return 0;
            return 2 * H(n - 1) - (2 * (n - 1) / n);
        }

        static float H(int n)
        {
            return (float)(Math.Log(n) + 0.5772156649f);
        }
    }

    public static class SaveSystem
    {

        public static Node[] LoadFile(string name)
        {
            if (File.Exists(@".\" + name + ".trees"))
            {
                string path = @".\" + name + ".trees";
                SaveContainer saveContainer;
                XmlSerializer serializer = new XmlSerializer(typeof(SaveContainer));
                FileStream fs = new FileStream(path, FileMode.Open);
                saveContainer = serializer.Deserialize(fs) as SaveContainer;
                fs.Close();
                for (uint i = 0; i < saveContainer.roots.Length; i++)
                {
                    saveContainer.roots[i].Left = saveContainer.roots[i].LeftID < 0 ? null : saveContainer.roots[saveContainer.roots[i].LeftID];
                    saveContainer.roots[i].Right = saveContainer.roots[i].RightID < 0 ? null : saveContainer.roots[saveContainer.roots[i].RightID];
                }
                EEGFilters.Threshold = saveContainer.threshold;
                return saveContainer.roots.ToList().GetRange(0, saveContainer.mainTreesCount).ToArray();
            }
            return null;
        }

        public static void SaveEEGPlotData(EEGPlotData[] eegPlotData, string name = "EEGPlotData")
        {
            if (!File.Exists(@".\" + name + ".eegpd"))
            {
                TextWriter writer = new StreamWriter(name + ".eegpd");
                XmlSerializer serializer = new XmlSerializer(typeof(EEGPlotData[]));
                serializer.Serialize(writer, eegPlotData);
                writer.Close();
            }
            else
                Debug.LogWarning("ERROR: File already exist");
        }

        public static void SaveKinectPlotData(KinectPlotData[] kinectPlotData, string name = "KinectPlotData")
        {
            if (!File.Exists(@".\" + name + ".kpd"))
            {
                TextWriter writer = new StreamWriter(name + ".kpd");
                XmlSerializer serializer = new XmlSerializer(typeof(KinectPlotData[]));
                serializer.Serialize(writer, kinectPlotData);
                writer.Close();
            }
            else
                Debug.LogWarning("ERROR: File already exist");
        }

    }

    [Serializable]
    public class SaveContainer
    {
        public float threshold;
        public int mainTreesCount = 0;
        public Node[] roots;

        public SaveContainer() { }

        public SaveContainer(int treesC, Node[] r, float t)
        {
            roots = r;
            mainTreesCount = treesC;
            threshold = t;
        }
    }

    [Serializable]
    public class EEGPlotData
    {
        public float TimeS;
        public float RawAtt, RawMed;
        public float GameAtt, GameMed;

        public EEGPlotData() { }

        public EEGPlotData(float rawAtt, float rawMed, float gameAtt, float gameMed,float timeS)
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

    [Serializable]
    public class Node
    {
        public Node? Left, Right;
        public SplitType SplitType;
        public float DataVal;
        public int SampleCount;
        public int LeftID = -1, RightID = -1;
    }

    public static class Extensions
    {
        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            var rng = new Random(DateTime.Now.Millisecond);
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }

        public static Vector3ToSave ToV3TS(this Vector3 source)
        {
            return new Vector3ToSave(source.x, source.y, source.z);
        }
    }

    public enum SplitType { Attension, Meditation };
}
