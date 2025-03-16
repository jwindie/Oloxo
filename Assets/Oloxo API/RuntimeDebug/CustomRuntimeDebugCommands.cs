using Joro.RuntimeDebugger;

public static class CustomRuntimeDebugCommands {

    public static void LoadCommands () {
        object[] commands = new object[] {
        };

        if (commands.Length > 0) {
            foreach (object o in commands) RuntimeDebugController.AddCommand (o);
        }
    }
}