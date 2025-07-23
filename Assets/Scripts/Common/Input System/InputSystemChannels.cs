using UniRx;

namespace Common.Input_System {

    public static class InputSystemChannels {

        public static ReactiveCommand Up { get; } = new();
        public static ReactiveCommand Down { get; } = new();
        public static ReactiveCommand Left { get; } = new();
        public static ReactiveCommand Right { get; } = new();

        public static ReactiveProperty<bool> UpPressed { get; } = new();
        public static ReactiveProperty<bool> DownPressed { get; } = new();
        public static ReactiveProperty<bool> LeftPressed { get; } = new();
        public static ReactiveProperty<bool> RightPressed { get; } = new();
        
    }

}