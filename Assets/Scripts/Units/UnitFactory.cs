using GulchGuardians.Constants;
using UnityEngine;

namespace GulchGuardians
{
    [CreateAssetMenu(fileName = "UnitFactory", menuName = "Scriptable Objects/Factories/Unit Factory")]
    public class UnitFactory : ScriptableObject
    {
        [SerializeField] private Unit UnitPrefab;

        public Unit CreateFromConfig(UnitConfig config)
        {
            Unit unit = Instantiate(UnitPrefab);
            unit.Initialize(
                attack: Random.Range(minInclusive: config.MinAttack, maxExclusive: config.MaxAttack + 1),
                health: Random.Range(minInclusive: config.MinHealth, maxExclusive: config.MaxHealth + 1),
                firstName: Name.RandomName(),
                isBoss: config.IsBoss
            );

            unit.Renderer.sprite = config.Sprites[Random.Range(minInclusive: 0, maxExclusive: config.Sprites.Count)];
            if (config.IsBoss) unit.Renderer.transform.localScale *= 1.25f;

            return unit;
        }
    }
}
