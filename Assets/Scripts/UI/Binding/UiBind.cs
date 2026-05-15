using System;
using UnityEngine.UIElements;

namespace Blitz.UI.Binding
{
    public static class UiBind
    {
        public static IVisualElementScheduledItem EveryFrame(VisualElement root, Action tick) =>
            root.schedule.Execute(tick).Every(16);
    }
}
