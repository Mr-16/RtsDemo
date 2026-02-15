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
        //arriveCostSq = arriveCost * arriveCost;
        //separationRadiusSq = separationRadius * separationRadius;

        Id = GameManager.Instance.UnitList.Count;
        GameManager.Instance.UnitList.Add(this);

        SelectedMark.SetActive(false);
        //velocity = Vector3.zero;
        brakeRadiusSq = brakeRadius * brakeRadius;
        sepRadiusSq = SepRadius * SepRadius;
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
    public FlowField curFlowField;
    private Vector3 curSeekForce = Vector3.zero;
    private Vector3 curBrakeForce = Vector3.zero;
    private Vector3 curSepForce = Vector3.zero;
    private Vector3 curTotalForce = Vector3.zero;
    private Vector3 curVelocity = Vector3.zero;
    private void Move()
    {
        if (Time.frameCount % moveBatchCount != Id % moveBatchCount)
        {
            transform.position = transform.position + curVelocity * Time.deltaTime;
            return;
        }

        //1. 得到全部力
        curSeekForce = GetSeekForce();
        curBrakeForce = GetBrakeForce();
        curSepForce = GetSepForce();
        
        //2. 合成力
        curTotalForce = Vector3.zero;
        curTotalForce += curSeekForce;
        curTotalForce += curBrakeForce;
        curTotalForce += curSepForce;
        //3. 计算加速度, 速度, 最后移动
        Vector3 curAcceleration = curTotalForce / Mass;
        curVelocity = curVelocity + curAcceleration * Time.deltaTime;
        curVelocity = Vector3.ClampMagnitude(curVelocity, MaxVelocity);
        curVelocity.y = 0;
        transform.position = transform.position + curVelocity * Time.deltaTime;
        transform.forward = curVelocity;

        GameManager.Instance.unitSh.Update(this, transform.position);
    }
    public float SeekStrength = 1000;
    private Vector3 GetSeekForce()
    {
        //Seek : 指向目标点的力, 由流畅获得
        if (curFlowField == null)
            return Vector3.zero;
        Vector3 res = SeekStrength * curFlowField.GetDir(transform.position);
        return res;
    }

    private float brakeRadius = 3f;
    private float brakeRadiusSq;
    public float BrakeStrength = 20;
    private Vector3 GetBrakeForce()
    {
        //Arrive : 接近终点时刹车的力
        if (curFlowField == null)
            return Vector3.zero;
        float distSq = (curFlowField.GetTargetPos() - transform.position).sqrMagnitude;
        if (distSq > brakeRadiusSq)
            return Vector3.zero;
        return -curVelocity * BrakeStrength;
    }

    public float SepRadius = 3;
    private float sepRadiusSq;
    public float SepStrength = 100;
    private Vector3 GetSepForce()
    {
        List<MonoBehaviour> sepMbList = GameManager.Instance.unitSh.Query(transform.position, SepRadius);

        Stopwatch sw6 = Stopwatch.StartNew();
        Vector3 sepForce = Vector3.zero;
        foreach (MonoBehaviour mb in sepMbList)
        {
            if (mb is not Unit unit)
                continue;
            if (unit == this)
                continue;
            Vector3 curVec = transform.position - unit.transform.position;
            float distSq = curVec.sqrMagnitude;
            if (distSq < 0.0001f)//防止0距离
                continue;
            if (distSq > brakeRadiusSq)
                continue;
            sepForce += curVec / distSq;
        }
        return sepForce * SepStrength;
    }
}
