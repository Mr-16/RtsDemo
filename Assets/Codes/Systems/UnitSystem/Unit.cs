using Assets.Codes.Game;
using Assets.Codes.Systems.FlowFieldSystem;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using Debug = UnityEngine.Debug;

public enum UnitState
{
    Idle,
    Move,
    Seek,
    Chase,
    Atk,
    Death,
}

public class Unit : MonoBehaviour
{

    public Vector3 SeekTarget;
    public byte Faction = 0;
    public GameObject BodyMesh;
    public float Mass = 10;//质量
    public float MaxVelocity = 30;//最大速度
    public FlowField CurFlowField;

    public float ViewRange = 20;
    private float viewRangeSq;
    public float AtkRange = 5;
    private float atkRangeSq;
    public float Atk = 10;
    public float MaxHp = 100;
    public float CurHp;
    public float AtkDuration = 2f;

    private Vector3 curVelocity = Vector3.zero;
    private Vector3 curDir = Vector3.zero;
    private List<Unit> curUnitList = new List<Unit>();
    private Unit curTargetEnemy = null;

    [SerializeField] public GameObject SelectedMark;
    void Start()
    {
        GameManager.Instance.UnitList.Add(this);
        SelectedMark.SetActive(false);
        sepRadiusSq = SepRadius * SepRadius;
        viewRangeSq = ViewRange * ViewRange;
        atkRangeSq = AtkRange * AtkRange;
        SeekTarget = transform.position;
        CurHp = MaxHp;
        InitBodyColor();
    }

    void Update()
    {
        UpdateState();
        //Debug.Log(_curState);
    }
    public void SetSelected(bool isSelected)
    {
        SelectedMark.SetActive(isSelected);
    }

    private void OnDestroy()
    {
        
    }

    //状态机框架
    private UnitState _curState = UnitState.Idle;
    private void ChangeState(UnitState newState)
    {
        ExitState(_curState);
        _curState = newState;
        EnterState(_curState);
    }
    private void EnterState(UnitState state)
    {
        switch (state)
        {
            case UnitState.Idle:
                EnterIdle();
                break;
            case UnitState.Move:
                EnterMove();
                break;
            case UnitState.Seek:
                EnterSeek();
                break;
            case UnitState.Chase:
                EnterChase();
                break;
            case UnitState.Atk:
                EnterAtk();
                break;
            case UnitState.Death:
                EnterDeath();
                break;
        }
    }
    private void UpdateState()
    {
        switch (_curState)
        {
            case UnitState.Idle:
                UpdateIdle();
                break;
            case UnitState.Move:
                UpdateMove();
                break;
            case UnitState.Seek:
                UpdateSeek();
                break;
            case UnitState.Chase:
                UpdateChase();
                break;
            case UnitState.Atk:
                UpdateAtk();
                break;
            case UnitState.Death:
                UpdateDeath();
                break;
        }
    }
    private void ExitState(UnitState state)
    {
        switch (state)
        {
            case UnitState.Idle:
                ExitIdle();
                break;
            case UnitState.Move:
                ExitMove();
                break;
            case UnitState.Seek:
                ExitSeek();
                break;
            case UnitState.Chase:
                ExitChase();
                break;
            case UnitState.Atk:
                ExitAtk();
                break;
            case UnitState.Death:
                ExitDeath();
                break;
        }
    }

    #region Idle
    private void EnterIdle()
    {
    }
    private void UpdateIdle()
    {
        if(CurHp == 0)
        {
            ChangeState(UnitState.Death);
            return;
        }
        curUnitList = GameManager.Instance.UnitSh.Query(transform.position, ViewRange);
        GameManager.Instance.UnitSh.Update(this, transform.position);
        curTargetEnemy = GetNearestEnemy();
        if(curTargetEnemy != null)
        {
            ChangeState(UnitState.Chase);
            return;
        }
        if (CurFlowField != null)
        {
            ChangeState(UnitState.Move);
            return;
        }
        if ((SeekTarget - transform.position).sqrMagnitude > 3)
        {
            ChangeState(UnitState.Seek);
            return;
        }
        Vector3 curTotalForce = Vector3.zero;
        curTotalForce = Vector3.zero;
        curTotalForce += GetSepForce();
        curTotalForce += GetBdSepForce();
        curTotalForce += GetFrictionForce();
        curVelocity = curVelocity + curTotalForce / Mass * Time.deltaTime;
        curVelocity = Vector3.ClampMagnitude(curVelocity, MaxVelocity);
        curVelocity.y = 0;
        transform.position = transform.position + curVelocity * Time.deltaTime;
    }
    private void ExitIdle()
    {

    }
    #endregion

