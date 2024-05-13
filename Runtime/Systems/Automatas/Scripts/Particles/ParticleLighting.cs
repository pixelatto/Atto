using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleLighting : MonoBehaviour
{
    ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        
    }

    void Update()
    {
        var particleCount = ParticleAutomata.instance.particles.Count;
        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
        emitParams.position = Vector3.down * 1000;
        ps.Emit(emitParams, particleCount);
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleCount];
        ps.GetParticles(particles);

        for (int i = 0; i < particleCount; i++)
        {
            var particle = ParticleAutomata.instance.particles[i];
            if (particle.lightEmission > 0)
            {
                particles[i].position = particle.position;
                particles[i].size = particle.lightEmission;
            }
        }
        ps.SetParticles(particles, particleCount);
    }
}
