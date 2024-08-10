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
    private bool _colorSame = true;
    public bool ColorSame {
        get => _colorSame;
        set => this.RaiseAndSetIfChanged(ref _colorSame, value);
    }
    private bool _colorByAuthor = false;
    public bool ColorByAuthor {
        get => _colorByAuthor;
        set => this.RaiseAndSetIfChanged(ref _colorByAuthor, value);
    }
    private bool _colorByDate = false;
    public bool ColorByDate {
        get => _colorByDate;
        set => this.RaiseAndSetIfChanged(ref _colorByDate, value);
    }
    private bool _colorByRegex = false;
    public bool ColorByRegex {
        get => _colorByRegex;
        set {
            if (_colorByRegex != value) {
                _colorByRegex = value;
                this.RaisePropertyChanged(nameof(ColorByRegex));
                this.RaisePropertyChanged(nameof(CheckValidity));
            }
        }
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
                this.RaisePropertyChanged(nameof(RegexValid));
                this.RaisePropertyChanged(nameof(GroupValid));
                this.RaisePropertyChanged(nameof(CheckValidity));
            }
        }
    }
    public bool RegexValid {
        get {
            if (!ColorByRegex) return true;
            try {
                Regex regex = new Regex(Regex);
            } catch (ArgumentException) {
                return false;
            }
            return true;
        }
    }
    private int _group = 1;
    public int Group {
        get => _group;
        set {
            if (_group != value) {
                _group = value;
                this.RaisePropertyChanged(nameof(Group));
                this.RaisePropertyChanged(nameof(GroupValid));
                this.RaisePropertyChanged(nameof(CheckValidity));
            }
        }
    }

    public bool GroupValid {
        get {
            if (!ColorByRegex) return true;
            try {
                Regex regex = new Regex(Regex);
                return regex.GetGroupNumbers().Contains(Group);
            } catch (ArgumentException) {
                return false;
            }
        }
    }

    public bool CheckValidity => RegexValid && GroupValid;
}

