using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YoutubeBrowser.Utility
{
    public static class Messages
    {
        public static void showMessageBox(string message, string caption, MessageBoxButton button)
        {
            MessageBox.Show(message, caption, button);
        }
    }
}
