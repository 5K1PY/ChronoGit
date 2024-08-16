using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ChronoGit.ViewModels;
using ChronoGit.Views;

namespace ChronoGit;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel("/home/skipy/dev/pisek/.git/rebase-merge/git-rebase-todo")// desktop.Args![0]),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
