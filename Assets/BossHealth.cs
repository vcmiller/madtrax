using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BossHealth : MonoBehaviour {

    public float maxHealth = 500;
    public UnityEngine.UI.Slider healthSlider;
    float _health;
    public float health
    {
        get
        {
            return _health;
        }
        private set
        {
            _health = value;
            healthSlider.value = _health / maxHealth;
            if(value <= 0)
            {
                IsKill();
            }
        }
    }

    private void Start()
    {
        _health = maxHealth;
    }

    void IsKill()
    {
        Destroy(gameObject);
    }

    public void Damage(float dmg)
    {
        health -= dmg;
    }
}
