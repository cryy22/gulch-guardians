using System.Collections;
using TMPro;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int Attack;
    public int Health;

    [SerializeField] private TMP_Text AttackText;
    [SerializeField] private TMP_Text HealthText;
    [SerializeField] private SpriteRenderer Renderer;

    private int _initialHealth;

    private void Awake()
    {
        Renderer.color = GenerateRandomColor();
        _initialHealth = Health;
    }

    private void Update()
    {
        UpdateStats();
        UpdateSize();
    }

    public bool AttackUnit(Unit target) { return target.TakeDamage(Attack); }

    public void Upgrade(int attack, int health)
    {
        Attack += attack;
        Health += health;
        _initialHealth += health;
    }

    private static Color GenerateRandomColor()
    {
        return Color.HSVToRGB(
            H: Random.Range(minInclusive: 0f, maxInclusive: 1f),
            S: Random.Range(minInclusive: 0.5f, maxInclusive: 1f),
            V: 1f
        );
    }

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

    private void UpdateSize()
    {
        float totalStats = Attack + _initialHealth;
        float lerpValue = Mathf.InverseLerp(a: 2, b: 10, value: totalStats);

        float sizeDelta = Mathf.Lerp(a: 1, b: 2f, t: lerpValue);
        transform.localScale = Vector3.one * sizeDelta;
    }
}
