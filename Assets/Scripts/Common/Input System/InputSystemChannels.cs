using UniRx;

namespace Common.Input_System {

    public static class InputSystemChannels {

        public static ReactiveCommand Up { get; } = new();
        public static ReactiveCommand Down { get; } = new();
        public static ReactiveCommand Left { get; } = new();
        public static ReactiveCommand Right { get; } = new();

    }

}