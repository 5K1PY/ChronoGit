using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ChronoGit.ViewModels;

namespace ChronoGit.Views;

public sealed partial class MainWindow : WindowBase {
    const double ITEM_HEIGHT = 60.0;

    MainWindowViewModel? dataContext;
    ItemsControl? commandsView;

    KeyboardControls? controls;

    public MainWindow() {
        InitializeComponent();
    }

    protected override void OnOpened(EventArgs e) {
        base.OnOpened(e);
        dataContext = (DataContext as MainWindowViewModel)!;
        commandsView = this.FindControl<ItemsControl>("CommandsView")!;
        controls = KeyboardControls.Default(dataContext);
    }

    protected override void WindowKeyDown(object sender, KeyEventArgs e) {
        base.WindowKeyDown(sender, e);

        NamedAction? action = controls!.GetAction(GetCurrentKeyCombination(e.Key));
        action?.Action.Invoke();

        ScrollViewer scrollCommands = this.FindControl<ScrollViewer>("ScrollCommandsView")!;
        double height = scrollCommands.Bounds.Height;
        double y_offset = scrollCommands.Offset.Y;

        double top_position = dataContext!.SelectedStart() * ITEM_HEIGHT;
        double bot_position = dataContext!.SelectedEnd() * ITEM_HEIGHT;
        double position = dataContext!.CurrentPosition * ITEM_HEIGHT;

        if (action?.ActionType == ActionType.ShiftUp) {
            y_offset = Math.Min(y_offset, top_position - ITEM_HEIGHT);
        } else if (action?.ActionType == ActionType.ShiftDown) {
            y_offset = Math.Max(y_offset, bot_position - height + 2*ITEM_HEIGHT);
        } else {
            y_offset = Math.Min(Math.Max(position - height + ITEM_HEIGHT, y_offset), position);
        }

        scrollCommands.Offset = new Vector(scrollCommands.Offset.X, y_offset);

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

    private async void RemapControls(object sender, RoutedEventArgs e) {
        RemapControlsWindow window = new() {
            DataContext = new RemapControlsViewModel(controls!.Export())
        };
        KeyboardControls? keyboardControls = await window.ShowDialog<KeyboardControls>(this);
        if (keyboardControls != null) {
            controls = keyboardControls;
            SaveConfiguration();
        }
    }

    private void ChangeCommitColors(object sender, RoutedEventArgs e) {
    }
}
