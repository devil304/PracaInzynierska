using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NeuroSky.ThinkGear;
using libStreamSDK;
using System.Threading;
using System.Text;

public class EEGThread
{
    bool _end = false;

    public void Start()
    {
        NativeThinkgear thinkgear = new NativeThinkgear();

        /* Print driver version number */
        Debug.Log("Version: " + NativeThinkgear.TG_GetVersion());

        /* Get a connection ID handle to ThinkGear */
        int connectionID = NativeThinkgear.TG_GetNewConnectionId();
        Debug.Log("Connection ID: " + connectionID);
        EEGDataExchange.CloseEEG += () =>
        {
            _end = true;
        };

        if (connectionID < 0)
        {
            Debug.LogError("ERROR: TG_GetNewConnectionId() returned: " + connectionID);
            return;
        }

        int errCode = 0;
        /* Set/open stream (raw bytes) log file for connection */
        errCode = NativeThinkgear.TG_SetStreamLog(connectionID, "streamLog.txt");
        Debug.Log("errCode for TG_SetStreamLog : " + errCode);
        if (errCode < 0)
        {
            Debug.LogError("ERROR: TG_SetStreamLog() returned: " + errCode);
            return;
        }

        /* Set/open data (ThinkGear values) log file for connection */
        errCode = NativeThinkgear.TG_SetDataLog(connectionID, "dataLog.txt");
        Debug.Log("errCode for TG_SetDataLog : " + errCode);
        if (errCode < 0)
        {
            Debug.LogError("ERROR: TG_SetDataLog() returned: " + errCode);
            return;
        }

        /* Attempt to connect the connection ID handle to serial ports "COMx" */
        int i = 0;
        for (int y = 0; y < 3; y++)
        {
            i = 0;
            do
            {
                for (int x = 0; x < 3; x++)
                {
                    if (_end) return;
                    Thread.Sleep(50);
                    string comPortName = "\\\\.\\COM" + i;

                    errCode = NativeThinkgear.TG_Connect(connectionID,
                                  comPortName,
                                  NativeThinkgear.Baudrate.TG_BAUD_115200,
                                  NativeThinkgear.SerialDataFormat.TG_STREAM_PACKETS);
                    if (errCode < 0)
                    {
                        Debug.LogError("ERROR: TG_Connect() returned: " + errCode);
                        //return;
                    }
                    else
                    {
                        errCode = NativeThinkgear.TG_ReadPackets(connectionID, 1);
                        Debug.Log("TG_ReadPackets returned: " + errCode);
                        if (errCode < 0)
                        {
                            NativeThinkgear.TG_Disconnect(connectionID);
                        }
                    }
                    if (errCode >= 0) break;
                }
                if (errCode < 0) i++;
            } while (errCode < 0 && i < 20);
            if (errCode >=0) break;
        }

        if (i >= 20)
        {
            Debug.LogError("ERROR: Too many ports scanned");
            return;
        }
        Debug.Log("TG_Connect() returned: " + errCode + " : " + i);

        /* Read 10 ThinkGear Packets from the connection, 1 Packet at a time */
        int packetsRead = 0;
        while (packetsRead < 10)
        {

            /* Attempt to read a Packet of data from the connection */
            errCode = NativeThinkgear.TG_ReadPackets(connectionID, 1);
            Debug.Log("TG_ReadPackets returned: " + errCode);
            /* If TG_ReadPackets() was able to read a complete Packet of data... */
            if (errCode == 1)
            {
                packetsRead++;

                /* If attention value has been updated by TG_ReadPackets()... */
                if (NativeThinkgear.TG_GetValueStatus(connectionID, NativeThinkgear.DataType.TG_DATA_RAW) != 0)
                {

                    /* Get and print out the updated attention value */
                    Debug.Log("New RAW value: : " + (int)NativeThinkgear.TG_GetValue(connectionID, NativeThinkgear.DataType.TG_DATA_RAW));

                } /* end "If attention value has been updated..." */

            } /* end "If a Packet of data was read..." */

        } /* end "Read 10 Packets of data from connection..." */

        Debug.Log("auto read test begin:");

        errCode = NativeThinkgear.TG_EnableAutoRead(connectionID, 1);
        if (errCode == 0)
        {
            packetsRead = 0;
            errCode = NativeThinkgear.MWM15_setFilterType(connectionID, NativeThinkgear.FilterType.MWM15_FILTER_TYPE_60HZ);
            Debug.Log("MWM15_setFilterType called: " + errCode);
            while (!_end) // it use as time
            {
                /* If raw value has been updated ... */
                if (NativeThinkgear.TG_GetValueStatus(connectionID, NativeThinkgear.DataType.TG_DATA_RAW) != 0)
                {
                    if (NativeThinkgear.TG_GetValueStatus(connectionID, NativeThinkgear.DataType.MWM15_DATA_FILTER_TYPE) != 0)
                    {
                        Debug.Log(" Find Filter Type:  " + NativeThinkgear.TG_GetValue(connectionID, NativeThinkgear.DataType.MWM15_DATA_FILTER_TYPE) + " index: " + packetsRead);
                        //break;
                    }

                    /* Get and print out the updated raw value */

                    //NativeThinkgear.TG_GetValue(connectionID, NativeThinkgear.DataType.TG_DATA_RAW);
                    //Debug.Log("New RAW value: : " + (int)NativeThinkgear.TG_GetValue(connectionID, NativeThinkgear.DataType.TG_DATA_RAW));
                    StringBuilder sb = new StringBuilder();
                    var statusR = NativeThinkgear.TG_GetValueStatus(connectionID, NativeThinkgear.DataType.TG_DATA_RAW);
                    var Raw = NativeThinkgear.TG_GetValue(connectionID, NativeThinkgear.DataType.TG_DATA_RAW);
                    sb.Append($"Raw status: {statusR}, Raw data: {Raw}; \n");
                    var statusP = NativeThinkgear.TG_GetValueStatus(connectionID, NativeThinkgear.DataType.TG_DATA_POOR_SIGNAL);
                    var Poor = NativeThinkgear.TG_GetValue(connectionID, NativeThinkgear.DataType.TG_DATA_POOR_SIGNAL);
                    sb.Append($"Poor status: {statusP}, Poor data: {Poor}; \n");
                    var statusA = NativeThinkgear.TG_GetValueStatus(connectionID, NativeThinkgear.DataType.TG_DATA_ATTENTION);
                    var Attension = NativeThinkgear.TG_GetValue(connectionID, NativeThinkgear.DataType.TG_DATA_ATTENTION);
                    sb.Append($"Attension status: {statusA}, Attension data: {Attension}; \n");
                    var statusM = NativeThinkgear.TG_GetValueStatus(connectionID, NativeThinkgear.DataType.TG_DATA_MEDITATION);
                    var Meditation = NativeThinkgear.TG_GetValue(connectionID, NativeThinkgear.DataType.TG_DATA_MEDITATION);
                    sb.Append($"Meditation status: {statusM}, Meditation data: {Meditation}; \n");

                    if (Poor==0 && Math.Abs(Raw) < 300 && Math.Abs(EEGDataExchange.Raw)< 300 && Math.Abs(Raw - EEGDataExchange.Raw)<128)
                    {
                        EEGDataExchange.Attension = Meditation>Attension?0:Attension-Meditation;
                        EEGDataExchange.Meditation = Meditation>Attension?Meditation-Attension:0;
                        EEGDataExchange.OnEEGUpdate?.Invoke();
                    }
                    sb.Append($"Calculated Attension: {EEGDataExchange.Attension}, calculated Meditation: {EEGDataExchange.Meditation}");

                    EEGDataExchange.DataInfo = sb.ToString();
                    EEGDataExchange.Raw = Raw;
                    EEGDataExchange.Poor = Poor;


                    packetsRead++;

                    /*if (packetsRead == 800 || packetsRead == 1600)  // call twice interval than 1s (512)
                    {
                        errCode = NativeThinkgear.MWM15_getFilterType(connectionID);
                        Debug.Log(" MWM15_getFilterType called: " + errCode);
                    }*/

                }


            }

            errCode = NativeThinkgear.TG_EnableAutoRead(connectionID, 0); //stop
            Debug.Log("auto read test stoped: " + errCode);

            NativeThinkgear.TG_Disconnect(connectionID); // disconnect test

            //Clean up
            NativeThinkgear.TG_FreeConnection(connectionID);
            return;
        }
        else
        {
            Debug.Log("auto read test failed: " + errCode);
        }
    }

