using Mafi;
using Mafi.Unity;
using Mafi.Unity.UiToolkit.Component;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace COILib.UI.Component {

    public class IndeterminateProgressBar : UiComponentDecorated<VisualElement> {
        private VisualElement _progress;
        private float barWidth = 20f;

        public IndeterminateProgressBar() : base(new VisualElement()) {
            var progressBar = new VisualElement();
            progressBar.style.flexGrow = 1;
            progressBar.style.backgroundColor = new StyleColor(Color.gray);
            progressBar.style.height = 4;

            _progress = new VisualElement();
            _progress.style.backgroundColor = new StyleColor(ColorRgba.Gold.ToColor());
            _progress.style.height = Length.Percent(100);
            _progress.style.width = new StyleLength(new Length(barWidth, LengthUnit.Percent));

            progressBar.Add(_progress);
            InnerElement.Add(progressBar);
            StartAnimation();
        }

        private void StartAnimation() {
            var animation = _progress.experimental.animation;
            AnimateProgress(animation);
        }

        private void AnimateProgress(ITransitionAnimations animation) {
            animation.Start(0f, 100f - barWidth, 2000, (element, value) => _progress.style.left = new StyleLength(new Length(value, LengthUnit.Percent))).KeepAlive()
            .OnCompleted(() => {
                animation.Start(100f - barWidth, 0f, 2000, (element, value) => _progress.style.left = new StyleLength(new Length(value, LengthUnit.Percent))).KeepAlive()
                .OnCompleted(() => AnimateProgress(animation)).Start();
            }).Start();
        }
    }
}