using UnityEngine;
using TMPro;
using System;
using NaughtyAttributes;

[RequireComponent(typeof(EnemyMovement))]
public class Enemy : MonoBehaviour
{
    public static event Action<int ,Vector3> OnDamageTaken;

    [BoxGroup("伤害冒字偏移")]
    [SerializeField]  float damageTextOffestUp=1.2f;

    [Header("Components")]
    private EnemyMovement _EnemyMovement;
    //是否显示绘制
    [Header("DEBUG"), SerializeField] bool _gizmos;

    [Header("Elements")]
    private Player _player;

    [Header("Spawn Sequence Related")]
    [SerializeField] SpriteRenderer _renderer;
    [SerializeField] SpriteRenderer _spawnIndicator;

    [Header("Attack")]
    [SerializeField] int _damage;
    [SerializeField] float _attackFrequency; //Frequency:频率
    float _attackDelay, _attackTimer;//Delay:延迟  Timer:计时器
    [SerializeField] float playerDetectionRadius;//半径
    //特效
    [Header("Effects"), SerializeField] ParticleSystem _passAwayParticle;

    /// <summary>
    /// 生命值
    /// </summary>
    [Header("Settings")]
    [SerializeField, ReadOnly] int _health;
    [SerializeField] int _maxHealth;

    [SerializeField]
    private TextMeshPro _healthText;

    //是否生成
    private bool hasSpawned;

    private CircleCollider2D _circleCollider;
    void Start()
    {
       _circleCollider = GetComponent<CircleCollider2D>();
        _health = _maxHealth;
       // _healthText.SetText(_health.ToString());

        _EnemyMovement= GetComponent<EnemyMovement>();
        _player = FindFirstObjectByType<Player>();
        if (_player == null)
        {
            Debug.LogWarning("No Player found,Auto-destroying...");
            Destroy(gameObject);
        }
        StartSpawnSequence();
    }

    private void StartSpawnSequence()
    {


        SetRenderersVisibility(false);
        Vector3 targetScale = _spawnIndicator.transform.localScale * 1.2f;
        LeanTween.scale(_spawnIndicator.gameObject, targetScale, .3f).setLoopPingPong(4).setOnComplete(SpawnSequenceCompleted);
        _attackDelay = 1 / _attackFrequency;
    }


    /// <summary>
    /// 生成顺序完成回调
    /// </summary>
    private void SpawnSequenceCompleted()
    {
        SetRenderersVisibility();
         hasSpawned = true;
        _EnemyMovement.SetPlayer(_player);
        _circleCollider.enabled = true;
    }

    private void  SetRenderersVisibility(bool visible=true)
    {
        _renderer.enabled = visible;
        _spawnIndicator.enabled = !visible;
    }


    // Update is called once per frame
    void Update()
    {
      

        if (_attackTimer >= _attackDelay)
            TryAttack();
        else
            Wait();
    }

    private void TryAttack()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, _player.transform.position);

        if (distanceToPlayer <= playerDetectionRadius)
        {
            Attack();
        }
    }

    private void Wait()
    {
        _attackTimer += Time.deltaTime;
    }


    private void Attack()
    {
       
        _attackTimer = 0;
        _player.TakeDamge(_damage);
    }

    //受到攻击
    public void TakeDamage(int damage)
    {
        int realDamage = Mathf.Min(damage, _health);
        _health -= realDamage;
   
      //  _healthText.SetText(_health.ToString());
      OnDamageTaken?.Invoke(damage, transform.position + Vector3.up * damageTextOffestUp);
        if (_health <= 0)
        {
            PassAway();
        }
    }


    private void PassAway()
    {
        _passAwayParticle.transform.SetParent(null);
        _passAwayParticle.Play();
        Destroy(this.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }

    private void OnDrawGizmos()
    {
        if (!_gizmos) return;
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);

        Gizmos.color = Color.white;

    }
}
