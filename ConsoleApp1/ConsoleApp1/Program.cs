using EEGDataAnalizer;
using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DoWork dw = new DoWork();
            dw.Work();
        }
    }

    public class DoWork
    {
        MeasuredDataConteiner[] Data;
        Node[] Roots;
        List<MeasuredData> md;
        Random random;

        float Threshold = 0;
        public void Work()
        {
            random = new Random(DateTime.UtcNow.Millisecond);
            string path = @"G:\PracaInzynierska\PracaInzynierska\EEGDataCollectorCompiled\Files\Justynka.eegm";
            Data = DataModel.DeserializeXMLFile(path);
            Console.WriteLine(Data[0].Data[0].Attention);
            md = new List<MeasuredData>();
            Array.ForEach(Data, d => md.AddRange(d.Data));
            Console.WriteLine(md.Count());
            //Roots = RootsGen(200,128);
            Roots = SaveSystem.LoadFile("TestTrees");
            List<float[]> dataPair = new List<float[]>();
            md.ForEach(d=>dataPair.Add(new float[] {d.Attention,d.Meditation }));
            var trees = Roots.Shuffle().ToList();
            List<KeyValuePair<int,float>> scores = new List<KeyValuePair<int, float>>();
            for (int j = 0; j < dataPair.Count; j++)
            {
                double scoresums = 0f;
                for (int i = 0; i < trees.Count; i++)
                {
                    scoresums += Evaluate(dataPair[j], trees[i], 0);
                }
                scoresums /= trees.Count;
                scores.Add(new KeyValuePair<int, float>(j,(float)Math.Pow(2f, -(scoresums/C(dataPair.Count)))));
            }
            var sorted = scores.OrderBy(s=>s.Value);
            var wtf = 0.9f * sorted.Count();
            Threshold = sorted.ElementAt((int)wtf).Value; //0.62
            SaveSystem.t = Threshold;
            var trimmed = sorted.Where(n => n.Value < Threshold); //227 //254 
            //SaveSystem.SaveTrees(Roots);
        }

        float Evaluate(float[] data,Node root, int currentDepth)
        {
            if(root.Left==null && root.Right == null)
            {
                return currentDepth+C(root.SampleCount);
            }
            if (data[(int)root.SplitType] < root.DataVal)
            {
                return Evaluate(data, root.Left, currentDepth + 1);
            }else if (data[(int)root.SplitType] >= root.DataVal)
            {
                return Evaluate(data, root.Right, currentDepth + 1);
            }
            return float.NegativeInfinity;
        }

        float C(int n)
        {
            if(n<=1) return 0;
            return 2 * H(n - 1) - (2 * (n - 1) / n);
        }

        float H(int n)
        {
            return (float)(Math.Log(n) + 0.5772156649f);
        }

        Node[] RootsGen(int forestCount, int samples)
        {
            Node[] nodes = new Node[forestCount];
            var depth = (int)Math.Log2(samples);
            for (int i = 0; i < forestCount; i++)
            {
                MeasuredData[] newSamples = md.Shuffle().ToList().GetRange(0,samples).ToArray();
                nodes[i] = TreeGen(newSamples,0,depth);
            }
            return nodes;
        }

        Node TreeGen(MeasuredData[] datas, int treeDepth, int treeLimit)
        {
            if (treeDepth == treeLimit || datas.Length <= 1)
            {
                var d = new Node();
                d.SampleCount = datas.Length;
                return d;
            }

            SplitType st = random.Next(2) == 0 ? SplitType.Attention : SplitType.Meditation;
            float minVal,maxVal;
            minVal = datas.ToList().Min(d => st == SplitType.Attention ? d.Attention : d.Meditation);
            maxVal = datas.ToList().Max(d => st == SplitType.Attention ? d.Attention : d.Meditation);
            var splitVal = random.Next((int)minVal,(int)maxVal+1);
            var datasBelow = datas.Where(d => (st == SplitType.Attention ? d.Attention : d.Meditation) < splitVal).ToArray();
            var datasAbove = datas.Where(d => (st == SplitType.Attention ? d.Attention : d.Meditation) >= splitVal).ToArray();
            Node n = new Node();
            n.Left = TreeGen(datasBelow,treeDepth+1,treeLimit);
            n.Right = TreeGen(datasAbove,treeDepth+1,treeLimit);
            n.SplitType = st;
            n.DataVal = splitVal;
            return n;
        }
    }

    public static class SaveSystem
    {
        static List<Node> rootsToSave;
        static int count = 0;
        public static float t;
        public static void SaveTrees(Node[] roots)
        {
            rootsToSave = new List<Node>();
            rootsToSave.AddRange(roots);
            count = roots.Length;

            for(int i = 0; i < rootsToSave.Count; i++)
            {
                rootsToSave[i].LeftID = rootsToSave[i].Left != null ? indexer(rootsToSave[i].Left) : -1;
                rootsToSave[i].RightID = rootsToSave[i].Right != null ? indexer(rootsToSave[i].Right) : -1;
            }
            SaveFile("TestTrees");
        }

        public static void SaveFile(string name)
        {
            if (!File.Exists(@".\" + name + ".trees"))
            {
                TextWriter writer = new StreamWriter(name + ".trees");
                XmlSerializer serializer = new XmlSerializer(typeof(SaveContainer));
                SaveContainer newSC = new SaveContainer(count, rootsToSave.ToArray(), t);
                serializer.Serialize(writer, newSC);
                writer.Close();
            }
            else
                Trace.WriteLine("ERROR: File already exist");
        }

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
                for (uint i = 0;i<saveContainer.roots.Length;i++)
                {
                    saveContainer.roots[i].Left = saveContainer.roots[i].LeftID < 0 ? null : saveContainer.roots[saveContainer.roots[i].LeftID];
                    saveContainer.roots[i].Right = saveContainer.roots[i].RightID < 0 ? null : saveContainer.roots[saveContainer.roots[i].RightID];
                }
                return saveContainer.roots.ToList().GetRange(0, saveContainer.mainTreesCount).ToArray();
            }
            else
                Trace.WriteLine("ERROR: File not exist");
            return null;
        }

        static int indexer(Node root)
        {
            rootsToSave.Add(root);
            int index = rootsToSave.Count - 1;
            root.LeftID = root.Left != null ? indexer(root.Left) : -1;
            root.RightID = root.Right != null ? indexer(root.Right) : -1;
            return index;
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

    public enum SplitType {Attention, Meditation };
}