using System;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace ICSharpCode.Core.WinForms
{
    public static class WinFormsPrinterService
    {
        private static PageSettings    _pageSettings;
        private static PrinterSettings _printSettings;   

        public static PageSettings PageSettings
        {
            get 
            {
                if (_pageSettings == null)
                {
                    PrinterSettings printSettings = WinFormsPrinterService.PrinterSettings;
                    _pageSettings = printSettings.DefaultPageSettings;
                }

                return _pageSettings; 
            }
            set 
            {
                if (value != null)
                {
                    _pageSettings = value; 
                }
            }
        }

        public static PrinterSettings PrinterSettings
        {
            get 
            {
                if (_printSettings == null)
                {
                    _printSettings = new PrinterSettings();

                    //if (Environment.OSVersion.Version.Major >= 6)
                    //{   
                    //    PrintDialog printDlg = new PrintDialog();
                    //    printDlg.PrinterSettings = _printSettings;
                    //    //printDlg.UseEXDialog = true;
                    //    printDlg.AllowCurrentPage = false;
                    //    printDlg.AllowPrintToFile = false;
                    //    printDlg.AllowSelection = false;
                    //    printDlg.AllowSomePages = false;

                    //    if (printDlg.ShowDialog(Form.ActiveForm) == DialogResult.OK)
                    //    {
                    //        _printSettings = printDlg.PrinterSettings;
                    //    }
                    //}
                }

                return _printSettings; 
            }
            set 
            { 
                if (value != null)
                {
                    _printSettings = value; 
                }
            }
        }

        public static void ShowPrinterSettings()
        {
            PrintDialog printDlg = new PrintDialog();
            printDlg.PrinterSettings = WinFormsPrinterService.PrinterSettings;
            printDlg.UseEXDialog = true;

            if (printDlg.ShowDialog(Form.ActiveForm) == DialogResult.OK)
            {
                WinFormsPrinterService.PrinterSettings = printDlg.PrinterSettings;
            }
        }

        public static void ShowPageSettings()
        {
            PageSetupDialog pageSetup = new PageSetupDialog();

            pageSetup.AllowPrinter = true;   // does not work on Vista!
            pageSetup.ShowHelp     = true;

            pageSetup.PageSettings    = WinFormsPrinterService.PageSettings;
            pageSetup.PrinterSettings = WinFormsPrinterService.PrinterSettings;

            if (pageSetup.ShowDialog(Form.ActiveForm) == DialogResult.OK)
            {
                WinFormsPrinterService.PageSettings = pageSetup.PageSettings;
                WinFormsPrinterService.PrinterSettings = pageSetup.PrinterSettings;
            }
        }
    }
}
