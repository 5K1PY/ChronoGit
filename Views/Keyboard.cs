using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using Avalonia.Input;
using ChronoGit.ViewModels;

namespace ChronoGit.Views;

public record struct KeyCombination(bool ShiftPressed, bool CtrlPressed, Key Key) {
    public override string ToString() {
        return (ShiftPressed ? "Shift+" : "") + (CtrlPressed ? "Ctrl+" : "") + Key.ToString();
    }
}

public enum ActionType {
    ModeChange,
    Move,
    ShiftUp,
    ShiftDown,
    Convert,
    Insert,
    Delete,
    History,
    Color,
}

public record class NamedAction(string Name, ActionType ActionType, Action Action);

public record class BoundAction(NamedAction NamedAction, KeyCombination KeyCombination);

public static class ActionDescriptions {
    public const string NORMAL_MODE = "Enter normal mode";
    public const string MOVE_UP = "Move up";
    public const string MOVE_DOWN = "Move down";
    public const string MOVE_TOP = "Move to top";
    public const string MOVE_BOTTOM = "Move to bottom";
    public const string CONVERT_DROP = "Convert selection to drop";
    public const string CONVERT_EDIT = "Convert selection to edit";
    public const string CONVERT_FIXUP = "Convert selection to fixup";
    public const string INSERT_MODE = "Enter insert mode";
    public const string SHIFT_DOWN = "Shift selection down";
    public const string SHIFT_UP = "Shift selection up";
    public const string ADD_LABEL_AFTER = "Add label after selection";
    public const string ADD_LABEL_BEFORE = "Add label before selection";
    public const string ADD_RESET_AFTER = "Add reset after selection";
    public const string ADD_RESET_BEFORE = "Add reset before selection";
    public const string CONVERT_PICK = "Convert selection to pick";
    public const string CONVERT_REWORD = "Convert selection to reword";
    public const string CONVERT_SQUASH = "Convert selection to squash";
    public const string VISUAL_MODE = "Toggle visual mode";
    public const string DELETE = "Delete selection";
    public const string UNDO = "Undo";
    public const string REDO = "Redo";
    public const string COLOR_SAME = "Color all commits same";
    public const string COLOR_BY_AUTHOR = "Color commits by author";
}

public class KeyboardControls {
    private readonly Dictionary<KeyCombination, NamedAction> actions;
    public KeyCombination NormalModeKeyCombination { get; private init; }
 
    private static readonly ImmutableArray<string> actionOrder = [
        ActionDescriptions.NORMAL_MODE,
        ActionDescriptions.VISUAL_MODE,
        ActionDescriptions.INSERT_MODE,
        ActionDescriptions.MOVE_UP,
        ActionDescriptions.MOVE_DOWN,
        ActionDescriptions.MOVE_TOP,
        ActionDescriptions.MOVE_BOTTOM,
        ActionDescriptions.SHIFT_UP,
        ActionDescriptions.SHIFT_DOWN,
        ActionDescriptions.CONVERT_PICK,
        ActionDescriptions.CONVERT_REWORD,
        ActionDescriptions.CONVERT_EDIT,
        ActionDescriptions.CONVERT_SQUASH,
        ActionDescriptions.CONVERT_FIXUP,
        ActionDescriptions.CONVERT_DROP,
        ActionDescriptions.ADD_LABEL_AFTER,
        ActionDescriptions.ADD_LABEL_BEFORE,
        ActionDescriptions.ADD_RESET_AFTER,
        ActionDescriptions.ADD_RESET_BEFORE,
        ActionDescriptions.DELETE,
        ActionDescriptions.UNDO,
        ActionDescriptions.REDO,
        ActionDescriptions.COLOR_SAME,
        ActionDescriptions.COLOR_BY_AUTHOR,
    ];

    public KeyboardControls(ICollection<BoundAction> actionsCollection) {
        actions = [];
        foreach (BoundAction boundAction in actionsCollection) {
            Debug.Assert(!actions.ContainsKey(boundAction.KeyCombination));
            actions[boundAction.KeyCombination] = boundAction.NamedAction;

            if (boundAction.NamedAction.Name == ActionDescriptions.NORMAL_MODE) {
                NormalModeKeyCombination = boundAction.KeyCombination;
            }
        }
    }

