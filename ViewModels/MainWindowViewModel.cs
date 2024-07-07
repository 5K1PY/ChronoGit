using ReactiveUI;
using ChronoGit.Models;
using System.Collections.ObjectModel;
using ChronoGit.Views;

namespace ChronoGit.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ObservableCollection<CommandViewModel> _actions;
    public ObservableCollection<CommandViewModel> Actions
    {
        get => _actions;
        set => this.RaiseAndSetIfChanged(ref _actions, value);
    }
    public MainWindowViewModel() {
        var commits = Init.GetCommits();
        _actions = new ObservableCollection<CommandViewModel>();
        foreach (PickCommand action in commits) {
            _actions.Add(new PickViewModel(action));
        }
    }
}
