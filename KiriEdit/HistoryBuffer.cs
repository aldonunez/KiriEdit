/*
   Copyright 2020 Aldo J. Nunez

   Licensed under the Apache License, Version 2.0.
   See the LICENSE.txt file for details.
*/

using System;
using System.Collections.Generic;

namespace KiriEdit
{
    public class HistoryBuffer
    {
        private List<HistoryCommand> _commands = new List<HistoryCommand>();
        private int _topUndoIndex = -1;

        public event EventHandler HistoryChanged;

        public int UndoCount => _topUndoIndex + 1;

        public int RedoCount => (_commands.Count - 1) - _topUndoIndex;

        public void Undo()
        {
            if (_topUndoIndex < 0)
                return;

            var cmd = _commands[_topUndoIndex];

            cmd.Unapply();

            _topUndoIndex--;

            OnHistoryChanged();
        }

        public void Redo()
        {
            if (_topUndoIndex >= _commands.Count - 1)
                return;

            _topUndoIndex++;

            var cmd = _commands[_topUndoIndex];

            cmd.Apply();

            OnHistoryChanged();
        }

        public void Add(HistoryCommand command)
        {
            int index = _topUndoIndex + 1;
            int count = _commands.Count - index;

            _commands.RemoveRange(index, count);

            _commands.Add(command);
            _topUndoIndex++;

            OnHistoryChanged();
        }

        private void OnHistoryChanged()
        {
            HistoryChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public abstract class HistoryCommand
    {
        public abstract void Apply();
        public abstract void Unapply();
    }
}
