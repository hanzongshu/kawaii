using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField,ReadOnly] int _health;
    [SerializeField] int _maxHealth;

    [Header("Elements")]
    [SerializeField] Slider _healthSlider;


    void Start()
    {
        _health = _maxHealth;
        _healthSlider.value = 1;
        Debug.Log("Health initialized to :"+_health);
    }

    public void TakeDamaer(int damage)
    {
        int realDamage = Mathf.Min(damage, _health);
        _health -= realDamage;

        float ratio =(float) _health / _maxHealth;
        _healthSlider.value = ratio;
        Debug.Log("health:" + _health);
        if (realDamage<=0)
        {
            PassAway();
        }
    }


    void PassAway()
    {
        Debug.Log("Ded");
    }
}
