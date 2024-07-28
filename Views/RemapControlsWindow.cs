using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ChronoGit.ViewModels;

namespace ChronoGit.Views;

public sealed partial class RemapControlsWindow : WindowBase {
    public RemapControlsWindow() {
        InitializeComponent();
    }

    protected override void OnOpened(EventArgs e) {
        base.OnOpened(e);
        dataContext = (DataContext as RemapControlsViewModel)!;
    }

    private RemapControlsViewModel? dataContext;
    private int? remappingPosition;
    private Button? remappingButton;
    public void RemapControl(object sender, RoutedEventArgs args) {
        if (remappingPosition != null) {
            remappingButton!.Content = dataContext!.Controls[(int) remappingPosition].KeyCombination;
        }
        remappingButton = (sender as Button)!;
        remappingButton.Content = "Listening...";
        remappingPosition = dataContext!.Controls.IndexOf((remappingButton.DataContext as BoundAction)!);
    }

    public void Cancel(object sender, RoutedEventArgs args) {
        Close(null);
    }

    public void Save(object sender, RoutedEventArgs args) {
        Close(new KeyboardControls(dataContext!.Controls));
    }

    protected override void WindowKeyDown(object sender, KeyEventArgs e) {
        base.WindowKeyDown(sender, e);
        if (remappingPosition != null && !ModifiersPressed.ContainsKey(e.Key)) {
            KeyCombination keyComb = GetCurrentKeyCombination(e.Key);
            dataContext!.Controls[(int) remappingPosition] = new BoundAction(
                dataContext!.Controls[(int) remappingPosition].NamedAction,
                keyComb
            );
        }
    }
}
