using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using saperdun.Properties;

namespace saperdun
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            string savedTheme = Settings.Default.Theme;
            ThemeManager.CurrentTheme = savedTheme;
            ThemeManager.ThemeChanged += OnThemeChanged;
            LoadTheme(ThemeManager.CurrentTheme);
        }
        private void OnThemeChanged(object sender, EventArgs e)
        {
            LoadTheme(ThemeManager.CurrentTheme);
        }
        private void LoadTheme(string themeName)
        {
            string themeUri = $"/Themes/{themeName}.xaml";
            try
            {
                Resources.MergedDictionaries.Clear();
                ResourceDictionary themeDictionary = (ResourceDictionary)Application.LoadComponent(new Uri(themeUri, UriKind.Relative));
                Resources.MergedDictionaries.Add(themeDictionary);
                setGlowEffect(themeName);
                foreach(Window window in Application.Current.Windows)
                {
                    window.InvalidateVisual();
                    window.UpdateLayout();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error loading theme: {ex.Message}");
            }
        }

        public static void setGlowEffect(string themeName, bool need = true)
        {
            ApplyGlowEffectToControls(themeName, need);
        }

        private static void ApplyGlowEffectToControls(string themeName, bool need = true)
        {
            foreach (Window window in Application.Current.Windows)
            {
                ApplyGlowEffectToVisualTree(window, themeName, need);
            }
        }

        private static void ApplyGlowEffectToVisualTree(DependencyObject parent, string themeName, bool need = true)
        {
            for(int i = 0; i< VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if(child is Control control)
                {
                    ThemeHelper.ApplyNeonEffect(control, themeName, need);
                }
                if (child is TextBlock textBlock)
                {
                    ThemeHelperTB.ApplyNeonEffectTB(textBlock, themeName);
                }
                ApplyGlowEffectToVisualTree(child, themeName, need);
            }
        }
    }
}
