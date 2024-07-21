using ReactiveUI;
using ChronoGit.Models;
using System.Collections.ObjectModel;
using System;

namespace ChronoGit.ViewModels;

public enum Mode {
    NormalMode,
    VisualMode
};

public class MainWindowViewModel : ViewModelBase {
    private ObservableCollection<CommandViewModel> _actions;
    public ObservableCollection<CommandViewModel> Actions
    {
        get => _actions;
        set => this.RaiseAndSetIfChanged(ref _actions, value);
    }
    public MainWindowViewModel() {
        var commits = Init.GetCommits();
        _actions = new ObservableCollection<CommandViewModel>();
        foreach (PickCommand action in commits) {
            _actions.Add(new PickViewModel(action));
        }
        _actions[0].Selected = true;
    }

    public Mode CurrentMode { get; private set; } = Mode.NormalMode;
    public int CurrentPosition { get; private set; } = 0;
    public int VisualModeStartPosition { get; private set; } = 0;

    public void UpArrowPressed() {
        if (CurrentPosition - 1 < 0) return;

        Actions[CurrentPosition].Selected = CurrentMode == Mode.NormalMode ?
            false : CurrentPosition <= VisualModeStartPosition;

        CurrentPosition--;
        Actions[CurrentPosition].Selected = true;
    }

    public void DownArrowPressed() {
        if (CurrentPosition + 1 >= Actions.Count) return;

        Actions[CurrentPosition].Selected = CurrentMode == Mode.NormalMode ?
            false : CurrentPosition >= VisualModeStartPosition;

        CurrentPosition++;
        Actions[CurrentPosition].Selected = true;
    }

    public void VPressed() {
        if (CurrentMode == Mode.NormalMode) {
            CurrentMode = Mode.VisualMode;
            VisualModeStartPosition = CurrentPosition;
        } else {
            for (int i = Math.Min(CurrentPosition, VisualModeStartPosition); i <= Math.Max(CurrentPosition, VisualModeStartPosition); i++) {
                Actions[i].Selected = false;
            }
            Actions[CurrentPosition].Selected = true;
            CurrentMode = Mode.NormalMode;
        }
    }
}
