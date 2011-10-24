
using System;

using SysPointF = System.Drawing.PointF;
using SysRectanleF = System.Drawing.RectangleF;

using SysPoint = System.Drawing.Point;
using SysRectangle = System.Drawing.Rectangle;

using System.Windows.Forms;

namespace Frogger
{
    
    public class Utils
    {
        
        public readonly static Random rand=new Random();

        private readonly static System.Globalization.CultureInfo cultureinfo = new System.Globalization.CultureInfo("");

        /// <summary>
        /// parsuje stringa na floata
        /// bo float.parse nie chce kropek tylko przecinki
        /// </summary>
        /// <param name="s">string w formacie 1.0 lub 1,0</param>
        /// <returns>wartość s we float</returns>
        public static float ParseFloat(string s)
        {
            return float.Parse(s, System.Globalization.NumberStyles.Float, cultureinfo);
        }

    }
    
    public enum Direction{Right, Down, Left , Up};
    
    public class Point
    {

        public SysPointF point;
        
        public float x
        {
            get {return point.X;}
            set {point.X = value;}
        }
        
        public float y
        {
            get {return point.Y;}
            set {point.Y = value;}
        }
        
        /// <summary>
        /// tworzy nowy zerowy punkt
        /// </summary>
        public Point() : this(0.0f,0.0f)
        {
        }
        
        /// <summary>
        /// tworzy wektor wskazujący dany kierunek
        /// o długości 1.0f
        /// </summary>
        /// <param name="dir"></param>
        public Point(Direction dir) : this()
        {
            
            switch(dir)
            {
                case Direction.Right:
                    x=1.0f;
                    break;
                case Direction.Down:
                    y=1.0f;
                    break;
                case Direction.Left:
                    x=-1.0f;
                    break;
                case Direction.Up:
                    y=-1.0f;
                    break;
            }
        }
        
        public Point(SysPointF point)
        {
            this.point=point;
        }

        public Point(float x,float y) : this(new SysPointF(x,y))
        {
        }
        
        public static Point operator+(Point a, Point b)
        {
            return new Point(a.x+b.x,a.y+b.y);
        }
        
        public static Point operator-(Point a, Point b)
        {
            return new Point(a.x-b.x,a.y-b.y);
        }
        
        public static Point operator*(Point p, float v)
        {
            return new Point(p.x*v,p.y*v);
        }
        
        public static Point operator%(Point p, float v)
        {
            return new Point(p.x % v, p.y % v);
        }

        /// <summary>
        /// zaokrągla punkt do 1.0
        /// </summary>
        /// <returns></returns>
        public Point Round()
        {
            return new Point((float)Math.Round((double)this.x), (float)Math.Round((double)this.y));
        }

        /// <summary>
        /// zwraca długość wektora
        /// </summary>
        /// <returns></returns>
        public float Length()
        {
            return (float)Math.Sqrt(x*x+y*y);
        }
        
        /// <summary>
        /// przekształca punkt na System.Drawing.Point
        /// </summary>
        /// <param name="scale">skala</param>
        /// <returns>System.Drawing.Point przemnożony przez scale i zaokrąglony</returns>
        public SysPoint toSysPoint(int scale)
        {
            return new SysPoint((int)(x*scale),(int)(y*scale));
        }

        public float DistanceTo(Point p)
        {
            return (this - p).Length();
        }


        /// <summary>
        /// przyciąga p do siatki o rozmiarze factor
        /// i początku w punkcie this
        /// </summary>
        /// <param name="p">przyciągany punkt</param>
        /// <param name="factor">rozmiar siatki</param>
        /// <returns>przyciągnięty punkt</returns>
        public Point Align(Point p, float factor)
        {

            Point half = new Point(0.5f*factor, 0.5f*factor);

            return p + (this - p - half) % factor + half; 

        }

        public override string ToString()
        {
            return "("+x+", "+y+")";
        }
    }
    
    public class Rectangle
    {
        public Point loc,size;
        
        public float x
        {
            get {return loc.point.X;}
            set {loc.point.X = value;}
        }
        
        public float y
        {
            get {return loc.point.Y;}
            set {loc.point.Y = value;}
        }
        
        public float w
        {
            get {return size.point.X;}
            set {size.point.X = value;}
        }
        
        public float h
        {
            get {return size.point.Y;}
            set {size.point.Y = value;}
        }
        
        /// <summary>
        /// tworzy nowy zerowy prostokąt
        /// </summary>
        public Rectangle() : this(0.0f,0.0f,0.0f,0.0f)
        {
        }
        
        public Rectangle(float x, float y, float w, float h)
        {
            
            this.loc=new Point(x,y);
            this.size=new Point(w,h);

        }
        
        public static Rectangle operator+(Rectangle r, Point p)
        {
            return new Rectangle(r.x+p.x,r.y+p.y,r.w,r.h);
        }

        /// <summary>
        /// oblicza iloczyn prostokątów
        /// jeżeli są rozłączne zwraca null
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public Rectangle Intersect(Rectangle r)
        {
            
            Rectangle i=new Rectangle();
            
            if(x<r.x)
            {
                i.x=x+w;
                i.w=i.x-r.x;
                
            }
            else
            {
                i.x=r.x+r.w;
                i.w=i.x-x;
            }
            
            if(y<r.y)
            {
                i.y=y+h;
                i.h=i.y-r.y;
                
            }
            else
            {
                i.y=r.y+r.h;
                i.h=i.y-y;
            }
            
            if(i.w <= 0.0f || i.h<=0.0f)
                return null;
            else
                return i;
        }

        /// <summary>
        /// zwraca pole powierzchni r
        /// jeżeli r jest nullem zraca 0.0f
        /// </summary>
        /// <param name="r">prostokąt</param>
        /// <returns>pole powierzchni r</returns>
        public static float Area(Rectangle r)
        {
            if (r == null)
                return 0.0f;
            else
                return r.Area();
        }

        /// <summary>
        /// zwraca pole powierzchni
        /// uwaga - intersect zwraca czasami null
        /// aby policzyć bezpiecznie takie pole trzeba użyć statycznej metody Area(Rectangle)
        /// </summary>
        /// <returns></returns>
        public float Area()
        {
            return w*h;
        }

        /// <summary>
        /// przekształca prostokąt na System.Drawing.Rectangle
        /// </summary>
        /// <param name="scale">skala przez którą zostanie przemnożony prostokąt przed zaokrągleniem</param>
        /// <returns></returns>
        public SysRectangle toSysRectangle(int scale)
        {
            return new SysRectangle(
                               (int)(x*scale),
                               (int)(y*scale),
                               (int)(w*scale),
                               (int)(h*scale)
                               );
        }

        public override string ToString()
        {
            return loc + "-" + size;
        }

    }

}
