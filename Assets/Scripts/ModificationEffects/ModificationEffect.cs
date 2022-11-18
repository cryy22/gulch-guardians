using System;
using System.Collections;
using GulchGuardians.Teams;
using GulchGuardians.Units;
using UnityEngine;

namespace GulchGuardians.ModificationEffects
{
    public abstract class ModificationEffect : ScriptableObject
    {
        public string Name;
        [TextArea] public string Description;

        public abstract TargetType Target { get; }

        public virtual bool CanBeAppliedTo(Context context) { return true; }

        public virtual void Prepare() { }

        public virtual IEnumerator Apply(Context context)
        {
            if (ParametersMeetTargetTypeRequirements(context) == false)
                throw new ArgumentException("Arguments do not meet target type requirements");
            yield break;
        }

        public virtual void CleanUp() { }

        public virtual GameObject GetPreviewGameObject() { return null; }

        private bool ParametersMeetTargetTypeRequirements(Context context)
        {
            return Target switch
            {
                TargetType.Unit              => context.Unit != null,
                TargetType.PlayerTeam        => context.PlayerTeam != null,
                TargetType.EnemyTeam         => context.EnemyTeam != null,
                TargetType.UnitAndPlayerTeam => context.Unit != null && context.PlayerTeam != null,
                _                            => throw new ArgumentOutOfRangeException(),
            };
        }

        public enum TargetType
        {
            Unit,
            PlayerTeam,
            EnemyTeam,
            UnitAndPlayerTeam,
        }

        public struct Context
        {
            public Unit Unit;
            public Team PlayerTeam;
            public Team EnemyTeam;
        }
    }
}
