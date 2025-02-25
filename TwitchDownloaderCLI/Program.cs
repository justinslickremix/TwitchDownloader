﻿using CommandLine;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TwitchDownloaderCLI.Modes;
using TwitchDownloaderCLI.Modes.Arguments;
using TwitchDownloaderCLI.Tools;

namespace TwitchDownloaderCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            string processFileName = Environment.ProcessPath.Split(Path.DirectorySeparatorChar).Last();
            if (args.Length == 0)
            {
                if (Path.GetExtension(processFileName).Equals(".exe"))
                {
                    // Some Windows users try to double click the executable
                    Console.WriteLine("This is a command line tool. Please open a terminal and run \"{0} help\" from there for more information.{1}Press any key to close...",
                        processFileName, Environment.NewLine);
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("Usage: {0} [VERB] [OPTIONS]{1}Try \'{2} help\' for more information.",
                        processFileName, Environment.NewLine, processFileName);
                }
                Environment.Exit(1);
            }

            string[] preParsedArgs;
            if (args.Any(x => x is "-m" or "--mode" or "--embed-emotes"))
            {
                // A legacy syntax was used, convert to new syntax
                preParsedArgs = PreParseArgs.Process(PreParseArgs.ConvertFromOldSyntax(args, processFileName));
            }
            else
            {
                preParsedArgs = PreParseArgs.Process(args);
            }

            Parser.Default.ParseArguments<VideoDownloadArgs, ClipDownloadArgs, ChatDownloadArgs, ChatDownloadUpdaterArgs, ChatRenderArgs, FfmpegArgs, CacheArgs>(preParsedArgs)
                .WithParsed<VideoDownloadArgs>(DownloadVideo.Download)
                .WithParsed<ClipDownloadArgs>(DownloadClip.Download)
                .WithParsed<ChatDownloadArgs>(DownloadChat.Download)
                .WithParsed<ChatDownloadUpdaterArgs>(DownloadChatUpdater.Update)
                .WithParsed<ChatRenderArgs>(RenderChat.Render)
                .WithParsed<FfmpegArgs>(FfmpegHandler.ParseArgs)
                .WithParsed<CacheArgs>(CacheHandler.ParseArgs)
                .WithNotParsed(_ => Environment.Exit(1));
        }
    }
}