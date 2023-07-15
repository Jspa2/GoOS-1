﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL2CPU.API.Attribs;
using PrismAPI.Graphics;

namespace GoOS.GUI.Apps
{
    public class DesktopIcons : Window
    {
        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.folder.bmp")] private static byte[] folderIconRaw;
        private static Canvas folderIcon = Image.FromBitmap(folderIconRaw, false);

        Button folderButton;

        public DesktopIcons()
        {
            Contents = new Canvas(64, 80);
            Contents.Clear(WindowManager.DesktopBackgroundColour);
            X = 10;
            Y = 10;
            Visible = true;
            Closable = false;
            HasTitlebar = false;

            folderButton = new Button(this, 10, 10, 64, 80, "Apps")
            {
                UseSystemStyle = false,
                BackgroundColour = Color.UbuntuPurple,
                TextColour = Color.White,

                Image = folderIcon
            };
            
            folderButton.Clicked = FolderClicked;

            folderButton.Render();
        }

        private static void FolderClicked()
        {
            WindowManager.AddWindow(new Apps.AppManager());
        }
    }
}
