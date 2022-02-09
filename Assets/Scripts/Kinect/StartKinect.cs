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

    KinectHandler kinect;
    // Start is called before the first frame update
    void Start()
    {
        kinect = new KinectHandler();
        kinect.ekstrapolate += extrapolate;
        //Kinect = new Thread(kinectThread.Start);
        kinect.Start();
        kinect.VRHands = _VRHands;
    }

    // Update is called once per frame
    void Update()
    {
        if(kinect.Joints!=null){
            JointData[] jd = kinect.Joints.Values.ToArray();
            
            Vector3 correctionPos = ((jd[6].Position/ kinect.correctionFactor) - _armature[0].parent.InverseTransformPoint(_VRHands[0].position)+((jd[7].Position/ kinect.correctionFactor) - _armature[0].parent.InverseTransformPoint(_VRHands[1].position)))/2;
            for(int i =0;i<jd.Length;i++){
                if (_rigidbodies[i])
                {
                    _rigidbodies[i].MovePosition(_armature[i].parent.TransformPoint((jd[i].Position / kinect.correctionFactor) - correctionPos));
                }
                else
                    _armature[i].localPosition = (jd[i].Position / kinect.correctionFactor) - correctionPos;
                _armature[i].localRotation = jd[i].Rotation;
            }
        }
    }

    private void OnDestroy()
    {
        kinect.InitCloseKinect();
    }

    void extrapolate()
    {
        StartCoroutine(waitAndExtrapolate());
    }

    IEnumerator waitAndExtrapolate()
    {
        yield return new WaitForSecondsRealtime(0.016f);
        if (kinect.JointsNew == null || kinect.JointsOld == null || kinect.Joints == null) yield break;
        List<Windows.Kinect.JointType> names = kinect.Joints.Keys.ToList();
        for(int i = 0; i < names.Count; i++)
        {
            var tmp = kinect.JointsNew[names[i]];
            Vector3 add = Vector3.zero;
            if ((tmp.Position - kinect.JointsOld[names[i]].Position).sqrMagnitude >= (0.01f * 0.01f))
            {
                add = ((tmp.Position - kinect.JointsOld[names[i]].Position) / 2);
            }
            tmp.Position = tmp.Position + add;
            kinect.Joints[names[i]] = tmp;
        }
    }
}
