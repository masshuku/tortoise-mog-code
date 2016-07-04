﻿/*
 * Copyright 2012 Matthew Cash. All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, are
 * permitted provided that the following conditions are met:
 * 
 *    1. Redistributions of source code must retain the above copyright notice, this list of
 *       conditions and the following disclaimer.
 * 
 *    2. Redistributions in binary form must reproduce the above copyright notice, this list
 *       of conditions and the following disclaimer in the documentation and/or other materials
 *       provided with the distribution.
 * 
 * THIS SOFTWARE IS PROVIDED BY Matthew Cash ``AS IS'' AND ANY EXPRESS OR IMPLIED
 * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
 * FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL Matthew Cash OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
 * ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 * The views and conclusions contained in the software and documentation are those of the
 * authors and should not be interpreted as representing official policies, either expressed
 * or implied, of Matthew Cash.
 */
using System;
using System.Text;
//using System.Drawing;

using Color = System.Drawing.Color;
using Tortoise.Shared.Drawing;

using Tortoise.Shared;
using Tortoise.Graphics.Input;
using System.Collections;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using XColor = Microsoft.Xna.Framework.Color;
//using MonoGame.Extended.BitmapFonts;

namespace Tortoise.Graphics.Rendering.GUI
{
    /// <summary>
    /// Description of TextBox.
    /// </summary>
    public class TextBox : Control
    {
        private struct KeysLastPressed
        {
            public Keys Key { get; set; }
            //public int TimeSinceLast;
        }

        private string _text = "";
        private string _visibleText = "";
        private Size _textSize;
        private int _markerPosition = 0;
        private FontManager _fontInfo;
        //private GorgonText _gorgonText;
        private char _passwordChar = '*';
        private bool _usePasswordChar = false;
        private int _cursorPosition;
        private Timer _flasherTimer;
        //private Timing.StopWatch _repeaterTimer;
        private bool _showMarker;

        private Color _textColor;

        protected bool _allowNewLines = false;

        public event System.EventHandler<KeyPressed> CancelCharacterInput;


        public bool UsePasswordCharacter
        {
            get { return _usePasswordChar; }
            set
            {
                _threadSafety.EnforceThreadSafety();
                _usePasswordChar = value;
            }
        }

        public char PasswordCharacter
        {
            get { return _passwordChar; }
            set
            {
                _threadSafety.EnforceThreadSafety();
                _passwordChar = value;
            }
        }

        public int CursorPosition
        {
            get { return _cursorPosition; }
            set
            {
                _threadSafety.EnforceThreadSafety();
                if (value < 0 || value > Text.Length)
                    throw new ArgumentException("CursorPosition must be between 0 and Text.Length");
                _cursorPosition = value;
            }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                _threadSafety.EnforceThreadSafety();
                if (_text.Length > value.Length)
                    _cursorPosition = value.Length;

                _text = value;

                //_textChanged = true;
                _redrawPreRenderd = true;
            }

        }

        public Color TextColor
        {
            get { return _textColor; }
            set
            {
                _threadSafety.EnforceThreadSafety();
                _textColor = value;
                //_textChanged = true;
                _redrawPreRenderd = true;
            }

        }

        public TextBox(TGraphics graphics, string name, int x, int y, int width, int height)
            : this(graphics, name, new Rectangle(x, y, width, height))
        {

        }
        public TextBox(TGraphics graphics, string name, Point location, Size size)
            : this(graphics, name, new Rectangle(location, size))
        {

        }
        public TextBox(TGraphics graphics, string name, Rectangle area)
            : base(graphics, name, area)
		{
            
            //_gorgonText = _graphics.Renderer2D.Renderables.CreateText(name + "_TextRenderer");
            _fontInfo = FontManager.GetInstance(graphics, 22, FontTypes.Sans);
            _flasherTimer = new Timer();
			
		    FocusChanged += delegate
            {
                _redrawPreRenderd = true;
            };
            _textColor = Color.Black;
		}

        internal override void Tick(TickEventArgs e)
        {
            _visibleText = Text;
            if (_usePasswordChar)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < _visibleText.Length; i++)
                    sb.Append(_passwordChar);

