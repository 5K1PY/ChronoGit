using LibGit2Sharp;
using System.Linq;
using System;
using System.Collections.Generic;

namespace ChronoGit.Models;

public static class Init {
    public static IEnumerable<PickCommand> GetCommits() {
        // TODO: Disposable
        var repo = new Repository("/home/skipy/dev/pisek/");
        List<PickCommand> actions = new();

        foreach (Commit c in repo.Commits.Take(15)) {
            actions.Add(new PickCommand(c));
        }

        return actions;
    }
}
