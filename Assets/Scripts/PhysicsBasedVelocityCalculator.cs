using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simulates physics-based velocity using applied continuous and impulse forces.
/// </summary>
public class PhysicsBasedVelocityCalculator : MonoBehaviour
{
    [Header("Physics Settings")]
    [Range(1f, 50f)]
    [SerializeField] private float mass = 1f;
    public float Mass
    {
        get => mass;
        set => mass = Mathf.Max(0.001f, value); // prevent divide-by-zero
    }

    // Velocity calculated from forces
    private Vector3 physicsVelocity=Vector3.zero;
    public Vector3 PhysicsVelocity { get => physicsVelocity; set => physicsVelocity = value; }

    // Forces that persist every frame (e.g., gravity)
    private readonly List<Vector3> continuousForces = new();
    public IReadOnlyList<Vector3> ContinuousForces => continuousForces;

    // One-time instant forces (e.g., explosion, jump)
    private readonly List<Vector3> impulseForces = new();
    public IReadOnlyList<Vector3> ImpulseForces => impulseForces;

    private void FixedUpdate()
    {
        physicsVelocity += CalculateVelocity(Time.fixedDeltaTime);
    }

    /// <summary>
    /// Calculates net force from all continuous and impulse sources.
    /// </summary>
    private Vector3 GetNetForce()
    {
        Vector3 totalForce = Vector3.zero;

        foreach (var force in continuousForces)
            totalForce += force;

        foreach (var impulse in impulseForces)
            totalForce += impulse;

        impulseForces.Clear(); // Clear after one use
        return totalForce;
    }

    /// <summary>
    /// Calculates acceleration from net force.
    /// </summary>
    private Vector3 GetAcceleration()
    {
        Vector3 netForce = GetNetForce();
      
        return netForce / Mathf.Max(mass, 0.0001f); // Avoid divide by zero
    }

    /// <summary>
    /// Calculates velocity change based on acceleration.
    /// </summary>
    private Vector3 CalculateVelocity(float deltaTime)
    {
        Vector3 acceleration = GetAcceleration();
        return acceleration * deltaTime;
    }

    // Public methods to safely apply forces
    public void AddForce(Vector3 force) => continuousForces.Add(force);
    public void AddImpulse(Vector3 impulse) => impulseForces.Add(impulse);
    public void SetContinuousForces(params Vector3[] newForces)
    {
        continuousForces.Clear();
        continuousForces.AddRange(newForces);
    }

    public void ResetVelocity() => physicsVelocity = Vector3.zero;
}
