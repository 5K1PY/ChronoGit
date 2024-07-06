using ReactiveUI;
using ChronoGit.Models;

namespace ChronoGit.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private Commit _commit;
    public Commit Commit
    {
        get => _commit;
        set => this.RaiseAndSetIfChanged(ref _commit, value);
    }
    public MainWindowViewModel() {
        Commit = new Commit("Initial commit message");
    }
}
