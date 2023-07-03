﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys = Cosmos.System;
using IL2CPU.API.Attribs;
using PrismAPI.Hardware.GPU;
using PrismAPI.Graphics;
using Cosmos.Core.Memory;

namespace GoOS.GUI
{
    public class WindowManager
    {
        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.mouse.bmp")]
        private static byte[] mouseRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.closebutton.bmp")]
        private static byte[] closeButtonRaw;

        public static uint mposx, mposy;
        public static ushort dy, dx;

        private static Canvas mouse = Image.FromBitmap(mouseRaw, false);
        private static Canvas closeButton = Image.FromBitmap(closeButtonRaw, false);

        //private static uint LastCursorX = 0, LastCursorY = 0;

        public static Display Canvas;

        public static List<Window> Windows = new List<Window>();
        public static bool runOnce = true;


        public static void Update()
        {
            if (runOnce)
            {
                Sys.MouseManager.ScreenWidth = 1280;
                Sys.MouseManager.ScreenHeight = 720;
                runOnce = false;
            }

            Canvas.Clear(Color.UbuntuPurple);

            for (int i = 0; i < Windows.Count; i++)
            {
                if (Windows[i] != null && Windows[i].Visible)
                {
                    // if (Sys.MouseManager.MouseState != Sys.MouseState.Left)
                    // {
                    //     Windows[i].Moving = false;
                    // }
                    // else
                    // {
                    // mposx = Sys.MouseManager.X;
                    // mposy = Sys.MouseManager.Y;
                    // if (Windows[i].Moving == false)
                    // {
                    //     dx = (ushort)(mposx - Windows[i].X);
                    //     dy = (ushort)(mposy - Windows[i].Y);
                    //     Windows[i].Moving = true;
                    // }
                    /*if (Sys.MouseManager.X > WindowList.X[i] && Sys.MouseManager.X < WindowList.X[i] + WindowList.canvas[i].Width && Sys.MouseManager.Y > WindowList.Y[i] && Sys.MouseManager.Y < WindowList.Y[i] + 16)
                    {

                    }*/

                    // if (Sys.MouseManager.X > Windows[i].X &&
                    //     Sys.MouseManager.X < Windows[i].X + Windows[i].Contents.Width &&
                    //     Sys.MouseManager.Y > Windows[i].Y && Sys.MouseManager.Y < Windows[i].Y + 16)
                    // {
                    //     Windows[i].Y = (ushort)(mposy - dy);
                    //     Windows[i].X = (ushort)(mposx - dx);
                    // }
                    if (Sys.MouseManager.MouseState == Sys.MouseState.Left)
                    {
                        if (Windows[i].Closeable)
                        {
                            if (Sys.MouseManager.X > Windows[i].X + Windows[i].Contents.Width - 14 &&
                                Sys.MouseManager.X < Windows[i].X + Windows[i].Contents.Width - 2 &&
                                Sys.MouseManager.Y > Windows[i].Y + 2 && Sys.MouseManager.Y < Windows[i].Y + 14)
                            {
                                Windows[i].Visible = false;
                            }
                        }
                    }


                    // Canvas.DrawString(360, Canvas.Height - 24,
                    //     "Window Location: " + Windows[i].X + ", " + Windows[i].Y + " | Cursor Predicted location: " +
                    //     dx + " " + dy,
                    //     BetterConsole.font,
                    //     Color.White, true); // debug

                    DrawWindow(Windows[i].Contents, Windows[i].X, Windows[i].Y, Windows[i].Title, Windows[i].Closeable);
                    Windows[i].Update();
                }
            }

            Canvas.DrawString(128, Canvas.Height - 32,
                Canvas.GetFPS() + "fps / " + Cosmos.Core.GCImplementation.GetAvailableRAM() + "mb", BetterConsole.font,
                Color.White, true); // debug

            Canvas.DrawImage((int)Sys.MouseManager.X, (int)Sys.MouseManager.Y, mouse, true);
            Canvas.Update();
            Heap.Collect();
            //LastCursorX = Sys.MouseManager.X; LastCursorY = Sys.MouseManager.Y;
        }

        private static Color DimWhite = new Color(50, 50, 50);

        public static void DrawWindow(Canvas cv, int X, int Y, string Title, bool Closeable)
        {
            //Canvas.DrawRectangle(X - 1, Y - 1, Convert.ToUInt16(cv.Width + 2), Convert.ToUInt16(cv.Height + 2 + 18), 3,
            //    DimWhite);
            //Canvas.DrawLine(X - 1, Y - 1 + 18, X - 1 + Convert.ToUInt16(cv.Width + 2), Y - 1 + 18, DimWhite); // No stupid outlines
            Canvas.DrawFilledRectangle(X, Y, Convert.ToUInt16(cv.Width + 1), 16, 0, Color.DeepGray);
            Canvas.DrawString(X + 1, Y, Title, BetterConsole.font, Color.White);
            if (Closeable) Canvas.DrawImage(X + cv.Width - 14, Y + 2, closeButton);
            Canvas.DrawImage(X, Y + 18, cv, false);
        }
    }
}