using GulchGuardians.Constants;
using UnityEngine;

namespace GulchGuardians
{
    [CreateAssetMenu(fileName = "UnitFactory", menuName = "Scriptable Objects/Factories/Unit Factory")]
    public class UnitFactory : ScriptableObject
    {
        [SerializeField] private Unit UnitPrefab;

        [SerializeField] private UnitConfig[] PlayerTeamUnitConfigs;
        [SerializeField] private UnitConfig[] EnemyTeamUnitConfigs;

        public Unit Create(bool isPlayerTeam)
        {
            UnitConfig[] configPool = isPlayerTeam ? PlayerTeamUnitConfigs : EnemyTeamUnitConfigs;
            UnitConfig config = configPool[Random.Range(minInclusive: 0, maxExclusive: configPool.Length)];

            return CreateFromConfig(config);
        }

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
