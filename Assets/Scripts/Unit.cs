using System.Collections;
using TMPro;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int Attack;
    public int Health;

    [SerializeField] private TMP_Text AttackText;
    [SerializeField] private TMP_Text HealthText;

    private void Update() { UpdateStats(); }

    public bool AttackUnit(Unit target) { return target.TakeDamage(Attack); }

    private bool TakeDamage(int damage)
    {
        Health -= damage;
        StartCoroutine(ShowHealthChange());

        if (Health > 0) return false;

        Destroy(gameObject);
        return true;
    }

    private IEnumerator ShowHealthChange()
    {
        Vector3 defaultScale = HealthText.transform.localScale;
        HealthText.transform.localScale = defaultScale * 1.5f;

        yield return new WaitForSeconds(0.5f);
        HealthText.transform.localScale = defaultScale;
    }

    private void UpdateStats()
    {
        AttackText.text = Attack.ToString();
        HealthText.text = Health.ToString();
    }
}
