using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayEEGVal : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI Poor, Raw, Attention, Meditation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Poor.text = EEGDataExchange.Poor.ToString();
        Raw.text = EEGDataExchange.Raw.ToString();
        Attention.text = EEGDataExchange.Attension.ToString();
        Meditation.text = EEGDataExchange.Meditation.ToString();
    }
}
