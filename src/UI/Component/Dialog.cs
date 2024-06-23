using Mafi;
using Mafi.Localization;
using Mafi.Unity.UiToolkit.Component;
using Mafi.Unity.UiToolkit.Library;
using System;

namespace COILib.UI.Component {

    public class ConfirmDialog : Window {
        private ButtonText _confirmButton;
        private ButtonText _cancelButton;
        private Label _label;
        private Action _onConfirm;
        private Action _onClose;

        public ConfirmDialog(LocStrFormatted title) : base(title) {
            BuildWindow();
        }

        protected virtual UiComponent GetContent() => _label = new Label("Please confirm to continue.".AsLoc()).AlignTextCenter().Width(Percent.Hundred);

        private void BuildWindow() {
            GetBackgroundDecorator().SetBackground(ColorRgba.Black.SetA(150));
            this.BackgroundCover().HeightAuto().Width(500.px());
            var _overlay = new UnityEngine.UIElements.VisualElement();

            _confirmButton = new ButtonText("Confirm".AsLoc(), () => {
                _onConfirm?.Invoke();
                HideSelf();
            });
            _cancelButton = new ButtonText("Cancel".AsLoc(), () => {
                _onClose?.Invoke();
                HideSelf();
            });
            var buttonRow = new Row() { _confirmButton.MarginRight(5.px()), _cancelButton }.AlignItemsCenterMiddle().Width(Percent.Hundred).MarginTop(10.px());
            var content = new Column() { GetContent(), buttonRow };
            Body.Add(content);
        }

        public ConfirmDialog SetText(LocStrFormatted text) {
            _label.Text(text);
            return this;
        }

        public ConfirmDialog SetConfirmText(LocStrFormatted confirmText) {
            _confirmButton.Text(confirmText);
            return this;
        }

        public ConfirmDialog SetCancelText(LocStrFormatted cancelText) {
            _cancelButton.Text(cancelText);
            return this;
        }

        public ConfirmDialog OnConfirm(Action onConfirm) {
            _onConfirm = onConfirm;
            return this;
        }

        public ConfirmDialog OnCancel(Action onCancel) {
            _onClose = onCancel;
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

    public class TextFieldDialog : ConfirmDialog {
        private TextFieldWarpper _warpper;

        public TextFieldDialog(LocStrFormatted title) : base(title) {
        }

        protected override UiComponent GetContent() => _warpper = new TextFieldWarpper(new BetterTextField());

        public TextFieldDialog SetPlaceHolder(LocStrFormatted placeHolder) {
            _warpper.Field.SetPlaceholder(placeHolder);
            return this;
        }

        public TextFieldDialog OnConfirm(Action<string> onConfirm) {
            OnConfirm(() => onConfirm?.Invoke(_warpper.Field.value));
            return this;
        }
    }

    public class TextFieldWarpper : UiComponent<BetterTextField> {
        public BetterTextField Field { get; private set; }

        public TextFieldWarpper(BetterTextField field) : base(field) {
            Field = field;
        }
    }
}