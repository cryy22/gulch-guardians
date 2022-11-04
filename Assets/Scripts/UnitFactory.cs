using GulchGuardians.Constants;
using UnityEngine;

namespace GulchGuardians
{
    [CreateAssetMenu(fileName = "UnitFactory", menuName = "Scriptable Objects/Factories/Unit Factory")]
    public class UnitFactory : ScriptableObject
    {
        [SerializeField] private Unit UnitPrefab;

        [SerializeField] private Sprite[] PlayerTeamSprites;
        [SerializeField] private Sprite[] EnemyTeamSprites;

        public Unit Create(bool isPlayerTeam)
        {
            Unit unit = Instantiate(UnitPrefab);
            unit.Initialize(
                firstName: Name.RandomName(),
                attack: Random.Range(minInclusive: 1, maxExclusive: 4),
                health: Random.Range(minInclusive: 1, maxExclusive: 8)
            );

            Sprite[] spritePool = isPlayerTeam ? PlayerTeamSprites : EnemyTeamSprites;
            Sprite sprite = spritePool[Random.Range(minInclusive: 0, maxExclusive: spritePool.Length)];
            unit.Renderer.sprite = sprite;

            return unit;
        }
    }
}
