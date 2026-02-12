using System;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Elements")]
    private Player player;

    [Header("Settings")]
    [SerializeField] float _moveSpeed;
   

 
   
    //生成渲染器
   
   public void SetPlayer(Player player)
    {
        this.player = player;
    }

    
   
    // Update is called once per frame
    void Update()
    {
       
        if(player != null)
        {
            FollowPlayer();
        }
    }

    private void FollowPlayer()
    {
        Vector2 diretion = (player.transform.position - transform.position).normalized;
        Vector2 targetPosition = (Vector2)transform.position + diretion * _moveSpeed * Time.deltaTime;
        transform.position = targetPosition;
    }

   
    [ContextMenu("打印")]
    private void 打印坐标()
    {
        Debug.Log("坐标：" +transform.position);
    }
}
