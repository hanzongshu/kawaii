using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
public class Player : MonoBehaviour
{
    [Header("Components")]
    PlayerHealth _playerHelth;
    private void Awake()
    {
      _playerHelth = GetComponent<PlayerHealth>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamge(int damage)
    {
        _playerHelth.TakeDamaer(damage);
    }
}
