using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LookAndPlayForm
{    
    public struct PointD
    {
        public double X;
        public double Y;

        public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }

        public PointD(PointD punto)
        {
            X = punto.X;
            Y = punto.Y;
        }

        public Point ToPoint()
        {
            return new Point((int)X, (int)Y);
        }

        public override bool Equals(object obj)
        {
            return obj is PointD && this == (PointD)obj;
        }
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }
        public static bool operator ==(PointD a, PointD b)
        {
            return a.X == b.X && a.Y == b.Y;
        }
        public static bool operator !=(PointD a, PointD b)
        {
            return !(a == b);
        }

        public static double distance(PointD a, PointD b)
        {
            double xCuadrado = (a.X - b.X) * (a.X - b.X);
            double yCuadrado = (a.Y - b.Y) * (a.Y - b.Y);
            double distancia = Math.Sqrt(xCuadrado + yCuadrado);
            return distancia;
        }

        public bool IsNaN()
        {
            if(double.IsNaN(X) && double.IsNaN(Y))
                return true;
            else
                return false;
        }
    }

    public struct PointI
    {
        public int X;
        public int Y;

        public PointI(int x, int y)
        {
            X = x;
            Y = y;
        }

        public PointI(PointI punto)
        {
            X = punto.X;
            Y = punto.Y;
        }
    }

    public enum clictype
    {
        dwell, mouse, meterse
    };

    public enum filtertype
    {
        meanMedian, average
    };

    public enum pointercontroltype
    {
        mouse, eyetracker, headtracker
    }

    public enum eyetrackertype
    {
        tobii, eyetribe
    }
}
