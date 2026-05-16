using Blitz.Core;
using Blitz.Gameplay;
using Blitz.UI.Binding;
using UnityEngine.UIElements;

namespace Blitz.UI.Presenters
{
    public sealed class HudPresenter
    {
        readonly IMatchSession _session;
        readonly IOnomatopoeiaMatchContent? _content;
        readonly Label _phase;
        readonly Label _score;
        readonly Label _timer;
        readonly Label _letter;
        readonly Label _cue;
        readonly Image? _cardFigure;
        IVisualElementScheduledItem? _tick;

        public HudPresenter(VisualElement root, IMatchSession session, IOnomatopoeiaMatchContent? content = null)
        {
            _session = session;
            _content = content;
            _phase = root.Q<Label>("phase")!;
            _score = root.Q<Label>("score")!;
            _timer = root.Q<Label>("timer")!;
            _letter = root.Q<Label>("card-letter")!;
            _cue = root.Q<Label>("card-cue")!;
            _cardFigure = root.Q<Image>("card-figure");
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

            if (_session.CurrentCard is not { } card || _session.ActiveSet is not { } activeSet)
            {
                _letter.text = "Letra: --";
                _cue.text = "Som: --";
                ClearCardFigure();
                return;
            }

            var trueForLetter = activeSet.TrueOnomatopoeiaForLetter(card.CardLetterId);
            _content?.TryGetDefinition(trueForLetter, out var letterDef);
            _letter.text = letterDef != null ? $"Letra: {letterDef.LetterDisplay}" : $"Letra: {card.CardLetterId.Value}";

            _content?.TryGetDefinition(card.CueOnomatopoeiaId, out var cueDef);
            if (cueDef != null && !string.IsNullOrWhiteSpace(cueDef.WrittenLabel))
                _cue.text = $"{cueDef.WrittenLabel} ({card.Mode})";
            else
                _cue.text = $"Som (cue): {card.CueOnomatopoeiaId.Value} ({card.Mode})";

            if (_cardFigure is null)
                return;

            _content?.TryGetDefinition(trueForLetter, out var figureDef);
            var sprite = figureDef?.FigureSprite;
            _cardFigure.sprite = sprite;
            _cardFigure.style.display = sprite != null ? DisplayStyle.Flex : DisplayStyle.None;
        }

        void ClearCardFigure()
        {
            if (_cardFigure is null)
                return;
            _cardFigure.sprite = null;
            _cardFigure.style.display = DisplayStyle.None;
        }
    }
}
