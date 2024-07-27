using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using ChronoGit.ViewModels;

namespace ChronoGit.Views;

public sealed partial class RemapControlsWindow : Window {
    public RemapControlsWindow() {
        InitializeComponent();
    }
    protected override void OnOpened(EventArgs e) {
        base.OnOpened(e);
    }
}
