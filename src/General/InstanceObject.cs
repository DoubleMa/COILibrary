namespace COILib.General;

public abstract class AInitializedClass {
    private bool Initialized { get; set; }

    protected abstract void OnInit();

    public void Init() {
        if (Initialized) {
            return;
        }

        OnInit();
        Initialized = true;
    }
}

public abstract class InstanceObject<T> : AInitializedClass where T : InstanceObject<T>, new() {
    private static T s_instance;
    public static T Instance => getInstance();

    private static T getInstance() {
        return Static.TryRun(() => s_instance ??= new T());
    }
}