using System;
using UnityEngine;

public static class EventBus<T> where T : struct
{
    public static event Action<T> OnEvent;
    public static void Publish(T message) => OnEvent?.Invoke(message);
}
