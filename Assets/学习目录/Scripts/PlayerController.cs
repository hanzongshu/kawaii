
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
        _rig.linearVelocity = _playerJoystick.GetMoveVector() * _movespeed ;
    }
}


