using System;
using System.Collections.Generic;
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
    MainWindowViewModel? dataContext;
    ItemsControl? commandsView;
    Dictionary<Key, Action>? controls;
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
        controls = new Dictionary<Key, Action>{
            {Key.Escape, dataContext.NormalMode},
            {Key.Up, dataContext.MoveUp},
            {Key.Down, dataContext.MoveDown},
            {Key.Home, dataContext.MoveToStart},
            {Key.End, dataContext.MoveToEnd},
            {Key.D, dataContext.ConvertToDrop},
            {Key.E, dataContext.ConvertToEdit},
            {Key.F, dataContext.ConvertToFixup},
            {Key.I, dataContext.InsertMode},
            {Key.J, dataContext.ShiftDown},
            {Key.K, dataContext.ShiftUp},
            {Key.L, dataContext.AddLabel},
            {Key.P, dataContext.ConvertToPick},
            {Key.R, dataContext.ConvertToReword},
            {Key.S, dataContext.ConvertToSquash},
            {Key.V, dataContext.ToggleVisualMode},
            {Key.Delete, dataContext.Delete},
        };
    }

    private void WindowKeyDown(object sender, KeyEventArgs e) {
        Action? action;
        if (controls!.TryGetValue(e.Key, out action)) {
            action();
        }

        // Can fail if Commands is empty
        Control? control = commandsView!.ContainerFromIndex(dataContext!.CurrentPosition);
        TextBox? FocusBox = control?.FindDescendant<TextBox>("FocusHere");
        if (dataContext.CurrentMode == Mode.InsertMode && FocusBox != null) {
            FocusBox.Focus();
        } else {
            FocusManager!.ClearFocus();
        }
    }
}
