using ReactiveUI;
using ChronoGit.Models;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChronoGit.ViewModels;

public enum Mode {
    NormalMode,
    InsertMode,
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

    private Mode _currentMode = Mode.NormalMode;
    public Mode CurrentMode {
        get => _currentMode;
        set => this.RaiseAndSetIfChanged(ref _currentMode, value);
    }
    private int _currentPosition = 0;
    public int CurrentPosition  {
        get => _currentPosition;
        set => this.RaiseAndSetIfChanged(ref _currentPosition, value);
    }
    public int VisualModeStartPosition { get; private set; } = 0;

    private int SelectedStart() {
        if (CurrentMode == Mode.NormalMode || CurrentMode == Mode.InsertMode) {
            return CurrentPosition;
        } else if (CurrentMode == Mode.VisualMode) {
            return Math.Min(CurrentPosition, VisualModeStartPosition);
        }
        throw new NotImplementedException();
    }

    private int SelectedEnd() {
        if (CurrentMode == Mode.NormalMode || CurrentMode == Mode.InsertMode) {
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

    public void NormalMode() {
        if (CurrentMode == Mode.VisualMode) {
            foreach (int i in SelectedRange()) {
                Commands[i].Selected = false;
            }
            Commands[CurrentPosition].Selected = true;
        }
        CurrentMode = Mode.NormalMode;
    }

    public void ToggleVisualMode() {
        if (CurrentMode == Mode.NormalMode) {
            CurrentMode = Mode.VisualMode;
            VisualModeStartPosition = CurrentPosition;
        } else {
            NormalMode();
        }
    }

    public void InsertMode() {
        CurrentMode = Mode.InsertMode;
    }

    public void MoveUp() {
        if (CurrentPosition - 1 < 0) return;

        Commands[CurrentPosition].Selected = (
            CurrentMode == Mode.VisualMode &&
            CurrentPosition <= VisualModeStartPosition
        );

        CurrentPosition--;
        Commands[CurrentPosition].Selected = true;
    }

    public void MoveDown() {
        if (CurrentPosition + 1 >= Commands.Count) return;

        Commands[CurrentPosition].Selected = (
            CurrentMode == Mode.VisualMode &&
            CurrentPosition >= VisualModeStartPosition
        );

        CurrentPosition++;
        Commands[CurrentPosition].Selected = true;
    }

    public void ShiftUp() {
        if (SelectedStart() == 0) return;

        foreach (int pos in SelectedRange()) {
            (Commands[pos-1], Commands[pos]) = (Commands[pos], Commands[pos-1]);
        }
        CurrentPosition--; VisualModeStartPosition--;
    }

    public void ShiftDown() {
        if (SelectedEnd() == Commands.Count) return;

        foreach (int pos in SelectedRange().Reverse()) {
            (Commands[pos], Commands[pos+1]) = (Commands[pos+1], Commands[pos]);
        }
        CurrentPosition++; VisualModeStartPosition++;
    }

    public void Delete() {
        int repeat = SelectedEnd() - SelectedStart() + 1;
        for (int i=0; i<repeat; i++) {
            Commands.RemoveAt(SelectedStart());
        }
        CurrentPosition = SelectedStart();
        if (CurrentPosition == Commands.Count) CurrentPosition--;
        Commands[CurrentPosition].Selected = true;
        NormalMode();
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

    public void ConvertToEdit() {
        ConvertCommitCommands(CommitCommandConversions.ToEdit);
        NormalMode();
    }

    public void ConvertToFixup() {
        ConvertCommitCommands(CommitCommandConversions.ToFixup);
        NormalMode();
    }

    public void ConvertToPick() {
        ConvertCommitCommands(CommitCommandConversions.ToPick);
        NormalMode();
    }

    public void ConvertToReword() {
        ConvertCommitCommands(CommitCommandConversions.ToReword);
        NormalMode();
    }

    public void ConvertToSquash() {
        ConvertCommitCommands(CommitCommandConversions.ToSquash);
        NormalMode();
    }

    // Creations

    private void Insert(CommandViewModel cvm) {
        Commands.Insert(SelectedEnd()+1, cvm);
        NormalMode();
        MoveDown();
    }

    public void AddLabel() {
        Insert(new LabelViewModel(new LabelCommand("")));
        InsertMode();
    }
}
