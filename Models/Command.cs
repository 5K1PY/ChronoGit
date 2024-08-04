using LibGit2Sharp;

namespace ChronoGit.Models;

public abstract class Command {
    public static bool operator==(Command? c1, Command? c2) {
        if (c1 is null) return c2 is null;
        return c1.Equals(c2);
    }

    public static bool operator!=(Command? c1, Command? c2) {
        if (c1 is null) return !(c2 is null);
        return !c1.Equals(c2);
    }

    public abstract override bool Equals(object? obj);
    public abstract override int GetHashCode();
}

public abstract class CommitCommand : Command {
    public abstract Commit CommandCommit { get; set; }
    public override bool Equals(object? obj) {
        return (
            GetType() == obj?.GetType() &&
            CommandCommit == (obj as CommitCommand)!.CommandCommit
        );
    }

    public override int GetHashCode() {
        return CommandCommit.GetHashCode();
    }
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
    public string Label = label;
    public override bool Equals(object? obj) {
        return (
            GetType() == obj?.GetType() &&
            Label == (obj as LabelCommand)!.Label
        );
    }

    public override int GetHashCode() {
        return Label.GetHashCode();
    }
}

public sealed class ResetCommand(string label) : Command {
    public string Label = label;

    public override bool Equals(object? obj) {
        return (
            GetType() == obj?.GetType() &&
            Label == (obj as ResetCommand)!.Label
        );
    }

    public override int GetHashCode() {
        return Label.GetHashCode();
    }
}

public sealed class MergeCommand(string label) {
    public string Label { get; set; } = label;
}

