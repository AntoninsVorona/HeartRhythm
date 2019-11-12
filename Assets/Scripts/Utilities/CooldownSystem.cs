using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CooldownSystem : MonoBehaviour
{
    [Serializable]
    public class IntContainer
    {
        private int value;

        public int Value
        {
            get => value;
            set => Interlocked.Exchange(ref this.value, value);
        }

        public IntContainer(int value)
        {
            Value = value;
        }

        public IntContainer(IntContainer value)
        {
            Value = value.Value;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    public static IEnumerator WaitForEndOfDuration(IntContainer duration)
    {
        yield return new WaitForSeconds(duration.Value);
    }

    public static IEnumerator WaitForEndOfDuration(IntContainer duration, Action<int> callback)
    {
        while (duration.Value > 0)
        {
            yield return new WaitForSeconds(duration.Value);
            duration.Value--;
            callback?.Invoke(duration.Value);
        }
    }

    // Standard Singleton Access 
    public static CooldownSystem Instance { get; private set; }
}