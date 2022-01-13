using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libStreamSDK;

namespace thinkgear_testapp_csharp_64
{
    class Program
    {
        public static void Mine2()
        {

            NativeThinkgear thinkgear = new NativeThinkgear();

            /* Print driver version number */
            Trace.WriteLine("Version: " + NativeThinkgear.TG_GetVersion());

            /* Get a connection ID handle to ThinkGear */
            int connectionID = NativeThinkgear.TG_GetNewConnectionId();
            Trace.WriteLine("Connection ID: " + connectionID);

            if (connectionID < 0)
            {
                Trace.WriteLine("ERROR: TG_GetNewConnectionId() returned: " + connectionID);
                return;
            }

            int errCode = 0;
            /* Set/open stream (raw bytes) log file for connection */
            errCode = NativeThinkgear.TG_SetStreamLog(connectionID, "streamLog.txt");
            Trace.WriteLine("errCode for TG_SetStreamLog : " + errCode);
            if (errCode < 0)
            {
                Trace.WriteLine("ERROR: TG_SetStreamLog() returned: " + errCode);
                return;
            }

            /* Set/open data (ThinkGear values) log file for connection */
            errCode = NativeThinkgear.TG_SetDataLog(connectionID, "dataLog.txt");
            Trace.WriteLine("errCode for TG_SetDataLog : " + errCode);
            if (errCode < 0)
            {
                Trace.WriteLine("ERROR: TG_SetDataLog() returned: " + errCode);
                return;
            }

            /* Attempt to connect the connection ID handle to serial port "COM5" */
            string comPortName = "\\\\.\\COM7";

            errCode = NativeThinkgear.TG_Connect(connectionID,
                          comPortName,
                          NativeThinkgear.Baudrate.TG_BAUD_115200,
                          NativeThinkgear.SerialDataFormat.TG_STREAM_PACKETS);
            if (errCode < 0)
            {
                Trace.WriteLine("ERROR: TG_Connect() returned: " + errCode);
                return;
            }

            /* Read 10 ThinkGear Packets from the connection, 1 Packet at a time */
            int packetsRead = 0;
            while (packetsRead < 10)
            {

                /* Attempt to read a Packet of data from the connection */
                errCode = NativeThinkgear.TG_ReadPackets(connectionID, 1);
                Trace.WriteLine("TG_ReadPackets returned: " + errCode);
                /* If TG_ReadPackets() was able to read a complete Packet of data... */
                if (errCode == 1)
                {
                    packetsRead++;

                    /* If attention value has been updated by TG_ReadPackets()... */
                    if (NativeThinkgear.TG_GetValueStatus(connectionID, NativeThinkgear.DataType.TG_DATA_RAW) != 0)
                    {

                        /* Get and print out the updated attention value */
                        Trace.WriteLine("New RAW value: : " + (int)NativeThinkgear.TG_GetValue(connectionID, NativeThinkgear.DataType.TG_DATA_RAW));

                    } /* end "If attention value has been updated..." */

                } /* end "If a Packet of data was read..." */

            } /* end "Read 10 Packets of data from connection..." */

            Trace.WriteLine("auto read test begin:");

            errCode = NativeThinkgear.TG_EnableAutoRead(connectionID, 1);
            if (errCode == 0)
            {
                packetsRead = 0;
                errCode = NativeThinkgear.MWM15_setFilterType(connectionID, NativeThinkgear.FilterType.MWM15_FILTER_TYPE_60HZ);
                Trace.WriteLine("MWM15_setFilterType called: " + errCode);
                while (packetsRead < 20000) // it use as time
                {
                    /* If raw value has been updated ... */
                    if (NativeThinkgear.TG_GetValueStatus(connectionID, NativeThinkgear.DataType.TG_DATA_RAW) != 0)
                    {
                        if (NativeThinkgear.TG_GetValueStatus(connectionID, NativeThinkgear.DataType.MWM15_DATA_FILTER_TYPE) != 0)
                        {
                            Trace.WriteLine(" Find Filter Type:  " + NativeThinkgear.TG_GetValue(connectionID, NativeThinkgear.DataType.MWM15_DATA_FILTER_TYPE) + " index: " + packetsRead);
                            //break;
                        }

                        /* Get and print out the updated raw value */
                        
                         //NativeThinkgear.TG_GetValue(connectionID, NativeThinkgear.DataType.TG_DATA_RAW);
                        Trace.WriteLine("New RAW value: : " + (int)NativeThinkgear.TG_GetValue(connectionID, NativeThinkgear.DataType.TG_DATA_RAW));

                        packetsRead++;

                        if (packetsRead == 800 || packetsRead == 1600)  // call twice interval than 1s (512)
                        {
                            errCode = NativeThinkgear.MWM15_getFilterType(connectionID);
                            Trace.WriteLine(" MWM15_getFilterType called: " + errCode);
                        }

                    }

                    
                }

                errCode = NativeThinkgear.TG_EnableAutoRead(connectionID, 0); //stop
                Trace.WriteLine("auto read test stoped: "+ errCode);
            }
            else
            {
                Trace.WriteLine("auto read test failed: " + errCode);
            }

            NativeThinkgear.TG_Disconnect(connectionID); // disconnect test

            /* Clean up */
            NativeThinkgear.TG_FreeConnection(connectionID);

            /* End program */
            //Trace.ReadLine();

        }
    }
}
