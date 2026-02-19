using Assets.Codes.Game;
using Assets.Codes.Systems.FlowFieldSystem;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class Unit : MonoBehaviour
{
    private Vector3 curDir = Vector3.zero;

    [SerializeField] public GameObject SelectedMark;
    void Start()
    {
        Id = GameManager.Instance.UnitList.Count;
        GameManager.Instance.UnitList.Add(this);

        SelectedMark.SetActive(false);
        //brakeRadiusSq = BrakeRadius * BrakeRadius;
        sepRadiusSq = SepRadius * SepRadius;
        SelfTargetPos = transform.position;
    }

    void Update()
    {
        Move();
        
    }
    public void SetSelected(bool isSelected)
    {
        SelectedMark.SetActive(isSelected);
    }

    public int Id;
    private const int moveBatchCount = 4; // 分4帧

    public float Mass = 10;//质量
    public float MaxVelocity = 30;//最大速度

    private Vector3 curTotalForce = Vector3.zero;
    private Vector3 curVelocity = Vector3.zero;
    private void Move()
    {
        //if (Time.frameCount % moveBatchCount != Id % moveBatchCount)
        //{
        //    transform.position = transform.position + curVelocity * Time.deltaTime;
        //    return;
        //}

        //1. 得到全部力, 并合成力
        curTotalForce = Vector3.zero;
        curTotalForce += GetFlowFieldSeekForce();
        curTotalForce += GetSelfTargetSeekForce();
        curTotalForce += GetSepForce();
        curTotalForce += GetBdSepForce();
        curTotalForce += GetFrictionForce();

        //2. 计算加速度, 速度, 最后移动
        Vector3 curAcceleration = curTotalForce / Mass;
        curVelocity = curVelocity + curAcceleration * Time.deltaTime;
        curVelocity = Vector3.ClampMagnitude(curVelocity, MaxVelocity);
        curVelocity.y = 0;
        transform.position = transform.position + curVelocity * Time.deltaTime;
        if(curVelocity != Vector3.zero)
            transform.forward = curVelocity;

        GameManager.Instance.UnitSh.Update(this, transform.position);
    }

    //目标力
    public FlowField curFlowField;
    
    public float FlowFieldTargetRadius = 10f;
    public float FlowFieldSeekStrength = 1000;
    private Vector3 GetFlowFieldSeekForce()
    {
        if(curFlowField == null)
            return Vector3.zero;
        float distSq = (curFlowField.GetTargetPos() - transform.position).sqrMagnitude;
        if(distSq < FlowFieldTargetRadius * FlowFieldTargetRadius)//到达了
        {
            curFlowField = null;
            return Vector3.zero;
        }
        return curFlowField.GetDir(transform.position) * FlowFieldSeekStrength;
    }

    public Vector3 SelfTargetPos;
    public float SelfTargetSeekStrength = 1000;
    private Vector3 GetSelfTargetSeekForce()
    {
        if (curFlowField != null)
            return Vector3.zero;
        float distSq = (SelfTargetPos - transform.position).sqrMagnitude;
        if (distSq < 0.01f)
            return Vector3.zero;
        return (SelfTargetPos - transform.position).normalized * SelfTargetSeekStrength;
    }


    //分离力
    public float SepRadius = 3;
    private float sepRadiusSq;
    public float SepStrength = 100;
    private Vector3 GetSepForce()
    {
        List<Unit> unitList = GameManager.Instance.UnitSh.Query(transform.position, SepRadius);
        Vector3 sepForce = Vector3.zero;
        foreach (Unit unit in unitList)
        {
            if (unit == this)
                continue;
            Vector3 curVec = transform.position - unit.transform.position;
            float distSq = curVec.sqrMagnitude;
            if (distSq < 0.0001f)//防止0距离
                continue;
            if (distSq > sepRadiusSq)
                continue;
            sepForce += curVec / distSq;
        }
        return sepForce * SepStrength;
    }

    public float BdSepStrength = 100;
    private Vector3 GetBdSepForce()
    {
        Vector3 bdSepForce = Vector3.zero;
        List<Building> bdList = GameManager.Instance.BdSh.Query(transform.position, 10);
        foreach(Building bd in bdList)
        {
            float bdSepRaduisSq = bd.SepRaduis * bd.SepRaduis;
            Vector3 curVec = transform.position - bd.transform.position;
            float distSq = curVec.sqrMagnitude;
            if (distSq > bdSepRaduisSq)
                continue;
            //if (distSq < 0.0001f)//防止0距离
            //    continue;
            bdSepForce += curVec / distSq;
        }
        return bdSepForce * BdSepStrength;
    }

    //摩檫力
    public float FrictionStrength = 50f;
    private Vector3 GetFrictionForce()
    {
        return -curVelocity * FrictionStrength;
    }
}
