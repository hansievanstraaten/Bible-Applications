using GeneralExtensions;
using System;
using System.Text;
using System.Windows;
using WPF.Tools.Dictionaries;

namespace Bibles.Common
{
    public static class ErrorLog
    {
        public static void ShowError(Exception err)
        {
#if DEBUG
            StringBuilder errString = new StringBuilder();

            errString.AppendLine("Full Error Message");

            errString.AppendLine(err.GetFullExceptionMessage());

            errString.AppendLine();

            errString.AppendLine("Full Source");

            errString.AppendLine(err.ExstendedSource());

            MessageBox.Show(errString.ToString());

#else
            MessageBox.Show(TranslationDictionary.Translate(err.InnerExceptionMessage()));
#endif
        }

    }
}
