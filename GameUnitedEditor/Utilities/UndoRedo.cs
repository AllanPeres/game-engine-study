using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameUnitedEditor.Utilities {
    interface IUndoRedo {
        string Name { get; set; }
        void Undo();
        void Redo();
    }

    class UndoRedo {
        private readonly ObservableCollection<IUndoRedo> _redos = new();
        private readonly ObservableCollection<IUndoRedo> _undos = new();
        public ReadOnlyObservableCollection<IUndoRedo> Redos { get; }
        public ReadOnlyObservableCollection<IUndoRedo> Undos { get; }

        public UndoRedo() {
            Redos = new ReadOnlyObservableCollection<IUndoRedo>(_redos);
            Undos = new ReadOnlyObservableCollection<IUndoRedo>(_undos);
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
                _undos.Insert(_undos.Count - 1, cmd);
            }
        }

        public void Reset() {
            _redos.Clear();
            _undos.Clear();
        }
    }
}
