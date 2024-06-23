namespace COILib.General {

    public abstract class AInitializedClass {
        protected bool Initialized { get; private set; } = false;

        protected abstract void OnInit();

        public void Init() {
            if (Initialized) return;
            OnInit();
            Initialized = true;
        }
    }

    public abstract class InstanceObject<T> : AInitializedClass where T : InstanceObject<T>, new() {
        private static T instance;
        public static T Instance => GetInstance();

        private static T GetInstance() {
            return Static.TryRun(() => {
                if (instance == null) {
                    instance = new T();
                    //instance.Init();
                }
                return instance;
            }, null);
        }
    }
}