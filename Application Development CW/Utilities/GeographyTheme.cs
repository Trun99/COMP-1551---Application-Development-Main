using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace QQuizzles.Utilities
{
    /// <summary>
    /// Geography-themed UI design constants and helper methods
    /// </summary>
    public static class GeographyTheme
    {
        // Modern Geography Color Palette
        public static readonly Color PrimaryOceanBlue = Color.FromArgb(33, 150, 243);     // Modern ocean blue
        public static readonly Color PrimaryForestGreen = Color.FromArgb(76, 175, 80);    // Modern forest green
        public static readonly Color SecondaryTeal = Color.FromArgb(0, 150, 136);         // Complementary teal
        public static readonly Color AccentAmber = Color.FromArgb(255, 193, 7);           // Warm accent
        public static readonly Color NeutralGray = Color.FromArgb(96, 125, 139);          // Modern gray
        public static readonly Color LightBackground = Color.FromArgb(250, 250, 250);     // Clean background
        public static readonly Color DarkText = Color.FromArgb(33, 33, 33);               // Modern dark text
        public static readonly Color LightText = Color.FromArgb(117, 117, 117);           // Secondary text
        public static readonly Color BorderLight = Color.FromArgb(224, 224, 224);         // Light borders
        public static readonly Color SurfaceWhite = Color.White;                          // Pure white surfaces

        // Modern UI Element Colors
        public static readonly Color HeaderBackground = PrimaryOceanBlue;
        public static readonly Color PanelBackground = LightBackground;
        public static readonly Color CardBackground = SurfaceWhite;
        public static readonly Color ButtonPrimary = PrimaryForestGreen;
        public static readonly Color ButtonSecondary = SecondaryTeal;
        public static readonly Color ButtonAccent = AccentAmber;
        public static readonly Color ButtonDanger = Color.FromArgb(244, 67, 54);
        public static readonly Color TextPrimary = DarkText;
        public static readonly Color TextSecondary = LightText;
        public static readonly Color BorderColor = BorderLight;

        // Modern Typography System
        public static readonly Font TitleFont = new Font("Segoe UI", 24, FontStyle.Bold);
        public static readonly Font HeaderFont = new Font("Segoe UI", 18, FontStyle.Bold);
        public static readonly Font SubHeaderFont = new Font("Segoe UI", 16, FontStyle.Bold);
        public static readonly Font BodyFont = new Font("Segoe UI", 12, FontStyle.Regular);
        public static readonly Font ButtonFont = new Font("Segoe UI", 13, FontStyle.Bold);
        public static readonly Font SmallFont = new Font("Segoe UI", 10, FontStyle.Regular);
        public static readonly Font CaptionFont = new Font("Segoe UI", 9, FontStyle.Regular);

        /// <summary>
        /// Apply modern flat button style
        /// </summary>
        public static void ApplyButtonStyle(Button button, ButtonStyle style = ButtonStyle.Primary)
        {
            // Modern flat button styling
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Font = ButtonFont;
            button.Cursor = Cursors.Hand;
            button.UseVisualStyleBackColor = false;

            // Add padding for better touch targets
            button.Padding = new Padding(16, 12, 16, 12);
            button.TextAlign = ContentAlignment.MiddleCenter;

            // Apply color scheme based on style
            switch (style)
            {
                case ButtonStyle.Primary:
                    button.BackColor = ButtonPrimary;
                    button.ForeColor = Color.White;
                    break;
                case ButtonStyle.Secondary:
                    button.BackColor = ButtonSecondary;
                    button.ForeColor = Color.White;
                    break;
                case ButtonStyle.Accent:
                    button.BackColor = ButtonAccent;
                    button.ForeColor = DarkText;
                    break;
                case ButtonStyle.Danger:
                    button.BackColor = ButtonDanger;
                    button.ForeColor = Color.White;
                    break;
                case ButtonStyle.Warning:
                    button.BackColor = AccentAmber;
                    button.ForeColor = DarkText;
                    break;
            }

            // Modern hover effects with smooth transitions
            Color originalColor = button.BackColor;
            Color hoverColor = ControlPaint.Dark(originalColor, 0.1f);
            Color pressedColor = ControlPaint.Dark(originalColor, 0.2f);

            button.MouseEnter += (s, e) => {
                button.BackColor = hoverColor;
                button.FlatAppearance.BorderSize = 1;
                button.FlatAppearance.BorderColor = ControlPaint.Dark(originalColor, 0.3f);
            };

            button.MouseLeave += (s, e) => {
                button.BackColor = originalColor;
                button.FlatAppearance.BorderSize = 0;
            };

            button.MouseDown += (s, e) => button.BackColor = pressedColor;
            button.MouseUp += (s, e) => button.BackColor = hoverColor;
        }

        /// <summary>
        /// Apply modern card-style panel design
        /// </summary>
        public static void ApplyPanelStyle(Panel panel, bool isHeader = false, bool isCard = false)
        {
            if (isHeader)
            {
                panel.BackColor = HeaderBackground;
                panel.BorderStyle = BorderStyle.None;

                // Modern flat header with subtle shadow effect
                panel.Paint += (s, e) =>
                {
                    // Fill header background
                    e.Graphics.FillRectangle(new SolidBrush(HeaderBackground), panel.ClientRectangle);

                    // Add bottom border for definition
                    using (var pen = new Pen(ControlPaint.Dark(HeaderBackground, 0.2f), 2))
                    {
                        e.Graphics.DrawLine(pen, 0, panel.Height - 1, panel.Width, panel.Height - 1);
                    }
                };
            }
            else if (isCard)
            {
                panel.BackColor = CardBackground;
                panel.BorderStyle = BorderStyle.None;
                panel.Padding = new Padding(20);

                // Modern card with subtle shadow
                panel.Paint += (s, e) =>
                {
                    var rect = panel.ClientRectangle;

                    // Draw card background
                    e.Graphics.FillRectangle(new SolidBrush(CardBackground), rect);

                    // Draw subtle border
                    using (var pen = new Pen(BorderColor, 1))
                    {
                        e.Graphics.DrawRectangle(pen, 0, 0, rect.Width - 1, rect.Height - 1);
                    }
                };
            }
            else
            {
                panel.BackColor = PanelBackground;
                panel.BorderStyle = BorderStyle.None;
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
            form.BackColor = LightBackground;
            form.Font = BodyFont;
        }

        /// <summary>
        /// Apply modern text box styling
        /// </summary>
        public static void ApplyTextBoxStyle(TextBox textBox)
        {
            textBox.Font = BodyFont;
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.BackColor = SurfaceWhite;
            textBox.ForeColor = TextPrimary;
            textBox.Padding = new Padding(12, 8, 12, 8);

            // Modern focus effects
            Color originalBorderColor = BorderColor;
            Color focusBorderColor = PrimaryOceanBlue;

            // Custom border painting for modern look
            textBox.Paint += (s, e) =>
            {
                if (textBox.Focused)
                {
                    using (var pen = new Pen(focusBorderColor, 2))
                    {
                        e.Graphics.DrawRectangle(pen, 0, 0, textBox.Width - 1, textBox.Height - 1);
                    }
                }
            };

            textBox.Enter += (s, e) => textBox.Invalidate();
            textBox.Leave += (s, e) => textBox.Invalidate();
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
        Accent,
        Danger,
        Warning
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
