using Crysc.Common;
using GulchGuardians.Constants;
using UnityEngine;

namespace GulchGuardians.Classes
{
    [CreateAssetMenu(fileName = "ClassIndex", menuName = "Indexes/Class Index")]
    public class ClassIndex : ScriptableObject
    {
        private static readonly InstanceCacher<ClassIndex> _cacher = new(AssetAddresses.ClassIndex);
        public static ClassIndex I => _cacher.I;

        [field: SerializeField] public ClassType Rookie { get; private set; }
        [field: SerializeField] public ClassType Healer { get; private set; }
        [field: SerializeField] public ClassType Saguaro { get; private set; }
    }
}
