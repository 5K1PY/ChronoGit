using LibGit2Sharp;

namespace ChronoGit.Models;

public abstract class Command {
    
}

public abstract class CommitCommand : Command {
    public abstract Commit CommandCommit { get; set; }
}

public sealed class PickCommand : CommitCommand {
    public override Commit CommandCommit { get; set; }
    public PickCommand(Commit c) {
        CommandCommit = c;
    }
}


