using ReactiveUI;
using LibGit2Sharp;
using ChronoGit.Models;
using System;
using Avalonia.Media;

namespace ChronoGit.ViewModels;

public abstract partial class CommandViewModel : ViewModelBase {
    private bool _selected = false;
    public bool Selected {
        get => _selected;
        set => this.RaiseAndSetIfChanged(ref _selected, value);
    }
    public IBrush BackgroundColor => Selected ? Brushes.LightYellow : Brushes.White;
}

public abstract partial class CommitCommandViewModel : CommandViewModel {
    protected abstract CommitCommand commitCommand { get; set; }
    public string Id => commitCommand.CommandCommit.Id.ToString().Substring(0, 7);
    public string Message => commitCommand.CommandCommit.Message;
    public string MessageShort => commitCommand.CommandCommit.MessageShort;
    public string Author {
        get {
            Signature author = commitCommand.CommandCommit.Author;
            return string.Format("{0} <{1}>", author.Name, author.Email);
        }
    }
}

public sealed partial class PickViewModel : CommitCommandViewModel {
    protected override CommitCommand commitCommand { get; set; }

    public PickViewModel(PickCommand pick) {
        commitCommand = pick;
    }
}
