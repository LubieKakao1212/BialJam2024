using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class FallingPrecipitation : MonoBehaviour
{
    [SerializeField]
    private Transform precipitationParticlePrototype;

    [SerializeField, Min(0)]
    private float spawnDelay;
    private float spawnTimer;

    [SerializeField, Min(0)]
    private float spawnWidth;

    [SerializeField, Min(0)]
    private float fallHeight;

    [SerializeField, Min(0)]
    private float gravity = 1;

    private ObjectPool<Transform> particlesPool;

    private List<(Transform particle, float speed)> fallingParticles = new List<(Transform particle, float speed)>();

    private void Awake()
    {
        particlesPool = new ObjectPool<Transform>(
            () => Instantiate(precipitationParticlePrototype, transform));
    }

    private void FixedUpdate()
    {
        float dt = Time.deltaTime;
        spawnTimer += dt;
        float halfWidth = spawnWidth / 2;
        if (spawnTimer > spawnDelay)
        {
            spawnTimer -= spawnDelay;
            var particle = particlesPool.Get();
            particle.gameObject.SetActive(true);
            particle.localPosition = new Vector3(Random.Range(-halfWidth, halfWidth), 0);
            fallingParticles.Add((particle, 0));
        }

        float halfGravity = gravity / 2;
        for (int i = fallingParticles.Count - 1; i >= 0; i--)
        {
            (Transform particle, float speed) precipitation = fallingParticles[i];
            precipitation.speed += halfGravity * dt;
            var position = precipitation.particle.localPosition;
            position.y -= precipitation.speed;
            precipitation.particle.localPosition = position;
            precipitation.speed += halfGravity * dt;
            fallingParticles[i] = precipitation;
            if (position.y < -fallHeight)
            {
                particlesPool.Release(precipitation.particle);
                precipitation.particle.gameObject.SetActive(false);
                fallingParticles.RemoveAt(i);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        var matrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(
            -new Vector3(0, fallHeight / 2), 
            new Vector3(spawnWidth, fallHeight));

        Gizmos.matrix = matrix;
    }
}
