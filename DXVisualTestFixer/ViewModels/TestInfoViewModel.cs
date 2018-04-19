﻿using DXVisualTestFixer.Core;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXVisualTestFixer.ViewModels {
    public interface ITestInfoViewModel { }

    public class TestInfoViewModel : BindableBase, ITestInfoViewModel {
        readonly IMainViewModel mainViewModel;

        MergerdTestViewType _MergerdTestViewType;

        public MergerdTestViewType MergerdTestViewType {
            get { return _MergerdTestViewType; }
            set { SetProperty(ref _MergerdTestViewType, value, OnMergerdTestViewTypeChanged); }
        }
        public Action MoveNextRow { get { return mainViewModel.RaiseMoveNext; } }
        public Action MovePrevRow { get { return mainViewModel.RaiseMovePrev; } }
        public TestInfoWrapper TestInfo { get { return mainViewModel.CurrentTest; } }

        public TestInfoViewModel(IMainViewModel mainViewModel) {
            this.mainViewModel = mainViewModel;
            MergerdTestViewType = mainViewModel.MergerdTestViewType;
        }

        void OnMergerdTestViewTypeChanged() {
            mainViewModel.MergerdTestViewType = MergerdTestViewType;
        }

        public void Valid() {
            TestInfo.CommitChange = true;
            MoveNextRow();
        }
        public void Invalid() {
            TestInfo.CommitChange = false;
            MoveNextRow();
        }
    }
}
