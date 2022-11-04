using System;
using UnityEngine;

namespace GulchGuardians
{
    public abstract class ModificationEffect : ScriptableObject
    {
        [TextArea] public string Description;

        public abstract string Name { get; }
        public abstract TargetType Target { get; }

        public virtual void Apply(Unit unit = null, Team team = null)
        {
            if (ParametersMeetTargetTypeRequirements(unit: unit, team: team) == false)
                throw new ArgumentException("Arguments do not meet target type requirements");
        }

        private bool ParametersMeetTargetTypeRequirements(Unit unit, Team team)
        {
            return Target switch
            {
                TargetType.Unit => unit != null,
                TargetType.Team => team != null,
                TargetType.Both => unit != null && team != null,
                _               => throw new ArgumentOutOfRangeException(),
            };
        }

        public enum TargetType
        {
            Unit,
            Team,
            Both,
        }
    }
}
