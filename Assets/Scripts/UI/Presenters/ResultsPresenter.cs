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

        public void Bind()
        {
            _placement.text = "Colocação: 1º (demo)";
            _summary.text = "Resumo: +100 por carta (stub)";
        }

        public void Unbind()
        {
        }
    }
}
