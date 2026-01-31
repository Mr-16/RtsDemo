using Assets.Codes.Game;
using Assets.Codes.Systems.FlowFieldSystem;
using UnityEngine;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
    [SerializeField] public GameObject SelectedMark;
    public FlowField flowField;

    [SerializeField] private float maxSpeed = 10f;       // 最大速度
    [SerializeField] private float maxForce = 100f;       // 最大加速度

    public float separationRadius = 0.3f;
    public float separationWeight = 115f;

    private Vector3 velocity;

    void Start()
    {
        GameManager.Instance().UnitList.Add(this);
        SelectedMark.SetActive(false);
        velocity = Vector3.zero;
    }

    void Update()
    {
        if (flowField == null) return;

        // 1. Desired velocity from Flow Field
        Vector3 desired = flowField.GetDir(transform.position).normalized * maxSpeed;

        //// 2. Separation force
        //Vector3 separation = Vector3.zero;
        //foreach (var other in GameManager.Instance().UnitList)
        //{
        //    if (other == this) continue;
        //    Vector3 toOther = transform.position - other.transform.position;
        //    float dist = toOther.magnitude;
        //    if (dist < separationRadius && dist > 0f)
        //    {
        //        separation += toOther.normalized / dist;
        //    }
        //}
        //separation *= separationWeight;

        //// 3. Steering = Desired - Velocity
        //Vector3 steering = (desired + separation) - velocity;
        //steering = Vector3.ClampMagnitude(steering, maxForce);

        // 4. Update velocity and position
        //velocity = Vector3.ClampMagnitude(velocity + steering * Time.deltaTime, maxSpeed);
        velocity = desired;
        transform.position += velocity * Time.deltaTime;

        // 5. Rotate to face movement
        if (velocity.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(velocity);
    }

    public void SetSelected(bool isSelected)
    {
        SelectedMark.SetActive(isSelected);
    }
}
