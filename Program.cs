using Avalonia;
using Avalonia.ReactiveUI;
using System;

namespace ChronoGit;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) {
        if (args.Length != 1 || args[0] == "-h" || args[0] == "--help") {
            Console.WriteLine("ChronoGit - Visual sequence editor for interactive rebasing.");
            Console.WriteLine("  Setting up as your sequence editor:");
            Console.WriteLine("    git config --global sequence.editor ChronoGit");
            Console.WriteLine("  Direct editing:");
            Console.WriteLine("    ./ChronoGit <rebase-todo-file>");

            return;
        }
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}
