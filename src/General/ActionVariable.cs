using System;

namespace COILib.General;

public class ActionVariable<T> {
    private T m_value;

    public event Action<T> Event;

    public ActionVariable(T value) {
        Value = value;
    }

    public T Value {
        get => m_value;
        set {
            if (Equals(m_value, value)) {
                return;
            }

            m_value = value;
            Event?.Invoke(m_value);
        }
    }

    public static implicit operator T(ActionVariable<T> variable) => variable.m_value;
}