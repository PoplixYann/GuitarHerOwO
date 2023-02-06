using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NiceZoneEffect : MonoBehaviour
{
    ParticleSystem particle;

    void Start()
    {
        particle = GetComponentInChildren<ParticleSystem>();
    }

    void Update()
    {
        if (!particle.IsAlive())
        {
            Destroy(gameObject);
        }
    }
}
