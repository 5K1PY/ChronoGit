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

    private int SelectedStart() {
        if (CurrentMode == Mode.NormalMode) {
            return CurrentPosition;
        } else if (CurrentMode == Mode.VisualMode) {
            return Math.Min(CurrentPosition, VisualModeStartPosition);
        }
        throw new NotImplementedException();
    }

    private int SelectedEnd() {
        if (CurrentMode == Mode.NormalMode) {
            return CurrentPosition;
        } else if (CurrentMode == Mode.VisualMode) {
            return Math.Max(CurrentPosition, VisualModeStartPosition);
        }
        throw new NotImplementedException();
    }

    private IEnumerable<int> SelectedRange() {
        for (int i = SelectedStart(); i <= SelectedEnd(); i++) {
            yield return i;
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
        if (SelectedStart() == 0) return;

        foreach (int pos in SelectedRange()) {
            (Commands[pos-1], Commands[pos]) = (Commands[pos], Commands[pos-1]);
        }
        CurrentPosition--; VisualModeStartPosition--;
    }

    public void DeletePressed() {
        int repeat = SelectedEnd() - SelectedStart() + 1;
        for (int i=0; i<repeat; i++) {
            Commands.RemoveAt(SelectedStart());
        }
        CurrentPosition = SelectedStart();
        if (CurrentPosition == Commands.Count) CurrentPosition--;
        Commands[CurrentPosition].Selected = true;
        ExitVisualMode();
    }

    public void ControlDownArrowPressed() {
        if (SelectedEnd() == Commands.Count) return;

        foreach (int pos in SelectedRange().Reverse()) {
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

    public void FPressed() {
        ConvertCommitCommands(CommitCommandConversions.ToFixup);
        ExitVisualMode();
    }

    public void PPressed() {
        ConvertCommitCommands(CommitCommandConversions.ToPick);
        ExitVisualMode();
    }

    public void RPressed() {
        ConvertCommitCommands(CommitCommandConversions.ToReword);
        ExitVisualMode();
    }

    public void SPressed() {
        ConvertCommitCommands(CommitCommandConversions.ToSquash);
        ExitVisualMode();
    }

    // Creations

    private void Insert(CommandViewModel cvm) {
        Commands.Insert(SelectedEnd()+1, cvm);
        ExitVisualMode();
        DownArrowPressed();
    }

    public void LPressed() {
        Insert(new LabelViewModel(new LabelCommand("")));
    }
}
