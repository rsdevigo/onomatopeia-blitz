using System;
using UnityEngine;

namespace Blitz.Gameplay.Feedback;

public static class GameplayFeedbackBus
{
    public static event Action<string>? ToastRequested;

    public static void RaiseToast(string message) => ToastRequested?.Invoke(message);
}
