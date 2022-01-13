using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class ThreadLoader
{
    Thread EEG;
    // Start is called before the first frame update
    void Start()
    {
        EEGThread EEGT = new EEGThread();
        EEG = new Thread(EEGT.Start);
        EEG.Start();
    }
}
