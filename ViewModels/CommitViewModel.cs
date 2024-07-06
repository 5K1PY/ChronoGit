using ReactiveUI;
using ChronoGit.Models;

namespace ChronoGit.ViewModels;

public partial class CommitViewModel : ViewModelBase {
    public CommitViewModel() {

    }

    public CommitViewModel(Commit commit) {
        _commitMessage = commit.CommitMessage;
    }

    private string _commitMessage = string.Empty;
    public string CommitMessage
    {
        get => _commitMessage;
        set => this.RaiseAndSetIfChanged(ref _commitMessage, value);
    }
}
