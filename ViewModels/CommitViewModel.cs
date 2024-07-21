using ReactiveUI;
using LibGit2Sharp;
using ChronoGit.Models;
using System;
using Avalonia.Media;

namespace ChronoGit.ViewModels;

public abstract partial class CommandViewModel : ViewModelBase {
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
    protected internal abstract CommitCommand commitCommand { get; set; }
    public string Id => commitCommand.CommandCommit.Id.ToString().Substring(0, 7);
    public string Message => commitCommand.CommandCommit.Message;
    public string MessageShort => commitCommand.CommandCommit.MessageShort;
    public string Author {
        get {
            Signature author = commitCommand.CommandCommit.Author;
            return string.Format("{0} <{1}>", author.Name, author.Email);
        }
    }
}

public sealed partial class PickViewModel : CommitCommandViewModel {
    protected internal override CommitCommand commitCommand { get; set; }

    public PickViewModel(PickCommand pick) {
        commitCommand = pick;
    }
}

public sealed partial class RewordViewModel : CommitCommandViewModel {
    protected internal override CommitCommand commitCommand { get; set; }
    public RewordViewModel(RewordCommand reword) {
        commitCommand = reword;
    }
}

public sealed partial class EditViewModel : CommitCommandViewModel {
    protected internal override CommitCommand commitCommand { get; set; }
    public EditViewModel(EditCommand edit) {
        commitCommand = edit;
    }
}

public sealed partial class SquashViewModel : CommitCommandViewModel {
    protected internal override CommitCommand commitCommand { get; set; }
    public SquashViewModel(SquashCommand squash) {
        commitCommand = squash;
    }
}

public sealed partial class FixupViewModel : CommitCommandViewModel {
    protected internal override CommitCommand commitCommand { get; set; }
    public FixupViewModel(FixupCommand fixup) {
        commitCommand = fixup;
    }
}

public static class CommitCommandConversions {
    public static PickViewModel ToPick(this CommitCommandViewModel ccvm) {
        return new PickViewModel(new PickCommand(ccvm.commitCommand.CommandCommit));
    }

    public static RewordViewModel ToReword(this CommitCommandViewModel ccvm) {
        return new RewordViewModel(new RewordCommand(ccvm.commitCommand.CommandCommit));
    }

    public static EditViewModel ToEdit(this CommitCommandViewModel ccvm) {
        return new EditViewModel(new EditCommand(ccvm.commitCommand.CommandCommit));
    }
}
