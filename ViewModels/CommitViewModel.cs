using ReactiveUI;
using LibGit2Sharp;
using ChronoGit.Models;
using System;

namespace ChronoGit.ViewModels;

public abstract partial class ActionViewModel : ViewModelBase {

}

public partial class PickViewModel : ActionViewModel {
    private Pick pickAction;
    public string Message => pickAction.ActionCommit.Message;
    public string MessageShort => pickAction.ActionCommit.MessageShort;

    public PickViewModel(Pick pick) {
        pickAction = pick;
    }

}
