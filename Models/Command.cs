using LibGit2Sharp;

namespace ChronoGit.Models;

public abstract class Command {
    
}

public abstract class CommitCommand : Command {
    public abstract Commit CommandCommit { get; set; }
}

public sealed class PickCommand(Commit commit) : CommitCommand {
    public override Commit CommandCommit { get; set; } = commit;
}

public sealed class RewordCommand(Commit commit) : CommitCommand {
    public override Commit CommandCommit { get; set; } = commit;
}

public sealed class EditCommand(Commit commit) : CommitCommand {
    public override Commit CommandCommit { get; set; } = commit;
}

public sealed class SquashCommand(Commit commit) : CommitCommand {
    public override Commit CommandCommit { get; set; } = commit;
}

public sealed class FixupCommand(Commit commit) : CommitCommand {
    public override Commit CommandCommit { get; set; } = commit;
}

public sealed class DropCommand(Commit commit) : CommitCommand {
    public override Commit CommandCommit { get; set; } = commit;
}

public sealed class LabelCommand(string label) : Command {
    public string Label { get; set; } = label;
}

public sealed class ResetCommand(string label) {
    public string Label { get; set; } = label;
}

public sealed class MergeCommand(string label) {
    public string Label { get; set; } = label;
}

