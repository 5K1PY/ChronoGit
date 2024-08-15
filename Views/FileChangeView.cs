using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ChronoGit.Views;

public partial class FileChangeView : UserControl {
    public FileChangeView() {
        InitializeComponent();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}
