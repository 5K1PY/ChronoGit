using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ChronoGit.Views;

public partial class PickView : UserControl {
    public PickView() {
        InitializeComponent();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}
