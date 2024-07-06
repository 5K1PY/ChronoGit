using ReactiveUI;
using ChronoGit.Models;
using System.Collections.ObjectModel;

namespace ChronoGit.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ObservableCollection<Commit> _commits;
    public ObservableCollection<Commit> Commits
    {
        get => _commits;
        set => this.RaiseAndSetIfChanged(ref _commits, value);
    }
    public MainWindowViewModel() {
        _commits = new ObservableCollection<Commit> {
            new Commit("Initial commit message"),
            new Commit("Commit 2"),
            new Commit("Commit 3"),
        };
    }
}
