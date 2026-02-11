using Assets.Codes.Game;
using Assets.Codes.Systems.FlowFieldSystem;
using UnityEngine;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
    [SerializeField] public GameObject SelectedMark;
    public FlowField flowField;

    [Header("Movement")]
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float maxForce = 50f;

    [Header("Arrive (FlowField Cost)")]
    [SerializeField] private float arriveCost = 1.5f;   // FlowField cost，而不是世界距离
    [SerializeField] private float stopSpeed = 0.1f;

    [Header("Separation")]
    [SerializeField] private float separationRadius = 0.5f;
    [SerializeField] private float separationWeight = 150f;

    [Header("Damping")]
    [SerializeField] private float damping = 5f;

    private Vector3 velocity;
    private float arriveCostSq;
    private float separationRadiusSq;

    void Start()
    {
        arriveCostSq = arriveCost * arriveCost;
        separationRadiusSq = separationRadius * separationRadius;

        GameManager.Instance.UnitList.Add(this);
        SelectedMark.SetActive(false);
        velocity = Vector3.zero;
    }

    void Update()
    {
        if (flowField == null) return;

        Vector3 steering = Vector3.zero;

        // ---------- FlowField + Arrive ----------
        Vector3 dir = flowField.GetDir(transform.position);
        float costSq = flowField.GetDistanceSq(transform.position);

        if (costSq > arriveCostSq)
        {
            Vector3 desiredVelocity = dir * maxSpeed;
            steering += desiredVelocity - velocity;
        }
        else
        {
            // 靠近目标，主动制动
            steering += -velocity;
        }

        // ---------- Separation ----------
        steering += CalculateSeparation() * separationWeight;

        // ---------- Clamp ----------
        steering = Vector3.ClampMagnitude(steering, maxForce);

        // ---------- Integrate ----------
        velocity += steering * Time.deltaTime;

        // 只有在几乎没有 steering 时才做阻尼
        if (steering.sqrMagnitude < 0.001f)
        {
            velocity = Vector3.Lerp(velocity, Vector3.zero, damping * Time.deltaTime);
        }

        if (velocity.magnitude < stopSpeed)
            velocity = Vector3.zero;

        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        transform.position += velocity * Time.deltaTime;

        if (velocity.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(velocity);
    }

    Vector3 CalculateSeparation()
    {
        Vector3 force = Vector3.zero;
        int count = 0;

        foreach (var other in GameManager.Instance.UnitList)
        {
            if (other == this) continue;

            Vector3 delta = transform.position - other.transform.position;
            float distSq = delta.sqrMagnitude;

            if (distSq > 0 && distSq < separationRadiusSq)
            {
                // Steering 层：方向性分离（不做 sqrt）
                force += delta / distSq;
                count++;

                // 约束层：极近距离强制推开，防重叠
                if (distSq < separationRadiusSq * 0.25f)
                {
                    transform.position += delta.normalized * 0.01f;
                }
            }
        }

        if (count > 0)
            force /= count;

        return force;
    }

    public void SetSelected(bool isSelected)
    {
        SelectedMark.SetActive(isSelected);
    }
}
