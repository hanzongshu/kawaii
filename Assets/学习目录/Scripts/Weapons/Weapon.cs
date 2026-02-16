using System;
using UnityEngine;
using System.Collections.Generic;
public class Weapon : MonoBehaviour
{
    enum State
    {
        Idle,
        Attack
    }

    Animator _animator;

    private State  _state;
    //=======================检测范围变量=======================
    [Header("层级")]
    [SerializeField] private LayerMask _enemyMask;
    [Header("检测范围,半径")]
    [SerializeField] float _Radius = 2f;//Radius:半径
    private readonly Collider2D[] _sharedhitBuff=new Collider2D[16];//16个通常足够检测周围一圈敌人了，
    private ContactFilter2D _contactFilter; //新增:接触过滤器(现代Unity物理检测的灵魂)
    [Header("瞄准速度(度/秒)"), SerializeField] 
    float _aimSpeedDegrees;
    float _radPerSec;
    //===========================================================

    Transform _cachedTransform;//缓存自身位置

    //=================降频搜索的控制变量=================
    private float _searchTimer; private float _searchInterval = 0.05f;//每0.05秒才去扫描一次周围
    Collider2D _currentTarget;//把找到的目标存起来
   //====================================================

    //===============武器检测敌人====================
    [Header("Elements")]  //Detection :检测  攻击检测范围 
    [SerializeField] Transform _hitDetectionTransform;
    private readonly Collider2D[] _weaponsHitBuff = new Collider2D[16];
    private ContactFilter2D _weaponsContactFilter;
    [Header("击中检测半径"),SerializeField] float _hitDetectionRadius;
    [Header("Attack"), SerializeField] int damage;
    //===============================================



    //============攻击变量============
    private HashSet<Collider2D> _damagedEnemies = new HashSet<Collider2D>(16);
    //=================================



    //==========攻击频率===========
    [Header("每秒攻击次数(秒)")]//Frequency:频率
    [SerializeField] float _attacksPerSecound = 1f;//武器原本的攻击间隔对应一秒一次
    [Header("原始动画的时长(秒)")]
    [SerializeField,ReadOnly] float _baseAnimLength = 1f;//重点
    private float _attackTimer;//攻击时间，大于0表示在冷却中，0表示可以攻击
    private bool _isAttacking = false;//替代状态机
    //Actual:实际攻击间隔
    private float ActualAttackInterval => 1f / _attacksPerSecound;
    //=============================

    public void Awake()
    {
        _animator = GetComponent<Animator>();
        _state = State.Idle;
        _cachedTransform = transform;
        //在Awake中初始化过滤器，一次设置，终身使用
        _contactFilter = new ContactFilter2D();
        _contactFilter.useLayerMask=true;
        _contactFilter.SetLayerMask(_enemyMask);
        _contactFilter.useTriggers = true;
        //武器过滤器初始化
        _weaponsContactFilter = new ContactFilter2D();
        _weaponsContactFilter.useLayerMask=true;
        _weaponsContactFilter.SetLayerMask(_enemyMask);
        _weaponsContactFilter.useTriggers=true;

        _radPerSec =_aimSpeedDegrees*Mathf.Deg2Rad;
    }


    /// <summary>
    /// 查找瞄准最近目标,比教程更好的0GC方式
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private Collider2D FindAndAimNearestEnemy()
    {
        int hitCount = Physics2D.OverlapCircle(_cachedTransform.position, _Radius, _contactFilter, _sharedhitBuff);

        if (hitCount == 0)
        {
            if (_currentTarget != null)
                _currentTarget = null;
            return null;
        }

        Collider2D closestTarget = null;
        float minSqrDistance = Mathf.Infinity;
        Vector3 currentPos = _cachedTransform.position;

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D col = _sharedhitBuff[i];
            Vector3 dirToTarget = col.transform.position - currentPos;
            float sqrDst = dirToTarget.sqrMagnitude;

            if (sqrDst < minSqrDistance)
            {
                minSqrDistance = sqrDst;
                closestTarget = col;
            }

        }

        return closestTarget;
    }



    private void Update()
    {
        //冷却计时器，只要在冷却中，就一直递减时间
        if (_attackTimer > 0)
        {
            _attackTimer -= Time.deltaTime;
        }
        //瞄准逻辑：不管是否在攻击，每帧尝试寻找新的敌人
        AutoAim();
        //3.触发攻击逻辑
        //如果有敌人+冷却完毕+没有在挥刀
        if(_currentTarget!=null && _attackTimer <= 0 && !_isAttacking)
        {
            StartAttack();
        }
        if (_isAttacking)
        {
            Attack();
        }
    }

    private void AutoAim()
    {
        //新版本终极写法，使用Filter和Buffer数组的重载方法
        //它依然返回检测到的数量，依然是绝对的0GC！
        _currentTarget = FindAndAimNearestEnemy();

        if (_currentTarget == null || !_currentTarget.gameObject.activeInHierarchy)
        {
            _cachedTransform.up = Vector3.RotateTowards(_cachedTransform.up, Vector3.up, _radPerSec * Time.deltaTime, 0f);
            return;
        }

        Vector3 targetDirection = _currentTarget.transform.position - _cachedTransform.position;
        //================插值旋转代码核心====================
        _cachedTransform.up = Vector3.RotateTowards(_cachedTransform.up, targetDirection, _radPerSec * Time.deltaTime, 0f);

    }



    //开始攻击

    private void StartAttack()
    {
        _isAttacking =true;
        _attackTimer = ActualAttackInterval;
        //动画速度 用原始速度除以现在攻击间隔
        _animator.speed = _baseAnimLength / ActualAttackInterval;
        _animator.Play("Attack_");
        _damagedEnemies.Clear();
    }

 
    //结束攻击
    private void StopAttack()
    {
      _isAttacking=false;
      
    }


    private void Attack()
    {

        int hitCount = Physics2D.OverlapCircle(_hitDetectionTransform.position, _hitDetectionRadius, _weaponsContactFilter, _weaponsHitBuff);
        if (hitCount == 0) return;
        for (int i = 0; i < hitCount; i++)
        {
            if (!_damagedEnemies.Contains(_weaponsHitBuff[i]))
            {
                if (_weaponsHitBuff[i].TryGetComponent<Enemy>(out Enemy enemy))
                {
                    enemy.TakeDamage(damage);
                    _damagedEnemies.Add(_weaponsHitBuff[i]);
                }


            }

        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _Radius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_hitDetectionTransform.position, _hitDetectionRadius);
    }

}
















