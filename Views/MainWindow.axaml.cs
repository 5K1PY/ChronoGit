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

    Dictionary<KeyCombination, Action>? controls;
    Dictionary<Key, bool> ModifiersPressed = new Dictionary<Key, bool>{
        {Key.LeftShift, false},
        {Key.RightShift, false},
        {Key.LeftCtrl, false},
        {Key.RightCtrl, false},
    };

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
        controls = new Dictionary<KeyCombination, Action>{
            {new KeyCombination(false, false, Key.Escape), dataContext.NormalMode},
            {new KeyCombination(false, false, Key.Up),     dataContext.MoveUp},
            {new KeyCombination(false, false, Key.Down),   dataContext.MoveDown},
            {new KeyCombination(false, false, Key.Home),   dataContext.MoveToStart},
            {new KeyCombination(false, false, Key.End),    dataContext.MoveToEnd},
            {new KeyCombination(false, false, Key.D),      dataContext.ConvertToDrop},
            {new KeyCombination(false, false, Key.E),      dataContext.ConvertToEdit},
            {new KeyCombination(false, false, Key.F),      dataContext.ConvertToFixup},
            {new KeyCombination(false, false, Key.I),      dataContext.InsertMode},
            {new KeyCombination(false, false, Key.J),      dataContext.ShiftDown},
            {new KeyCombination(false, false, Key.K),      dataContext.ShiftUp},
            {new KeyCombination(false, false, Key.L),      dataContext.AddLabel},
            {new KeyCombination(false, false, Key.P),      dataContext.ConvertToPick},
            {new KeyCombination(false, false, Key.R),      dataContext.ConvertToReword},
            {new KeyCombination(false, false, Key.S),      dataContext.ConvertToSquash},
            {new KeyCombination(false, false, Key.V),      dataContext.ToggleVisualMode},
            {new KeyCombination(false, false, Key.Delete), dataContext.Delete},
        };
    }

    private KeyCombination GetCurrentKeyCombination(Key key) {
        return new KeyCombination(
            ModifiersPressed[Key.LeftShift] || ModifiersPressed[Key.RightShift],
            ModifiersPressed[Key.LeftCtrl] || ModifiersPressed[Key.RightCtrl],
            key
        );
    }

    private void WindowKeyDown(object sender, KeyEventArgs e) {
        Action? action;
        if (controls!.TryGetValue(GetCurrentKeyCombination(e.Key), out action)) {
            action();
        } else if (ModifiersPressed.ContainsKey(e.Key)) {
            ModifiersPressed[e.Key] = true;
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
}
