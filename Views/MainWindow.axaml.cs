using System;
using Avalonia.Controls;
using Avalonia.Input;
using ChronoGit.ViewModels;

namespace ChronoGit.Views;

public sealed partial class MainWindow : WindowBase {
    MainWindowViewModel? dataContext;
    ItemsControl? commandsView;

    KeyboardControls? controls;

    public MainWindow() {
        InitializeComponent();

        MenuItem remapControls = this.FindControl<MenuItem>("RemapControls")!;
        remapControls.PointerPressed += RemapControls_PointerPressed;
    }

    protected override void OnOpened(EventArgs e) {
        base.OnOpened(e);
        dataContext = (DataContext as MainWindowViewModel)!;
        commandsView = this.FindControl<ItemsControl>("CommandsView")!;
        controls = KeyboardControls.Default(dataContext);
    }

    protected override void WindowKeyDown(object sender, KeyEventArgs e) {
        base.WindowKeyDown(sender, e);
        controls!.GetAction(GetCurrentKeyCombination(e.Key))?.Invoke();

        // Can fail if Commands is empty
        Control? control = commandsView!.ContainerFromIndex(dataContext!.CurrentPosition);
        TextBox? FocusBox = control?.FindDescendant<TextBox>("FocusHere");
        if (dataContext.CurrentMode == Mode.InsertMode && FocusBox != null) {
            FocusBox.Focus();
        } else {
            FocusManager!.ClearFocus();
        }
    }

    private void SaveConfiguration() {
        // TODO
    }

    private async void RemapControls_PointerPressed(object? sender, PointerPressedEventArgs e) {
        RemapControlsWindow window = new() {
            DataContext = new RemapControlsViewModel(controls!.Export())
        };
        KeyboardControls? keyboardControls = await window.ShowDialog<KeyboardControls>(this);
        if (keyboardControls != null) {
            controls = keyboardControls;
            SaveConfiguration();
        }
    }
}
