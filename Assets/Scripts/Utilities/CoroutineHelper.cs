using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GulchGuardians
{
    public static class CoroutineHelper
    {
        public static IEnumerator RunConcurrently<T>(
            IEnumerable<T> behaviours,
            Func<T, IEnumerator> enumerator
        ) where T : MonoBehaviour
        {
            yield return RunConcurrently(
                behaviours
                    .Select(behaviour => behaviour.StartCoroutine(enumerator(behaviour)))
                    .ToArray()
            );
        }

        public static IEnumerator RunConcurrently(params Coroutine[] coroutines)
        {
            foreach (Coroutine coroutine in coroutines) yield return coroutine;
        }
    }
}
