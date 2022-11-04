using System.Collections;
using UnityEngine;

namespace GulchGuardians
{
    [CreateAssetMenu(
        fileName = "ArmorUp",
        menuName = "Scriptable Objects/Modification Effects/Armor Up"
    )]
    public class ArmorUp : ModificationEffect
    {
        private const string _name = "Armor Up";

        public override string Name => _name;
        public override TargetType Target => TargetType.Unit;

        public override IEnumerator Apply(Context context)
        {
            yield return base.Apply(context);
            yield return context.Unit!.Upgrade(attack: 0, health: 2);
        }
    }
}
