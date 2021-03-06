﻿using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Grid;
using DXVisualTestFixer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXVisualTestFixer.UI.Behaviors {
    public class GridControlClearFilterBehavior : Behavior<GridControl> {
        protected override void OnAttached() {
            base.OnAttached();
            AssociatedObject.ItemsSourceChanged += AssociatedObject_ItemsSourceChanged;
        }
        protected override void OnDetaching() {
            base.OnDetaching();
            AssociatedObject.ItemsSourceChanged -= AssociatedObject_ItemsSourceChanged;
        }

        void AssociatedObject_ItemsSourceChanged(object sender, ItemsSourceChangedEventArgs e) {
            AssociatedObject.ClearColumnFilter(nameof(ITestInfoModel.ProblemName));
        }
    }
}