                _visibleText = sb.ToString();
            }

            PointF stringSize = FontManager.MeasureString(_fontInfo, _visibleText);
            FontManager.DrawString(_fontInfo, _text, new Point(0, 0), TextColor);

            _textSize.Width = (int)stringSize.X;
            _textSize.Height = (int)stringSize.Y;

            /*
            if (_visibleText == "")
                _textSize = _fontInfo.Font.SizeText(" ");
            else
                _textSize = _fontInfo.Font.SizeText(_visibleText);

            //_textSize.Width += (int)(_visibleText.Length * 1.5);

            */
            if (HasFocus)
            {
                if (_flasherTimer.ElapsedMilliseconds > 500)
                {
                    _flasherTimer.Reset();
                    _showMarker = !_showMarker;
                    _redrawPreRenderd = true;
                }

                stringSize = FontManager.MeasureString(_fontInfo, _visibleText.Substring(0, _cursorPosition));
                _markerPosition = (int)stringSize.X;

                
            }
            

            /*
             * This is code pulled from a random project i have. Its not goanna get used, but its here
             * as a reference when i put in some form of selection code.
             * 
            //Rectangle rSize = TransferInfo.FontDraw.MeasureString(null, tText, DrawTextFormat.None,Color.White);
            VisualPart = tText;
            Sub_cursorPosition = SelPos;

			
            if (rSize.Width >= tWidth)
            //start choppin it up
            {

                while (TransferInfo.FontDraw.MeasureString(null, VisualPart.Substring(Sub_cursorPosition), DrawTextFormat.None, Color.White).Width > tWidth)
                {
                    VisualPart = VisualPart.Remove(0, 1);
                    Sub_cursorPosition--;
                }




            }*/
            base.Tick(e);
        }

        internal override bool OnMouseUp(MouseEventArgs e)
        {
            _threadSafety.EnforceThreadSafety();
            if (IsPointOver(e.MouseData.Position))
            {
                this.HasFocus = true;
                doMouseUp(e);
                return true;
            }
            return false;
        }

        internal override bool OnKeyboardDown(KeyEventArgs e)
        {
            if (!HasFocus) return false;
            return doKeyboardDown(e);
        }

        internal override bool OnKeyboardUp(KeyEventArgs e)
        {
            if (!HasFocus) return false;
            //we base all work on the up part.
            
            HandleKey(e.EventData.PressedKeys);
            _flasherTimer.Reset();
            _showMarker = true;

            doKeyboardUp(e);
            return true;
        }

        internal override bool OnKeyboardPress(KeyEventArgs e)
        {
            if (!HasFocus) return false;
            HandleKeyPress(e.EventData.PressedKeys);
            _flasherTimer.Reset();
            _showMarker = true;

            doKeyboardPress(e);
            return true;
        }

        private void HandleKeyPress(IEnumerable<Keys> keyData)
        {

            foreach (Keys key in keyData)
            {
                //TODO: Messy, make more readable.


                //Check for any KeyWasPressed events
                //Then check if Cancel was set to true
                //Useful for sub-classes with special input checkers
                if (CancelCharacterInput != null)
                {
                    KeyPressed kp = new KeyPressed();
                    kp.Key = key;
                    kp.Cancel = false;
                    CancelCharacterInput(this, kp);
                    if (kp.Cancel)
                        return;
                }

                StringBuilder sb = new StringBuilder();
                sb.Append(Text.Substring(0, _cursorPosition));
                sb.Append(key);
                sb.Append(Text.Substring(_cursorPosition));
                _cursorPosition += 1;
                Text = sb.ToString();
            }
        
        }

        private void HandleKey(IEnumerable<Keys> keyData)
        {
            foreach (Keys key in keyData)
                HandleKey(key);
        }

        private void HandleKey(Keys key)
        {
            switch (key)
            {
                case (Keys.Back):
                    if (_cursorPosition > 0)
                    {
                        Text = Text.Remove(_cursorPosition - 1, 1);
                        //_cursorPosition -= 1;
                    }
                    break;
                case (Keys.Delete):

                    if (_cursorPosition < Text.Length)
                    {
                        Text = Text.Remove(_cursorPosition, 1);
                    }
                    break;
                case (Keys.End):
                    _cursorPosition = Text.Length;
                    break;
                case (Keys.Home):
                    _cursorPosition = 0;
                    break;
                case (Keys.Left):
                    if (_cursorPosition > 0)
                        _cursorPosition -= 1;
                    break;
                case (Keys.Right):
                    if (_cursorPosition < Text.Length)
                        _cursorPosition += 1;
                    break;

                
            }
        }

        internal void Render(Surface target)
        {
            if (_visible)
                base.Render();

        }

        protected new void Redraw_PreRenderd()
        {
            _preRenderdSurface.BeginChanges();
            _preRenderdSurface.Fill(_backgroundColor);


            //_graphics.SpriteBatch.DrawString(_fontInfo.Bitmap, _text, new Vector2(0, 0), XColor.Black);
            FontManager.DrawString(_fontInfo, _text, new Point(0, 0), TextColor);


            if (_showMarker && HasFocus)
            {
                Point drawPos = new Point();
                drawPos.X= _markerPosition;
                //_graphics.SpriteBatch.DrawString(_fontInfo.Bitmap, "|", drawPos, XColor.Black);
                FontManager.DrawString(_fontInfo, "|", drawPos, TextColor);
            }


            _preRenderdSurface.EndChanges();
            /*
            Point[] linePoints = new Point[5];
            linePoints[0] = new Point(0, 0);
            linePoints[1] = new Point(0, Height - 2);
            linePoints[2] = new Point(Width - 2, Height - 2);
            linePoints[3] = new Point(Width - 2, 0);
            linePoints[4] = new Point(0, 0);

            _preRenderdSurface.Draw(new Polygon(new ArrayList(linePoints)), Color.Gray);

            linePoints[0] = new Point(1, 1);
            linePoints[1] = new Point(1, Height - 1);
            linePoints[2] = new Point(Width - 1, Height - 1);
            linePoints[3] = new Point(Width - 1, 1);
            linePoints[4] = new Point(1, 1);

            _preRenderdSurface.Draw(new Polygon(new ArrayList(linePoints)), Color.Black);
            */

        }

        private Point GetDrawPosition()
        {
            return new Point(4, (Size.Height / 2) - (_textSize.Height / 2));
        }
    }


}