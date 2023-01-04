using Crysc.Registries;

namespace GulchGuardians.Classes
{
    public class ClassRegistrar : MouseEventRegistrar<ClassType>
    {
        private IClassProvider _provider;

        public override ClassType Registrant => _provider.Class;

        protected override void Awake()
        {
            base.Awake();
            _provider = GetComponent<IClassProvider>();
        }
    }
}
