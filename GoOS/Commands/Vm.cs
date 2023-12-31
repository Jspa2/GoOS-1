﻿using System.IO;

namespace GoOS.Commands
{
    internal class Vm
    {
        public static void command(string args)
        {
            string uprootdir = @"0:\content\vrt\";
            string rootdir = @"0:\content\vrt\ChaOS\";

            if (!Directory.Exists(uprootdir))
            {
                Directory.CreateDirectory(uprootdir);
                if (!Directory.Exists(rootdir))
                {
                    Directory.CreateDirectory(rootdir);
                }
            }
            else if (!Directory.Exists(rootdir))
            {
                Directory.CreateDirectory(rootdir);
            }

            GoOS.Virtualisation.ChaOS.ChaOS.boot(@"0:\content\vrt\ChaOS\");
        }
    }
}