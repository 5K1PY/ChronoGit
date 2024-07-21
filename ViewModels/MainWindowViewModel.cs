using ReactiveUI;
using ChronoGit.Models;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChronoGit.ViewModels;

public enum Mode {
    NormalMode,
    VisualMode
};

public class MainWindowViewModel : ViewModelBase {
    private ObservableCollection<CommandViewModel> _commands;
    public ObservableCollection<CommandViewModel> Commands
    {
        get => _commands;
        set => this.RaiseAndSetIfChanged(ref _commands, value);
    }
    public MainWindowViewModel() {
        var commits = Init.GetCommits();
        _commands = new ObservableCollection<CommandViewModel>();
        foreach (PickCommand action in commits) {
            _commands.Add(new PickViewModel(action));
        }
        _commands[0].Selected = true;
    }

    public Mode CurrentMode { get; private set; } = Mode.NormalMode;
    public int CurrentPosition { get; private set; } = 0;
    public int VisualModeStartPosition { get; private set; } = 0;
    private IEnumerable<int> SelectedRange() {
        if (CurrentMode == Mode.NormalMode) {
            yield return CurrentPosition;
        } else if (CurrentMode == Mode.VisualMode) {
            for (int i = Math.Min(CurrentPosition, VisualModeStartPosition); i <= Math.Max(CurrentPosition, VisualModeStartPosition); i++) {
                yield return i;
            }
        }
    }

    private void ExitVisualMode() {
        if (CurrentMode == Mode.VisualMode) {
            foreach (int i in SelectedRange()) {
                Commands[i].Selected = false;
            }
            Commands[CurrentPosition].Selected = true;
            CurrentMode = Mode.NormalMode;
        }
    }

    public void EscPressed() {
        ExitVisualMode();
    }

    public void VPressed() {
        if (CurrentMode == Mode.NormalMode) {
            CurrentMode = Mode.VisualMode;
            VisualModeStartPosition = CurrentPosition;
        } else {
            ExitVisualMode();
        }
    }

    public void UpArrowPressed() {
        if (CurrentPosition - 1 < 0) return;

        Commands[CurrentPosition].Selected = CurrentMode == Mode.NormalMode ?
            false : CurrentPosition <= VisualModeStartPosition;

        CurrentPosition--;
        Commands[CurrentPosition].Selected = true;
    }

    public void DownArrowPressed() {
        if (CurrentPosition + 1 >= Commands.Count) return;

        Commands[CurrentPosition].Selected = CurrentMode == Mode.NormalMode ?
            false : CurrentPosition >= VisualModeStartPosition;

        CurrentPosition++;
        Commands[CurrentPosition].Selected = true;
    }

    public void ControlUpArrowPressed() {
        IEnumerable<int> range = SelectedRange();
        if (range.First() == 0) return;
        foreach (int pos in range) {
            (Commands[pos-1], Commands[pos]) = (Commands[pos], Commands[pos-1]);
        }
        CurrentPosition--; VisualModeStartPosition--;
    }

    public void ControlDownArrowPressed() {
        IEnumerable<int> range = SelectedRange().Reverse();
        if (range.First() == Commands.Count) return;
        foreach (int pos in range) {
            (Commands[pos], Commands[pos+1]) = (Commands[pos+1], Commands[pos]);
        }
        CurrentPosition++; VisualModeStartPosition++;
    }

    // Conversions

    private delegate CommandViewModel ConvertCommitCommand(CommitCommandViewModel ccvm);
    private void ConvertCommitCommands(ConvertCommitCommand convert) {
        foreach (int pos in SelectedRange()) {
            CommitCommandViewModel? ccvm = Commands[pos] as CommitCommandViewModel;
            if (ccvm != null) {
                Commands[pos] = convert(ccvm).AsSelected();
            }
        }
    }

    public void EPressed() {
        ConvertCommitCommands(CommitCommandConversions.ToEdit);
        ExitVisualMode();
    }

    public void RPressed() {
        ConvertCommitCommands(CommitCommandConversions.ToReword);
        ExitVisualMode();
    }

    public void PPressed() {
        ConvertCommitCommands(CommitCommandConversions.ToPick);
        ExitVisualMode();
    }
}
