using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using ChronoGit.ViewModels;

namespace ChronoGit.Views;

public static class FindExtensions {
    public static T? FindDescendant<T>(this Visual visual, string name) where T : Visual {
        if (visual.Name == name && visual is T)
            return (T) visual;
        foreach (var child in visual.GetVisualChildren()) {
            T? childRes = child.FindDescendant<T>(name);
            if (childRes != null) return childRes;
        }
        return null;
    }
}

public partial class MainWindow : Window {
    MainWindowViewModel dataContext;
    ItemsControl commandsView;

    public MainWindow() {
        InitializeComponent();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    protected override void OnOpened(EventArgs e) {
        base.OnOpened(e);
        dataContext = (DataContext as MainWindowViewModel)!;
        commandsView = this.FindControl<ItemsControl>("CommandsView")!;
    }

    private void WindowKeyDown(object sender, KeyEventArgs e) {
        if (dataContext.Commands[dataContext.CurrentPosition] is LabelViewModel) {
            commandsView.ContainerFromIndex(dataContext.CurrentPosition)!.FindDescendant<TextBox>("Focus")!.Focus();
        }
    }
}