    public static KeyboardControls Default(MainWindowViewModel dataContext) {
        return new KeyboardControls(new List<BoundAction>{
            new BoundAction(new NamedAction(ActionDescriptions.NORMAL_MODE,      ActionType.ModeChange, dataContext.NormalMode),       new KeyCombination(false, false, Key.Escape)), 
            new BoundAction(new NamedAction(ActionDescriptions.VISUAL_MODE,      ActionType.ModeChange, dataContext.ToggleVisualMode), new KeyCombination(false, false, Key.V)), 
            new BoundAction(new NamedAction(ActionDescriptions.INSERT_MODE,      ActionType.ModeChange, dataContext.InsertMode),       new KeyCombination(false, false, Key.I)), 
            new BoundAction(new NamedAction(ActionDescriptions.MOVE_UP,          ActionType.Move,       dataContext.MoveUp),           new KeyCombination(false, false, Key.Up)), 
            new BoundAction(new NamedAction(ActionDescriptions.MOVE_DOWN,        ActionType.Move,       dataContext.MoveDown),         new KeyCombination(false, false, Key.Down)), 
            new BoundAction(new NamedAction(ActionDescriptions.MOVE_TOP,         ActionType.Move,       dataContext.MoveToTop),        new KeyCombination(false, false, Key.Home)), 
            new BoundAction(new NamedAction(ActionDescriptions.MOVE_BOTTOM,      ActionType.Move,       dataContext.MoveToBottom),     new KeyCombination(false, false, Key.End)), 
            new BoundAction(new NamedAction(ActionDescriptions.SHIFT_UP,         ActionType.ShiftUp,    dataContext.ShiftUp),          new KeyCombination(false, false, Key.K)), 
            new BoundAction(new NamedAction(ActionDescriptions.SHIFT_DOWN,       ActionType.ShiftDown,  dataContext.ShiftDown),        new KeyCombination(false, false, Key.J)), 
            new BoundAction(new NamedAction(ActionDescriptions.CONVERT_PICK,     ActionType.Convert,    dataContext.ConvertToPick),    new KeyCombination(false, false, Key.P)), 
            new BoundAction(new NamedAction(ActionDescriptions.CONVERT_REWORD,   ActionType.Convert,    dataContext.ConvertToReword),  new KeyCombination(false, false, Key.R)), 
            new BoundAction(new NamedAction(ActionDescriptions.CONVERT_EDIT,     ActionType.Convert,    dataContext.ConvertToEdit),    new KeyCombination(false, false, Key.E)), 
            new BoundAction(new NamedAction(ActionDescriptions.CONVERT_SQUASH,   ActionType.Convert,    dataContext.ConvertToSquash),  new KeyCombination(false, false, Key.S)), 
            new BoundAction(new NamedAction(ActionDescriptions.CONVERT_FIXUP,    ActionType.Convert,    dataContext.ConvertToFixup),   new KeyCombination(false, false, Key.F)), 
            new BoundAction(new NamedAction(ActionDescriptions.CONVERT_DROP,     ActionType.Convert,    dataContext.ConvertToDrop),    new KeyCombination(false, false, Key.D)), 
            new BoundAction(new NamedAction(ActionDescriptions.ADD_LABEL_AFTER,  ActionType.Insert,     dataContext.AddLabelAfter),    new KeyCombination(false, false, Key.L)), 
            new BoundAction(new NamedAction(ActionDescriptions.ADD_LABEL_BEFORE, ActionType.Insert,     dataContext.AddLabelBefore),   new KeyCombination(true,  false, Key.L)), 
            new BoundAction(new NamedAction(ActionDescriptions.ADD_RESET_AFTER,  ActionType.Insert,     dataContext.AddResetAfter),    new KeyCombination(false, false, Key.T)), 
            new BoundAction(new NamedAction(ActionDescriptions.ADD_RESET_BEFORE, ActionType.Insert,     dataContext.AddResetBefore),   new KeyCombination(true,  false, Key.T)), 
            new BoundAction(new NamedAction(ActionDescriptions.DELETE,           ActionType.Delete,     dataContext.Delete),           new KeyCombination(false, false, Key.Delete)), 
            new BoundAction(new NamedAction(ActionDescriptions.UNDO,             ActionType.History,    dataContext.Undo),             new KeyCombination(false, false, Key.U)), 
            new BoundAction(new NamedAction(ActionDescriptions.REDO,             ActionType.History,    dataContext.Redo),             new KeyCombination(false, true,  Key.R)), 
            new BoundAction(new NamedAction(ActionDescriptions.COLOR_SAME,       ActionType.Color,      dataContext.ColorSame),        new KeyCombination(false, true,  Key.S)), 
            new BoundAction(new NamedAction(ActionDescriptions.COLOR_BY_AUTHOR,  ActionType.Color,      dataContext.ColorByAuthor),    new KeyCombination(false, true,  Key.A)), 
        });
    }

    public List<BoundAction> Export() {
        List<BoundAction> export = [];
        foreach (var (keyComb, namedAction) in actions) {
            export.Add(new BoundAction(namedAction, keyComb));
        }
        export.Sort((boundAction1, boundAction2) => {
            return actionOrder.IndexOf(boundAction1.NamedAction.Name).CompareTo(actionOrder.IndexOf(boundAction2.NamedAction.Name));
        });
        return export;
    }

    public NamedAction? GetAction(KeyCombination keyComb) {
        actions.TryGetValue(keyComb, out NamedAction? action);
        return action;
    }
}
