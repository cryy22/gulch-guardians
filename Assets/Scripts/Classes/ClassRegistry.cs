using Crysc.Common;
using Crysc.Registries;
using GulchGuardians.Constants;
using UnityEngine;

namespace GulchGuardians.Classes
{
    [CreateAssetMenu(fileName = "ClassRegistry", menuName = "Registries/Class")]
    public class ClassRegistry : Registry<ClassType>
    {
        private static readonly InstanceCacher<ClassRegistry> _cacher = new(AssetAddresses.ClassRegistry);
        private static ClassRegistry I => _cacher.I;
    }
}
