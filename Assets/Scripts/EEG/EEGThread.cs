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
    int connectionID;

    public void Start()
    {
        connectionID = NativeThinkgear.TG_GetNewConnectionId();
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
        Debug.Log("Final init Code: " + errCode + " : " + i);

        int packetsRead = 0;
        while (packetsRead < 111)
        {
            errCode = NativeThinkgear.TG_ReadPackets(connectionID, 1);
            Debug.Log("TG_ReadPackets returned: " + errCode);
            if (errCode == 1)
            {
                packetsRead++;
                if (NativeThinkgear.TG_GetValueStatus(connectionID, NativeThinkgear.DataType.TG_DATA_RAW) != 0)
                {
                    Debug.Log("New RAW value: : " + (int)NativeThinkgear.TG_GetValue(connectionID, NativeThinkgear.DataType.TG_DATA_RAW));
                }
                if (NativeThinkgear.TG_GetValueStatus(connectionID, NativeThinkgear.DataType.TG_DATA_ATTENTION) != 0)
                    break;
            }
        }
        if (packetsRead == 111 && NativeThinkgear.TG_GetValueStatus(connectionID, NativeThinkgear.DataType.TG_DATA_ATTENTION) == 0)
        {
            Debug.LogError("Can't get full packet in 10 attempts, STOPPING");
            return;
        }
        AutoReadData();
    }

    void AutoReadData()
    {
        Debug.Log("auto read begin:");
        int errCode = 0;
        errCode = NativeThinkgear.TG_EnableAutoRead(connectionID, 1);
        if (errCode == 0)
        {
            errCode = NativeThinkgear.MWM15_setFilterType(connectionID, NativeThinkgear.FilterType.MWM15_FILTER_TYPE_60HZ);
            Debug.Log("MWM15_setFilterType called: " + errCode);
            while (!_end)
            {
                if (NativeThinkgear.TG_GetValueStatus(connectionID, NativeThinkgear.DataType.TG_DATA_RAW) != 0)
                {
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

                    if (Poor == 0 && Math.Abs(Raw) < 300 && Math.Abs(EEGDataExchange.Raw) < 300 && Math.Abs(Raw - EEGDataExchange.Raw) < 128)
                    {
                        EEGDataExchange.Attension = Meditation > Attension ? 0 : Attension - Meditation;
                        EEGDataExchange.Meditation = Meditation > Attension ? Meditation - Attension : 0;
                        EEGDataExchange.OnEEGUpdate?.Invoke();
                    }
                    sb.Append($"Calculated Attension: {EEGDataExchange.Attension}, calculated Meditation: {EEGDataExchange.Meditation}");

                    EEGDataExchange.DataInfo = sb.ToString();
                    EEGDataExchange.Raw = Raw;
                    EEGDataExchange.Poor = Poor;
                }
            }

            errCode = NativeThinkgear.TG_EnableAutoRead(connectionID, 0); //stop auto read
            Debug.Log("auto read stoped: " + errCode);

            NativeThinkgear.TG_Disconnect(connectionID); // disconnect

            //Clean up
            NativeThinkgear.TG_FreeConnection(connectionID);
            return;
        }
        else
        {
            Debug.Log("auto read failed: " + errCode);
        }
    }
}
