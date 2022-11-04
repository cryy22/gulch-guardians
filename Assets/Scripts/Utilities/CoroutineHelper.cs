using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GulchGuardians
{
    public class CoroutineHelper
    {
        public static IEnumerator RunConcurrently<T>(
            IEnumerable<T> behaviours,
            Func<T, IEnumerator> enumerator
        ) where T : MonoBehaviour
        {
            List<Coroutine> coroutines = behaviours
                .Select(behaviour => behaviour.StartCoroutine(enumerator(behaviour)))
                .ToList();

            foreach (Coroutine coroutine in coroutines) yield return coroutine;
        }
    }
}
