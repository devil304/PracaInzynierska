using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ThreadLoader : MonoBehaviour
{
    Thread EEG;
    // Start is called before the first frame update
    void Start()
    {
        EEGThread EEGT = new EEGThread();
        EEG = new Thread(EEGT.Start);
        EEG.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        EEGDataExchange.InitCloseEEG();
    }
}
