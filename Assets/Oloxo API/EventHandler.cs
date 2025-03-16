namespace Oloxo.Global {

    /// <summary>
    /// Events throughout the game. Local to the namespace.
    /// </summary>
    public class EventHandler {

        public readonly Event StartLoadProcess = new Event ();
        public readonly Event EndLoadProcess = new Event ();
        public readonly Event<float> SetLoadProgress = new Event<float> ();
        public readonly Event Quit = new Event ();
        public readonly Event<object> OnFailedToQuit = new Event<object> ();
        public readonly Event SessionModified = new Event ();
    }
}