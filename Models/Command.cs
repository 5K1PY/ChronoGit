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
    public abstract string Export();
}

public abstract class CommitCommand : Command {
    public abstract Commit CommandCommit { get; init; }
    protected abstract string ExportText { get; init; }
    public override string Export() => $"{ExportText} {CommandCommit.Sha} {CommandCommit.MessageShort}";

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
    protected abstract string ExportText { get; init; }
    public override string Export() => $"{ExportText} {Argument}";

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

public abstract class CommittingCommand : CommitCommand {
    public abstract Command? GlobalCommand { get; set; }
    public override string Export() {
        string text = base.Export();
        if (GlobalCommand is not null) text += "\n" + GlobalCommand.Export();
        return text;
    }
}

public sealed class PickCommand(Commit commit, Command? globalCommand) : CommittingCommand {
    public override Commit CommandCommit { get; init; } = commit;
    public override Command? GlobalCommand { get; set; } = globalCommand;
    protected override string ExportText { get; init; } = "pick";
}

public sealed class RewordCommand(Commit commit, Command? globalCommand) : CommittingCommand {
    public override Commit CommandCommit { get; init; } = commit;
    public override Command? GlobalCommand { get; set; } = globalCommand;
    protected override string ExportText { get; init; } = "reword";
}

public sealed class EditCommand(Commit commit, Command? globalCommand) : CommittingCommand {
    public override Commit CommandCommit { get; init; } = commit;
    public override Command? GlobalCommand { get; set; } = globalCommand;
    protected override string ExportText { get; init; } = "edit";
}

public sealed class SquashCommand(Commit commit) : CommitCommand {
    public override Commit CommandCommit { get; init; } = commit;
    protected override string ExportText { get; init; } = "squash";
}

public sealed class FixupCommand(Commit commit) : CommitCommand {
    public override Commit CommandCommit { get; init; } = commit;
    protected override string ExportText { get; init; } = "fixup";
}

public sealed class DropCommand(Commit commit) : CommitCommand {
    public override Commit CommandCommit { get; init; } = commit;
    protected override string ExportText { get; init; } = "drop";
}

public sealed class BreakCommand : Command {
    public override string Export() => "break";

    public override bool Equals(object? obj) {
        return GetType() == obj?.GetType();
    }

    public override int GetHashCode() {
        return 0;
    }
}

public sealed class ExecCommand(string script) : ArgumentCommand {
    public override string Argument { get; set; } = script;
    protected override string ExportText { get; init; } = "exec";
}

public sealed class LabelCommand(string label) : ArgumentCommand {
    public override string Argument { get; set; } = label;
    protected override string ExportText { get; init; } = "label";
}

public sealed class ResetCommand(string label) : ArgumentCommand {
    public override string Argument { get; set; } = label;
    protected override string ExportText { get; init; } = "reset";
}

public sealed class MergeCommand(string label) : ArgumentCommand {
    public override string Argument { get; set; } = label;
    protected override string ExportText { get; init; } = "merge";
}

