using ReactiveUI;
using ChronoGit.Models;
using System;
using System.Globalization;
using LibGit2Sharp;
using System.Linq;
using System.Collections.Generic;

namespace ChronoGit.ViewModels;

public enum CommitColor {
    Red = 0,
    Orange = 1,
    Yellow = 2,
    Green = 3,
    Cyan = 4,
    Blue = 5,
    Purple = 6,
    Pink = 7,
}

public static class CommitColorExtensions {
    public static CommitColor Next(this CommitColor color) {
        return (CommitColor) (((int) color + 1) % Enum.GetValues(typeof(CommitColor)).Length);
    }
}

public abstract partial class CommandViewModel : ViewModelBase {
    protected internal abstract Command Command { get; set; }

    public static bool operator==(CommandViewModel? cvm1, CommandViewModel? cvm2) {
        if (cvm1 is null) return cvm2 is null;
        return cvm1.Equals(cvm2);
    }

    public static bool operator!=(CommandViewModel? cvm1, CommandViewModel? cvm2) {
        if (cvm1 is null) return !(cvm2 is null);
        return !cvm1.Equals(cvm2);
    }
    public override bool Equals(object? obj) {
        return (
            GetType() == obj?.GetType() &&
            Command == (obj as CommandViewModel)!.Command
        );
    }
    public override int GetHashCode() {
        return Command.GetHashCode();
    }

    private bool _selected = false;
    public bool Selected {
        get => _selected;
        set => this.RaiseAndSetIfChanged(ref _selected, value);
    }
    public CommandViewModel AsSelected() {
        Selected = true;
        return this;
    }
    public CommandViewModel AsNotSelected() {
        Selected = false;
        return this;
    }
}

public abstract partial class CommitCommandViewModel : CommandViewModel {
    protected internal CommitCommand CommitCommand => (CommitCommand) Command;
    public abstract Repository Repository { get; init; }

    public string Id => CommitCommand.CommandCommit.Id.ToString().Substring(0, 7);
    public string Message => CommitCommand.CommandCommit.Message;
    public string MessageShort => CommitCommand.CommandCommit.MessageShort;
    public string MessageRest => Message.Substring(Message.IndexOf('\n'));
    public string Author => CommitCommand.CommandCommit.Author.Name.ToString();
    public string AuthorEmail => CommitCommand.CommandCommit.Author.Email.ToString();
    public string FullAuthor => string.Format("{0} <{1}>", Author, AuthorEmail);
    private DateTimeOffset dateTimeOffset => CommitCommand.CommandCommit.Author.When;
    public string Date => dateTimeOffset.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    public string DateTime => dateTimeOffset.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
    public string FullDateTime { get {
        return dateTimeOffset.ToString("ddd dd MMM HH:mm:ss yyyy K", CultureInfo.InvariantCulture);
    }}

    public List<FileChangeViewModel> CommitChanges {
        get {
            Commit commit = CommitCommand.CommandCommit;
            Commit parentCommit = commit.Parents.First();
            List<FileChangeViewModel> res = [];
            foreach(var change in Repository.Diff.Compare<TreeChanges>(parentCommit.Tree, commit.Tree)) {
                res.Add(new(Repository, commit, change));
            }
            return res;
        }
    }

    protected abstract string IconFilePrefix { get; init; }
    private CommitColor _color;
    public CommitColor Color {
        get => _color;
        set {
            if (_color != value) {
                _color = value;
                this.RaisePropertyChanged(nameof(Color));
                this.RaisePropertyChanged(nameof(IconPath));
            }
        }
    }
    public string IconPath => $"/Assets/{IconFilePrefix}_{Color.ToString().ToLower()}.svg";
}
public abstract partial class ArgumentCommandViewModel : CommandViewModel {
    protected internal ArgumentCommand ArgumentCommand => (ArgumentCommand) Command;
    public string Argument {
        get => ArgumentCommand.Argument;
        set {
            if (ArgumentCommand.Argument != value) {
                ArgumentCommand.Argument = value;
                this.RaisePropertyChanged(nameof(Argument));
            }
        }
    }
}

public sealed partial class PickViewModel : CommitCommandViewModel {
    protected internal override Command Command { get; set; }
    internal PickCommand PickCommand => (PickCommand) Command;
    protected override string IconFilePrefix { get; init; } = "pick";
    public override Repository Repository { get; init; }

    public PickViewModel(PickCommand pick, CommitColor color, Repository repo) {
        Repository = repo;
        Command = pick;
        Color = color;
    }
}

public sealed partial class RewordViewModel : CommitCommandViewModel {
    protected internal override Command Command { get; set; }
    internal RewordCommand RewordCommand => (RewordCommand) Command;
    protected override string IconFilePrefix { get; init; } = "reword";
    public override Repository Repository { get; init; }

