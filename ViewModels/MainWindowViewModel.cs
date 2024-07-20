using ReactiveUI;
using ChronoGit.Models;
using System.Collections.ObjectModel;

namespace ChronoGit.ViewModels;

public class MainWindowViewModel : ViewModelBase {
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
        _actions[0].Selected = true;
    }

    public int Selected = 0;

    public void UpArrowPressed() {
        if (Selected - 1 >= 0) {
            Actions[Selected].Selected = false;
            Selected--;
            Actions[Selected].Selected = true;
        }
    }

    public void DownArrowPressed() {
        if (Selected + 1 < Actions.Count) {
            Actions[Selected].Selected = false;
            Selected++;
            Actions[Selected].Selected = true;
        }
    }
}
