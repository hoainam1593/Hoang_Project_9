using System;
using UnityEngine;
using UnityEngine.UI;
using R3;

public class HealthBarUICtrl : MonoBehaviour
{
    private static Vector3 fixedPosition => Vector3.up * 0.1f;
    private Slider slider;
    private float maxHealth;

    private IDisposable d1;
    private IDisposable d2;
    
    [SerializeField] GameObject healthBar;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void Init(ReactiveProperty<Vector3> pos, float maxHp, ReactiveProperty<float> crrHp)
    {
        maxHealth = maxHp;
        slider.value = 1;
        transform.position = pos.Value + fixedPosition;
        
        d1 = crrHp.Subscribe(value => OnTakeDamage(value));
        d2 = pos.Subscribe(pos => transform.position = pos + fixedPosition);
    }

    private void OnTakeDamage(float crrHp)
    {
        healthBar.SetActive(true);
        slider.value = crrHp / maxHealth;
    }

    public void OnDespawn()
    {
        healthBar.SetActive(false);
        d1.Dispose();
        d2.Dispose();
    }
}
