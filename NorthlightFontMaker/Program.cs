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

using Mono.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace NorthlightFontMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            string ToolVersion;
            try
            {
                ToolVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                ToolVersion = ToolVersion.Remove(ToolVersion.Length - 2);
            }
            catch
            {
                ToolVersion = "1.0.0";
            }
            string originalBINFNT = null;
            string fntBMF = null;
            string output = null;
            //string version = null;
            //bool show_list = false;
            string command = null;

            // Change current culture
            CultureInfo culture;
            culture = CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            //List<string> SupportedGame = DefaultConfig.GetSupportedList();

            var p = new OptionSet()
            {
                {"fnt2binfnt", "Convert FNT to BINFNT",
                v => {command = "fnt2binfnt"; } },
                {"binfnt2fnt", "Convert BINFNT to FNT",
                v=> {command = "binfnt2fnt"; } },
                //{ "l|list", "show list supported games",
                //    v => show_list = v != null }
            };
            p.Parse(args);

            switch (command)
            {
                case "fnt2binfnt":
                    p = new OptionSet() {
                //{ "v|version=", "(required) Name of game. (FC2,FC3,...)",
                //   v => version = v  },
                { "f|originalBINFNT=", "(required) Original BINFNT file (*.binfnt|*.Fire_Font_Descriptor)",
                    v => originalBINFNT = v },
                { "b|charDesc=", "(required) Character description file (*.fnt)",
                    v => fntBMF = v },
                { "o|NewBINFNT=",
                   "(optional) Output new BINFNT file",
                    v => output = v },
                };
                    break;
                case "binfnt2fnt":
                    p = new OptionSet() {
                //{ "v|version=", "(required) Name of game",
                //   v => version = v  },
                { "f|originalBINFNT=", "(required) Original BINFNT file (*.binfnt)",
                    v => originalBINFNT = v },
                { "o|NewFNT=",
                   "(optional) Output FNT file",
                    v => output = v },
                };
                    break;
            }
            p.Parse(args);

            //if (show_list)
            //{
            //    PrintSupportedGame();
            //    Console.ReadKey();
            //    return;
            //}
            //else
            if (args.Length == 0 || originalBINFNT == null || (fntBMF == null && command == "fnt2binfnt"))
            {
                ShowHelp(p);
                return;
            }
            //else if (SupportedGame.FirstOrDefault(x => x.Contains(version)) == null)
            //{
            //    PrintSupportedGame();
            //    return;
            //}

            if (!originalBINFNT.EndsWith(".binfnt", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Unknown BINFNT file.");
                ShowHelp(p);
                return;
            }

            if (command == "fnt2binfnt")
            {
                if (!fntBMF.EndsWith(".fnt", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Unknown character description file.");
                    ShowHelp(p);
                    return;
                }
            }

            // CreateBINFNT
            try
            {
                switch (command)
                {
                    case "fnt2binfnt":
                        if (output == null)
                            output = originalBINFNT + ".new";
                        ConverterFunction.CreateBINFNTfromFNT(originalBINFNT, fntBMF, output);
                        break;
                    case "binfnt2fnt":
                        if (output == null)
                            output = originalBINFNT + ".fnt";
                        ConverterFunction.ConvertBINFNTtoFNT(originalBINFNT, output);
                        break;
                }
                Done();
            }
            finally
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(0);
            }

            void ShowHelp(OptionSet p)
            {
                switch (command)
                {
                    case "fnt2binfnt":
                        Console.WriteLine("\nUsage: NorthlightFontMaker --fnt2binfnt [OPTIONS]");
                        break;
                    case "binfnt2fnt":
                        Console.WriteLine("\nUsage: NorthlightFontMaker --binfnt2fnt [OPTIONS]");
                        break;
                    default:
                        PrintCredit();
                        Console.WriteLine("\nUsage: NorthlightFontMaker [OPTIONS]");
                        break;
                }

                Console.WriteLine("Options:");
                p.WriteOptionDescriptions(Console.Out);

                if (command == null)
                {
                    Console.WriteLine("\nExample:");
                    Console.WriteLine("NorthlightFontMaker --fnt2binfnt -f customer_facing.binfnt -b test.fnt -o customer_facing.binfnt.new");
                    Console.WriteLine("NorthlightFontMaker --binfnt2fnt -f customer_facing.binfnt -o customer_facing.binfnt.fnt");
                    Console.WriteLine("\nMore usage: https://github.com/eprilx/NorthlightFontMaker#usage");
                    Console.Write("More update: ");
                    Console.WriteLine("https://github.com/eprilx/NorthlightFontMaker/releases");
                }
            }

            //void PrintSupportedGame()
            //{
            //    Console.WriteLine("Supported games: ");
            //    foreach (string game in SupportedGame)
            //    {
            //        Console.WriteLine(game);
            //    }
            //}
            void PrintCredit()
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\nNorthlightFontMaker v" + ToolVersion);
                Console.WriteLine(" by eprilx");
                Console.Write("Special thanks to: ");
                Console.WriteLine("Rick Gibbed for Gibbed-IO");
                Console.ResetColor();
            }
            void Done()
            {
                Console.Write("\n********************************************");
                PrintCredit();
                Console.WriteLine("********************************************");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\n" + output + " has been created!");
                Console.ResetColor();
            }
        }
    }
}
