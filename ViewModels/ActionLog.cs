using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChronoGit.Models;

namespace ChronoGit.ViewModels;

public static class ObservableCollectionExtensions {
    public static List<T> Slice<T>(this ObservableCollection<T> observableCollection, int start, int length) {
        List<T> result = [];
        for (int i=0; i<length; i++) {
            result.Add(observableCollection[start+i]);
        }
        return result;
    }
}

public abstract class ActionLog {
    public abstract int? PositionBefore { get; protected init; }
    public abstract int? PositionAfter { get; protected init; }
    public abstract Action<MainWindowViewModel> Change { get; protected init; }
    public abstract Action<MainWindowViewModel> UndoChange { get; protected init; }
    public abstract bool ChangesAnything { get; protected init; }
}

public sealed class ReplaceRangeLog : ActionLog {
    public override int? PositionBefore { get; protected init; }
    public override int? PositionAfter { get; protected init; }
    public override Action<MainWindowViewModel> Change { get; protected init; }
    public override Action<MainWindowViewModel> UndoChange { get; protected init; }
    public override bool ChangesAnything { get; protected init; }
    public ReplaceRangeLog(ObservableCollection<CommandViewModel> currentCommands, int start, IList<CommandViewModel> replaceBy, int? positionBefore = null, int? positionAfter = null) {
        List<CommandViewModel> replaced = currentCommands.Slice(start, replaceBy.Count);
        PositionBefore = positionBefore ?? start;
        PositionAfter = positionAfter ?? start;
        Change = (mwvm) => {
            for (int i=0; i<replaceBy.Count; i++) {
                mwvm.Commands[start + i] = replaceBy[i];
            }
        };
        UndoChange = (mwvm) => {
            for (int i=0; i<replaced.Count; i++) {
                mwvm.Commands[start + i] = replaced[i];
            }
        };

        ChangesAnything = false;
        for (int i=0; i<replaceBy.Count; i++) {
            ChangesAnything |= !replaced[i].Equals(replaceBy[i]);
        }
    }
}

public sealed class InsertLog : ActionLog {
    public override int? PositionBefore { get; protected init; }
    public override int? PositionAfter { get; protected init; }
    public override Action<MainWindowViewModel> Change { get; protected init; }
    public override Action<MainWindowViewModel> UndoChange { get; protected init; }
    public override bool ChangesAnything { get; protected init; } = true;
    public InsertLog(int position, CommandViewModel inserted) {
        inserted = inserted.AsNotSelected();
        PositionBefore = position - 1;
        PositionAfter = position;
        Change = (mwvm) => {
            mwvm.Commands.Insert(position, inserted);
        };
        UndoChange = (mwvm) => {
            mwvm.Commands.RemoveAt(position);
        };
    }
}

public sealed class RemoveRangeLog : ActionLog {
    public override int? PositionBefore { get; protected init; }
    public override int? PositionAfter { get; protected init; }
    public override Action<MainWindowViewModel> Change { get; protected init; }
    public override Action<MainWindowViewModel> UndoChange { get; protected init; }
    public override bool ChangesAnything { get; protected init; } = true;
    public RemoveRangeLog(ObservableCollection<CommandViewModel> currentCommands, int start, int length) {
        List<CommandViewModel> removed = currentCommands.Slice(start, length);
        PositionBefore = PositionAfter = start;
        Change = (mwvm) => {
            for (int i=length-1; i>=0; i--) {
                mwvm.Commands.RemoveAt(start + i);
            }
        };
        UndoChange = (mwvm) => {
            for (int i=0; i<length; i++) {
                mwvm.Commands.Insert(start + i, removed[i]);
            }
        };
    }
}

public sealed class ChangeArgumentLog : ActionLog {
    public override int? PositionBefore { get; protected init; }
    public override int? PositionAfter { get; protected init; }
    public override Action<MainWindowViewModel> Change { get; protected init; }
    public override Action<MainWindowViewModel> UndoChange { get; protected init; }
    public override bool ChangesAnything { get; protected init; }

    public ChangeArgumentLog(int index, string textBefore, string textAfter) {
        PositionBefore = PositionAfter = index;
        ChangesAnything = textBefore != textAfter;
        Change = (mwvm) => {
            (mwvm.Commands[index] as ArgumentCommandViewModel)!.Argument = textAfter;
        };
        UndoChange = (mwvm) => {
            (mwvm.Commands[index] as ArgumentCommandViewModel)!.Argument = textBefore;
        };
    }
}

public sealed class ChangeGlobalCommandLog : ActionLog {
    public override int? PositionBefore { get; protected init; } = null;
    public override int? PositionAfter { get; protected init; } = null;
    public override Action<MainWindowViewModel> Change { get; protected init; }
    public override Action<MainWindowViewModel> UndoChange { get; protected init; }
    public override bool ChangesAnything { get; protected init; }

    public ChangeGlobalCommandLog(Command? globalCommandFrom, Command? globalCommandTo) {
        ChangesAnything = globalCommandFrom != globalCommandTo;
        Change = (mwvm) => {
            mwvm.GlobalCommand = globalCommandTo;
        };
        UndoChange = (mwvm) => {
            mwvm.GlobalCommand = globalCommandFrom;
        };
    }

}
