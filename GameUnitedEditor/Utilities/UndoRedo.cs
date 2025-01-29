using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameUnitedEditor.Utilities {
    public interface IUndoRedo {
        string Name { get; }
        void Undo();
        void Redo();
    }

    public class UndoRedoAction : IUndoRedo {

        public Action _undoAction;
        public Action _redoAction;
        public string Name { get; }

        public UndoRedoAction(string name) {
            Name = name;
        }

        public UndoRedoAction(Action undoAction, Action redoAction, string name) : this(name) {
            Debug.Assert(undoAction != null && redoAction != null);
            _undoAction = undoAction;
            _redoAction = redoAction;
        }

        public void Redo() => _redoAction();

        public void Undo() => _undoAction();
    }

    public class UndoRedo {
        private readonly ObservableCollection<IUndoRedo> _redos = new();
        private readonly ObservableCollection<IUndoRedo> _undos = new();
        public ReadOnlyObservableCollection<IUndoRedo> Redos { get; }
        public ReadOnlyObservableCollection<IUndoRedo> Undos { get; }

        public UndoRedo() {
            Redos = new ReadOnlyObservableCollection<IUndoRedo>(_redos);
            Undos = new ReadOnlyObservableCollection<IUndoRedo>(_undos);
        }

        public void Add(IUndoRedo cmd) {
            _undos.Add(cmd);
            _redos.Clear();
        }

        public void Undo() {
            if (_undos.Any()) {
                var cmd = _undos.Last();
                cmd.Undo();
                _undos.RemoveAt(_undos.Count - 1);
                _redos.Insert(0, cmd);
            }
        }

        public void Redo() {
            if (_redos.Any()) {
                var cmd = _redos.First();
                cmd.Redo();
                _redos.RemoveAt(0);
                _undos.Add(cmd);
            }
        }

        public void Reset() {
            _redos.Clear();
            _undos.Clear();
        }
    }
}
