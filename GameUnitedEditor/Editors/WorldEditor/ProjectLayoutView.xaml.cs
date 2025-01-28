﻿using GameUnitedEditor.GameProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GameUnitedEditor.Editors {
    public partial class ProjectLayoutView : UserControl {
        public ProjectLayoutView() {
            InitializeComponent();
        }

        private void OnAddScene_Button_Click(object sender, RoutedEventArgs e) {
            var vm = DataContext as Project;
            vm.AddScene("New Scene " + vm.Scenes.Count);
        }
    }
}
