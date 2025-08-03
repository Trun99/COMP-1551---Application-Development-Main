using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using QQuizzles.Models;

namespace QQuizzles.Utilities
{
    /// <summary>
    /// Helper class for continent-related operations and image generation
    /// </summary>
    public static class ContinentHelper
    {
        /// <summary>
        /// Get all available continents
        /// </summary>
        public static List<Continent> GetAllContinents()
        {
            return new List<Continent>
            {
                Continent.Asia,
                Continent.Europe,
                Continent.America,
                Continent.Africa,
                Continent.Oceania,
                Continent.Antarctica
            };
        }

        /// <summary>
        /// Get display name for continent
        /// </summary>
        public static string GetDisplayName(Continent continent)
        {
            return continent switch
            {
                Continent.Asia => "Asia",
                Continent.Europe => "Europe", 
                Continent.America => "America",
                Continent.Africa => "Africa",
                Continent.Oceania => "Oceania",
                Continent.Antarctica => "Antarctica",
                _ => continent.ToString()
            };
        }

        /// <summary>
        /// Get color associated with continent
        /// </summary>
        public static Color GetColor(Continent continent)
        {
            return continent switch
            {
                Continent.Asia => Color.FromArgb(255, 193, 7),      // Amber
                Continent.Europe => Color.FromArgb(33, 150, 243),   // Blue
                Continent.America => Color.FromArgb(76, 175, 80),   // Green
                Continent.Africa => Color.FromArgb(255, 87, 34),    // Deep Orange
                Continent.Oceania => Color.FromArgb(0, 150, 136),   // Teal
                Continent.Antarctica => Color.FromArgb(96, 125, 139), // Blue Grey
                _ => Color.Gray
            };
        }

        /// <summary>
        /// Create a modern continent representation image
        /// </summary>
        public static Bitmap CreateContinentImage(Continent continent, int width = 300, int height = 200)
        {
            var bitmap = new Bitmap(width, height);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                // Enable anti-aliasing for smoother graphics
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                
                // Modern gradient background
                using (var bgBrush = new LinearGradientBrush(
                    new Rectangle(0, 0, width, height),
                    Color.FromArgb(240, 248, 255),
                    Color.FromArgb(230, 240, 250),
                    45f))
                {
                    graphics.FillRectangle(bgBrush, 0, 0, width, height);
                }
                
                // Draw modern continent representation
                DrawModernContinentDesign(graphics, continent, width, height);
                
                // Modern typography for continent name
                string name = GetDisplayName(continent);
                using (var font = new Font("Segoe UI", 18, FontStyle.Bold))
                using (var textBrush = new SolidBrush(Color.FromArgb(33, 33, 33)))
                {
                    var textSize = graphics.MeasureString(name, font);
                    float x = (width - textSize.Width) / 2;
                    float y = height - 35;
                    
                    // Draw text with modern styling
                    graphics.DrawString(name, font, textBrush, x, y);
                }
                
                // Modern border
                using (var pen = new Pen(Color.FromArgb(224, 224, 224), 1))
                {
                    graphics.DrawRectangle(pen, 0, 0, width - 1, height - 1);
                }
            }
            
            return bitmap;
        }

        private static void DrawModernContinentDesign(Graphics graphics, Continent continent, int width, int height)
        {
            switch (continent)
            {
                case Continent.Asia:
                    DrawAsiaDesign(graphics, width, height);
                    break;
                case Continent.Europe:
                    DrawEuropeDesign(graphics, width, height);
                    break;
                case Continent.America:
                    DrawAmericaDesign(graphics, width, height);
                    break;
                case Continent.Africa:
                    DrawAfricaDesign(graphics, width, height);
                    break;
                case Continent.Oceania:
                    DrawOceaniaDesign(graphics, width, height);
                    break;
                case Continent.Antarctica:
                    DrawAntarcticaDesign(graphics, width, height);
                    break;
            }
        }
    }
}
