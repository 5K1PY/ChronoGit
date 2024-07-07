using LibGit2Sharp;

namespace ChronoGit.Models;

public abstract class Action {
    
}

public abstract class CommitAction : Action {
    public Commit ActionCommit { get; set; }
}

public class Pick : CommitAction {
    public Pick(Commit c) {
        ActionCommit = c;
    }
}


