using System.Drawing;
using System.Windows.Forms;

namespace Binarysharp.MemoryManagement.Extensions
{
    /// <summary>
    ///     A class containing extension methods for windows forms rich text box's.
    /// </summary>
    public static class RichTextBoxExtensions
    {
        #region Methods

        public static void AppendText(this RichTextBox box, string text, Color color, params object[] args)
        {
            text = string.Format(text, args);
            if (color == Color.Empty)
            {
                box.AppendText(text);
                return;
            }

            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;

            box.SelectionStart = box.TextLength;
            box.ScrollToCaret();
        }

        public static void AppendLine(this RichTextBox box, string text, Color color, params object[] args)
        {
            if (!box.InvokeRequired)
            {
                box.AppendText("\n" + text, color == Color.Empty ? box.ForeColor : color, args);
            }
            else
            {
                box.InvokeAppendText("\n" + text, color == Color.Empty ? box.ForeColor : color, args);
            }
        }

        private static void InvokeAppendText(this RichTextBox box, string text, Color color, params object[] args)
        {
            box.Invoke((MethodInvoker) delegate { box.AppendText(text, color, args); });
        }

        #endregion
    }
}