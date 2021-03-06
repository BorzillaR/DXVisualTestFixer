﻿using DevExpress.Xpf.Grid;
using DXVisualTestFixer.Common;
using DXVisualTestFixer.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DXVisualTestFixer.UI.Controls {
    public class TableViewAdvNavigation : TableView {
        public void MoveNextDataRow() {
            RepeatMoveAction(i => ++i);
        }
        public void MovePrevDataRow() {
            RepeatMoveAction(i => --i);
        }
        void RepeatMoveAction(Func<int, int> action) {
            int tryCount = 0;
            int focusedRowHandleCandidate = FocusedRowHandle;
            int visibleIndex = Grid.GetRowVisibleIndexByHandle(FocusedRowHandle);
            do {
                visibleIndex = action(visibleIndex);
                focusedRowHandleCandidate = Grid.GetRowHandleByVisibleIndex(visibleIndex);
            }
            while(focusedRowHandleCandidate < 0 && tryCount++ < 5);
            if(focusedRowHandleCandidate >= 0)
                FocusedRowHandle = focusedRowHandleCandidate;
        }

        public void ProcessDoubleClick(RowDoubleClickEventArgs e) {
            if(!e.HitInfo.InRow)
                return;
            if(Grid.IsGroupRowHandle(e.HitInfo.RowHandle))
                return;
            if(e.HitInfo.Column.FieldName != "Theme")
                return;
            e.Handled = true;
            InverceCommitChange(e.HitInfo.RowHandle);
        }

        TestInfoModel GetValidTestInfoModel(int rowHandle) {
            if(!Grid.IsValidRowHandle(rowHandle) || Grid.IsGroupRowHandle(rowHandle))
                return null;
            TestInfoModel testInfoModel = Grid.GetRow(rowHandle) as TestInfoModel;
            if(testInfoModel == null || testInfoModel.Valid == TestState.Error)
                return null;
            return testInfoModel;
        }
        void InverceCommitChange(int rowHandle) {
            var model = GetValidTestInfoModel(rowHandle);
            if(model != null)
                model.CommitChange = !model.CommitChange;
        }
        void SetCommitChange(int rowHandle, bool value) {
            var model = GetValidTestInfoModel(rowHandle);
            if(model != null)
                model.CommitChange = value;
        }

        public void CommitAllInViewport() {
            int i = 0;
            while(i++ < Grid.VisibleRowCount - 1)
                SetCommitChange(Grid.GetRowHandleByVisibleIndex(i), true);
        }
        public void ClearCommitsInViewport() {
            int i = 0;
            while(i++ < Grid.VisibleRowCount - 1)
                SetCommitChange(Grid.GetRowHandleByVisibleIndex(i), false);
        }


        public void ProcessKeyDown(KeyEventArgs e) {
            if(e.Key != Key.Space)
                return;
            if(SearchControl != null && SearchControl.IsKeyboardFocusWithin)
                return;
            e.Handled = true;
            InverceCommitChange(FocusedRowHandle);
        }
    }
}
