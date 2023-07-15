﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.System;
using IL2CPU.API.Attribs;
using PrismAPI.Graphics;
using PrismAPI.Hardware.GPU;

namespace GoOS.GUI.Apps
{
    public class StartMenu : Window
    {
        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.user.bmp")] private static byte[] userImageRaw;
        private static Canvas userImage = Image.FromBitmap(userImageRaw, false);

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.shutdown.bmp")] private static byte[] shutdownIconRaw;
        private static Canvas shutdownIcon = Image.FromBitmap(shutdownIconRaw, false);

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.gterm.bmp")] private static byte[] gtermIconRaw;
        private static Canvas gtermIcon = Image.FromBitmap(gtermIconRaw, false);

        List<Button> appButtons = new();

        private const int buttonHeight = 32;

        private void AddAppButton(string name, Action clickedAction)
        {
            appButtons.Add(new Button(this, 8, (ushort)(48 + (appButtons.Count * buttonHeight)), (ushort)(Contents.Width - 16), buttonHeight, name)
            {
                Clicked = clickedAction
            });
        }

        private void AddAppButtons()
        {
            AddAppButton("GoOS Applications", () => {
                WindowManager.AddWindow(new AppManager());
                CloseStartMenu();
            });

            AddAppButton("GTerm", () => {
                WindowManager.AddWindow(new GTerm());
                CloseStartMenu();
            });

            AddAppButton("Clock", () =>
            {
                WindowManager.AddWindow(new Clock());
                CloseStartMenu();
            });
        }

        private void AddPowerButton()
        {
            Button button = new Button(
                this,
                (ushort)(Contents.Width - 96 - 8),
                (ushort)(Contents.Height - 64 - 8),
                96,
                64,
                "Power..."
            );

            button.Image = shutdownIcon;

            button.Clicked = Power_Click;
        }

        private void AddExitGuiButton()
        {
            Button button = new Button(
                this,
                8,
                (ushort)(Contents.Height - 64 - 8),
                96,
                64,
                "Exit"
            );

            button.Image = gtermIcon;

            button.Clicked = ExitGui_Click;
        }

        public StartMenu()
        {
            Contents = new Canvas(256, 384);
            Contents.Clear(Color.DeepGray);
            X = 0;
            Y = WindowManager.Canvas.Height - 28 - Contents.Height;
            HasTitlebar = false;
            Visible = false;

            Contents.DrawImage(8, 8, userImage);
            Contents.DrawString(40, 16, Kernel.username, BetterConsole.font, Color.White);

            AddAppButtons();

            AddPowerButton();

            AddExitGuiButton();

            foreach (Control control in Controls)
            {
                control.Render();
            }
        }

        private void ExitGui_Click()
        {
            Dialogue.Show(
                "GoOS",
                "Exit to the console?",
                new()
                {
                    new() { Text = "Exit", Callback = () =>
                    {
                        WindowManager.CloseAll();

                        BetterConsole.ConsoleMode = true;

                        WindowManager.Canvas = Display.GetDisplay(
                            800,
                            600
                        );
                        
                        // ^ TODO not hardcode.
                    }},

                    new() { Text = "Cancel" }
                },
                gtermIcon
            );

            CloseStartMenu();
        }

        private void Power_Click()
        {
            Dialogue.Show(
                "GoOS",
                "What would you like to do?",
                new()
                {
                    new() { Text = "Shut Down", Callback = () =>
                    {
                        Power.Shutdown();
                    }},

                    new() { Text = "Reboot", Callback = () =>
                    {
                        Power.Reboot();
                    }}
                },
                shutdownIcon
            );

            CloseStartMenu();
        }

        private void OpenStartMenu()
        {
            Visible = true;

            WindowManager.MoveWindowToFront(this);

            WindowManager.GetWindowByType<Taskbar>().HandleStartMenuOpen();
        }

        private void CloseStartMenu()
        {
            Visible = false;

            WindowManager.GetWindowByType<Taskbar>().HandleStartMenuClose();
        }

        public void ToggleStartMenu()
        {
            if (Visible)
            {
                CloseStartMenu();
            }
            else
            {
                OpenStartMenu();
            }
        }
    }
}
