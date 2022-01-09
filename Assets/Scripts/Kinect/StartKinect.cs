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
    [SerializeField] Rigidbody[] _rigidbodies;

    [Tooltip("Left0, Right1")]
    [SerializeField] Transform[] _VRHands;
    // Start is called before the first frame update
    void Start()
    {
        KinectHandler kinect = new KinectHandler();
        KinectDataExchange.ekstrapolate += ekstrapolate;
        //Kinect = new Thread(kinectThread.Start);
        kinect.Start();
        KinectDataExchange.VRHands = _VRHands;
    }

    // Update is called once per frame
    void Update()
    {
        if(KinectDataExchange.Joints!=null){
            JointData[] jd = KinectDataExchange.Joints.Values.ToArray();
            
            Vector3 correctionPos = (((jd[6].Position/KinectDataExchange.correctionFactor) - _armature[0].parent.InverseTransformPoint(_VRHands[0].position))+((jd[7].Position/KinectDataExchange.correctionFactor) - _armature[0].parent.InverseTransformPoint(_VRHands[1].position)))/2;
            for(int i =0;i<jd.Length;i++){
                if (_rigidbodies[i])
                {
                    _rigidbodies[i].MovePosition(_armature[i].parent.TransformPoint((jd[i].Position / KinectDataExchange.correctionFactor) - correctionPos));
                }
                else
                    _armature[i].localPosition = (jd[i].Position / KinectDataExchange.correctionFactor) - correctionPos;
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
        StartCoroutine(waitAndExtrapolate());
    }

    IEnumerator waitAndExtrapolate()
    {
        yield return new WaitForSecondsRealtime(0.016f);
        if (KinectDataExchange.JointsNew == null || KinectDataExchange.JointsOld == null || KinectDataExchange.Joints == null) yield break;
        List<Windows.Kinect.JointType> names = KinectDataExchange.Joints.Keys.ToList();
        for(int i = 0; i < names.Count; i++)
        {
            var tmp = KinectDataExchange.JointsNew[names[i]];
            tmp.Position = tmp.Position + ((tmp.Position - KinectDataExchange.JointsOld[names[i]].Position) / 2);
            KinectDataExchange.Joints[names[i]] = tmp;
        }
    }
}
