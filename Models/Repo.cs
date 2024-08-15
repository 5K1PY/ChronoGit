using LibGit2Sharp;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace ChronoGit.Models;

// TODO: IDisposable
public class Repo(string path) {
    public Repository repo { get; init;} = new(path);

    public IEnumerable<PickCommand> GetCommits(string filePath) {
        List<PickCommand> actions = new();

        using (StreamReader reader = new StreamReader(filePath)) {
            string? line;
            while ((line = reader.ReadLine()) != null) {
                if (line.StartsWith("pick")) {
                    string[] parts = line.Split(" ");
                    actions.Add(new PickCommand(repo.Lookup<Commit>(parts[1])));

                }
            }
        }

        // Clear file so we don't do anything on close
        File.WriteAllText(filePath, "");

        return actions;
    }
}
