using Mafi;
using Mafi.Localization;
using Mafi.Unity.UiToolkit.Component;
using Mafi.Unity.UiToolkit.Library;
using System;
using UnityEngine.UIElements;
using Column = Mafi.Unity.UiToolkit.Library.Column;
using Label = Mafi.Unity.UiToolkit.Library.Label;

namespace COILib.UI.Component;

public class ConfirmDialog : Window {
    private ButtonText m_confirmButton;
    private ButtonText m_cancelButton;
    private Label m_label;
    private Action m_onConfirm;
    private Action m_onClose;

    public ConfirmDialog(LocStrFormatted title) : base(title) {
        buildWindow();
    }

    protected virtual UiComponent GetContent() => m_label = new Label("Please confirm to continue.".AsLoc()).AlignTextCenter().Width(Percent.Hundred);

    private void buildWindow() {
        GetBackgroundDecorator().SetBackground(ColorRgba.Black.SetA(150));
        this.BackgroundCover().HeightAuto().Width(500.px());
        VisualElement overlay = new();

        m_confirmButton = new ButtonText("Confirm".AsLoc(), () => {
            m_onConfirm?.Invoke();
            HideSelf();
        });
        m_cancelButton = new ButtonText("Cancel".AsLoc(), () => {
            m_onClose?.Invoke();
            HideSelf();
        });
        var buttonRow = new Row() { m_confirmButton.MarginRight(5.px()), m_cancelButton }.AlignItemsCenterMiddle().Width(Percent.Hundred).MarginTop(10.px());
        var content = new Column() { GetContent(), buttonRow };
        Body.Add(content);
    }

    public ConfirmDialog SetText(LocStrFormatted text) {
        m_label.Text(text);
        return this;
    }

    public ConfirmDialog SetConfirmText(LocStrFormatted confirmText) {
        m_confirmButton.Text(confirmText);
        return this;
    }

    public ConfirmDialog SetCancelText(LocStrFormatted cancelText) {
        m_cancelButton.Text(cancelText);
        return this;
    }

    public ConfirmDialog OnConfirm(Action onConfirm) {
        m_onConfirm = onConfirm;
        return this;
    }

    public ConfirmDialog OnCancel(Action onCancel) {
        m_onClose = onCancel;
        return this;
    }

    public void Show(UiComponent parent) {
        parent.Add(this);
        this.Show();
    }

    protected void HideSelf() {
        SetVisible(false);
        RemoveFromHierarchy();
    }
}

public class TextFieldDialog(LocStrFormatted title) : ConfirmDialog(title) {
    private TextFieldWrapper m_wrapper;

    protected override UiComponent GetContent() => m_wrapper = new TextFieldWrapper(new BetterTextField());

    public TextFieldDialog SetPlaceHolder(LocStrFormatted placeHolder) {
        m_wrapper.Field.SetPlaceholder(placeHolder);
        return this;
    }

    public TextFieldDialog OnConfirm(Action<string> onConfirm) {
        OnConfirm(() => onConfirm?.Invoke(m_wrapper.Field.value));
        return this;
    }
}

public class TextFieldWrapper(BetterTextField field) : UiComponent<BetterTextField>(field) {
    public BetterTextField Field { get; private set; } = field;
}