    #region Move
    private void EnterMove()
    {

    }
    private void UpdateMove()
    {
        if (CurHp == 0)
        {
            ChangeState(UnitState.Death);
            return;
        }
        curUnitList = GameManager.Instance.UnitSh.Query(transform.position, ViewRange);
        curTargetEnemy = GetNearestEnemy();
        if (curTargetEnemy != null)
        {
            ChangeState(UnitState.Chase);
            return;
        }
        if ((CurFlowField.GetTargetPos() - transform.position).sqrMagnitude < 10)
        {
            ChangeState(UnitState.Idle);
            return;
        }
        Vector3 curTotalForce = Vector3.zero;
        curTotalForce = Vector3.zero;
        curTotalForce += GetSepForce();
        curTotalForce += GetBdSepForce();
        curTotalForce += GetFrictionForce();
        curTotalForce += GetFlowFieldForce();
        curVelocity = curVelocity + curTotalForce / Mass * Time.deltaTime;
        curVelocity = Vector3.ClampMagnitude(curVelocity, MaxVelocity);
        curVelocity.y = 0;
        transform.position = transform.position + curVelocity * Time.deltaTime;
        if (curVelocity != Vector3.zero)
            transform.forward = curVelocity;
        GameManager.Instance.UnitSh.Update(this, transform.position);
    }
    private void ExitMove()
    {
        CurFlowField = null;
    }
    #endregion

    #region Seek
    private void EnterSeek()
    {

    }
    private void UpdateSeek()
    {
        if (CurHp == 0)
        {
            ChangeState(UnitState.Death);
            return;
        }
        curUnitList = GameManager.Instance.UnitSh.Query(transform.position, ViewRange);
        GameManager.Instance.UnitSh.Update(this, transform.position);
        curTargetEnemy = GetNearestEnemy();
        if (curTargetEnemy != null)
        {
            ChangeState(UnitState.Chase);
            return;
        }
        if ((SeekTarget - transform.position).sqrMagnitude < 3)
        {
            ChangeState(UnitState.Idle);
            return;
        }
        Vector3 curTotalForce = Vector3.zero;
        curTotalForce = Vector3.zero;
        curTotalForce += GetSepForce();
        curTotalForce += GetBdSepForce();
        curTotalForce += GetFrictionForce();
        curTotalForce += GetSeekForce(SeekTarget);
        curVelocity = curVelocity + curTotalForce / Mass * Time.deltaTime;
        curVelocity = Vector3.ClampMagnitude(curVelocity, MaxVelocity);
        curVelocity.y = 0;
        transform.position = transform.position + curVelocity * Time.deltaTime;
        if (curVelocity != Vector3.zero)
            transform.forward = curVelocity;
    }
    private void ExitSeek()
    {

    }
    #endregion

    #region Chase
    private void EnterChase()
    {

    }
    private void UpdateChase()
    {
        if (CurHp == 0)
        {
            ChangeState(UnitState.Death);
            return;
        }
        curUnitList = GameManager.Instance.UnitSh.Query(transform.position, ViewRange);
        GameManager.Instance.UnitSh.Update(this, transform.position);
        curTargetEnemy = GetNearestEnemy();
        if (curTargetEnemy == null)
        {
            ChangeState(UnitState.Idle);
            return;
        }
        if ((curTargetEnemy.transform.position - transform.position).sqrMagnitude < atkRangeSq)
        {
            ChangeState(UnitState.Atk);
            return;
        }
        Vector3 curTotalForce = Vector3.zero;
        curTotalForce = Vector3.zero;
        curTotalForce += GetSepForce();
        curTotalForce += GetBdSepForce();
        curTotalForce += GetFrictionForce();
        curTotalForce += GetSeekForce(curTargetEnemy.transform.position);
        curVelocity = curVelocity + curTotalForce / Mass * Time.deltaTime;
        curVelocity = Vector3.ClampMagnitude(curVelocity, MaxVelocity);
        curVelocity.y = 0;
        transform.position = transform.position + curVelocity * Time.deltaTime;
        if (curVelocity != Vector3.zero)
            transform.forward = curVelocity;
    }
    private void ExitChase()
    {

    }
    #endregion

