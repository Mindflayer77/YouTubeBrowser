using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YoutubeBrowser.Utility
{
    /// <summary>
    /// Class message
    /// </summary>
    public static class Messages
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <param name="button"></param>
        public static void showMessageBox(string message, string caption, MessageBoxButton button)
        {
            MessageBox.Show(message, caption, button);
        }
    }
}
