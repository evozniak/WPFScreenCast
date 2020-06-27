using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace WPFSceenCast
{
    [Serializable]
    public class PixelModificado
    {

        public int corAlpha { get; set; }
        public int corRed { get; set; }
        public int corGreen { get; set; }
        public int corBlue { get; set; }
        //public Color Cor
        //{
        //    get { return Color.FromArgb(corAlpha, corRed, corGreen, corBlue); }
        //    set
        //    {
        //        corAlpha = value.A;
        //        corRed = value.R;
        //        corGreen = value.G;
        //        corBlue = value.B;
        //    }
        //}
        public int X { get; set; }
        public int Y { get; set; }

    }
}
