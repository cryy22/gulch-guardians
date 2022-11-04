using System.Collections;
using UnityEngine;

namespace GulchGuardians
{
    [CreateAssetMenu(
        fileName = "HoneBlade",
        menuName = "Scriptable Objects/Modification Effects/Hone Blade"
    )]
    public class HoneBlade : ModificationEffect
    {
        private const string _name = "Hone Blade";

        public override string Name => _name;
        public override TargetType Target => TargetType.Unit;

        public override IEnumerator Apply(Context context)
        {
            yield return base.Apply(context);
            yield return context.Unit!.Upgrade(attack: 2, health: 0);
        }
    }
}
