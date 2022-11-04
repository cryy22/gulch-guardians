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
                firstName: Name.RandomName(),
                attack: Random.Range(minInclusive: config.MinAttack, maxExclusive: config.MaxAttack + 1),
                health: Random.Range(minInclusive: config.MinHealth, maxExclusive: config.MaxHealth + 1)
            );

            unit.Renderer.sprite = config.Sprites[Random.Range(minInclusive: 0, maxExclusive: config.Sprites.Count)];

            return unit;
        }
    }
}