    #region Atk
    private float atkTimer = 0;
    private void EnterAtk()
    {
        curTargetEnemy.TakeDmg(Atk);
    }
    private async void UpdateAtk()
    {
        if (CurHp == 0)
        {
            ChangeState(UnitState.Death);
            return;
        }
        curUnitList = GameManager.Instance.UnitSh.Query(transform.position, ViewRange);
        GameManager.Instance.UnitSh.Update(this, transform.position);
        Vector3 curTotalForce = Vector3.zero;
        curTotalForce = Vector3.zero;
        curTotalForce += GetSepForce();
        curTotalForce += GetBdSepForce();
        curTotalForce += GetFrictionForce();
        curVelocity = curVelocity + curTotalForce / Mass * Time.deltaTime;
        curVelocity = Vector3.ClampMagnitude(curVelocity, MaxVelocity);
        curVelocity.y = 0;
        transform.position = transform.position + curVelocity * Time.deltaTime;
        if (curVelocity != Vector3.zero)
            transform.forward = curVelocity;
        //await Task.Delay(2000);
        atkTimer += Time.deltaTime;
        if(atkTimer > AtkDuration)
        {
            ChangeState(UnitState.Chase);
            return;
        }
    }
    private void ExitAtk()
    {
        atkTimer = 0;
    }
    #endregion

    #region Death
    private void EnterDeath()
    {
        GameManager.Instance.UnitList.Remove(this);
        GameManager.Instance.UnitSh.Remove(this);
        SelectionManager.Instance.RemoveUnit(this);
        Destroy(gameObject);
    }
    private void UpdateDeath()
    {
        
    }
    private void ExitDeath()
    {
        
    }
    #endregion


    //目标力
    public float FlowFieldTargetRadius = 10f;
    public float FlowFieldSeekStrength = 1000;
    private Vector3 GetFlowFieldForce()
    {
        return CurFlowField.GetDir(transform.position) * FlowFieldSeekStrength;
    }

    public float SelfTargetSeekStrength = 1000;
    private Vector3 GetSeekForce(Vector3 targetPos)
    {
        return (targetPos - transform.position).normalized * SelfTargetSeekStrength;
    }

    //分离力
    public float SepRadius = 3;
    private float sepRadiusSq;
    public float SepStrength = 100;
    private Vector3 GetSepForce()
    {
        Vector3 sepForce = Vector3.zero;
        foreach (Unit unit in curUnitList)
        {
            if (unit == this)
                continue;
            Vector3 curVec = transform.position - unit.transform.position;
            float distSq = curVec.sqrMagnitude;
            if (distSq > sepRadiusSq)
                continue;
            if (distSq < 0.0001f)//防止0距离
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
        foreach (Building bd in bdList)
        {
            float bdSepRaduisSq = bd.SepRaduis * bd.SepRaduis;
            Vector3 curVec = transform.position - bd.transform.position;
            float distSq = curVec.sqrMagnitude;
            if (distSq > bdSepRaduisSq)
                continue;
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

    private void InitBodyColor()
    {
        MeshRenderer meshRender = BodyMesh.GetComponent<MeshRenderer>();
        switch (Faction)
        {
            case 0:
                meshRender.material.color = Color.red;
                break;
            case 1:
                meshRender.material.color = Color.blue;
                break;
        }
    }

    private Unit GetNearestEnemy()
    {
        Unit nearestEnemy = null;
        float curMinDistSq = float.MaxValue;
        foreach (Unit curUnit in curUnitList)
        {
            if(curUnit == null)
                continue;
            if (curUnit.Faction == Faction)
                continue;
            float curDistSq = (curUnit.transform.position - transform.position).sqrMagnitude;
            if (curDistSq > viewRangeSq)
                continue;
            if(curDistSq < curMinDistSq)
            {
                nearestEnemy = curUnit;
                curMinDistSq = curDistSq;
            }
        }
        return nearestEnemy;
    }

    public void TakeDmg(float dmg)
    {
        CurHp -= dmg;
        if(CurHp <= 0)
        {
            CurHp = 0;
        }
    }
}
