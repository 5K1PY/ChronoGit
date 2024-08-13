using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;

namespace ChronoGit.Views;

public static class FindExtensions {
    public static T? FindDescendant<T>(this Visual visual, string name) where T : Visual {
        if (visual.Name == name && visual is T)
            return (T) visual;
        foreach (var child in visual.GetVisualChildren()) {
            T? childRes = child.FindDescendant<T>(name);
            if (childRes != null) return childRes;
        }
        return null;
    }
}

public partial class WindowBase : Window {
    protected Dictionary<Key, bool> ModifiersPressed = new Dictionary<Key, bool>{
        {Key.LeftShift, false},
        {Key.RightShift, false},
        {Key.LeftCtrl, false},
        {Key.RightCtrl, false},
    };

    protected void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    protected KeyCombination GetCurrentKeyCombination(Key key) {
        return new KeyCombination(
            ModifiersPressed[Key.LeftShift] || ModifiersPressed[Key.RightShift],
            ModifiersPressed[Key.LeftCtrl] || ModifiersPressed[Key.RightCtrl],
            key
        );
    }

    protected virtual void WindowKeyDown(object sender, KeyEventArgs e) {
        if (ModifiersPressed.ContainsKey(e.Key)) {
            ModifiersPressed[e.Key] = true;
        }
    }

    protected virtual void WindowKeyUp(object sender, KeyEventArgs e) {
        if (ModifiersPressed.ContainsKey(e.Key)) {
            ModifiersPressed[e.Key] = false;
        }
    }
}
