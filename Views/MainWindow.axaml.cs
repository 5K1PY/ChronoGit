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

    // Shift pressed?, Control pressed?, Key
    Dictionary<Tuple<bool, bool, Key>, Action>? controls;
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
        controls = new Dictionary<Tuple<bool, bool, Key>, Action>{
            {new Tuple<bool, bool, Key>(false, false, Key.Escape), dataContext.NormalMode},
            {new Tuple<bool, bool, Key>(false, false, Key.Up),     dataContext.MoveUp},
            {new Tuple<bool, bool, Key>(false, false, Key.Down),   dataContext.MoveDown},
            {new Tuple<bool, bool, Key>(false, false, Key.Home),   dataContext.MoveToStart},
            {new Tuple<bool, bool, Key>(false, false, Key.End),    dataContext.MoveToEnd},
            {new Tuple<bool, bool, Key>(false, false, Key.D),      dataContext.ConvertToDrop},
            {new Tuple<bool, bool, Key>(false, false, Key.E),      dataContext.ConvertToEdit},
            {new Tuple<bool, bool, Key>(false, false, Key.F),      dataContext.ConvertToFixup},
            {new Tuple<bool, bool, Key>(false, false, Key.I),      dataContext.InsertMode},
            {new Tuple<bool, bool, Key>(false, false, Key.J),      dataContext.ShiftDown},
            {new Tuple<bool, bool, Key>(false, false, Key.K),      dataContext.ShiftUp},
            {new Tuple<bool, bool, Key>(false, false, Key.L),      dataContext.AddLabel},
            {new Tuple<bool, bool, Key>(false, false, Key.P),      dataContext.ConvertToPick},
            {new Tuple<bool, bool, Key>(false, false, Key.R),      dataContext.ConvertToReword},
            {new Tuple<bool, bool, Key>(false, false, Key.S),      dataContext.ConvertToSquash},
            {new Tuple<bool, bool, Key>(false, false, Key.V),      dataContext.ToggleVisualMode},
            {new Tuple<bool, bool, Key>(false, false, Key.Delete), dataContext.Delete},
        };
    }

    private Tuple<bool, bool, Key> GetCurrentKeyCombination(Key key) {
        return new Tuple<bool, bool, Key>(
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
