using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NeuroSky.ThinkGear;

public class EEGThread
{
    private Connector connector;

    public void Start()
    {
        Debug.Log("Started EEG Thread");
        EEGDataExchange.CloseEEG += () => { connector.Close(); };
        connector = new Connector();
        connector.DeviceConnected += new EventHandler(OnDeviceConnected);
        connector.DeviceFound += new EventHandler(OnDeviceFound);
        connector.DeviceNotFound += new EventHandler(OnDeviceNotFound);
        connector.DeviceConnectFail += new EventHandler(OnDeviceConnectFail);
        connector.DeviceDisconnected += new EventHandler(OnDeviceDisconnected);
        connector.DeviceValidating += new EventHandler(OnDeviceValidating);
        connector.ConnectScan();
    }

    private void OnDeviceNotFound(object sender, EventArgs e)
    {
        Debug.LogWarning("NotFound");
    }

    private void OnDeviceValidating(object sender, EventArgs e)
    {
        Debug.Log("Validating");
    }

    private void OnDeviceDisconnected(object sender, EventArgs e)
    {
        Debug.Log("Disconnected");
    }

    private void OnDeviceConnectFail(object sender, EventArgs e)
    {
        Debug.LogWarning("ConnectFail");
    }

    private void OnDeviceFound(object sender, EventArgs e)
    {
        Debug.Log("Found");
    }

    private void OnDeviceConnected(object sender, EventArgs e)
    {
        Debug.Log("Connected");
        Connector.DeviceEventArgs deviceEventArgs = (Connector.DeviceEventArgs)e;
        Debug.Log("New Headset Created." + deviceEventArgs.Device.PortName);

        deviceEventArgs.Device.DataReceived += new EventHandler(OnDataReceived);
    }

    private void OnDataReceived(object sender, EventArgs e)
    {
        Debug.Log("DataReceived");
        /* Cast the event sender as a Device object, and e as the Device's DataEventArgs */
        Device d = (Device)sender;
        Device.DataEventArgs de = (Device.DataEventArgs)e;

        /* Create a TGParser to parse the Device's DataRowArray[] */
        TGParser tgParser = new TGParser();
        tgParser.Read(de.DataRowArray);

        /* Loop through parsed data TGParser for its parsed data... */
        for (int i = 0; i < tgParser.ParsedData.Length; i++)
        {

            // See the Data Types documentation for valid keys such
            // as "Raw", "PoorSignal", "Attention", etc.

            if (tgParser.ParsedData[i].ContainsKey("Raw"))
            {
                //Debug.Log("Raw Value:" + tgParser.ParsedData[i]["Raw"]);
            }

            if (tgParser.ParsedData[i].ContainsKey("PoorSignal"))
            {
                Debug.Log("PQ Value:" + tgParser.ParsedData[i]["PoorSignal"]);
            }

            if (tgParser.ParsedData[i].ContainsKey("Attention"))
            {
                Debug.Log("Att Value:" + tgParser.ParsedData[i]["Attention"]);
            }

            if (tgParser.ParsedData[i].ContainsKey("Meditation"))
            {
                Debug.Log("Med Value:" + tgParser.ParsedData[i]["Meditation"]);
            }


        }
    }
}
