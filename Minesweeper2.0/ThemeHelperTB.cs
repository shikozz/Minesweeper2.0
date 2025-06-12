using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace saperdun
{
    public static class ThemeHelperTB
    {
        public static void ApplyNeonEffectTB(TextBlock textBlock, string currentTheme)
        {
            Color glowColor = (Color)Application.Current.FindResource("glowEffectColor");
            double blurRadius = 20;
            double opacity = 0.7;

            switch (currentTheme.ToLower())
            {
                case "cyberpunktheme":
                    textBlock.Effect = new DropShadowEffect
                    {
                        Color = glowColor,
                        BlurRadius = blurRadius,
                        Opacity = opacity,
                        ShadowDepth = 0
                    };
                    break;
                default:
                    textBlock.Effect = null;
                    break;
            }
        }
    }
}
