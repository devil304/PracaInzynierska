using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Windows.Kinect;

public class KinectHandler
{
    private KinectSensor _Sensor;
    private BodyFrameReader _Reader;
    private Body[] _Data = null;
    
    public Body[] GetData()
    {
        return _Data;
    }

    int _trackedBodyID = -1;
    

    public void Start () 
    {
        _Sensor = KinectSensor.GetDefault();

        if (_Sensor != null)
        {
            _Reader = _Sensor.BodyFrameSource.OpenReader();
            
            if (!_Sensor.IsOpen)
            {
                _Sensor.Open();
                _Reader.FrameArrived += OnFrameRecived;
            }
        }
    }

    JointType[] _jointsToTrack = {JointType.AnkleLeft,JointType.AnkleRight,JointType.ElbowLeft,JointType.ElbowRight,JointType.FootLeft,JointType.FootRight,JointType.HandLeft,JointType.HandRight,JointType.Head,JointType.HipLeft,JointType.HipRight,JointType.KneeLeft,JointType.KneeRight,JointType.Neck, JointType.ShoulderLeft, JointType.ShoulderRight,JointType.SpineBase, JointType.SpineMid,JointType.SpineShoulder};

    private void OnFrameRecived(object sender, BodyFrameArrivedEventArgs e)
    {
        var frame = e.FrameReference.AcquireFrame();
        if (frame != null)
        {
            if (_Data == null)
            {
                _Data = new Body[_Sensor.BodyFrameSource.BodyCount];
            }
            
            frame.GetAndRefreshBodyData(_Data);
            
            frame.Dispose();
            foreach(Body b in _Data){
                if(b.IsTracked && _trackedBodyID<0){
                    _trackedBodyID = (int)b.TrackingId;
                    Debug.Log(_trackedBodyID);
                }
                //Debug.Log(b.IsTracked + " : " + b.TrackingId);
                if((int)b.TrackingId == _trackedBodyID && b.IsTracked){
                    Dictionary<JointType,JointData> tmpJoints = new Dictionary<JointType, JointData>();
                    foreach (JointType jt in _jointsToTrack)
                    {
                        if(b.Joints[jt].TrackingState == TrackingState.Tracked){
                            var pos = b.Joints[jt].Position;
                            var rot = b.JointOrientations[jt].Orientation;
                            tmpJoints.Add(jt,new JointData(new Vector3(pos.X,pos.Y,-pos.Z), new Quaternion(rot.X,rot.Y,rot.Z,rot.W)));
                        }else if(Joints != null && Joints.ContainsKey(jt))
                            tmpJoints.Add(jt,Joints[jt]);
                    }
                    var vrHandsDist = (VRHands[0].position - VRHands[1].position).sqrMagnitude;
                    if(vrHandsDist>=0.25f*0.25f)
                        AddCorrection((tmpJoints[JointType.HandRight].Position - tmpJoints[JointType.HandLeft].Position).sqrMagnitude/vrHandsDist);
                    UpdateJoints(tmpJoints);
                }
            }
        }
    }

    public Dictionary<JointType, JointData> JointsOld;
    public Dictionary<JointType, JointData> JointsNew;
    public Dictionary<JointType, JointData> Joints;

    public Action ekstrapolate;

    public Transform[] VRHands;
    public float correctionFactor => correctionQueue.Count > 0 ? correctionQueue.Average() : 1;

    Queue<float> correctionQueue = new Queue<float>();

    public void UpdateJoints(Dictionary<JointType, JointData> joints)
    {
        JointsOld = JointsNew;
        JointsNew = joints;
        Joints = JointsNew;
        ekstrapolate?.Invoke();
    }

    public void AddCorrection(float correction)
    {
        correctionQueue.Enqueue(correction);
        if (correctionQueue.Count > 60)
            correctionQueue.Dequeue();
    }

    public void InitCloseKinect()
    {
        if (_Reader != null)
        {
            _Reader.Dispose();
            _Reader = null;
        }

        if (_Sensor != null)
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close();
            }

            _Sensor = null;
        }
    }
}

public struct JointData{
    public Vector3 Position;
    public Quaternion Rotation;

    public JointData(Vector3 position, Quaternion rotation)
    {
        Position = position;
        Rotation = rotation;
    }
}
