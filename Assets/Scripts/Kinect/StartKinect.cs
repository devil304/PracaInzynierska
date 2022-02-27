using System.Linq;
using System.Collections;
using System.Collections.Generic;
//using System.Threading;
using UnityEngine;
using EEGProcessing;
using Windows.Kinect;

public class StartKinect : MonoBehaviour
{
    //Thread Kinect;

    [Tooltip("AnkleLeft0, AnkleRight1, ElbowLeft2, ElbowRight3, FootLeft4, FootRight5, HandLeft6, HandRight7, Head8, HipLeft9, HipRight10, KneeLeft11, KneeRight12, Neck13,  ShoulderLeft14,  ShoulderRight15, SpineBase16,  SpineMid17, SpineShoulder18")]
    [SerializeField] Transform[] _armature;
    [SerializeField] Rigidbody[] _rigidbodies;

    [Tooltip("Left0, Right1")]
    [SerializeField] Transform[] _VRHands;

    List<KinectPlotData> KinectDataForPlot = new List<KinectPlotData>();

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
    float t = 0;
    void Update()
    {
        if (kinect.JointsAct == null || kinect.JointsAct.Count != kinect.Joints.Count)
            kinect.JointsAct = kinect.Joints;
        if(kinect.Joints!=null){

            if(kinect.JointsEkstrOld!=null){
                foreach(var j in kinect.Joints.Keys.ToArray())
                {
                    if (kinect.Joints[j].Position != kinect.JointsEkstrOld[j].Position)
                    {
                        t = 0;
                        break;
                    }
                }

                t += Time.deltaTime;
                foreach(var j in kinect.JointsAct.Keys.ToArray())
                {
                    var tmp = kinect.JointsAct[j];
                    tmp.Position = Vector3.Lerp(kinect.JointsEkstrOld[j].Position,kinect.Joints[j].Position,Mathf.Clamp01(t/0.016f));
                    kinect.JointsAct[j] = tmp;
                }
            }

            JointData[] jd = kinect.JointsAct.Values.ToArray();

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
            if (kinect.JointsOld != null && kinect.JointsOld.ContainsKey(JointType.AnkleRight) && kinect.JointsOld.ContainsKey(JointType.ElbowLeft) && kinect.JointsOld.ContainsKey(JointType.HipRight)) {
                Vector3ToSave RS1 = kinect.JointsNew[JointType.AnkleRight].Position.ToV3TS();
                Vector3ToSave RS2 = kinect.JointsNew[JointType.ElbowLeft].Position.ToV3TS();
                Vector3ToSave RS3 = kinect.JointsNew[JointType.HipRight].Position.ToV3TS();
                Vector3ToSave S1 = kinect.JointsAct[JointType.AnkleRight].Position.ToV3TS();
                Vector3ToSave S2 = kinect.JointsAct[JointType.ElbowLeft].Position.ToV3TS();
                Vector3ToSave S3 = kinect.JointsAct[JointType.HipRight].Position.ToV3TS();
                KinectDataForPlot.Add(new KinectPlotData(RS1,RS2,RS3,S1,S2,S3,Time.realtimeSinceStartup));
            }
        }
    }

    private void OnDestroy()
    {
        kinect.InitCloseKinect();
        if (KinectDataForPlot.Count > 0)
            SaveSystem.SaveKinectPlotData(KinectDataForPlot.ToArray());
    }

    void extrapolate()
    {
        StartCoroutine(waitAndExtrapolate());
    }

    IEnumerator waitAndExtrapolate()
    {
        yield return new WaitForSecondsRealtime(0.016f);
        if (kinect.JointsNew == null || kinect.JointsOld == null || kinect.Joints == null) yield break;
        List<JointType> names = kinect.Joints.Keys.ToList();
        kinect.JointsEkstrOld = kinect.Joints;
        Dictionary<JointType, JointData> tmpJ = new Dictionary<JointType, JointData>();
        for(int i = 0; i < names.Count; i++)
        {
            var tmp = kinect.JointsNew[names[i]];
            Vector3 add = Vector3.zero;
            if ((tmp.Position - kinect.JointsOld[names[i]].Position).sqrMagnitude >= (0.01f * 0.01f))
            {
                add = (tmp.Position - kinect.JointsOld[names[i]].Position) / 2;
            }
            tmp.Position = tmp.Position + add;
            tmpJ.Add(names[i], tmp);
            /*var rotationDifference = kinect.JointsNew[names[i]].Rotation * Quaternion.Inverse(kinect.JointsOld[names[i]].Rotation);
            float angle;
            Vector3 axis;
            rotationDifference.ToAngleAxis(out angle, out axis);
            angle = angle > 180? angle - 360:angle;
            angle = angle * 1.5f % 360;
            var newRotation = Quaternion.AngleAxis(angle, axis) * kinect.JointsOld[names[i]].Rotation;*/
            //kinect.Joints[names[i]] = tmp;
        }
        kinect.Joints = tmpJ;
    }
}
