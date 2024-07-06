namespace ChronoGit.Models;

public class Commit
{
    public string CommitMessage { get; set; }

    public  Commit(string commitMessage) {
        CommitMessage = commitMessage;
    }
}
