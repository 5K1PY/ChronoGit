using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibGit2Sharp;

namespace ChronoGit.ViewModels;

public sealed class FileChangeViewModel : ViewModelBase {
    public string Filename { get; init; }
    public ChangeKind ChangeKind { get; init; }
    public List<ILineDiff> Diff { get; init; } = [];
    public FileChangeViewModel(Repository repo, Commit commit, TreeEntryChanges changes) {
        Filename = changes.Path;
        ChangeKind = changes.Status;
        var patchEntries = repo.Diff.Compare<Patch>(commit.Parents.First().Tree, commit.Tree, [Filename]);
        
        var lastChange = patchEntries.Last();
        int maxLineNumber = Math.Max(
            lastChange.AddedLines.Select(x => x.LineNumber).LastOrDefault(0),
            lastChange.DeletedLines.Select(x => x.LineNumber).LastOrDefault(0)
        ) + 5;
        int maxLineNumLength = maxLineNumber.ToString().Length;
        
        var lines = patchEntries.Content.Split('\n');
        int lineAdditionsNumber = 0;
        int lineDeletionsNumber = 0;
        for (int i=0; i<lines.Length; i++) {
            string line = lines[i];
            if (line.StartsWith("+++") || line.StartsWith("---")) {

            } else if (line.StartsWith("@@")) {
                string[] parts = line.Split(" ");
                lineDeletionsNumber = int.Parse(parts[1][1..].Split(",")[0]);
                lineAdditionsNumber = int.Parse(parts[2][1..].Split(",")[0]);
            } else if (line.StartsWith("+")) {
                Diff.Add(new Insertion(maxLineNumLength, lineAdditionsNumber++, line[1..]));
            } else if (line.StartsWith("-")) {
                Diff.Add(new Deletion(maxLineNumLength, lineDeletionsNumber++, line[1..]));
            } else if (line.StartsWith(" ")) {
                Diff.Add(new NoChange(maxLineNumLength, lineDeletionsNumber++, lineAdditionsNumber++, line[1..]));
            }
        }
    }
}

public interface IDiffContent {
}

public abstract class ILineDiff : IDiffContent {
    public abstract int MaxLineNumberLen { get; init; }
    public abstract string Color { get; }
    public abstract string Content { get; }

    public string Pad() => new(' ', MaxLineNumberLen);
    public string Pad(int x) {
        return x.ToString().PadLeft(MaxLineNumberLen);
    }
}

public sealed class NoChange(int maxLen, int lineFrom, int lineTo, string content) : ILineDiff {
    public override int MaxLineNumberLen { get; init; } = maxLen;
    public override string Color => "Grey";
    public override string Content => $"{Pad(lineFrom)} {Pad(lineTo)} |   {content}";
    private readonly int lineFrom = lineFrom;
    private readonly int lineTo = lineTo;
    private readonly string content = content;
}

public sealed class Insertion(int maxLen, int lineNumber, string content) : ILineDiff {
    public override int MaxLineNumberLen { get; init; } = maxLen;
    public override string Color => "Green";
    public override string Content => $"{Pad()} {Pad(lineNumber)} | + {content}";
    private readonly int lineNumber = lineNumber;
    private readonly string content = content;
}

public sealed class Deletion(int maxLen, int lineNumber, string content) : ILineDiff {
    public override int MaxLineNumberLen { get; init; } = maxLen;
    public override string Color => "Red";
    public override string Content => $"{Pad(lineNumber)} {Pad()} | - {content}";
    private readonly int lineNumber = lineNumber;
    private string content = content;
}

