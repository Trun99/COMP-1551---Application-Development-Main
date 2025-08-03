using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace QQuizzles.Utilities
{
    /// <summary>
    /// Geography-themed UI design constants and helper methods
    /// </summary>
    public static class GeographyTheme
    {
        // Primary Geography Color Palette
        public static readonly Color EarthBrown = Color.FromArgb(139, 69, 19);
        public static readonly Color OceanBlue = Color.FromArgb(25, 118, 210);
        public static readonly Color ForestGreen = Color.FromArgb(76, 175, 80);
        public static readonly Color DesertSand = Color.FromArgb(255, 193, 7);
        public static readonly Color MountainGray = Color.FromArgb(96, 125, 139);
        public static readonly Color SkyBlue = Color.FromArgb(135, 206, 235);
        public static readonly Color SunsetOrange = Color.FromArgb(255, 152, 0);
        public static readonly Color DeepOcean = Color.FromArgb(13, 71, 161);
        public static readonly Color LightEarth = Color.FromArgb(215, 204, 200);
        public static readonly Color DarkEarth = Color.FromArgb(62, 39, 35);

        // UI Element Colors
        public static readonly Color HeaderBackground = OceanBlue;
        public static readonly Color PanelBackground = Color.FromArgb(250, 248, 246);
        public static readonly Color ButtonPrimary = ForestGreen;
        public static readonly Color ButtonSecondary = DesertSand;
        public static readonly Color ButtonDanger = Color.FromArgb(244, 67, 54);
        public static readonly Color ButtonInfo = SkyBlue;
        public static readonly Color TextPrimary = DarkEarth;
        public static readonly Color TextSecondary = MountainGray;
        public static readonly Color BorderColor = Color.FromArgb(189, 189, 189);

        // Fonts
        public static readonly Font TitleFont = new Font("Segoe UI", 20, FontStyle.Bold);
        public static readonly Font HeaderFont = new Font("Segoe UI", 16, FontStyle.Bold);
        public static readonly Font SubHeaderFont = new Font("Segoe UI", 14, FontStyle.Bold);
        public static readonly Font BodyFont = new Font("Segoe UI", 11, FontStyle.Regular);
        public static readonly Font ButtonFont = new Font("Segoe UI", 12, FontStyle.Bold);
        public static readonly Font SmallFont = new Font("Segoe UI", 9, FontStyle.Regular);

        /// <summary>
        /// Apply geography theme to a button
        /// </summary>
        public static void ApplyButtonStyle(Button button, ButtonStyle style = ButtonStyle.Primary)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Font = ButtonFont;
            button.Cursor = Cursors.Hand;

            switch (style)
            {
                case ButtonStyle.Primary:
                    button.BackColor = ButtonPrimary;
                    button.ForeColor = Color.White;
                    break;
                case ButtonStyle.Secondary:
                    button.BackColor = ButtonSecondary;
                    button.ForeColor = DarkEarth;
                    break;
                case ButtonStyle.Danger:
                    button.BackColor = ButtonDanger;
                    button.ForeColor = Color.White;
                    break;
                case ButtonStyle.Info:
                    button.BackColor = ButtonInfo;
                    button.ForeColor = DarkEarth;
                    break;
            }

            // Add hover effects
            Color originalColor = button.BackColor;
            Color hoverColor = ControlPaint.Light(originalColor, 0.2f);
            
            button.MouseEnter += (s, e) => button.BackColor = hoverColor;
            button.MouseLeave += (s, e) => button.BackColor = originalColor;
        }

        /// <summary>
        /// Apply geography theme to a panel
        /// </summary>
        public static void ApplyPanelStyle(Panel panel, bool isHeader = false)
        {
            if (isHeader)
            {
                panel.BackColor = HeaderBackground;
                // Add gradient effect
                panel.Paint += (s, e) =>
                {
                    using (var brush = new LinearGradientBrush(
                        panel.ClientRectangle,
                        HeaderBackground,
                        ControlPaint.Dark(HeaderBackground, 0.1f),
                        LinearGradientMode.Vertical))
                    {
                        e.Graphics.FillRectangle(brush, panel.ClientRectangle);
                    }
                };
            }
            else
            {
                panel.BackColor = PanelBackground;
                panel.BorderStyle = BorderStyle.FixedSingle;
            }
        }

        /// <summary>
        /// Apply geography theme to a label
        /// </summary>
        public static void ApplyLabelStyle(Label label, LabelStyle style = LabelStyle.Body)
        {
            switch (style)
            {
                case LabelStyle.Title:
                    label.Font = TitleFont;
                    label.ForeColor = TextPrimary;
                    break;
                case LabelStyle.Header:
                    label.Font = HeaderFont;
                    label.ForeColor = Color.White;
                    break;
                case LabelStyle.SubHeader:
                    label.Font = SubHeaderFont;
                    label.ForeColor = TextPrimary;
                    break;
                case LabelStyle.Body:
                    label.Font = BodyFont;
                    label.ForeColor = TextPrimary;
                    break;
                case LabelStyle.Small:
                    label.Font = SmallFont;
                    label.ForeColor = TextSecondary;
                    break;
            }
        }

        /// <summary>
        /// Apply geography theme to a form
        /// </summary>
        public static void ApplyFormStyle(Form form)
        {
            form.BackColor = LightEarth;
            form.Font = BodyFont;
        }

        /// <summary>
        /// Create a geography-themed gradient background
        /// </summary>
        public static void PaintGradientBackground(Graphics graphics, Rectangle bounds, Color startColor, Color endColor)
        {
            using (var brush = new LinearGradientBrush(bounds, startColor, endColor, LinearGradientMode.Vertical))
            {
                graphics.FillRectangle(brush, bounds);
            }
        }

        /// <summary>
        /// Apply geography theme to text boxes
        /// </summary>
        public static void ApplyTextBoxStyle(TextBox textBox)
        {
            textBox.Font = BodyFont;
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.BackColor = Color.White;
            textBox.ForeColor = TextPrimary;
        }

        /// <summary>
        /// Apply geography theme to combo boxes
        /// </summary>
        public static void ApplyComboBoxStyle(ComboBox comboBox)
        {
            comboBox.Font = BodyFont;
            comboBox.BackColor = Color.White;
            comboBox.ForeColor = TextPrimary;
            comboBox.FlatStyle = FlatStyle.Flat;
        }
    }

    public enum ButtonStyle
    {
        Primary,
        Secondary,
        Danger,
        Info
    }

    public enum LabelStyle
    {
        Title,
        Header,
        SubHeader,
        Body,
        Small
    }
}
