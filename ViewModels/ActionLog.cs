using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LibGit2Sharp;

namespace ChronoGit.ViewModels;

public static class ObservableCollectionExtensions {
    public static List<T> Slice<T>(this ObservableCollection<T> observableCollection, int start, int length) {
        List<T> result = [];
        for (int i=0; i<length; i++) {
            result.Add(observableCollection[start+i]);
        }
        return result;
    }
};

public abstract class ActionLog {
    public abstract int PositionBefore { get; protected init; }
    public abstract int PositionAfter { get; protected init; }
    public abstract Action<ObservableCollection<CommandViewModel>> Change { get; protected init; }
    public abstract Action<ObservableCollection<CommandViewModel>> UndoChange { get; protected init; }
    public abstract bool ChangesAnything { get; protected init; }
};

public sealed class ReplaceRangeLog : ActionLog {
    public override int PositionBefore { get; protected init; }
    public override int PositionAfter { get; protected init; }
    public override Action<ObservableCollection<CommandViewModel>> Change { get; protected init; }
    public override Action<ObservableCollection<CommandViewModel>> UndoChange { get; protected init; }
    public override bool ChangesAnything { get; protected init; }
    public ReplaceRangeLog(ObservableCollection<CommandViewModel> currentCommands, int start, IList<CommandViewModel> replaceBy, int? positionBefore = null, int? positionAfter = null) {
        List<CommandViewModel> replaced = currentCommands.Slice(start, replaceBy.Count);
        PositionBefore = positionBefore ?? start;
        PositionAfter = positionAfter ?? start;
        Change = (commands) => {
            for (int i=0; i<replaceBy.Count; i++) {
                commands[start + i] = replaceBy[i];
            }
        };
        UndoChange = (commands) => {
            for (int i=0; i<replaced.Count; i++) {
                commands[start + i] = replaced[i];
            }
        };

        ChangesAnything = false;
        for (int i=0; i<replaceBy.Count; i++) {
            ChangesAnything |= !replaced[i].Equals(replaceBy[i]);
        }
    }
}

public sealed class InsertLog : ActionLog {
    public override int PositionBefore { get; protected init; }
    public override int PositionAfter { get; protected init; }
    public override Action<ObservableCollection<CommandViewModel>> Change { get; protected init; }
    public override Action<ObservableCollection<CommandViewModel>> UndoChange { get; protected init; }
    public override bool ChangesAnything { get; protected init; } = true;
    public InsertLog(int position, CommandViewModel inserted) {
        inserted = inserted.AsNotSelected();
        PositionBefore = position - 1;
        PositionAfter = position;
        Change = (commands) => {
            commands.Insert(position, inserted);
        };
        UndoChange = (commands) => {
            commands.RemoveAt(position);
        };
    }
}

public sealed class RemoveRangeLog : ActionLog {
    public override int PositionBefore { get; protected init; }
    public override int PositionAfter { get; protected init; }
    public override Action<ObservableCollection<CommandViewModel>> Change { get; protected init; }
    public override Action<ObservableCollection<CommandViewModel>> UndoChange { get; protected init; }
    public override bool ChangesAnything { get; protected init; } = true;
    public RemoveRangeLog(ObservableCollection<CommandViewModel> currentCommands, int start, int length) {
        List<CommandViewModel> removed = currentCommands.Slice(start, length);
        PositionBefore = PositionAfter = start;
        Change = (commands) => {
            for (int i=length-1; i>=0; i--) {
                commands.RemoveAt(start + i);
            }
        };
        UndoChange = (commands) => {
            for (int i=0; i<length; i++) {
                commands.Insert(start + i, removed[i]);
            }
        };
    }
}
