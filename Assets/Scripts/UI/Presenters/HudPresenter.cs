using Blitz.Core;
using Blitz.Gameplay;
using Blitz.Gameplay.Content;
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
        readonly Label _prompt;
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
            _prompt = root.Q<Label>("prompt")!;
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
                _cue.text = "Estímulo: --";
                _prompt.text = "Diz o som!";
                ClearCardFigure();
                return;
            }

            var cardLetterDef = ResolveDefinition(activeSet.TrueOnomatopoeiaForLetter(card.CardLetterId));
            _letter.text = cardLetterDef != null
                ? $"Letra da carta: {cardLetterDef.LetterDisplay}"
                : $"Letra da carta: {card.CardLetterId.Value}";

            var cueDef = ResolveDefinition(card.CueOnomatopoeiaId);
            var cueLabel = cueDef != null && !string.IsNullOrWhiteSpace(cueDef.WrittenLabel)
                ? cueDef.WrittenLabel
                : card.CueOnomatopoeiaId.Value.ToString();

            _cue.text = $"Som/figura na carta: {cueLabel}";

            if (card.Mode == CardMode.HasTruePair)
            {
                _prompt.text =
                    "PAR — o estímulo é desta letra. Clique na mesa o objeto com este som/figura.";
            }
            else
            {
                var cueLetter = activeSet.LetterWhoseTrueOnomatopoeiaIs(card.CueOnomatopoeiaId);
                var cueLetterDef = ResolveDefinition(activeSet.TrueOnomatopoeiaForLetter(cueLetter));
                var cardLetterDisplay = cardLetterDef?.LetterDisplay ?? card.CardLetterId.Value.ToString();
                var cueLetterDisplay = cueLetterDef?.LetterDisplay ?? cueLetter.Value.ToString();
                _prompt.text =
                    $"EXCLUSÃO — o estímulo é da letra {cueLetterDisplay}, não da {cardLetterDisplay}. " +
                    $"Clique o objeto da terceira letra (nem {cardLetterDisplay} nem {cueLetterDisplay}).";
            }

            if (_cardFigure is null)
                return;

            var sprite = cueDef?.FigureSprite;
            _cardFigure.sprite = sprite;
            _cardFigure.style.display = sprite != null ? DisplayStyle.Flex : DisplayStyle.None;
        }

        OnomatopoeiaDefinition? ResolveDefinition(OnomatopoeiaId id)
        {
            if (_content is null)
                return null;

            return _content.TryGetDefinition(id, out var def) ? def : null;
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
