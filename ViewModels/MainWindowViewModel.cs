using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ReactiveUI;
using ChronoGit.Models;
using System.Linq;

namespace ChronoGit.ViewModels;

public enum Mode {
    NormalMode,
    InsertMode,
    VisualMode
};

public class MainWindowViewModel : ViewModelBase {
    private ObservableCollection<CommandViewModel> _commands;
    public ObservableCollection<CommandViewModel> Commands {
        get => _commands;
        set {
            Console.WriteLine("update");
            this.RaiseAndSetIfChanged(ref _commands, value);
        }
    }
    public bool CommandsEmpty => !Commands.Any();
    public MainWindowViewModel() {
        var commits = Init.GetCommits();
        _commands = new ObservableCollection<CommandViewModel>();
        foreach (PickCommand action in commits) {
            _commands.Add(new PickViewModel(action));
        }
        _commands[0].Selected = true;
        Commands.CollectionChanged += (s, e) => this.RaisePropertyChanged(nameof(CommandsEmpty));

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

    public int SelectedStart() {
        if (CurrentMode == Mode.NormalMode || CurrentMode == Mode.InsertMode) {
            return CurrentPosition;
        } else if (CurrentMode == Mode.VisualMode) {
            return Math.Min(CurrentPosition, VisualModeStartPosition);
        }
        throw new NotImplementedException();
    }

    public int SelectedEnd() {
        if (Commands.Count == 0) return -1;
        if (CurrentMode == Mode.NormalMode || CurrentMode == Mode.InsertMode) {
            return CurrentPosition;
        } else if (CurrentMode == Mode.VisualMode) {
            return Math.Max(CurrentPosition, VisualModeStartPosition);
        }
        throw new NotImplementedException();
    }

    public int SelectedRangeLength() {
        return SelectedEnd() - SelectedStart() + 1;
    }

    public IEnumerable<int> SelectedRange() {
        for (int i = SelectedStart(); i <= SelectedEnd(); i++) {
            yield return i;
        }
    }

    public void NormalMode() {
        if (CurrentMode == Mode.VisualMode) {
            foreach (int i in SelectedRange()) {
                if (i != CurrentPosition) {
                    Commands[i].Selected = false;
                }
            }
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

    private void MovePositionTo(int targetPosition) {
        targetPosition = Math.Max(0, Math.Min(targetPosition, Commands.Count-1));
        if (Commands.Count == 0) return;

        for (int i=Math.Min(targetPosition, CurrentPosition); i<=Math.Max(targetPosition, CurrentPosition); i++) {
            Commands[i].Selected = (
                i == targetPosition ||
                CurrentMode == Mode.VisualMode &&
                Math.Min(targetPosition, VisualModeStartPosition) <= i &&
                i <= Math.Max(targetPosition, VisualModeStartPosition)
            );
        }
        CurrentPosition = targetPosition;
    }

    public void MoveUp() {
        MovePositionTo(CurrentPosition - 1);
    }

    public void MoveDown() {
        MovePositionTo(CurrentPosition + 1);
    }

    public void MoveToTop() {
        MovePositionTo(0);
    }

    public void MoveToBottom() {
        MovePositionTo(Commands.Count);
    }

    int historyPosition = 0;
    private List<ActionLog> history = [];

    private void RunAction(Action<ObservableCollection<CommandViewModel>> action) {
        foreach (int i in SelectedRange()) {
            Commands[i].Selected = false;
        }

        action(Commands);
        CurrentPosition = Math.Max(0, Math.Min(CurrentPosition, Commands.Count-1));
        VisualModeStartPosition = Math.Max(0, Math.Min(VisualModeStartPosition, Commands.Count-1));

        foreach (int i in SelectedRange()) {
            Commands[i].Selected = true;
        }
    }

    private void Act(ActionLog action) {
        if (historyPosition < history.Count) {
            history.RemoveRange(historyPosition, history.Count - historyPosition);
        }

        RunAction(action.Change);
        history.Add(action);
        historyPosition++;
    }

    public void Undo() {
        if (historyPosition > 0) {
            historyPosition--;
            RunAction(history[historyPosition].UndoChange);
            MovePositionTo(history[historyPosition].PositionBefore);
            NormalMode();
        }
    }

    public void Redo() {
        if (historyPosition < history.Count) {
            RunAction(history[historyPosition].Change);
            MovePositionTo(history[historyPosition].PositionAfter);
            historyPosition++;
            NormalMode();
        }
    }

    public void ShiftUp() {
        if (SelectedStart() == 0) return;

        List<CommandViewModel> replace = Commands.Slice(SelectedStart(), SelectedRangeLength());
        replace.Add(Commands[SelectedStart()-1]);
        Act(new ReplaceRangeLog(Commands, SelectedStart()-1, replace, positionBefore: SelectedStart()));

        if (CurrentMode == Mode.VisualMode) {
            Commands[VisualModeStartPosition--].Selected = CurrentPosition > VisualModeStartPosition;
            Commands[VisualModeStartPosition].Selected = true;
        }
        MovePositionTo(CurrentPosition-1);
    }

    public void ShiftDown() {
        if (SelectedEnd()+1 == Commands.Count) return;

        List<CommandViewModel> replace = Commands.Slice(SelectedStart(), SelectedRangeLength());
        replace.Insert(0, Commands[SelectedEnd()+1]);
        Act(new ReplaceRangeLog(Commands, SelectedStart(), replace, positionAfter: SelectedStart()+1));

        if (CurrentMode == Mode.VisualMode) {
            Commands[VisualModeStartPosition++].Selected = CurrentPosition < VisualModeStartPosition;
            Commands[VisualModeStartPosition].Selected = true;
        }
        MovePositionTo(CurrentPosition+1);
    }

    public void Delete() {
        Act(new RemoveRangeLog(Commands, SelectedStart(), SelectedRangeLength()));
        MovePositionTo(SelectedStart());
        NormalMode();
    }

    // Conversions

    private delegate CommandViewModel ConvertCommitCommand(CommitCommandViewModel ccvm);
    private void ConvertCommitCommands(ConvertCommitCommand convert) {
        List<CommandViewModel> replaced = [];
        foreach (int pos in SelectedRange()) {
            CommitCommandViewModel? ccvm = Commands[pos] as CommitCommandViewModel;
            replaced.Add(
                ccvm != null ? convert(ccvm) : Commands[pos]
            );
        }
        Act(new ReplaceRangeLog(Commands, SelectedStart(), replaced));
        NormalMode();
    }

    public void ConvertToEdit() {
        ConvertCommitCommands(CommitCommandConversions.ToEdit);
    }

    public void ConvertToFixup() {
        ConvertCommitCommands(CommitCommandConversions.ToFixup);
    }

    public void ConvertToPick() {
        ConvertCommitCommands(CommitCommandConversions.ToPick);
    }

    public void ConvertToReword() {
        ConvertCommitCommands(CommitCommandConversions.ToReword);
    }

    public void ConvertToSquash() {
        ConvertCommitCommands(CommitCommandConversions.ToSquash);
    }

    public void ConvertToDrop() {
        ConvertCommitCommands(CommitCommandConversions.ToDrop);
    }

    // Creations

    private void InsertAndMoveTo(CommandViewModel cvm, int targetPosition) {
        Act(new InsertLog(targetPosition, cvm));
        MovePositionTo(targetPosition);
        NormalMode();
    }

    private void InsertBefore(CommandViewModel cvm) {
        InsertAndMoveTo(cvm, SelectedStart());
    }

    private void InsertAfter(CommandViewModel cvm) {
        InsertAndMoveTo(cvm, SelectedEnd()+1);
    }

    public void AddLabelBefore() {
        InsertBefore(new LabelViewModel());
        InsertMode();
    }

    public void AddLabelAfter() {
        InsertAfter(new LabelViewModel());
        InsertMode();
    }
}
