using UnityEngine;

namespace GulchGuardians
{
    public class UnitFactory : MonoBehaviour
    {
        [SerializeField] private Unit UnitPrefab;
        [SerializeField] private SoundFXPlayer SoundFXPlayer;

        [SerializeField] private Sprite[] PlayerTeamSprites;
        [SerializeField] private Sprite[] EnemyTeamSprites;

        public Unit Create(bool isPlayerTeam)
        {
            Unit unit = Instantiate(UnitPrefab);
            unit.SetInitialStats(
                attack: Random.Range(minInclusive: 1, maxExclusive: 5),
                health: Random.Range(minInclusive: 1, maxExclusive: 5)
            );
            unit.SoundFXPlayer = SoundFXPlayer;

            Sprite[] spritePool = isPlayerTeam ? PlayerTeamSprites : EnemyTeamSprites;
            Sprite sprite = spritePool[Random.Range(minInclusive: 0, maxExclusive: spritePool.Length)];
            unit.Renderer.sprite = sprite;

            return unit;
        }
    }
}
