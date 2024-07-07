using ReactiveUI;
using ChronoGit.Models;
using System.Collections.ObjectModel;

namespace ChronoGit.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ObservableCollection<PickViewModel> _actions;
    public ObservableCollection<PickViewModel> Actions
    {
        get => _actions;
        set => this.RaiseAndSetIfChanged(ref _actions, value);
    }
    public MainWindowViewModel() {
        var commits = Init.GetCommits();
        _actions = new ObservableCollection<PickViewModel>();
        foreach (Pick action in commits) {
            _actions.Add(new PickViewModel(action));
        }
    }
}
