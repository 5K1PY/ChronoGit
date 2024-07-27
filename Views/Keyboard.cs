using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using Avalonia.Controls;
using Avalonia.Input;
using ChronoGit.ViewModels;

namespace ChronoGit.Views;

public record struct KeyCombination(bool ShiftPressed, bool CtrlPressed, Key Key) {
    public override string ToString() {
        return (ShiftPressed ? "Shift+" : "") + (CtrlPressed ? "Ctrl+" : "") + Key.ToString();
    }
}

public record class NamedAction(string Name, Action Action);

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
}

public class KeyboardControls(Dictionary<KeyCombination, NamedAction> actions) {
    private Dictionary<KeyCombination, NamedAction> actions = actions;
 
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
        ActionDescriptions.DELETE
    ];

    public static KeyboardControls Default(MainWindowViewModel dataContext) {
        return new KeyboardControls(new Dictionary<KeyCombination, NamedAction>{
            {new KeyCombination(false, false, Key.Escape), new NamedAction(ActionDescriptions.NORMAL_MODE, dataContext.NormalMode)},
            {new KeyCombination(false, false, Key.Up),     new NamedAction(ActionDescriptions.MOVE_UP, dataContext.MoveUp)},
            {new KeyCombination(false, false, Key.Down),   new NamedAction(ActionDescriptions.MOVE_DOWN, dataContext.MoveDown)},
            {new KeyCombination(false, false, Key.Home),   new NamedAction(ActionDescriptions.MOVE_TOP, dataContext.MoveToTop)},
            {new KeyCombination(false, false, Key.End),    new NamedAction(ActionDescriptions.MOVE_BOTTOM, dataContext.MoveToBottom)},
            {new KeyCombination(false, false, Key.D),      new NamedAction(ActionDescriptions.CONVERT_DROP, dataContext.ConvertToDrop)},
            {new KeyCombination(false, false, Key.E),      new NamedAction(ActionDescriptions.CONVERT_EDIT, dataContext.ConvertToEdit)},
            {new KeyCombination(false, false, Key.F),      new NamedAction(ActionDescriptions.CONVERT_FIXUP, dataContext.ConvertToFixup)},
            {new KeyCombination(false, false, Key.I),      new NamedAction(ActionDescriptions.INSERT_MODE, dataContext.InsertMode)},
            {new KeyCombination(false, false, Key.J),      new NamedAction(ActionDescriptions.SHIFT_DOWN, dataContext.ShiftDown)},
            {new KeyCombination(false, false, Key.K),      new NamedAction(ActionDescriptions.SHIFT_UP, dataContext.ShiftUp)},
            {new KeyCombination(false, false, Key.L),      new NamedAction(ActionDescriptions.ADD_LABEL_AFTER, dataContext.AddLabelAfter)},
            {new KeyCombination(true,  false, Key.L),      new NamedAction(ActionDescriptions.ADD_LABEL_BEFORE, dataContext.AddLabelBefore)},
            {new KeyCombination(false, false, Key.P),      new NamedAction(ActionDescriptions.CONVERT_PICK, dataContext.ConvertToPick)},
            {new KeyCombination(false, false, Key.R),      new NamedAction(ActionDescriptions.CONVERT_REWORD, dataContext.ConvertToReword)},
            {new KeyCombination(false, false, Key.S),      new NamedAction(ActionDescriptions.CONVERT_SQUASH, dataContext.ConvertToSquash)},
            {new KeyCombination(false, false, Key.V),      new NamedAction(ActionDescriptions.VISUAL_MODE, dataContext.ToggleVisualMode)},
            {new KeyCombination(false, false, Key.Delete), new NamedAction(ActionDescriptions.DELETE, dataContext.Delete)},
        });
    }

    public List<Tuple<NamedAction, KeyCombination>> Export() {
        List<Tuple<NamedAction, KeyCombination>> export = [];
        foreach (var (keyComb, namedAction) in actions) {
            export.Add(new Tuple<NamedAction, KeyCombination>(namedAction, keyComb));
        }
        export.Sort((tuple1, tuple2) => {
            return actionOrder.IndexOf(tuple1.Item1.Name).CompareTo(actionOrder.IndexOf(tuple2.Item1.Name));
        });
        return export;
    }

    public Action? GetAction(KeyCombination keyComb) {
        actions.TryGetValue(keyComb, out NamedAction? action);
        return action?.Action;
    }
}
