using UnityEngine;

public class WindForceInflunce : MonoBehaviour
{
    [SerializeField]
    private WindMeasurePoint windMeasurePoint;
    [SerializeField]
    private CharacterController characterController;

    [Header("Thresholds")]
    [SerializeField, Min(0)]
    private float inAirWindInfluenceThreshold;
    [SerializeField, Min(0)]
    private float onGroundWindInfluenceThreshold;

    [Header("Multipliers")]
    [SerializeField, Min(0)]
    private float onGroundMultiplier;
    [SerializeField, Min(0)]
    private float inAirMultiplier;

    public float InfluenceMultiplier => characterController.isGrounded ? onGroundMultiplier : inAirMultiplier;

    private void Reset()
    {
        windMeasurePoint = GetComponentInChildren<WindMeasurePoint>();
        characterController = GetComponentInParent<CharacterController>();
    }

    private void Update()
    {
        var windVelocity = windMeasurePoint.WindVelocity;
        float windSpeedSqaured = windVelocity.sqrMagnitude;
        if (ShouldInfluenceOnGround(windSpeedSqaured) || ShouldInfluenceInAir(windSpeedSqaured))
        {
            float dt = Time.deltaTime;
            var windInfluence = new Vector3(windVelocity.x, 0, windVelocity.y);
            characterController.Move(windInfluence * dt);
        }
    }

    private bool ShouldInfluenceInAir(float windSpeedSqaured)
    {
        return characterController.isGrounded == false && windSpeedSqaured > inAirWindInfluenceThreshold * inAirWindInfluenceThreshold;
    }

    private bool ShouldInfluenceOnGround(float windSpeedSqaured)
    {
        return windSpeedSqaured > onGroundWindInfluenceThreshold * onGroundWindInfluenceThreshold;
    }

    private void OnValidate()
    {
        onGroundWindInfluenceThreshold = Mathf.Max(onGroundWindInfluenceThreshold, inAirWindInfluenceThreshold);
        inAirWindInfluenceThreshold = Mathf.Min(onGroundWindInfluenceThreshold, inAirWindInfluenceThreshold);

        onGroundMultiplier = Mathf.Min(onGroundMultiplier, inAirMultiplier);
        inAirMultiplier = Mathf.Max(onGroundMultiplier, inAirMultiplier);
    }
}
