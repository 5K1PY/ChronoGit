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

public sealed partial class MainWindow : Window {
    MainWindowViewModel? dataContext;
    ItemsControl? commandsView;

    KeyboardControls? controls;
    Dictionary<Key, bool> ModifiersPressed = new Dictionary<Key, bool>{
        {Key.LeftShift, false},
        {Key.RightShift, false},
        {Key.LeftCtrl, false},
        {Key.RightCtrl, false},
    };

    public MainWindow() {
        InitializeComponent();

        MenuItem remapControls = this.FindControl<MenuItem>("RemapControls")!;
        remapControls.PointerPressed += RemapControls_PointerPressed;
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    protected override void OnOpened(EventArgs e) {
        base.OnOpened(e);
        dataContext = (DataContext as MainWindowViewModel)!;
        commandsView = this.FindControl<ItemsControl>("CommandsView")!;
        controls = KeyboardControls.Default(dataContext);
    }

    private KeyCombination GetCurrentKeyCombination(Key key) {
        return new KeyCombination(
            ModifiersPressed[Key.LeftShift] || ModifiersPressed[Key.RightShift],
            ModifiersPressed[Key.LeftCtrl] || ModifiersPressed[Key.RightCtrl],
            key
        );
    }

    private void WindowKeyDown(object sender, KeyEventArgs e) {
        if (ModifiersPressed.ContainsKey(e.Key)) {
            ModifiersPressed[e.Key] = true;
        } else {
            controls!.GetAction(GetCurrentKeyCombination(e.Key))?.Invoke();
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

    private void WindowKeyUp(object sender, KeyEventArgs e) {
        if (ModifiersPressed.ContainsKey(e.Key)) {
            ModifiersPressed[e.Key] = false;
        }
    }
    
    private void RemapControls_PointerPressed(object? sender, PointerPressedEventArgs e) {
        RemapControlsWindow window = new() {
            DataContext = new RemapControlsViewModel(controls!.Export())
        };
        window.ShowDialog(this);
    }
}
