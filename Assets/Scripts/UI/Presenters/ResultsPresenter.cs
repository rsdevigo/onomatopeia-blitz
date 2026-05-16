using UnityEngine.UIElements;

namespace Blitz.UI.Presenters
{
    public sealed class ResultsPresenter
    {
        readonly Label _placement;
        readonly Label _summary;

        public ResultsPresenter(VisualElement root)
        {
            _placement = root.Q<Label>("placement")!;
            _summary = root.Q<Label>("summary")!;
        }

        public void Bind(int finalScore)
        {
            _placement.text = "Partida concluída";
            _summary.text = $"Pontuação final: {finalScore} carta(s) correta(s).";
        }

        public void Unbind()
        {
        }
    }
}
