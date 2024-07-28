using ReactiveUI;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using ChronoGit.Views;

namespace ChronoGit.ViewModels;


public class RemapControlsViewModel : ViewModelBase {
    private ObservableCollection<BoundAction> _controls;

    public ObservableCollection<BoundAction> Controls {
        get => _controls;
        set => this.RaiseAndSetIfChanged(ref _controls, value);
    }

    public RemapControlsViewModel(ICollection<BoundAction> controls) {
        _controls = new ObservableCollection<BoundAction>(controls);

        Controls.CollectionChanged += (s, e) => this.RaisePropertyChanged(nameof(CollisionsPresent));
        Controls.CollectionChanged += (s, e) => this.RaisePropertyChanged(nameof(CollisionsText));
    }

    public bool CollisionsPresent => GetCollisions().Count > 0;
    public string CollisionsText {
        get {
            Dictionary<KeyCombination, List<NamedAction>> collisions = GetCollisions();
            if (collisions.Count == 0) return "";
            KeyCombination collidingKeyComb = collisions.Keys.First();
            string text = "Multiple use of same shortcut:";
            foreach (NamedAction namedAction in collisions[collidingKeyComb][..2]) {
                text += $"\n - {namedAction.Name}";
            }
            return text;
        }
    }

    public Dictionary<KeyCombination, List<NamedAction>> GetCollisions() {
        Dictionary<KeyCombination, List<NamedAction>> collisionDict = [];
        foreach (BoundAction boundAction in Controls) {
            if (collisionDict.ContainsKey(boundAction.KeyCombination)) {
                collisionDict[boundAction.KeyCombination].Add(boundAction.NamedAction);
            } else {
                collisionDict[boundAction.KeyCombination] = [boundAction.NamedAction];
            }
        }

        foreach (KeyCombination keyComb in collisionDict.Keys) {
            if (collisionDict[keyComb].Count <= 1) {
                collisionDict.Remove(keyComb);
            }
        }

        return collisionDict;
    }
}
