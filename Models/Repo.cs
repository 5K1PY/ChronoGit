using LibGit2Sharp;
using System.Linq;
using System.Collections.Generic;

namespace ChronoGit.Models;

public static class Init {
    public static IEnumerable<Pick> GetCommits() {
        // TODO: Disposable
        var repo = new Repository("/home/skipy/dev/pisek/");
        List<Pick> actions = new();

        foreach (Commit c in repo.Commits.Take(15)) {
            actions.Add(new Pick(c));
        }

        return actions;
    }
}