    public RewordViewModel(RewordCommand reword, CommitColor color, Repository repo) {
        Command = reword;
        Color = color;
        Repository = repo;
    }
}

public sealed partial class EditViewModel : CommitCommandViewModel {
    protected internal override Command Command { get; set; }
    internal EditCommand EditCommand => (EditCommand) Command;
    protected override string IconFilePrefix { get; init; } = "edit";
    public override Repository Repository { get; init; }

    public EditViewModel(EditCommand edit, CommitColor color, Repository repo) {
        Command = edit;
        Color = color;
        Repository = repo;
    }
}

public sealed partial class SquashViewModel : CommitCommandViewModel {
    protected internal override Command Command { get; set; }
    internal SquashCommand SquashCommand => (SquashCommand) Command;
    protected override string IconFilePrefix { get; init; } = "squash";
    public override Repository Repository { get; init; }

    public SquashViewModel(SquashCommand squash, CommitColor color, Repository repo) {
        Command = squash;
        Color = color;
        Repository = repo;
    }
}

public sealed partial class FixupViewModel : CommitCommandViewModel {
    protected internal override Command Command { get; set; }
    internal FixupCommand FixupCommand => (FixupCommand) Command;
    protected override string IconFilePrefix { get; init; } = "fixup";
    public override Repository Repository { get; init; }

    public FixupViewModel(FixupCommand fixup, CommitColor color, Repository repo) {
        Command = fixup;
        Color = color;
        Repository = repo;
    }
}

public sealed partial class DropViewModel : CommitCommandViewModel {
    protected internal override Command Command { get; set; }
    internal DropCommand DropCommand => (DropCommand) Command;
    protected override string IconFilePrefix { get; init; } = "drop";
    public override Repository Repository { get; init; }

    public DropViewModel(DropCommand drop, CommitColor color, Repository repo) {
        Command = drop;
        Color = color;
        Repository = repo;
    }
}

public sealed partial class BreakViewModel : CommandViewModel {
    protected internal override Command Command { get; set; } = new BreakCommand();
    internal BreakCommand BreakCommand => (BreakCommand) Command;
}

public sealed partial class ExecViewModel : ArgumentCommandViewModel {
    protected internal override Command Command { get; set; }
    internal ExecCommand ExecCommand => (ExecCommand) Command;
    public ExecViewModel() {
        Command = new ExecCommand("");
    }

    public ExecViewModel(ExecCommand exec) {
        Command = exec;
    }
}

public sealed partial class LabelViewModel : ArgumentCommandViewModel {
    protected internal override Command Command { get; set; }
    internal LabelCommand LabelCommand => (LabelCommand) Command;
    public LabelViewModel() {
        Command = new LabelCommand("");
    }

    public LabelViewModel(LabelCommand label) {
        Command = label;
    }
}

public sealed partial class ResetViewModel : ArgumentCommandViewModel {
    protected internal override Command Command { get; set; }
    internal ResetCommand ResetCommand => (ResetCommand) Command;
    public ResetViewModel() {
        Command = new ResetCommand("");
    }

    public ResetViewModel(ResetCommand reset) {
        Command = reset;
    }
}

public sealed partial class MergeViewModel : ArgumentCommandViewModel {
    protected internal override Command Command { get; set; }
    internal MergeCommand MergeCommand => (MergeCommand) Command;
    public MergeViewModel() {
        Command = new MergeCommand("");
    }

    public MergeViewModel(MergeCommand merge) {
        Command = merge;
    }
}

public static class CommitCommandConversions {
    public static PickViewModel ToPick(this CommitCommandViewModel ccvm) {
        return new PickViewModel(new PickCommand(ccvm.CommitCommand.CommandCommit), ccvm.Color, ccvm.Repository);
    }

    public static RewordViewModel ToReword(this CommitCommandViewModel ccvm) {
        return new RewordViewModel(new RewordCommand(ccvm.CommitCommand.CommandCommit), ccvm.Color, ccvm.Repository);
    }

    public static EditViewModel ToEdit(this CommitCommandViewModel ccvm) {
        return new EditViewModel(new EditCommand(ccvm.CommitCommand.CommandCommit), ccvm.Color, ccvm.Repository);
    }

    public static SquashViewModel ToSquash(this CommitCommandViewModel ccvm) {
        return new SquashViewModel(new SquashCommand(ccvm.CommitCommand.CommandCommit), ccvm.Color, ccvm.Repository);
    }

    public static FixupViewModel ToFixup(this CommitCommandViewModel ccvm) {
        return new FixupViewModel(new FixupCommand(ccvm.CommitCommand.CommandCommit), ccvm.Color, ccvm.Repository);
    }

    public static DropViewModel ToDrop(this CommitCommandViewModel ccvm) {
        return new DropViewModel(new DropCommand(ccvm.CommitCommand.CommandCommit), ccvm.Color, ccvm.Repository);
    }
}