    /*private Connector connector;

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
        *//* Cast the event sender as a Device object, and e as the Device's DataEventArgs *//*
        Device d = (Device)sender;
        Device.DataEventArgs de = (Device.DataEventArgs)e;

        *//* Create a TGParser to parse the Device's DataRowArray[] *//*
        TGParser tgParser = new TGParser();
        tgParser.Read(de.DataRowArray);

        *//* Loop through parsed data TGParser for its parsed data... *//*
        for (int i = 0; i < tgParser.ParsedData.Length; i++)
        {

            // See the Data Types documentation for valid keys such
            // as "Raw", "PoorSignal", "Attention", etc.

            if (tgParser.ParsedData[i].ContainsKey("Raw"))
            {
                //Debug.Log("Raw Value:" + tgParser.ParsedData[i]["Raw"]);
                EEGDataExchange.Raw = (float)tgParser.ParsedData[i]["Raw"];
            }

            if (tgParser.ParsedData[i].ContainsKey("PoorSignal"))
            {
                //Debug.Log("PQ Value:" + tgParser.ParsedData[i]["PoorSignal"]);
                EEGDataExchange.Poor = (float)tgParser.ParsedData[i]["PoorSignal"];
            }

            if (tgParser.ParsedData[i].ContainsKey("Attention"))
            {
                //Debug.Log("Att Value:" + tgParser.ParsedData[i]["Attention"]);
                EEGDataExchange.Attension = (float)tgParser.ParsedData[i]["Attention"];
            }

            if (tgParser.ParsedData[i].ContainsKey("Meditation"))
            {
                //Debug.Log("Med Value:" + tgParser.ParsedData[i]["Meditation"]);
                EEGDataExchange.Meditation = (float)tgParser.ParsedData[i]["Meditation"];
            }


        }
    }*/
}
