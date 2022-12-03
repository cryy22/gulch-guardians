using System.Collections.Generic;
using Crysc.Initialization;
using GulchGuardians.Units;
using UnityEngine;

namespace GulchGuardians.Squads
{
    [CreateAssetMenu(fileName = "SquadFactory", menuName = "Factories/Squad Factory")]
    public class SquadFactory : InitializationFactory<Squad, SquadConfig, SquadInitParams>
    {
        [SerializeField] private UnitFactory UnitFactory;

        protected override SquadInitParams GetInitParams(SquadConfig config)
        {
            return new SquadInitParams(GenerateUnits(config.UnitConfigQuantities));
        }

        private IEnumerable<Unit> GenerateUnits(IEnumerable<SquadConfig.UnitConfigQuantity> configQuantities)
        {
            var units = new List<Unit>();

            foreach (SquadConfig.UnitConfigQuantity configQuantity in configQuantities)
                for (var i = 0; i < configQuantity.Quantity; i++)
                    units.Insert(
                        index: Random.Range(minInclusive: 0, maxExclusive: units.Count + 1),
                        item: UnitFactory.Create(configQuantity.UnitConfig)
                    );

            return units;
        }
    }
}
