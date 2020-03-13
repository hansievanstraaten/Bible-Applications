using System;
using System.Windows;
using GeneralExtensions;
using WPF.Tools.BaseClasses;

namespace ViSo.Dialogs.Controls
{
    public static class ControlDialog
    {
        private static ControlWindow window;

        public static bool? ShowDialog(string windowTitle, UserControlBase control, string boolUpdateMethod)
        {
            try
            {
                ControlDialog.window = new ControlWindow(windowTitle, control, boolUpdateMethod, true);

                return ControlDialog.window.ShowDialog();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());

                return false;
            }
            finally
            {
                ControlDialog.window = null;
            }
        }

        public static void Show(string windowTitle, UserControlBase control, string boolUpdateMethod, Window owner = null, bool isTopMost = false)
        {
            try
            {
                ControlDialog.window = new ControlWindow(windowTitle, control, boolUpdateMethod, false);

                if (owner != null)
                {
                    ControlDialog.window.Owner = owner;
                }

                ControlDialog.window.Topmost = isTopMost;

                ControlDialog.window.Show();

            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
            finally
            {
                ControlDialog.window = null;
            }
        }
    }
}
