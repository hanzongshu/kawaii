using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField,ReadOnly] int _health;
    [SerializeField] int _maxHealth;

    [Header("Elements")]
    [SerializeField] Slider _healthSlider;

    [SerializeField] TextMeshProUGUI _healthText;

    void Start()
    {
        _health = _maxHealth;
        _healthSlider.value = 1;
       UpdateUI();
    }

    public void TakeDamaer(int damage)
    {
        int realDamage = Mathf.Min(damage, _health);
        _health -= realDamage;

        UpdateUI();

        
        Debug.Log("health:" + _health);
        if (realDamage<=0)
        {
            PassAway();
        }
    }


    void PassAway()
    {
        Debug.Log("Ded");
        SceneManager.LoadScene(0);
    }
    private void UpdateUI()
    {
        float ratio = (float)_health / _maxHealth;
        _healthSlider.value = ratio;
        _healthText.SetText(_health + "/" + _maxHealth);
    }
}
