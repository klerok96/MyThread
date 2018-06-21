using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MyThread
{
    public static class ControlHelpers
    {
        public static ProgressBar FindEmptyPogressBar(MainWindow window)
        {
            /// casting the content into panel
            Panel mainContainer = (Panel)window.Content;

            /// GetAll UIElement
            UIElementCollection element = mainContainer.Children;

            /// casting the UIElementCollection into List
            List<FrameworkElement> lstElement = element.Cast<FrameworkElement>().ToList();

            /// Geting all Control from list
            var lstControl = lstElement.OfType<ProgressBar>();

            foreach (var progressBar in lstControl)
            {
                if (progressBar.Value == 0)
                {
                    return progressBar;
                }
            }

            return null;
        }

        public static int GetIdProgressBar(this ProgressBar progressBar)
        {
            return Convert.ToInt32(progressBar.Name.Substring(progressBar.Name.Length - 1));
        }
    }
}
