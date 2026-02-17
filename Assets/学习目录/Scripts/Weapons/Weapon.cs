using System;
using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes; // 核心：引入 NaughtyAttributes 命名空间

public class Weapon : MonoBehaviour
{
    Animator _animator;

    // ==========================================
    // ⚔️ 1. 核心战斗属性 (最常调整的数值放最上面)
    // ==========================================
    [BoxGroup("1. 核心战斗属性")]
    [SerializeField] int damage;

    [BoxGroup("1. 核心战斗属性")]
    [SerializeField] float _attacksPerSecound = 1f;


    // ==========================================
    // 🎯 2. 索敌与瞄准设定
    // ==========================================
    [BoxGroup("2. 索敌与瞄准设定")]
    [SerializeField] private LayerMask _enemyMask;

    [BoxGroup("2. 索敌与瞄准设定")]
    [SerializeField] float _Radius = 2f;

    [BoxGroup("2. 索敌与瞄准设定")]
    [SerializeField] float _aimSpeedDegrees;


    // ==========================================
    // 💥 3. 命中判定区设定 (Hitbox)
    // ==========================================
    [BoxGroup("3. 命中判定区设定")]
    [SerializeField] Transform _hitDetectionTransform;

    [BoxGroup("3. 命中判定区设定")]
    [SerializeField] float _hitDetectionRadius;
    [BoxGroup("3. 命中判定区设定")]
    [Tooltip("命中碰撞器盒子")]
    [SerializeField] BoxCollider2D _hitDetectionCollider;

    // ==========================================
    // 📊 4. 动画与内部换算数据 (设为折叠组，保持清爽)
    // ==========================================
    [Foldout("4. 运行与动画数据 (只读)")]
    [SerializeField, ReadOnly] float _actualTime;

    [Foldout("4. 运行与动画数据 (只读)")]
    [SerializeField, ReadOnly] float _baseAnimLength = 1f;


    // ==========================================
    // 内部私有变量 (面板不可见，不影响排版)
    // ==========================================
    private readonly Collider2D[] _sharedhitBuff = new Collider2D[16];//瞄准判断碰撞体
    private ContactFilter2D _contactFilter;//瞄准的连接过滤器
    private float _radPerSec;
    Transform _cachedTransform;
    private float _searchTimer;
    private float _searchInterval = 0.05f;
    Collider2D _currentTarget;
    private readonly Collider2D[] _weaponsHitBuff = new Collider2D[16];
    private ContactFilter2D _weaponsContactFilter;
    private HashSet<Collider2D> _damagedEnemiesList = new HashSet<Collider2D>(16);
    private float _attackTimer;
    private bool _isAttacking = false;

    private float ActualAttackInterval => _actualTime;

    // ==========================================
    // 生命周期与逻辑代码 (完全保持你的原样)
    // ==========================================

    public void Awake()
    {
        _animator = GetComponent<Animator>();
        _cachedTransform = transform;

        _contactFilter = new ContactFilter2D();
        _contactFilter.useLayerMask = true;
        _contactFilter.SetLayerMask(_enemyMask);
        _contactFilter.useTriggers = true;

        _weaponsContactFilter = new ContactFilter2D();
        _weaponsContactFilter.useLayerMask = true;
        _weaponsContactFilter.SetLayerMask(_enemyMask);
        _weaponsContactFilter.useTriggers = true;

        _radPerSec = _aimSpeedDegrees * Mathf.Deg2Rad;
    }

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
        if (_attackTimer > 0)
        {
            _attackTimer -= Time.deltaTime;
        }

        AutoAim();

        if (_currentTarget != null && _attackTimer <= 0 && !_isAttacking)
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
        _currentTarget = FindAndAimNearestEnemy();

        if (_currentTarget == null || !_currentTarget.gameObject.activeInHierarchy)
        {
            _cachedTransform.up = Vector3.RotateTowards(_cachedTransform.up, Vector3.up, _radPerSec * Time.deltaTime, 0f);
            return;
        }

        Vector3 targetDirection = _currentTarget.transform.position - _cachedTransform.position;
        _cachedTransform.up = Vector3.RotateTowards(_cachedTransform.up, targetDirection, _radPerSec * Time.deltaTime, 0f);
    }

    private void StartAttack()
    {
        _isAttacking = true;
        _attackTimer = ActualAttackInterval;
        _animator.speed = _baseAnimLength / ActualAttackInterval;
        _animator.Play("Attack_");
        _damagedEnemiesList.Clear();
    }

    private void StopAttack()
    {
        _isAttacking = false;
        _animator.speed = 1f; // 修复：攻击结束恢复原速，防止影响后续逻辑
    }

    private void Attack()
    {
        int hitCount = Physics2D.OverlapCollider(_hitDetectionCollider,_weaponsContactFilter,_weaponsHitBuff);
        if (hitCount == 0) return;

        for (int i = 0; i < hitCount; i++)
        {
            if (!_damagedEnemiesList.Contains(_weaponsHitBuff[i]))
            {
                if (_weaponsHitBuff[i].TryGetComponent<Enemy>(out Enemy enemy))
                {
                    enemy.TakeDamage(damage);
                    _damagedEnemiesList.Add(_weaponsHitBuff[i]);
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

    private void OnValidate()
    {
        if (_attacksPerSecound <= 0.01f)
        {
            _attacksPerSecound = 0.01f;
        }

        _actualTime = 1f / _attacksPerSecound;
    }


    // ==========================================
   
    // ==========================================
}