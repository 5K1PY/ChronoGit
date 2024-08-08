using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Text.RegularExpressions;

namespace ChronoGit.ViewModels;


public class ChangeColorsViewModel : ViewModelBase {
    public ObservableCollection<CommitColor> CommitColors { get; init; } = new(Enum.GetValues(typeof(CommitColor)).Cast<CommitColor>());

    public ChangeColorsViewModel()
    {
        CommitColors = new ObservableCollection<CommitColor>(Enum.GetValues(typeof(CommitColor)).Cast<CommitColor>());
        ChosenColor = CommitColors.First();  // Default selection
    }

    private CommitColor _chosenColor;
    public CommitColor ChosenColor {
        get => _chosenColor;
        set => this.RaiseAndSetIfChanged(ref _chosenColor, value);
    }

    private string _regex = "";
    public string Regex {
        get => _regex;
        set {
            if (_regex != value) {
                _regex = value;
                this.RaisePropertyChanged(nameof(Regex));
                this.RaisePropertyChanged(nameof(CheckValidity));
            }
        }
    }
    private int _group = 1;
    public int Group {
        get => _group;
        set {
            if (_group != value) {
                _group = value;
                this.RaisePropertyChanged(nameof(Group));
                this.RaisePropertyChanged(nameof(CheckValidity));
            }
        }
    }

    public bool CheckValidity {
        get {
            // TODO: Only if color by regex
            try {
                Regex regex = new Regex(Regex);
                return regex.GetGroupNumbers().Contains(Group);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Invalid regex pattern: {ex.Message}");
                return false;
            }
        }
    }
}

