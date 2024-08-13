using LibGit2Sharp;
using System.Linq;
using System.Collections.Generic;

namespace ChronoGit.Models;

// TODO: IDisposable
public class Repo(string path) {
    private Repository repository = new(path);
    public IEnumerable<PickCommand> GetCommits(int count) {
        List<PickCommand> actions = new();
        foreach (Commit c in repository.Commits.Take(count)) {
            actions.Add(new PickCommand(c));
        }
        return actions;
    }
}
