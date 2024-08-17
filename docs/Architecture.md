# Architecture
ChronoGit is built with Avalonia & ReactiveUI.

ChronoGit uses Model-View-ViewModel design pattern. You can read more
about it [here](https://github.com/AvaloniaUI/Avalonia.Samples/tree/main/src/Avalonia.Samples/MVVM/BasicMvvmSample).

## Overview
### Views
Contains views of objects displayed on screen with .axaml appearance. Code handles clicks and key-presses.

  - `WindowBase.cs` - Base class for windows with key-press  detection.
  - `Keyboard.cs` - Machinery for key-presses calling correct action that allows key remapping.
  - `MainWindow` - Top-level layout, calling actions in ViewModels & opening other windows.
  - `PickView`, `RewordView`, `EditView`, `SquashView`, `FixupView`, `DropView`, `BreakView`,
  `ExecView`, `LabelView`, `ResetView`, `MergeView` - Views for each command.
  - `CommitDetailsView` - Component for showing [commit details](/README.md#commit-details).
  - `FileChangeView` - Component for showing file diff in commit details.
  - `RemapControlsWindow` - Window for where controls can be remapped.
  - `ChangeColorsWindow` - Window for [recoloring commits](/README.md#coloring-commits).
  - `GlobalCommandWindow` - Window for setting [global command](/README.md#global-commands).

### ViewModels
Contains the code for user actions.

- `ViewModelBase` - Base class for ViewModels
- `MainWindowViewModel` - View model for `MainWindow`. Executes actions on commands,
   save current vim-mode, and logs history.
- `CommandViewModel` - ViewModel for commands. Relays changes to Models and serves repository data.
- `ActionLog` - Container classes for logging history.
- `FileChangeViewModel` - Classes to store file diff and be displayed in `FileChangeView`.
- `GlobalCommandViewModel` - View model for `ChangeColorsWindow`
- `ChangeColorsViewModel` - View model for `ChangeColorsWindow`
- `RemapControlsViewModel` - View model for `RemapControlsWindow`.
- `Converters` - Converters used to convert values in bindings.

### Models
Represent data to be then written into todo-list.

- `Commands` - Stores data for each command.
- `GitTodoList` - Wrapper class around rebasing todo list.

## How key-presses lead to correct action call?

Key-presses are detected in `WindowKeyDown` method of current window.
`WindowBase.WindowKeyDown` handles storing current state of modifier keys (`Shift` and `Control`).

Current key bindings settings are stored in `KeyboardControls` object (`Keyboard.cs`).
`MainWindow` detects current key combination and translates it to `MainWindowViewModel`
method using aforementioned `KeyboardControls`.

When we want to call an action, we first get `ViewData` that we need to pass for that action.
(Page height for `PageUp` / `PageDown`.) Then the action is finally called.

Then we need to adjust view accordingly. First we adjust scroll position to keep currently selected
command on screen. (This is done differently depending on what the last action was.)

Finally we adjust focus. If we are in InsertMode, then we must be on argument command (`exec`, `label`, `merge`
or `reset`.) Therefore we want to focus TextBox for it's argument. Otherwise we clear focus.

However, if user is currently in InsertMode and not trying to exit (or holds `Ctrl`),
we do not call any action and return right away.

### Remapping keys

Remapping keys changes the `KeyboardControls` object and saves current configuration into
a configuration file next to the executable to keep changes between runs of the application.
This configuration file is loaded on startup.

## How Undo & Redo works

Undo & Redo is implemented using [Generate State](https://stackoverflow.com/questions/3541383/undo-redo-implementation#answer-3542670).
That is, we store current state and all actions that changed commands (and their inversions).
If we want to go back in history, we simply run the inversion. For going forward, we execute
the action again.

## Used libraries

- [Avalonia](https://github.com/AvaloniaUI/Avalonia) - for everything
- [Svg.Skia](https://github.com/wieslawsoltes/Svg.Skia) - displaying `svg`s of command icons
- [LibGit2Sharp](https://github.com/libgit2/libgit2sharp/) - interacting with `git`
- [System.Configuration.ConfigurationManager](https://github.com/dotnet/runtime/tree/main/src/libraries/System.Configuration.ConfigurationManager) - for saving current keybindings
