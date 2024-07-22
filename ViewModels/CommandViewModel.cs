using ReactiveUI;
using LibGit2Sharp;
using ChronoGit.Models;
using System;
using Avalonia.Media;
using System.Reflection.Emit;

namespace ChronoGit.ViewModels;

public abstract partial class CommandViewModel : ViewModelBase {
    protected internal abstract Command Command { get; set; }
    private bool _selected = false;
    public bool Selected {
        get => _selected;
        set => this.RaiseAndSetIfChanged(ref _selected, value);
    }
    public CommandViewModel AsSelected() {
        Selected = true;
        return this;
    }
}

public abstract partial class CommitCommandViewModel : CommandViewModel {
    protected internal CommitCommand CommitCommand => (CommitCommand) Command;
    public string Id => CommitCommand.CommandCommit.Id.ToString().Substring(0, 7);
    public string Message => CommitCommand.CommandCommit.Message;
    public string MessageShort => CommitCommand.CommandCommit.MessageShort;
    public string Author {
        get {
            Signature author = CommitCommand.CommandCommit.Author;
            return string.Format("{0} <{1}>", author.Name, author.Email);
        }
    }
}

public sealed partial class PickViewModel(PickCommand pick) : CommitCommandViewModel {
    protected internal override Command Command { get; set; } = pick;
}

public sealed partial class RewordViewModel(RewordCommand reword) : CommitCommandViewModel {
    protected internal override Command Command { get; set; } = reword;
}

public sealed partial class EditViewModel(EditCommand edit) : CommitCommandViewModel {
    protected internal override Command Command { get; set; } = edit;
}

public sealed partial class SquashViewModel(SquashCommand squash) : CommitCommandViewModel {
    protected internal override Command Command { get; set; } = squash;
}

public sealed partial class FixupViewModel(FixupCommand fixup) : CommitCommandViewModel {
    protected internal override Command Command { get; set; } = fixup;
}

public sealed partial class DropViewModel(DropCommand drop) : CommitCommandViewModel {
    protected internal override Command Command { get; set; } = drop;
}

public sealed partial class LabelViewModel(LabelCommand label) : CommandViewModel {
    protected internal override Command Command { get; set; } = label;
    internal LabelCommand LabelCommand => (LabelCommand) Command;
    public string Label {
        get => LabelCommand.Label;
        set => this.RaiseAndSetIfChanged(ref LabelCommand.Label, value);
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
