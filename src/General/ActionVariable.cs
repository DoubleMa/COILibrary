using System;

namespace COILib.General {

    public class ActionVariable<T> {
        private T _value;

        public event Action<T> Event;

        public ActionVariable(T value) {
            Value = value;
        }

        public T Value {
            get => _value;
            set {
                if (!Equals(_value, value)) {
                    _value = value;
                    Event?.Invoke(_value);
                }
            }
        }

        public static implicit operator T(ActionVariable<T> variable) => variable._value;
    }
}