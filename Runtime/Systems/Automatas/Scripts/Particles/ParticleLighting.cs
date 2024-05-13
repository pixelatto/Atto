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
        List<Vector2> lightPoints = new List<Vector2>();
        List<float> lightRadius = new List<float>();
        List<float> lightEmissions = new List<float>();

        for (int i = 0; i < ParticleAutomata.instance.particles.Count; i++)
        {
            var particle = ParticleAutomata.instance.particles[i];
            if (particle.lightEmission > 0 && particle.lightRadius > 0)
            {
                lightPoints.Add(particle.position);
                lightRadius.Add(particle.lightRadius);
                lightEmissions.Add(particle.lightEmission);
            }
        }

        var pixelCamera = CellularAutomata.instance.pixelCamera;

        for (int j = pixelCamera.lookAheadPixelRect.y; j <= pixelCamera.lookAheadPixelRect.y + pixelCamera.lookAheadPixelRect.height; j++)
        {
            for (int i = pixelCamera.lookAheadPixelRect.x; i <= pixelCamera.lookAheadPixelRect.x + pixelCamera.lookAheadPixelRect.width; i++)
            {
                var currentPosition = new Vector2Int(i, j);
                var currentCell = CellularAutomata.instance.GetCell(currentPosition);
                if (currentCell != null  && !currentCell.IsEmpty() && currentCell.lightEmission > 0 && currentCell.lightRadius > 0)
                {
                    lightPoints.Add(CellularAutomata.PixelToWorldPosition(currentPosition));
                    lightRadius.Add(currentCell.lightRadius);
                    lightEmissions.Add(currentCell.lightEmission);
                }
            }
        }

        var lightPointsCount = lightPoints.Count;
        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
        emitParams.position = Vector3.down * 1000;
        ps.Emit(emitParams, lightPointsCount);
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[lightPointsCount];
        ps.GetParticles(particles);

        for (int i = 0; i < lightPointsCount; i++)
        {
            particles[i].position = lightPoints[i];
            particles[i].startSize = lightRadius[i];
            particles[i].startColor = new Color(1, 1, 1, lightEmissions[i]);
        }
        ps.SetParticles(particles, lightPointsCount);
    }
}
