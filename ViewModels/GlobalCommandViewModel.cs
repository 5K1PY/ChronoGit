using ChronoGit.Models;
using ReactiveUI;

namespace ChronoGit.ViewModels;

public class GlobalCommandViewModel : ViewModelBase {
    public GlobalCommandViewModel(Command? current) {
        if (current is BreakCommand) {
            SetBreak = true;
        } else if (current is ExecCommand exec) {
            SetExec = true;
            ExecCommand = exec.Argument;
        } else {
            SetNull = true;
        }
    }

    private bool _setNull = false;
    public bool SetNull {
        get => _setNull;
        set => this.RaiseAndSetIfChanged(ref _setNull, value);
    }
    private bool _setBreak = false;
    public bool SetBreak {
        get => _setBreak;
        set => this.RaiseAndSetIfChanged(ref _setBreak, value);
    }
    private bool _setExec = false;
    public bool SetExec {
        get => _setExec;
        set => this.RaiseAndSetIfChanged(ref _setExec, value);
    }

    private string _execCommand = "";
    public string ExecCommand {
        get => _execCommand;
        set => this.RaiseAndSetIfChanged(ref _execCommand, value);
    }
}
