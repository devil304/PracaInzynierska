using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EEGProcessing
{
    static class EEGFilters
    {
        static Node[] Roots;

        public static float Threshold = 0f;

        static EEGFilters()
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
            return (float)scoresums;
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
        static List<Node> rootsToSave;
        static int count = 0;

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
                return saveContainer.roots.ToList().GetRange(0, saveContainer.mainTreesCount).ToArray();
            }
            return null;
        }

        static int indexer(Node root)
        {
            rootsToSave.Add(root);
            root.LeftID = root.Left != null ? indexer(root.Left) : -1;
            root.RightID = root.Right != null ? indexer(root.Right) : -1;
            return rootsToSave.Count - 1;
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
    }

    public enum SplitType { Attension, Meditation };
}
