using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
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
    private float _searchTimer; private float _searchInterval = 0.2f;//每0.2秒才去扫描一次周围
    Collider2D _currentTarget;//把找到的目标存起来
    //====================================================

    public void Awake()
    {
        _cachedTransform = transform;
        //在Awake中初始化过滤器，一次设置，终身使用
        _contactFilter = new ContactFilter2D();
        _contactFilter.useLayerMask=true;
        _contactFilter.SetLayerMask(_enemyMask);
        _contactFilter.useTriggers = true;
        _radPerSec =_aimSpeedDegrees*Mathf.Deg2Rad;
    }

    private void Update()
    {
        _searchTimer += Time.deltaTime;
        if (_searchTimer > _searchInterval)
        {
            _searchTimer = 0;
           _currentTarget= FindAndAimNearestEnemy();
        }
        AutoAim();
    }

    /// <summary>
    /// 查找瞄准最近目标,比教程更好的0GC方式
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private Collider2D FindAndAimNearestEnemy()
{
        int hitCount = Physics2D.OverlapCircle(_cachedTransform.position, _Radius, _contactFilter, _sharedhitBuff);

        if (hitCount == 0) return null;

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

    private void AutoAim()
    {
        //新版本终极写法，使用Filter和Buffer数组的重载方法
        //它依然返回检测到的数量，依然是绝对的0GC！
      
        if (_currentTarget == null || !_currentTarget.gameObject.activeInHierarchy)
        {
            _cachedTransform.up = Vector3.RotateTowards(_cachedTransform.up, Vector3.up, _radPerSec * Time.deltaTime, 0f);
        } 


        Vector3 targetDirection = _currentTarget.transform.position - _cachedTransform.position;
        //================插值旋转代码核心====================
        _cachedTransform.up = Vector3.RotateTowards(_cachedTransform.up, targetDirection, _radPerSec * Time.deltaTime, 0f);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _Radius);
    }

}
