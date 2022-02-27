using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayEEGVal : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI Poor, Raw, Attention, Meditation, DataBlock;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Poor)
            Poor.text = EEGDataExchange.Poor.ToString();
        if(Raw)
            Raw.text = EEGDataExchange.Raw.ToString();
        if(Attention)
            Attention.text = EEGDataExchange.GameAttention.ToString();
        if(Meditation)
            Meditation.text = EEGDataExchange.GameMeditation.ToString();
        if(DataBlock)
            DataBlock.text = EEGDataExchange.DataInfo.ToString();
    }
}
