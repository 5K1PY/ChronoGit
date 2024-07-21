using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ChronoGit.Views;

public partial class EditView : UserControl {
    public EditView() {
        InitializeComponent();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}
