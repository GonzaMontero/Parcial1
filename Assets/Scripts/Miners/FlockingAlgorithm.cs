using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Xml.Linq;

public class FlockingAlgorithm : MonoBehaviour
{
    public Vector3 BaseRotation;
    public float MaxSpeed;
    public float MaxForce;
    public float CheckRadius;

    public float SeparationMultiplier;
    public float CohesionMultiplier;
    public float AlignmentMultiplier;
    public float DirectionMultiplier;

    public Vector2 velocity;
    public Vector2 acceleration;

    private Action<Vector2> onUpdatePos;
    private Func<Vector2> onGetPos;
    private bool flockingEnabled = false;
    private Vector2 target;

    private void Start()
    {
        float angle = Random.Range(0, 2 * Mathf.PI);
        velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    private void Update()
    {
        if (!flockingEnabled) return;

        Vector2 currentPos = onGetPos.Invoke();

        Collider2D[] otherColliders = Physics2D.OverlapCircleAll(currentPos, CheckRadius);
        List<FlockingAlgorithm> boids = otherColliders.Select(others => others.GetComponent<FlockingAlgorithm>()).ToList();
        boids.Remove(this);

        if (boids.Any())
            acceleration = Alignment(boids) * AlignmentMultiplier + Separation(boids) * 
                SeparationMultiplier + Cohesion(boids) * CohesionMultiplier + 
                DirectionToTarget() * DirectionMultiplier;
        else
            acceleration = DirectionToTarget();

        velocity += acceleration;
        velocity = LimitMagnitude(velocity, MaxSpeed);
        currentPos += velocity * Time.deltaTime;
        onUpdatePos?.Invoke(currentPos);

        float newAngle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
    }

    #region Boid Movements
    /// <summary>
    /// Average boids velocities and applies steering to match desired direction
    /// </summary>
    private Vector2 Alignment(IEnumerable<FlockingAlgorithm> boids)
    {
        Vector2 velocity = Vector2.zero;

        foreach(var boid in boids)
        {
            velocity += boid.velocity;
        }

        velocity /= boids.Count();

        var steer = Steer(velocity.normalized * MaxSpeed);
        return steer;
    }

    /// <summary>
    /// Average position of neighboring boids and steers towards average position
    /// </summary>
    private Vector2 Cohesion(IEnumerable<FlockingAlgorithm> boids)
    {
        Vector2 sumPositions = Vector2.zero;

        foreach (var boid in boids)
        {
            sumPositions += boid.onGetPos.Invoke();
        }

        Vector2 average = sumPositions / boids.Count();
        Vector2 direction = average - onGetPos.Invoke();

        return Steer(direction.normalized * MaxSpeed);
    }

    /// <summary>
    /// Calculates boid separation by steering away from nearby boids
    /// </summary>
    private Vector2 Separation(IEnumerable<FlockingAlgorithm> boids)
    {
        Vector2 direction = Vector2.zero;
        boids = boids.Where(o => DistanceTo(o) <= CheckRadius / 2);

        foreach (var boid in boids)
        {
            Vector2 difference = onGetPos.Invoke() - boid.onGetPos.Invoke();
            direction += difference.normalized / difference.magnitude;
        }

        direction /= boids.Count();

        return Steer(direction.normalized * MaxSpeed);
    }

    /// <summary>
    /// Calculates steering force for boid while checking current velocity and limiting force
    /// </summary>
    private Vector2 Steer(Vector2 desired)
    {
        var steer = desired - velocity;
        steer = LimitMagnitude(steer, MaxForce);

        return steer;
    }
    #endregion

    #region Utils
    private float DistanceTo(FlockingAlgorithm boid)
    {
        return Vector3.Distance(boid.transform.position, onGetPos.Invoke());
    }

    private Vector2 LimitMagnitude(Vector2 baseVector, float maxMagnitude)
    {
        if (baseVector.sqrMagnitude > maxMagnitude * maxMagnitude)
        {
            baseVector = baseVector.normalized * maxMagnitude;
        }

        return baseVector;
    }

    private Vector2 DirectionToTarget()
    {
        if (target == null)
        {
            return Vector2.zero;
        }

        return (target - onGetPos.Invoke()).normalized;
    }
    #endregion

    #region Public Access Methods
    public void Init(Action<Vector2> onUpdatePos, Func<Vector2> onGetPos)
    {
        this.onUpdatePos = onUpdatePos;
        this.onGetPos = onGetPos;
    }

    public void UpdateTarget(Vector2 target)
    {
        this.target = target;
    }

    public void ToggleFlocking(bool enabled)
    {
        flockingEnabled = enabled;
    }
    #endregion
}
