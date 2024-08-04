using ReactiveUI;
using LibGit2Sharp;
using ChronoGit.Models;
using System;

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

    public string Id => CommitCommand.CommandCommit.Id.ToString().Substring(0, 7);
    public string Message => CommitCommand.CommandCommit.Message;
    public string MessageShort => CommitCommand.CommandCommit.MessageShort;
    public string Author => CommitCommand.CommandCommit.Author.ToString();
    public string AuthorEmail => CommitCommand.CommandCommit.Author.Email.ToString();
    public string FullAuthor => string.Format("{0} <{1}>", Author, AuthorEmail);

    protected abstract string IconFilePrefix { get; init; }
    private CommitColor _color = CommitColor.Red;
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
                this.RaisePropertyChanged(Argument);
            }
        }
    }
}

public sealed partial class PickViewModel(PickCommand pick) : CommitCommandViewModel {
    protected internal override Command Command { get; set; } = pick;
    internal PickCommand PickCommand => (PickCommand) Command;
    protected override string IconFilePrefix { get; init; } = "pick";
}

public sealed partial class RewordViewModel(RewordCommand reword) : CommitCommandViewModel {
    protected internal override Command Command { get; set; } = reword;
    internal RewordCommand RewordCommand => (RewordCommand) Command;
    protected override string IconFilePrefix { get; init; } = "reword";
}

public sealed partial class EditViewModel(EditCommand edit) : CommitCommandViewModel {
    protected internal override Command Command { get; set; } = edit;
    internal EditCommand EditCommand => (EditCommand) Command;
    protected override string IconFilePrefix { get; init; } = "edit";
}

public sealed partial class SquashViewModel(SquashCommand squash) : CommitCommandViewModel {
    protected internal override Command Command { get; set; } = squash;
    internal SquashCommand SquashCommand => (SquashCommand) Command;
    protected override string IconFilePrefix { get; init; } = "squash";
}

public sealed partial class FixupViewModel(FixupCommand fixup) : CommitCommandViewModel {
    protected internal override Command Command { get; set; } = fixup;
    internal FixupCommand FixupCommand => (FixupCommand) Command;
    protected override string IconFilePrefix { get; init; } = "fixup";
}

public sealed partial class DropViewModel(DropCommand drop) : CommitCommandViewModel {
    protected internal override Command Command { get; set; } = drop;
    internal DropCommand DropCommand => (DropCommand) Command;
    protected override string IconFilePrefix { get; init; } = "drop";
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
        return new PickViewModel(new PickCommand(ccvm.CommitCommand.CommandCommit));
    }

    public static RewordViewModel ToReword(this CommitCommandViewModel ccvm) {
        return new RewordViewModel(new RewordCommand(ccvm.CommitCommand.CommandCommit));
    }

    public static EditViewModel ToEdit(this CommitCommandViewModel ccvm) {
        return new EditViewModel(new EditCommand(ccvm.CommitCommand.CommandCommit));
    }

    public static SquashViewModel ToSquash(this CommitCommandViewModel ccvm) {
        return new SquashViewModel(new SquashCommand(ccvm.CommitCommand.CommandCommit));
    }

    public static FixupViewModel ToFixup(this CommitCommandViewModel ccvm) {
        return new FixupViewModel(new FixupCommand(ccvm.CommitCommand.CommandCommit));
    }

    public static DropViewModel ToDrop(this CommitCommandViewModel ccvm) {
        return new DropViewModel(new DropCommand(ccvm.CommitCommand.CommandCommit));
    }
}
