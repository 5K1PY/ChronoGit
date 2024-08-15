using LibGit2Sharp;
using System.Linq;
using System.Collections.Generic;

namespace ChronoGit.Models;

// TODO: IDisposable
public class Repo(string path) {
    public Repository repo { get; init;} = new(path);

    public IEnumerable<PickCommand> GetCommits(string until_label) {
        List<PickCommand> actions = new();

        Commit? until = repo.Lookup<Commit>(until_label);
        var commits = repo.Commits.QueryBy(new CommitFilter {
            ExcludeReachableFrom = until,
            SortBy = CommitSortStrategies.Topological
        });
        foreach (Commit c in commits) {
            if (c.Parents.Count() <= 1)
                actions.Add(new PickCommand(c));
        }
        actions.Reverse();
        return actions;
    }
}
