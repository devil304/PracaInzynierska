using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Valve.VR;

public class SqueezeAndSelect : MonoBehaviour
{
    [SerializeField] SteamVR_Input_Sources source;
    [SerializeField] Transform rayOrigin;
    [SerializeField] LayerMask rayMask;

    List<pullEEG> cache = new List<pullEEG> ();
 
    // Update is called once per frame
    void Update()
    {
        SteamVR_Action_Single action = SteamVR_Actions._default.TriggerAnalog;
        float gripSqueeze = action.GetAxis(source);
        if (gripSqueeze > 0.5f)
        {
            RaycastHit[] hitsRes = new RaycastHit[10];
            int hits = Physics.SphereCastNonAlloc(rayOrigin.position,0.1f,rayOrigin.transform.forward,hitsRes,5f,rayMask);
            hitsRes = hitsRes.Where(x=>!x.Equals(default(RaycastHit))).ToArray();
            hitsRes = hitsRes.OrderByDescending(x=>x.distance).ToArray();

            List<pullEEG> cacheTmp = new List<pullEEG>();
            for (int i = 0; i < hits; i++)
            {
                var tmp = cache.FirstOrDefault(n => n.gameObject == hitsRes[i].collider.gameObject);
                if (tmp != default(pullEEG))
                {
                    cacheTmp.Add(tmp);
                }
                else {
                    pullEEG pullEEG;
                    if (hitsRes[i].collider.gameObject.TryGetComponent(out pullEEG))
                    {
                        cacheTmp.Add(pullEEG);
                    }
                }
            }
            var cacheDiff = cache.Except(cacheTmp);
            foreach (pullEEG p in cacheDiff)
                p.deactivate();
            foreach (pullEEG p in cacheTmp)
                p.activate(transform);
            cache.Clear();
            cache.AddRange(cacheTmp);
        }
    }
}
