using System.Linq;
using System.Collections;
using System.Collections.Generic;
//using System.Threading;
using UnityEngine;

public class StartKinect : MonoBehaviour
{
    //Thread Kinect;

    [Tooltip("AnkleLeft0, AnkleRight1, ElbowLeft2, ElbowRight3, FootLeft4, FootRight5, HandLeft6, HandRight7, Head8, HipLeft9, HipRight10, KneeLeft11, KneeRight12, Neck13,  ShoulderLeft14,  ShoulderRight15, SpineBase16,  SpineMid17, SpineShoulder18")]
    [SerializeField] Transform[] _armature;
    [SerializeField] Transform _VRHead;

    [Tooltip("Left0, Right1")]
    [SerializeField] Transform[] _VRHands;
    // Start is called before the first frame update
    void Start()
    {
        KinectThread kinect = new KinectThread();
        KinectDataExchange.ekstrapolate += ekstrapolate;
        //Kinect = new Thread(kinectThread.Start);
        kinect.Start();
        KinectDataExchange.VRHead = _VRHead;
        KinectDataExchange.VRHands = _VRHands;
    }

    // Update is called once per frame
    void Update()
    {
        if(KinectDataExchange.Joints!=null){
            JointData[] jd = KinectDataExchange.Joints.Values.ToArray();
            
            Vector3 correctionPos = ((jd[8].Position/KinectDataExchange.correctionFactor) - _armature[0].parent.InverseTransformPoint(_VRHead.position)+((jd[6].Position/KinectDataExchange.correctionFactor) - _armature[0].parent.InverseTransformPoint(_VRHands[0].position))+((jd[7].Position/KinectDataExchange.correctionFactor) - _armature[0].parent.InverseTransformPoint(_VRHands[1].position)))/3;
            for(int i =0;i<jd.Length;i++){
                _armature[i].localPosition = (jd[i].Position/KinectDataExchange.correctionFactor) - correctionPos;
                _armature[i].localRotation = jd[i].Rotation;
            }
        }
    }

    private void OnDestroy()
    {
        KinectDataExchange.InitCloseKinect();
    }

    void ekstrapolate()
    {
        StartCoroutine(waitAndEsktrapolate());
    }

    IEnumerator waitAndEsktrapolate()
    {
        yield return new WaitForSecondsRealtime(0.016f);
        if (KinectDataExchange.JointsNew == null || KinectDataExchange.JointsOld == null || KinectDataExchange.Joints == null) yield break;
        List<Windows.Kinect.JointType> names = KinectDataExchange.Joints.Keys.ToList();
        for(int i = 0; i < names.Count; i++)
        {
            var tmp = KinectDataExchange.JointsNew[names[i]];
            tmp.Position = tmp.Position + ((tmp.Position - KinectDataExchange.JointsOld[names[i]].Position) / 2);
            var rotation = tmp.Rotation.eulerAngles + ((tmp.Rotation.eulerAngles - KinectDataExchange.JointsOld[names[i]].Rotation.eulerAngles) / 2);
            tmp.Rotation = Quaternion.Euler(rotation);
            KinectDataExchange.Joints[names[i]] = tmp;
        }
    }
}
