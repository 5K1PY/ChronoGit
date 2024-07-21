using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ChronoGit.Views;

public partial class RewordView : UserControl {
    public RewordView() {
        InitializeComponent();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}
