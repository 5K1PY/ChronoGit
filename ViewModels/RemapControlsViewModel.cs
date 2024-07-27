using ReactiveUI;
using ChronoGit.Models;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using ChronoGit.Views;

namespace ChronoGit.ViewModels;


public class RemapControlsViewModel(ICollection<Tuple<NamedAction, KeyCombination>> controls) : ViewModelBase {
    private ObservableCollection<Tuple<NamedAction, KeyCombination>> _controls = new(controls);

    public ObservableCollection<Tuple<NamedAction, KeyCombination>> Controls {
        get => _controls;
        set => this.RaiseAndSetIfChanged(ref _controls, value);
    }
}
