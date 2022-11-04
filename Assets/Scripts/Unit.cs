using TMPro;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int Attack;
    public int Health;

    [SerializeField] private TMP_Text AttackText;
    [SerializeField] private TMP_Text HealthText;

    private void Update()
    {
        UpdateStats();
    }

    public void AttackUnit(Unit target)
    {
        target.TakeDamage(Attack);
    }
    
    public void TakeDamage(int damage)
    {
        Health -= damage;
    }
    

    private void UpdateStats()
    {
        AttackText.text = Attack.ToString();
        HealthText.text = Health.ToString();
    }
}
