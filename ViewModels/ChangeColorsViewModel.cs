using ReactiveUI;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using ChronoGit.Views;
using System;

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
        set => this.RaiseAndSetIfChanged(ref _regex, value);
    }
    private int _group = 1;
    public int Group {
        get => _group;
        set => this.RaiseAndSetIfChanged(ref _group, value);
    }
}

