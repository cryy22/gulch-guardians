using UnityEngine;

namespace InfiniteSAPPrototype
{
    public class UnitFactory : MonoBehaviour
    {
        [SerializeField] private Unit UnitPrefab;

        public Unit Create()
        {
            Unit unit = Instantiate(UnitPrefab);
            unit.Attack = Random.Range(minInclusive: 1, maxExclusive: 5);
            unit.Health = Random.Range(minInclusive: 1, maxExclusive: 5);

            return unit;
        }
    }
}
