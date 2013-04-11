using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace KinderKinect.Utils
{
    class MouseCursor : ICursor
    {
        private Vector2 _position; 
        public Vector2 Position
        {
            get { return _position; }
        }


        public MouseCursor()
        {
            _position = new Vector2();
        }

        public void Update()
        {
           _position.X = Mouse.GetState().X;
           _position.Y = Mouse.GetState().Y;
        }


        public void BeginHoverState(TimeSpan timeToCompletion)
        {
      
        }

        public void BreakHoverState()
        {
           
        }


        public Rectangle rect
        {
            get { throw new NotImplementedException(); }
        }
    }
}
