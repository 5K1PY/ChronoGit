using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ReactiveUI;
using ChronoGit.Models;
using System.Linq;
using System.Text.RegularExpressions;
using LibGit2Sharp;

namespace ChronoGit.ViewModels;

public record class ViewData(int CommandsPerPage);

public enum VimMode {
    NormalMode,
    InsertMode,
    VisualMode
};

public enum ViewMode {
    JustCommands,
    CommitDetails
};

public sealed class MainWindowViewModel : ViewModelBase {
    private Repo Repo { get; init; }
    private ObservableCollection<CommandViewModel> _commands;
    public ObservableCollection<CommandViewModel> Commands {
        get => _commands;
        set {
            this.RaiseAndSetIfChanged(ref _commands, value);
        }
    }
    private IEnumerable<CommitCommandViewModel> CommitCommands() {
        foreach (CommandViewModel cvm in Commands) {
            if (cvm is CommitCommandViewModel model)
                yield return model;
        }
    }
    public bool CommandsEmpty => !Commands.Any();
    public void Finish() {
        Repo.Export(Commands.Select(c => c.Command));
        Repo.Dispose();
    }
    
    public void Abort() {
        Repo.Abort();
        Repo.Dispose();
    }

    public CommitColor DefaultCommitColor = CommitColor.Red;
    public MainWindowViewModel(string filePath) {
        Repo = new Repo(filePath);
        var commits = Repo.GetCommits();

        _commands = new ObservableCollection<CommandViewModel>();
        foreach (PickCommand action in commits) {
            _commands.Add(new PickViewModel(action, DefaultCommitColor, Repo.repo));
        }
        if(_commands.Count > 0)
            _commands[0].Selected = true;
        Commands.CollectionChanged += (s, e) => this.RaisePropertyChanged(nameof(CommandsEmpty));
    }

    private ViewMode _viewMode = ViewMode.JustCommands;
    public ViewMode ViewMode {
        get => _viewMode;
        set {
            if (_viewMode != value) {
                _viewMode = value;
                this.RaisePropertyChanged(nameof(ViewMode));
                this.RaisePropertyChanged(nameof(SplitView));
                this.RaisePropertyChanged(nameof(ShowCommitDetails));
            }
        }
    }
    public bool SplitView => ViewMode != ViewMode.JustCommands; 
    public bool ShowCommitDetails => ViewMode == ViewMode.CommitDetails;

    public void ToggleCommitDetails(ViewData _) {
        Console.WriteLine("here");
        ViewMode = ViewMode == ViewMode.CommitDetails ? ViewMode.JustCommands : ViewMode.CommitDetails;
    }

    private VimMode _vimMode = VimMode.NormalMode;
    public VimMode VimMode {
        get => _vimMode;
        set => this.RaiseAndSetIfChanged(ref _vimMode, value);
    }
    private int _currentPosition = 0;
    public int CurrentPosition {
        get => _currentPosition;
        set {
            if (_currentPosition != value) {
                _currentPosition = value;
                this.RaisePropertyChanged(nameof(CurrentPosition));
                this.RaisePropertyChanged(nameof(CurrentCommit));
            }
        }
    }
    public int VisualModeStartPosition { get; private set; } = 0;
    private string BeforeInsertModeArgument = "";
    
    public CommitCommandViewModel? CurrentCommit {
        get {
            if (Commands[CurrentPosition] is CommitCommandViewModel ccvm) {
                return ccvm;
            } else {
                return null;
            } 
        }
    }

    public int SelectedStart() {
        if (VimMode == VimMode.NormalMode || VimMode == VimMode.InsertMode) {
            return CurrentPosition;
        } else if (VimMode == VimMode.VisualMode) {
            return Math.Min(CurrentPosition, VisualModeStartPosition);
        }
        throw new NotImplementedException();
    }

