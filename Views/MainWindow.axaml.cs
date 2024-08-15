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

    KeyboardControls? controls;

    public MainWindow() {
        InitializeComponent();
    }

    protected override void OnOpened(EventArgs e) {
        base.OnOpened(e);
        dataContext = (DataContext as MainWindowViewModel)!;
        controls = KeyboardControls.Default(dataContext);
    }

    private ViewData GetViewData() {
        return new ViewData((int) (ScrollCommands.Bounds.Height / ITEM_HEIGHT));
    }

    protected override void WindowKeyDown(object sender, KeyEventArgs e) {
        base.WindowKeyDown(sender, e);

        KeyCombination currentKeyCombination = GetCurrentKeyCombination(e.Key);

        NamedAction? action = controls!.GetAction(currentKeyCombination);

        if (
            dataContext!.VimMode != VimMode.InsertMode ||
            currentKeyCombination.CtrlPressed || 
            action?.ActionType == ActionType.ExitVimMode
        ) {
            e.Handled = true;
        } else {
            return;
        }

        action?.Action.Invoke(GetViewData());

        double height = ScrollCommands.Bounds.Height;
        int commandsPerPage = (int) (height / ITEM_HEIGHT);
        double y_offset = ScrollCommands.Offset.Y;
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

        if (action?.Name == ActionDescriptions.MOVE_PAGE_UP) {
            ScrollCommands.Offset -= new Vector(0, commandsPerPage * ITEM_HEIGHT);
        } else if (action?.Name == ActionDescriptions.MOVE_PAGE_DOWN) {
            ScrollCommands.Offset += new Vector(0, commandsPerPage * ITEM_HEIGHT);
        } else {
            ScrollCommands.Offset = new Vector(ScrollCommands.Offset.X, y_offset);
        }

        Control? control = CommandsView.ContainerFromIndex(dataContext!.CurrentPosition);
        control?.UpdateLayout(); // FindDescendant doesn't work on not yet updated
        TextBox? FocusBox = control?.FindDescendant<TextBox>("FocusHere");
        if (dataContext.VimMode == VimMode.InsertMode && FocusBox != null) {
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

    private async void ChangeCommitColors(object sender, RoutedEventArgs e) {
        ChangeColorsWindow window = new() {
            DataContext = new ChangeColorsViewModel()
        };
        ColorByActionData? data = await window.ShowDialog<ColorByActionData>(this);

        if (data is ColorSameData data1) {
            dataContext!.DefaultCommitColor = data1.ChosenColor;
            dataContext!.ColorSame(GetViewData());
        } else if (data is ColorByAuthorData) {
            dataContext!.ColorByAuthor(GetViewData());
        } else if (data is ColorByDateData) {
            dataContext!.ColorByDate(GetViewData());
        } else if (data is ColorByRegexData data2) {
            dataContext!.ColorByRegex(GetViewData(), data2.Regex, data2.Group);
        }
    }

    private void Finish(object sender, RoutedEventArgs e) {
        dataContext!.Finish();
        Close();
    }

    private void Abort(object sender, RoutedEventArgs e) {
        dataContext!.Abort();
        Close();
    }
}
