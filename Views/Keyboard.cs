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

public record class NamedAction(string Name, Action Action);

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
    public const string CONVERT_PICK = "Convert selection to pick";
    public const string CONVERT_REWORD = "Convert selection to reword";
    public const string CONVERT_SQUASH = "Convert selection to squash";
    public const string VISUAL_MODE = "Toggle visual mode";
    public const string DELETE = "Delete selection";
    public const string UNDO = "Undo";
    public const string REDO = "Redo";
}

public class KeyboardControls {
    private Dictionary<KeyCombination, NamedAction> actions;
 
    private static ImmutableArray<string> actionOrder = [
        ActionDescriptions.NORMAL_MODE,
        ActionDescriptions.VISUAL_MODE,
        ActionDescriptions.INSERT_MODE,
        ActionDescriptions.MOVE_UP,
        ActionDescriptions.MOVE_DOWN,
        ActionDescriptions.MOVE_TOP,
        ActionDescriptions.MOVE_BOTTOM,
        ActionDescriptions.SHIFT_DOWN,
        ActionDescriptions.SHIFT_UP,
        ActionDescriptions.CONVERT_PICK,
        ActionDescriptions.CONVERT_REWORD,
        ActionDescriptions.CONVERT_EDIT,
        ActionDescriptions.CONVERT_SQUASH,
        ActionDescriptions.CONVERT_FIXUP,
        ActionDescriptions.CONVERT_DROP,
        ActionDescriptions.ADD_LABEL_AFTER,
        ActionDescriptions.ADD_LABEL_BEFORE,
        ActionDescriptions.DELETE,
        ActionDescriptions.UNDO,
        ActionDescriptions.REDO
    ];

    public KeyboardControls(ICollection<BoundAction> actionsCollection) {
        actions = [];
        foreach (BoundAction boundAction in actionsCollection) {
            Debug.Assert(!actions.ContainsKey(boundAction.KeyCombination));
            actions[boundAction.KeyCombination] = boundAction.NamedAction;
        }
    }

    public static KeyboardControls Default(MainWindowViewModel dataContext) {
        return new KeyboardControls(new List<BoundAction>{
            new BoundAction(new NamedAction(ActionDescriptions.NORMAL_MODE, dataContext.NormalMode),          new KeyCombination(false, false, Key.Escape)), 
            new BoundAction(new NamedAction(ActionDescriptions.MOVE_UP, dataContext.MoveUp),                  new KeyCombination(false, false, Key.Up)), 
            new BoundAction(new NamedAction(ActionDescriptions.MOVE_DOWN, dataContext.MoveDown),              new KeyCombination(false, false, Key.Down)), 
            new BoundAction(new NamedAction(ActionDescriptions.MOVE_TOP, dataContext.MoveToTop),              new KeyCombination(false, false, Key.Home)), 
            new BoundAction(new NamedAction(ActionDescriptions.MOVE_BOTTOM, dataContext.MoveToBottom),        new KeyCombination(false, false, Key.End)), 
            new BoundAction(new NamedAction(ActionDescriptions.CONVERT_DROP, dataContext.ConvertToDrop),      new KeyCombination(false, false, Key.D)), 
            new BoundAction(new NamedAction(ActionDescriptions.CONVERT_EDIT, dataContext.ConvertToEdit),      new KeyCombination(false, false, Key.E)), 
            new BoundAction(new NamedAction(ActionDescriptions.CONVERT_FIXUP, dataContext.ConvertToFixup),    new KeyCombination(false, false, Key.F)), 
            new BoundAction(new NamedAction(ActionDescriptions.INSERT_MODE, dataContext.InsertMode),          new KeyCombination(false, false, Key.I)), 
            new BoundAction(new NamedAction(ActionDescriptions.SHIFT_DOWN, dataContext.ShiftDown),            new KeyCombination(false, false, Key.J)), 
            new BoundAction(new NamedAction(ActionDescriptions.SHIFT_UP, dataContext.ShiftUp),                new KeyCombination(false, false, Key.K)), 
            new BoundAction(new NamedAction(ActionDescriptions.ADD_LABEL_AFTER, dataContext.AddLabelAfter),   new KeyCombination(false, false, Key.L)), 
            new BoundAction(new NamedAction(ActionDescriptions.ADD_LABEL_BEFORE, dataContext.AddLabelBefore), new KeyCombination(true,  false, Key.L)), 
            new BoundAction(new NamedAction(ActionDescriptions.CONVERT_PICK, dataContext.ConvertToPick),      new KeyCombination(false, false, Key.P)), 
            new BoundAction(new NamedAction(ActionDescriptions.CONVERT_REWORD, dataContext.ConvertToReword),  new KeyCombination(false, false, Key.R)), 
            new BoundAction(new NamedAction(ActionDescriptions.CONVERT_SQUASH, dataContext.ConvertToSquash),  new KeyCombination(false, false, Key.S)), 
            new BoundAction(new NamedAction(ActionDescriptions.VISUAL_MODE, dataContext.ToggleVisualMode),    new KeyCombination(false, false, Key.V)), 
            new BoundAction(new NamedAction(ActionDescriptions.DELETE, dataContext.Delete),                   new KeyCombination(false, false, Key.Delete)), 
            new BoundAction(new NamedAction(ActionDescriptions.UNDO, dataContext.Undo),                       new KeyCombination(false, false, Key.U)), 
            new BoundAction(new NamedAction(ActionDescriptions.REDO, dataContext.Redo),                       new KeyCombination(false, true,  Key.R)), 
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

    public Action? GetAction(KeyCombination keyComb) {
        actions.TryGetValue(keyComb, out NamedAction? action);
        return action?.Action;
    }
}
