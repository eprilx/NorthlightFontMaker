/*
MIT License

Copyright (c) 2021 eprilx

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using AutoUpdaterDotNET;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace NorthlightFontMakerGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // Change current culture
            CultureInfo culture;
            culture = CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            string ToolVersion;
            try
            {
                ToolVersion = Assembly.LoadFrom("NorthlightFontMaker.dll").GetName().Version.ToString();
                ToolVersion = ToolVersion.Remove(ToolVersion.Length - 2);
                Hide();
                AutoUpdater.Synchronous = true;
                AutoUpdater.InstalledVersion = new Version(ToolVersion);
                AutoUpdater.UpdateFormSize = new System.Drawing.Size(1200, 800);
                AutoUpdater.Start("https://raw.githubusercontent.com/eprilx/NorthlightFontMaker/master/AutoUpdate.xml");
            }
            catch
            {
                ToolVersion = "1.0.0";
            }
            InitializeComponent();
            Title = "NorthlightFontMaker GUI v" + ToolVersion;
            Show();
        }

        private void RunNorthlightFontMakerConsole(List<string> args)
        {
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            exePath += "NorthlightFontMaker.exe";
            if (!File.Exists(exePath))
            { MessageBox.Show("Missing " + exePath, "Error", MessageBoxButton.OK, MessageBoxImage.Error); return; }
            string strCmdText = "";
            foreach (string str in args)
            {
                strCmdText += "\"" + str + "\"";
            }

            Console.WriteLine(strCmdText);

            Process process = new Process();
            process.StartInfo.FileName = exePath;
            process.StartInfo.Arguments = strCmdText;
            process.Start();
            process.WaitForExit();
        }

        private void rbtn2_Checked(object sender, RoutedEventArgs e)
        {
            if (txb3.Text != "(Optional)")
                txb3.Text = txb2.Text.ToString() + ".fnt";
        }

        private void rbtn1_Checked(object sender, RoutedEventArgs e)
        {
            if (txb3 != null)
                if (txb3.Text != "(Optional)")
                    txb3.Text = txb2.Text.ToString() + ".new";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<string> args = new List<string>();
            string fntInput = txb1.Text.ToString();
            string inputBINFNT = txb2.Text.ToString();
            string output = txb3.Text.ToString();
            if (!File.Exists(inputBINFNT))
            { MessageBox.Show("Missing original binfnt file", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return; }
            if (rbtn1.IsChecked == true)
            {
                if (!File.Exists(fntInput))
                { MessageBox.Show("Missing char desc file", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                args = new List<string> { "-fnt2binfnt", "-f", inputBINFNT, "-b", fntInput, "-o", output };
            }
            else if (rbtn2.IsChecked == true)
            {
                args = new List<string> { "-binfnt2fnt", "-f", inputBINFNT, "-o", output };
            }

            RunNorthlightFontMakerConsole(args);
        }


        private void txb2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txb3 == null)
            {
                return;
            }
            if (rbtn1.IsChecked == true)
            {
                txb3.Text = txb2.Text.ToString() + ".new";
            }
            else if (rbtn2.IsChecked == true)
            {
                txb3.Text = txb2.Text.ToString() + ".fnt";
            }
        }

        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "FNT file (*.fnt)|*.fnt";
            openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (openFileDialog.ShowDialog() == true)
            {
                txb1.Text = openFileDialog.FileName;
            }
        }

        private void btn2_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "BINFNT file (*.binfnt)|*.binfnt";
            openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (openFileDialog.ShowDialog() == true)
            {
                txb2.Text = openFileDialog.FileName;
            }
        }

        private void btn3_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (rbtn1.IsChecked == true)
                saveFileDialog.Filter = "New BINFNT file|*.binfnt";
            else if (rbtn2.IsChecked == true)
                saveFileDialog.Filter = "FNT file|*.fnt";
            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName != "")
            {
                txb3.Text = saveFileDialog.FileName;
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = e.Uri.AbsoluteUri;
            process.Start();
            e.Handled = true;
        }

    }
}
