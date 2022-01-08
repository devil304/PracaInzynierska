using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pullEEG : MonoBehaviour
{
    [SerializeField] float attensionThreshold = 0.25f;
    [SerializeField] float decreaseTime = 2f;
    [SerializeField] Rigidbody rb;
    [SerializeField] float speedMultiplier = 5f;
    [SerializeField] float speedDecreasePerSec = 0.5f;
    [SerializeField] float maxDistFromTarget = 7.5f;

    public float speed;
    Transform target;
    bool active = false;
    Material myMat;
    float intensityBase;

    private void OnEnable()
    {
        myMat = GetComponent<Renderer>().material;
        intensityBase = myMat.GetFloat("_EmissiveIntensity");
    }

    public void activate(Transform targ)
    {
        if (!active)
        {
            myMat.SetFloat("_EmissiveIntensity", intensityBase*1.5f);
            target = targ;
            active = true;
            EEGDataExchange.OnEEGUpdate += NewEEGData;
        }
    }

    public void deactivate()
    {
        if (active)
        {
            myMat.SetFloat("_EmissiveIntensity",intensityBase);
            active = false;
            if (rb.isKinematic)
                rb.isKinematic = false;
            speed = 0;
            EEGDataExchange.OnEEGUpdate -= NewEEGData;
        }
    }

    private void NewEEGData()
    {
        if (EEGDataExchange.Attension >= attensionThreshold)
        {
            speed = EEGDataExchange.Attension;
        }
    }

    void Update()
    {
        if (!target)
        {
            if (rb.isKinematic)
                rb.isKinematic = false;
            return;
        }
        if ((target.position - transform.position).sqrMagnitude < 0.025f * 0.025f) return;

        if ((target.position - transform.position).sqrMagnitude > maxDistFromTarget * maxDistFromTarget)
        {
            speed = 0;
            deactivate();
        }
        if (speed > 0)
        {
            if(!rb.isKinematic)
                rb.isKinematic = true;
            rb.MovePosition(transform.position + ((target.position - transform.position).normalized * (speed/100f) * speedMultiplier * Time.deltaTime));
            myMat.SetFloat("_EmissiveIntensity", intensityBase + (intensityBase*0.5f* (speed / 100f)));
            speed -= Time.deltaTime * speedDecreasePerSec;
        }
        else if (speed <= 0)
        {
            speed = 0;
            if (rb.isKinematic)
                rb.isKinematic = false;
        }
    }
}
