
using System;
using System.Collections.Generic;

namespace Frogger
{

    /// <summary>
    /// poziom na którym znajduje się dany obiekt
    /// ma to znaczenie przy rysowaniu i kolizjach
    /// </summary>
    public enum Level {Min,Background,Floor,Walking,Air,Max,Invisible};

    public enum BoardState {Active, Won, Lost, InnerLevel};

    public class FroggerBoard
    {
        
        public List<FroggerObject> objects;
        List<FroggerObject> objects_to_remove;
        
        FroggerAnimation circle;
        Point circle_pos;

        public BoardState State;
        public FroggerBoard inner_board;

        public Rectangle size = new Rectangle(0.0f, 0.0f, 20f, 15f);

        public FroggerBoard()
        {

            circle=new FroggerAnimation("circle");
            circle_pos=new Point(-1.0f,-1.0f);
            

            objects=new List<FroggerObject>();
            objects_to_remove=new List<FroggerObject>();

            
        }

        public void Add(FroggerObject fo)
        {
            objects.Add(fo);
        }
        
        /// <summary>
        /// dodaje obiekt do kolejki obiektów do usunięcia
        /// obiekt zostanie usunięty przed następnym Tick()
        /// </summary>
        /// <param name="fo"></param>
        public void Remove(FroggerObject fo)
        {
            objects_to_remove.Add(fo);
        }

        public void PushBoard(FroggerBoard b)
        {
            State = BoardState.InnerLevel;
            inner_board = b;
        }

        /// <summary>
        /// zwraca jeden obiekt typu T znajdujący się na planszy
        /// </summary>
        /// <typeparam name="T">typ należący do FroggerObject</typeparam>
        /// <returns>obiekt typu T</returns>
        public T Get<T>() where T : FroggerObject
        {
            foreach (FroggerObject fo in objects)
                if (fo is T)
                    return fo as T;

            return null;

        }

        public void Draw(Graphics g)
        {
            
            circle.Draw(g,circle_pos);
            
            for(int i=(int)(Level.Min)+1;i<(int)(Level.Max);i++)
                foreach(FroggerObject fo in objects)
                    if((int)fo.level==i)
                        fo.Draw(g);
        }
        
        public void Tick(float seconds)
        {
            
            if(Utils.rand.Next(100)==0 && circle.frame==6)
            {
                circle_pos.x=(float)Utils.rand.NextDouble()*20.0f;
                circle_pos.y=(float)Utils.rand.NextDouble()*14.0f;
                
                circle.Seek(0);
            }
            
            circle.Tick(seconds);
            
            foreach(FroggerObject fo0 in objects)
                foreach(FroggerObject fo1 in objects)
                    if(fo0!=fo1)
                    {
                    
                        Rectangle intersection=fo0.Intersection(fo1);
                        if(intersection!=null)
                            fo0.Collide(fo1,intersection);
                    }
            
            foreach(FroggerObject fo in objects)
                fo.Tick(seconds);
            
            foreach(FroggerObject fo in objects_to_remove)
                objects.Remove(fo);
            
            objects_to_remove=new List<FroggerObject>();
                
            
        }
        
        public void Win()
        {
            foreach(FroggerObject fo in objects)
                if(fo is LittleFrog)
                    return;

            State = BoardState.Won;
        }
        
        public void Lost()
        {
            State = BoardState.Lost;
        }
        
        public override string ToString()
        {

            string s = "";

            foreach( FroggerObject fo in objects)
            {
                s += fo + "\r\n";
            }

            return s;

        }

        public FroggerObject GetAt(Point p)
        {
            foreach (FroggerObject fo in objects)
            {
                if (fo.rect.loc.DistanceTo(p) < 0.1f)
                    return fo;
            }

            return null;

        }

    }
    
    public abstract class FroggerObject
    {
        
        protected FroggerBoard board;
        
        //TODO na private
        public Rectangle rect;

        //TODO na private
        public Point delta;

        protected Point speed;
        protected float distance;
        
        /// <summary>
        /// poziom na którym znajduje się obiekt
        /// </summary>
        public Level level;

        /// <summary>
        /// pozycja obiektu na planszy
        /// </summary>
        public Point Location
        {
            get
            {
                return this.rect.loc;
            }

            set
            {
                this.rect.loc = value;
            }
        }

        public FroggerObject(FroggerBoard board, Rectangle rect)
        {
            
            this.board=board;

            this.rect=rect;
            this.delta=new Point();
    
            this.speed=new Point();
            this.distance=0.0f;
            
        }
        
        
        public virtual void Draw(Graphics g)
        {
        }
        
        public void MoveBy(Point delta)
        {

            this.delta+=delta;

        }

        public void Stop()
        {
            this.distance = 0.0f;
        }

        /// <summary>
        /// zacznij ruch w danym kierunku o podaną odległość
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="distance"></param>
        public void Go(Point speed, float distance)
        {

            //TODO prawdopodobnie powinno się wektorem określać przesunięcie a prędkość floatem

            this.speed=speed;
            this.distance=distance;
                
        }
        
        public virtual void Tick(float seconds)
        {

                    
            rect+=delta;
            
            this.delta.x=this.delta.y=0.0f;
            
            if(this.distance>0.0f)
            {

                Point distance=speed*seconds;
                
                float length=distance.Length();
                
                if(length>this.distance)
                {
                    distance*=this.distance/length;
                    this.distance=0.0f;
                }
                else
                {
                    this.distance-=length;
                }

                delta+=distance;
                
            }

        }
        
        public Rectangle Intersection(FroggerObject fo)
        {
            return this.rect.Intersect(fo.rect);
        }
        
        public virtual void Collide(FroggerObject fo, Rectangle intersection)
        {
        }
        
        public virtual void Remove()
        {
            
            this.board.Remove(this);
            
        }


        public override string ToString()
        {

            return this.GetType().Name.ToLower() + " " + this.rect.loc;

        }

    }
        
}
