using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ChronoGit.Models;
using ChronoGit.ViewModels;

namespace ChronoGit.Views;

public sealed partial class GlobalCommandWindow : WindowBase {
    private GlobalCommandViewModel? dataContext;
    public GlobalCommandWindow() {
        InitializeComponent();
    }

    protected override void OnOpened(EventArgs e) {
        base.OnOpened(e);
        dataContext = (DataContext as GlobalCommandViewModel)!;
    }
    public void Cancel(object sender, RoutedEventArgs args) {
        Close(null);
    }

    public void Save(object sender, RoutedEventArgs args) {
        Command? data = null;
        if (SetBreak.IsChecked == true) {
            data = new BreakCommand();
        } else if (SetExec.IsChecked == true) {
            data = new ExecCommand(dataContext!.ExecCommand);
        }

        Close(new Tuple<Command?>(data));
    }

    private void FilterNumbers(object? sender, TextInputEventArgs e) {
        string new_text = (sender as TextBox)!.Text + e.Text;
        if (!int.TryParse(new_text, out _)) {
            e.Handled = true;
        }
    }
}
