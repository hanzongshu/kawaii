
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] MobileJoystick _playerJoystick;
    [SerializeField] Rigidbody2D _rig;
    [SerializeField] float _movespeed;
   
    void Start()
    {
        _rig??= GetComponent<Rigidbody2D>();
    }

   
    void Update()
    {
       
    }

    private void FixedUpdate()
    {
        //1.获取输入(注意这里去掉了括号，对应你最新的属性写法)
        Vector3 currentMove = _playerJoystick.GetMoveVector;

        //2.性能优化：只有输入变化 或者当前有输入才更新物理速度
        //如果当前没动，且上一帧也没动，直接返回，不写内存也不写物理引擎
        if (currentMove.sqrMagnitude < 0.001f && _rig.linearVelocity.sqrMagnitude < 0.001f)
            return;
        _rig.linearVelocity = currentMove * _movespeed;

    }
}


