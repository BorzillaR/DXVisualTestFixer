﻿using DevExpress.Xpf.Core;
using DXVisualTestFixer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DXVisualTestFixer.UI.Services {
    public class AppearanceService : IAppearanceService {
        public void SetTheme(string themeName, string palette) {
            var palettes = typeof(PredefinedThemePalettes).GetFields(BindingFlags.Public | BindingFlags.Static).Where(f => f.FieldType == typeof(PredefinedThemePalette)).ToList();
            foreach(var p in palettes) {
                if(((PredefinedThemePalette)p.GetValue(null)).Name != palette)
                    continue;
                var theme = Theme.CreateTheme(PredefinedThemePalettes.DarkLilac, Theme.Themes.Single(x => x.Name == themeName));
                Theme.RegisterTheme(theme);
                ApplicationThemeHelper.ApplicationThemeName = theme.Name;
                return;
            }
            throw new ArgumentException($"Theme {themeName} with palette {palette} does not found. Contact Petr Zinovyev, please.");
        }
    }
}
