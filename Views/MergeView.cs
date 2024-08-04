using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ChronoGit.Views;

public partial class MergeView : UserControl {
    public MergeView() {
        InitializeComponent();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}
