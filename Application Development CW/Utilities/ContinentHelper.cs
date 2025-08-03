using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using QQuizzles.Models;

namespace QQuizzles.Utilities
{
    /// <summary>
    /// Helper class for continent-related operations and simple image generation
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
        /// Create a simple continent text representation
        /// </summary>
        public static Bitmap CreateContinentImage(Continent continent, int width = 300, int height = 200)
        {
            var bitmap = new Bitmap(width, height);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                // Enable anti-aliasing for smoother graphics
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                
                // Simple solid background with continent color
                Color continentColor = GetColor(continent);
                using (var bgBrush = new SolidBrush(Color.FromArgb(30, continentColor.R, continentColor.G, continentColor.B)))
                {
                    graphics.FillRectangle(bgBrush, 0, 0, width, height);
                }
                
                // Draw continent name in center
                string name = GetDisplayName(continent);
                using (var font = new Font("Segoe UI", 24, FontStyle.Bold))
                using (var textBrush = new SolidBrush(continentColor))
                {
                    var textSize = graphics.MeasureString(name, font);
                    float x = (width - textSize.Width) / 2;
                    float y = (height - textSize.Height) / 2;
                    
                    // Draw text centered
                    graphics.DrawString(name, font, textBrush, x, y);
                }
                
                // Simple border
                using (var pen = new Pen(continentColor, 2))
                {
                    graphics.DrawRectangle(pen, 1, 1, width - 3, height - 3);
                }
            }
            
            return bitmap;
        }

        /// <summary>
        /// Get continent emoji representation
        /// </summary>
        public static string GetEmoji(Continent continent)
        {
            return continent switch
            {
                Continent.Asia => "🌏",
                Continent.Europe => "🌍",
                Continent.America => "🌎",
                Continent.Africa => "🌍",
                Continent.Oceania => "🏝️",
                Continent.Antarctica => "🧊",
                _ => "🌍"
            };
        }

        /// <summary>
        /// Get short name for continent
        /// </summary>
        public static string GetShortName(Continent continent)
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
        /// Get all continent names as string array
        /// </summary>
        public static string[] GetAllContinentNames()
        {
            return new string[]
            {
                "Asia",
                "Europe",
                "America",
                "Africa",
                "Oceania",
                "Antarctica"
            };
        }

        /// <summary>
        /// Get continent image (simple colored rectangle for now)
        /// </summary>
        public static Image GetContinentImage(Continent continent)
        {
            var bitmap = new Bitmap(280, 200);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Fill background
                graphics.FillRectangle(new SolidBrush(Color.FromArgb(240, 248, 255)), 0, 0, 280, 200);

                // Draw continent representation
                var continentColor = GetColor(continent);
                var brush = new SolidBrush(continentColor);

                // Simple continent shape representation
                switch (continent)
                {
                    case Continent.Asia:
                        graphics.FillEllipse(brush, 50, 50, 180, 100);
                        break;
                    case Continent.Europe:
                        graphics.FillRectangle(brush, 80, 60, 120, 80);
                        break;
                    case Continent.America:
                        graphics.FillRectangle(brush, 60, 30, 40, 140);
                        graphics.FillRectangle(brush, 120, 50, 60, 120);
                        break;
                    case Continent.Africa:
                        graphics.FillEllipse(brush, 90, 40, 100, 120);
                        break;
                    case Continent.Oceania:
                        graphics.FillEllipse(brush, 100, 80, 80, 40);
                        break;
                    case Continent.Antarctica:
                        graphics.FillRectangle(brush, 40, 120, 200, 60);
                        break;
                }

                // Add continent name
                var font = new Font("Arial", 14, FontStyle.Bold);
                var textBrush = new SolidBrush(Color.FromArgb(33, 33, 33));
                var text = GetDisplayName(continent);
                var textSize = graphics.MeasureString(text, font);
                var textX = (280 - textSize.Width) / 2;
                var textY = 170;

                graphics.DrawString(text, font, textBrush, textX, textY);
            }

            return bitmap;
        }
    }
}
