using UnityEngine;

namespace InfiniteSAPPrototype
{
    public class UnitFactory : MonoBehaviour
    {
        [SerializeField] private Unit UnitPrefab;

        [SerializeField] private Sprite[] PlayerTeamSprites;
        [SerializeField] private Sprite[] EnemyTeamSprites;

        public Unit Create(bool isPlayerTeam)
        {
            Unit unit = Instantiate(UnitPrefab);
            unit.Attack = Random.Range(minInclusive: 1, maxExclusive: 5);
            unit.Health = Random.Range(minInclusive: 1, maxExclusive: 5);

            Sprite[] spritePool = isPlayerTeam ? PlayerTeamSprites : EnemyTeamSprites;
            Sprite sprite = spritePool[Random.Range(minInclusive: 0, maxExclusive: spritePool.Length)];
            unit.Renderer.sprite = sprite;

            return unit;
        }
    }
}
