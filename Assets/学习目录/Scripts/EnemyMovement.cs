using System;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Elements")]
    private Player player;

    [Header("Settings")]
    [SerializeField] float _moveSpeed;
    [SerializeField] float playerDetectionRadius;
    //是否显示绘制
    [Header("DEBUG"), SerializeField] bool _gizmos;
    //特效
    [Header("Effects"), SerializeField] ParticleSystem _passAwayParticle;
    //生成渲染器
    [Header("Spawn Sequence Related") ]
   [SerializeField] SpriteRenderer _renderer;
   [SerializeField] SpriteRenderer _spawnIndicator;
    //是否生成
    private bool hasSpawned;

    void Start()
    {
        player = FindFirstObjectByType<Player>();
        if(player == null)
        {
            Debug.LogWarning("No Player found,Auto-destroying...");
            Destroy(gameObject);
        }
        //Hide the renderer
        _renderer.enabled = false;
        //Show the spawn indicator
        _spawnIndicator.enabled = true;
        //Scale up& down the spawn indicator
        Vector3 targetScale = _spawnIndicator.transform.localScale*1.2f;
        LeanTween.scale(_spawnIndicator.gameObject,targetScale,.3f).setLoopPingPong(4).setOnComplete(SpawnSequenceCompleted);
        //Show the enemy after 3 seconds
        //Hide the spawn indicator

        //Prevent Following & Atacking during the spawn sequence


        //_renderer.gameObject.SetActive(true);
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasSpawned)
            return;
        
         FollowPlayer();
         TryAttack();
        
       
    }

    /// <summary>
    /// 生成顺序完成回调
    /// </summary>
  private void SpawnSequenceCompleted()
    {
        _renderer.enabled= true;
        _spawnIndicator.enabled=false;
       hasSpawned = true;
    }


    private void FollowPlayer()
    {
        Vector2 diretion = (player.transform.position - transform.position).normalized;
        Vector2 targetPosition = (Vector2)transform.position + diretion * _moveSpeed * Time.deltaTime;
        transform.position = targetPosition;
    }

    private void TryAttack()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if(distanceToPlayer <= playerDetectionRadius)
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
        if(!_gizmos)return;
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }


    [ContextMenu("打印")]
    private void 打印坐标()
    {
        Debug.Log("坐标：" +transform.position);
    }
}
