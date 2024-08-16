using LibGit2Sharp;
using System.IO;
using System.Collections.Generic;
using System;

namespace ChronoGit.Models;

public class Repo(string todoListFilePath) : IDisposable {
    private string todoListFilePath = todoListFilePath;
    public Repository repo { get; init;} = new(Repository.Discover(todoListFilePath));

    public IEnumerable<PickCommand> GetCommits() {
        List<PickCommand> actions = [];

        using (StreamReader reader = new(todoListFilePath)) {
            string? line;
            while ((line = reader.ReadLine()) != null) {
                if (line.StartsWith("pick")) {
                    string[] parts = line.Split(" ");
                    actions.Add(new PickCommand(repo.Lookup<Commit>(parts[1]), null));
                }
            }
        }
        return actions;
    }

    public void Export(IEnumerable<Command> commands) {
        using (StreamWriter writer = new(todoListFilePath)) {
            foreach (Command c in commands) {
                writer.WriteLine(c.Export());
            }
        }
    }

    public void Abort() {
        File.WriteAllText(todoListFilePath, "");
    }

    public void Dispose() {
        repo.Dispose();
    }
}