    public int SelectedEnd() {
        if (Commands.Count == 0) return -1;
        if (VimMode == VimMode.NormalMode || VimMode == VimMode.InsertMode) {
            return CurrentPosition;
        } else if (VimMode == VimMode.VisualMode) {
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
        if (VimMode == VimMode.VisualMode) {
            foreach (int i in SelectedRange()) {
                if (i != CurrentPosition) {
                    Commands[i].Selected = false;
                }
            }
        } else if (VimMode == VimMode.InsertMode) {
            Act(new ChangeArgumentLog(
                CurrentPosition,
                BeforeInsertModeArgument,
                (Commands[CurrentPosition] as ArgumentCommandViewModel)!.Argument
            ));
        }
        VimMode = VimMode.NormalMode;
    }

    public void ExitCurrentMode(ViewData _) {
        NormalMode();
    }

    public void ToggleVisualMode(ViewData _) {
        if (VimMode == VimMode.NormalMode) {
            VimMode = VimMode.VisualMode;
            VisualModeStartPosition = CurrentPosition;
        } else {
            NormalMode();
        }
    }

    public void InsertMode() {
        if (VimMode != VimMode.NormalMode) return;

        ArgumentCommandViewModel? acvm = Commands[CurrentPosition] as ArgumentCommandViewModel;
        if (acvm is not null) {
            VimMode = VimMode.InsertMode;
            BeforeInsertModeArgument = acvm.Argument;
        }
    }

    public void EnterInsertMode(ViewData _) {
        InsertMode();
    }

    private void MovePositionTo(int targetPosition) {
        targetPosition = Math.Max(0, Math.Min(targetPosition, Commands.Count-1));
        if (Commands.Count == 0) return;

        for (int i=Math.Min(targetPosition, CurrentPosition); i<=Math.Max(targetPosition, CurrentPosition); i++) {
            Commands[i].Selected = (
                i == targetPosition ||
                VimMode == VimMode.VisualMode &&
                Math.Min(targetPosition, VisualModeStartPosition) <= i &&
                i <= Math.Max(targetPosition, VisualModeStartPosition)
            );
        }
        CurrentPosition = targetPosition;
    }

    public void MoveUp(ViewData _) {
        MovePositionTo(CurrentPosition - 1);
    }

    public void MoveDown(ViewData _) {
        MovePositionTo(CurrentPosition + 1);
    }

    public void MovePageUp(ViewData viewData) {
        MovePositionTo(CurrentPosition - viewData.CommandsPerPage);
    }

    public void MovePageDown(ViewData viewData) {
        MovePositionTo(CurrentPosition + viewData.CommandsPerPage);
    }

    public void MoveToTop(ViewData _) {
        MovePositionTo(0);
    }

    public void MoveToBottom(ViewData _) {
        MovePositionTo(Commands.Count);
    }

    int historyPosition = 0;
    private List<ActionLog> history = [];

    private void RunAction(Action<MainWindowViewModel> action) {
        foreach (int i in SelectedRange()) {
            Commands[i].Selected = false;
        }

        action(this);
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

        if (action.ChangesAnything) {
            RunAction(action.Change);
            history.Add(action);
            historyPosition++;
        }
    }

    public void Undo(ViewData _) {
        if (historyPosition > 0) {
            historyPosition--;
            RunAction(history[historyPosition].UndoChange);

            int? pos = history[historyPosition].PositionBefore;
            if (pos is not null) MovePositionTo((int) pos);
            
            NormalMode();
        }
    }

    public void Redo(ViewData _) {
        if (historyPosition < history.Count) {
            RunAction(history[historyPosition].Change);

            int? pos = history[historyPosition].PositionAfter;
            if (pos is not null) MovePositionTo((int) pos);

            historyPosition++;
            NormalMode();
        }
    }

    public void ShiftUp(ViewData _) {
        if (SelectedStart() == 0) return;

        List<CommandViewModel> replace = Commands.Slice(SelectedStart(), SelectedRangeLength());
        replace.Add(Commands[SelectedStart()-1]);
        Act(new ReplaceRangeLog(Commands, SelectedStart()-1, replace, positionBefore: SelectedStart()));

        if (VimMode == VimMode.VisualMode) {
            Commands[VisualModeStartPosition--].Selected = CurrentPosition > VisualModeStartPosition;
            Commands[VisualModeStartPosition].Selected = true;
        }
        MovePositionTo(CurrentPosition-1);
    }

    public void ShiftDown(ViewData _) {
        if (SelectedEnd()+1 == Commands.Count) return;

        List<CommandViewModel> replace = Commands.Slice(SelectedStart(), SelectedRangeLength());
        replace.Insert(0, Commands[SelectedEnd()+1]);
        Act(new ReplaceRangeLog(Commands, SelectedStart(), replace, positionAfter: SelectedStart()+1));

        if (VimMode == VimMode.VisualMode) {
            Commands[VisualModeStartPosition++].Selected = CurrentPosition < VisualModeStartPosition;
            Commands[VisualModeStartPosition].Selected = true;
        }
        MovePositionTo(CurrentPosition+1);
    }

    public void Delete(ViewData _) {
        Act(new RemoveRangeLog(Commands, SelectedStart(), SelectedRangeLength()));
        MovePositionTo(SelectedStart());
        NormalMode();
    }

    // Conversions

    private delegate CommandViewModel ConvertCommitCommand(CommitCommandViewModel ccvm, Command? GlobalCommand);
    private void ConvertCommitCommands(ConvertCommitCommand convert) {
        List<CommandViewModel> replaced = [];
        foreach (int pos in SelectedRange()) {
            CommitCommandViewModel? ccvm = Commands[pos] as CommitCommandViewModel;
            replaced.Add(
                ccvm != null ? convert(ccvm, GlobalCommand) : Commands[pos]
            );
        }
        Act(new ReplaceRangeLog(Commands, SelectedStart(), replaced));
        NormalMode();
    }

    public void ConvertToEdit(ViewData _) {
        ConvertCommitCommands(CommitCommandConversions.ToEdit);
    }

    public void ConvertToFixup(ViewData _) {
        ConvertCommitCommands(CommitCommandConversions.ToFixup);
    }

    public void ConvertToPick(ViewData _) {
        ConvertCommitCommands(CommitCommandConversions.ToPick);
    }

    public void ConvertToReword(ViewData _) {
        ConvertCommitCommands(CommitCommandConversions.ToReword);
    }

    public void ConvertToSquash(ViewData _) {
        ConvertCommitCommands(CommitCommandConversions.ToSquash);
    }

    public void ConvertToDrop(ViewData _) {
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

    public void AddBreakBefore(ViewData _) {
        InsertBefore(new BreakViewModel());
    }

    public void AddBreakAfter(ViewData _) {
        InsertAfter(new BreakViewModel());
    }

    public void AddExecBefore(ViewData _) {
        InsertBefore(new ExecViewModel());
        InsertMode();
    }

    public void AddExecAfter(ViewData _) {
        InsertAfter(new ExecViewModel());
        InsertMode();
    }

    public void AddLabelBefore(ViewData _) {
        InsertBefore(new LabelViewModel());
        InsertMode();
    }

    public void AddLabelAfter(ViewData _) {
        InsertAfter(new LabelViewModel());
        InsertMode();
    }

    public void AddResetBefore(ViewData _) {
        InsertBefore(new ResetViewModel());
        InsertMode();
    }

    public void AddResetAfter(ViewData _) {
        InsertAfter(new ResetViewModel());
        InsertMode();
    }

    public void AddMergeBefore(ViewData _) {
        InsertBefore(new MergeViewModel());
        InsertMode();
    }

    public void AddMergeAfter(ViewData _) {
        InsertAfter(new MergeViewModel());
        InsertMode();
    }

    // Colors
    public void ColorSame(ViewData _) {
        foreach (CommitCommandViewModel ccvm in CommitCommands()) {
            ccvm.Color = DefaultCommitColor;
        }
    }

    private void ColorBy<T>(Func<CommitCommandViewModel, T> getGroup) where T : notnull {
        CommitColor currentColor = 0;
        Dictionary<T, CommitColor> getColor = [];

        foreach (CommitCommandViewModel ccvm in CommitCommands()) {
            T key = getGroup(ccvm);
            if (!getColor.ContainsKey(key)) {
                getColor[key] = currentColor;
                currentColor = currentColor.Next();
            }
            ccvm.Color = getColor[key];
        }
    }

    public void ColorByAuthor(ViewData _) {
        ColorBy(ccvm => ccvm.Author);
    }

    public void ColorByDate(ViewData _) {
        ColorBy(ccvm => ccvm.Date);
    }

    public void ColorByRegex(ViewData _, string regex, int group) {
        ColorBy(ccvm => {
            Match match = Regex.Match(ccvm.MessageShort, regex);
            if (match.Success) return match.Groups[group].Value;
            else return "";
        });
    }

    private Command? _globalCommand = null;
    public Command? GlobalCommand { 
        get => _globalCommand;
        set {
            foreach (CommandViewModel command in Commands) {
                if (command is CommittingCommandViewModel c) {
                    c.SetGlobalCommand(value);
                }
            }
            _globalCommand = value;
        }
    }

    public void ChangeGlobalCommand(Command? newGlobalCommand) {
        Act(new ChangeGlobalCommandLog(GlobalCommand, newGlobalCommand));
    }
}
