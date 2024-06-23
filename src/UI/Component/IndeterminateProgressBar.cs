using Mafi;
using Mafi.Unity;
using Mafi.Unity.UiToolkit.Component;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace COILib.UI.Component;

public class IndeterminateProgressBar : UiComponentDecorated<VisualElement> {
    private readonly VisualElement m_progress;
    private const float BAR_WIDTH = 20f;

    public IndeterminateProgressBar() : base(new VisualElement()) {
        var progressBar = new VisualElement {
            style = {
                flexGrow = 1,
                backgroundColor = new StyleColor(Color.gray),
                height = 4
            }
        };

        m_progress = new VisualElement {
            style = {
                backgroundColor = new StyleColor(ColorRgba.Gold.ToColor()),
                height = Length.Percent(100),
                width = new StyleLength(new Length(BAR_WIDTH, LengthUnit.Percent))
            }
        };

        progressBar.Add(m_progress);
        InnerElement.Add(progressBar);
        startAnimation();
    }

    private void startAnimation() {
        var animation = m_progress.experimental.animation;
        animateProgress(animation);
    }

    private void animateProgress(ITransitionAnimations animation) {
        animation.Start(0f, 100f - BAR_WIDTH, 2000, (element, value) => m_progress.style.left = new StyleLength(new Length(value, LengthUnit.Percent))).KeepAlive()
            .OnCompleted(() => {
                animation.Start(100f - BAR_WIDTH, 0f, 2000, (element, value) => m_progress.style.left = new StyleLength(new Length(value, LengthUnit.Percent))).KeepAlive()
                    .OnCompleted(() => animateProgress(animation)).Start();
            }).Start();
    }
}