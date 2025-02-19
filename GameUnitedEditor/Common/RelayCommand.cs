﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GameUnitedEditor {
    internal class RelayCommand<T> : ICommand {

        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null) {
            _execute = execute ?? throw new ArgumentException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) {
            return _canExecute?.Invoke((T) parameter) ?? true;
        }

        public void Execute(object parameter) {
            _execute((T) parameter);
        }
    }
}
