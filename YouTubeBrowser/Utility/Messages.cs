using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YoutubeBrowser.Utility
{
    /// <summary>
    /// Utility class for displaying messages to the user
    /// </summary>
    public static class Messages
    {
        /// <summary>
        /// Static method for displaying MessageBox objects with information
        /// </summary>
        /// <param name="message">Main content of the message</param>
        /// <param name="caption">Caption of the message</param>
        /// <param name="button">Type of button to be displayed: Ok | OkCancel | YesNoCancel | YesNo </param>
        /// 
        public static void showMessageBox(string message, string caption, MessageBoxButton button)
        {
            MessageBox.Show(message, caption, button);
        }
    }
}
