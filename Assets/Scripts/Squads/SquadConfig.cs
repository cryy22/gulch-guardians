using System;
using System.Collections.Generic;
using GulchGuardians.Units;
using UnityEngine;

namespace GulchGuardians.Squads
{
    [CreateAssetMenu(fileName = "NewSquadConfig", menuName = "Config/Squad")]
    public class SquadConfig : ScriptableObject
    {
        public List<UnitConfigQuantity> UnitConfigQuantities;

        [Serializable]
        public struct UnitConfigQuantity
        {
            public UnitConfig UnitConfig;
            public int Quantity;
        }
    }
}
