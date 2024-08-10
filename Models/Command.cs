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

public abstract class ArgumentCommand : Command {
    public abstract string Argument { get; set; }
    public override bool Equals(object? obj) {
        return (
            GetType() == obj?.GetType() &&
            Argument == (obj as ArgumentCommand)!.Argument
        );
    }
    public override int GetHashCode() {
        return Argument.GetHashCode();
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

public sealed class BreakCommand : Command {
    public override bool Equals(object? obj) {
        return GetType() == obj?.GetType();
    }

    public override int GetHashCode() {
        return 0;
    }
}

public sealed class ExecCommand(string script) : ArgumentCommand {
    public override string Argument { get; set; } = script;
}

public sealed class LabelCommand(string label) : ArgumentCommand {
    public override string Argument { get; set; } = label;
}

public sealed class ResetCommand(string label) : ArgumentCommand {
    public override string Argument { get; set; } = label;
}

public sealed class MergeCommand(string label) : ArgumentCommand {
    public override string Argument { get; set; } = label;
}

