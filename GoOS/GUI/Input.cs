﻿using Cosmos.System;
using PrismAPI.Graphics.Rasterizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrismAPI.Graphics;
using GoOS.GUI.Models;

namespace GoOS.GUI
{
    public class Input : Control
    {
        public Input(Window parent, ushort x, ushort y, ushort width, ushort height, string placeholder)
            : base(parent, x, y, width, height)
        {
            PlaceholderText = placeholder;
        }

        public Action Submitted;
        public Action Changed;

        private const int padding = 3;

        public string Text
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < lines.Count; i++)
                {
                    builder.Append(lines[i]);
                    if (i != lines.Count - 1)
                    {
                        builder.AppendLine();
                    }
                }
                return builder.ToString();
            }
            set
            {
                lines = value.Split('\n').ToList();

                caretLine = -1;
                caretCol = 0;

                Render();
            }
        }

        private string _placeholderText = string.Empty;
        public string PlaceholderText
        {
            get
            {
                return _placeholderText;
            }
            set
            {
                _placeholderText = value;
                Render();
            }
        }

        public bool ReadOnly { get; set; } = false;

        // Todo.
        // public bool MultiLine { get; set; } = false;

        public bool Shield { get; set; } = false;

        private void MoveCaret(int line, int col)
        {
            if (caretLine == line && caretCol == col) return;
            caretLine = Math.Clamp(line, 0, lines.Count - 1);
            caretCol = Math.Clamp(col, 0, lines[caretLine].Length);
            Render();
        }

        private int GetEndXAtCol(int col)
        {
            string here = lines[caretLine].Substring(0, col);
            return BetterConsole.font.MeasureString(here);
        }

        internal override void HandleDown(MouseEventArgs args)
        {
            caretLine = 0;
            // todo fix this algo since it's backwards
            for (int i = 0; i < lines[caretLine].Length; i++)
            {
                string here = lines[caretLine].Substring(0, i);
                int hereWidth = BetterConsole.font.MeasureString(here);
                if (args.X <= hereWidth)
                {
                    MoveCaret(0, i);
                    Render();
                    return;
                }
            }
        }

        internal override void HandleUnfocus()
        {
            caretLine = -1;
            caretCol = 0;

            Render();
        }

        private void AutoScroll()
        {
            // TODO account for padding in here, on both sides
            // also fix it

            if (caretLine == -1) return;

            // if (scrollY + Height < (caretLine + 1) * fontHeight)
            // {
            //     // Scroll up.
            //     scrollY = ((caretLine + 1) * fontHeight) - Height;
            //     MarkAllLines();
            // }
            // if (caretLine * fontHeight < scrollY)
            // {
            //     // Scroll down.
            //     scrollY = caretLine * fontHeight;
            //     MarkAllLines();
            // }

            if (GetEndXAtCol(caretCol) < -scrollX)
            {
                // Scroll right.
                scrollX = GetEndXAtCol(caretCol) - Contents.Width;
            }
            if (GetEndXAtCol(caretCol) < scrollX)
            {
                // Scroll left.
                scrollX = GetEndXAtCol(caretCol);
            }
        }

        internal override void HandleKey(KeyEvent key)
        {
            if (caretLine == -1 || ReadOnly) return;
            switch (key.Key)
            {
                case ConsoleKeyEx.LeftArrow:
                    if (caretCol == 0)
                    {
                        if (caretLine == 0) return;
                        caretLine--;
                        caretCol = lines[caretLine].Length;
                    }
                    else
                    {
                        caretCol--;
                    }
                    break;
                case ConsoleKeyEx.RightArrow:
                    if (caretCol == lines[caretLine].Length)
                    {
                        if (caretLine == lines.Count - 1) return;
                        caretLine++;

                        caretCol = 0;
                    }
                    else
                    {
                        caretCol++;
                    }
                    break;
                case ConsoleKeyEx.UpArrow:
                    if (caretLine == 0) return;

                    caretLine--;
                    caretCol = Math.Min(lines[caretLine].Length, caretCol);
                    break;
                case ConsoleKeyEx.DownArrow:
                    if (caretLine == lines.Count - 1) return;

                    caretLine++;
                    caretCol = Math.Min(lines[caretLine].Length, caretCol);
                    break;
                case ConsoleKeyEx.Enter:
                    // if (!MultiLine)
                    // {
                        Submitted?.Invoke();

                        caretLine = -1;
                        caretCol = 0;
                        
                        break;
                    // }
                    // 
                    // lines.Insert(caretLine + 1, lines[caretLine].Substring(caretCol));
                    // lines[caretLine] = lines[caretLine].Substring(0, caretCol);
                    // 
                    // caretLine++;
                    // caretCol = 0;
                    // 
                    // Changed?.Invoke();
                    // break;
                case ConsoleKeyEx.Backspace:
                    if (caretCol == 0)
                    {
                        if (caretLine == 0) return;

                        caretLine--;
                        caretCol = lines[caretLine].Length;

                        lines[caretLine] += lines[caretLine + 1];
                        lines.RemoveAt(caretLine + 1);

                        Changed?.Invoke();
                    }
                    else
                    {
                        lines[caretLine] = lines[caretLine].Remove(caretCol - 1, 1);
                        caretCol--;

                        Changed?.Invoke();
                    }
                    break;
                default:
                    lines[caretLine] = lines[caretLine].Insert(caretCol, key.KeyChar.ToString());
                    caretCol++;

                    Changed?.Invoke();
                    break;
            }

            Render();
        }

        private List<string> lines = new List<string>() { string.Empty };

        private int caretLine = -1;
        private int caretCol = 0;

        private int scrollX = 0;
        private int scrollY = 0;

        private void DrawBorder()
        {
            // Dark shadow.
            Contents.DrawLine(0, 0, Contents.Width - 1, 0, Color.Black);
            Contents.DrawLine(0, 0, 0, Contents.Height - 1, Color.Black);

            // Highlight.
            Contents.DrawLine(1, Contents.Height - 2, Contents.Width - 2, Contents.Height - 2, new Color(216, 216, 216));
            Contents.DrawLine(Contents.Width - 2, 1, Contents.Width - 2, Contents.Height - 1, new Color(216, 216, 216));

            // Light highlight.
            Contents.DrawLine(0, Contents.Height - 1, Contents.Width, Contents.Height - 1, Color.White);
            Contents.DrawLine(Contents.Width - 1, 0, Contents.Width - 1, Contents.Height - 1, Color.White);
        }

        public override void Render()
        {
            AutoScroll();

            // Background.
            Contents.Clear(Color.White);

            if (Text == string.Empty)
            {
                Contents.DrawRectangle(0, 0, Contents.Width, Contents.Height, 0, Color.DeepGray);
                Contents.DrawString(0, 0, PlaceholderText, BetterConsole.font, Color.LightGray);

                if (caretLine == 0)
                {
                    int caretX = 1;
                    Contents.DrawLine(caretX, padding, caretX, Contents.Height - padding, Color.Black);
                }

                DrawBorder();

                Parent.RenderControls();
                return;
            }

            Contents.DrawString(-scrollX, 0, Shield ? new string('*', lines[0].Length) : lines[0], BetterConsole.font, Color.Black);

            if (caretLine == 0)
            {
                int caretX = GetEndXAtCol(caretCol);
                Contents.DrawLine(caretX, padding, caretX, Contents.Height - padding, Color.Black);
            }

            DrawBorder();

            Parent.RenderControls();
        }
    }
}