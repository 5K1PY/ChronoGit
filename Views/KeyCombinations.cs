using System;
using Avalonia.Input;

namespace ChronoGit.Views;

public record struct KeyCombination(bool ShiftPressed, bool CtrlPressed, Key Key);

