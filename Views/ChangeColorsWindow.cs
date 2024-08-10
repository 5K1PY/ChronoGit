using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ChronoGit.ViewModels;

namespace ChronoGit.Views;

public record class ColorByActionData;
public record class ColorSameData(CommitColor ChosenColor) : ColorByActionData;
public record class ColorByAuthorData : ColorByActionData;
public record class ColorByDateData : ColorByActionData;
public record class ColorByRegexData(string Regex, int Group) : ColorByActionData;

public sealed partial class ChangeColorsWindow : WindowBase {
    private ChangeColorsViewModel? dataContext;
    public ChangeColorsWindow() {
        InitializeComponent();
        Groups.AddHandler(TextInputEvent, FilterNumbers, RoutingStrategies.Tunnel);
    }

    protected override void OnOpened(EventArgs e) {
        base.OnOpened(e);
        dataContext = (DataContext as ChangeColorsViewModel)!;
    }
    public void Cancel(object sender, RoutedEventArgs args) {
        Close(null);
    }

    public void Save(object sender, RoutedEventArgs args) {
        ColorByActionData? data = null;
        if (ColorSame.IsChecked == true) {
            data = new ColorSameData(dataContext!.ChosenColor);
        } else if (ColorByAuthor.IsChecked == true) {
            data = new ColorByAuthorData();
        } else if (ColorByDate.IsChecked == true) {
            data = new ColorByDateData();
        } else if (ColorByRegex.IsChecked == true) {
            data = new ColorByRegexData(dataContext!.Regex, dataContext!.Group);
        }

        Close(data);
    }

    private void FilterNumbers(object? sender, TextInputEventArgs e) {
        string new_text = (sender as TextBox)!.Text + e.Text;
        if (!int.TryParse(new_text, out _)) {
            e.Handled = true;
        }
    }
}
