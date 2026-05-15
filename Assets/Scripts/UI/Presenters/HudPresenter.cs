using Blitz.Gameplay;
using Blitz.UI.Binding;
using UnityEngine.UIElements;

namespace Blitz.UI.Presenters
{
    public sealed class HudPresenter
    {
        readonly IMatchSession _session;
        readonly Label _phase;
        readonly Label _score;
        readonly Label _timer;
        readonly Label _letter;
        readonly Label _cue;
        IVisualElementScheduledItem? _tick;

        public HudPresenter(VisualElement root, IMatchSession session)
        {
            _session = session;
            _phase = root.Q<Label>("phase")!;
            _score = root.Q<Label>("score")!;
            _timer = root.Q<Label>("timer")!;
            _letter = root.Q<Label>("card-letter")!;
            _cue = root.Q<Label>("card-cue")!;
        }

        public void Bind(VisualElement root)
        {
            _session.StateChanged += Refresh;
            Refresh();
            _tick = UiBind.EveryFrame(root, Refresh);
        }

        public void Unbind()
        {
            _session.StateChanged -= Refresh;
            _tick?.Pause();
            _tick = null;
        }

        void Refresh()
        {
            _phase.text = $"Fase: {_session.Phase}";
            _score.text = $"Pontos: {_session.Score}";
            _timer.text = $"Janela: {_session.GrabTimeRemaining:0.00}s";

            if (_session.CurrentCard is { } card)
            {
                _letter.text = $"Letra: {card.CardLetterId.Value}";
                _cue.text = $"Som (cue): {card.CuePhonemeId.Value} ({card.Mode})";
            }
            else
            {
                _letter.text = "Letra: --";
                _cue.text = "Som: --";
            }
        }
    }
}
