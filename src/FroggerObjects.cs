
using System;

using PointF = System.Drawing.PointF;
using RectangleF = System.Drawing.RectangleF;


namespace Frogger
{

    /// <summary>
    /// mała żabka którą trzeba zebrać żeby ukończyć poziom
    /// </summary>
    public class LittleFrog : FroggerObject
    {
        
        FroggerAnimation animation;
        
        public LittleFrog(FroggerBoard board,Point p) : base(board,new Rectangle(p.x,p.y,1.0f,1.0f))
        {
            
            level=Level.Walking;
            animation=new FroggerAnimation("little_frog");
            
        }
        
        public override void Draw (Graphics g)
        {
            animation.Draw(g,rect.loc);
        }
        
        public override void Collide (FroggerObject fo, Rectangle intersection)
        {
            if( fo is Frog && intersection.Area()/rect.Area() > 0.75)
            {
                this.Remove();
            }
        }
        
        public override void Tick (float seconds)
        {
            base.Tick (seconds);
            
            this.animation.Tick(seconds);
        }

    }
    
    /// <summary>
    /// mócha która po zebraniu dodaje życie
    /// </summary>
    public class Fly : FroggerObject
    {
        
        FroggerAnimation animation;
        
        public Fly(FroggerBoard board,Point p) : base(board,new Rectangle(p.x,p.y,1.0f,1.0f))
        {
            
            level=Level.Air;
            animation=new FroggerAnimation("fly");
            
            //this.Go(new Point(1.0f,0.0f),float.PositiveInfinity);
            
        }
        
        public override void Draw (Graphics g)
        {
            animation.Draw(g,rect.loc);
        }
        
        public override void Collide (FroggerObject fo, Rectangle intersection)
        {
            
            
            if( fo is Frog && intersection.Area()/rect.Area() > 0.75)
            {
                (fo as Frog).Lives++;

                this.Remove();
            }
        }
        
        public override void Tick (float seconds)
        {
            base.Tick (seconds);
            
            this.animation.Tick(seconds);
        }

    }
    
    /// <summary>
    /// kamyk
    /// </summary>
    public class Rock : FroggerObject
    {
        
        FroggerAnimation animation;
        
        public Rock(FroggerBoard board,Point p) : base(board,new Rectangle(p.x,p.y,1.0f,1.0f))
        {
            level=Level.Floor;
            animation=new FroggerAnimation("rock");
            animation.Seek(Utils.rand.Next(3));
            
            //this.MoveBy(new Point((float)Utils.rand.NextDouble()*0.1f-0.05f,(float)Utils.rand.NextDouble()*0.1f-0.05f));
            
        }
        
        public override void Draw (Graphics g)
        {
            animation.Draw(g,rect.loc);
        }
        
    }

    /// <summary>
    /// abstrakcyjna klasa implementująca fizykę poruszającego się
    /// obiektu na którym można stanąć
    /// </summary>
    public abstract class MovingFloor : FroggerObject
    {
        /// <summary>
        /// czy obiekt już się odbił
        /// </summary>
        bool bounce = false;

        /// <summary>
        /// czy występuje kolizja
        /// </summary>
        bool colision = false;

        public MovingFloor(FroggerBoard board, Rectangle r) : base(board, r)
        {
            this.level = Level.Floor;
        }

        public override void Tick(float seconds)
        {

            //jest kolizjia i jeszcze sie nie odbiliśmy
            if (colision && !bounce)
            {
                bounce = true;
                this.speed *= -1.0f;
            }

            //nie ma kolizji - resetujemy odbicie
            if (!colision)
                bounce = false;

            //zresetuj stan kolizji
            colision = false;
            

            //wyjazd za ekran
            if (this.rect.x > board.size.w)
                this.rect.x -= board.size.w + this.rect.w;

            if (this.rect.y > board.size.h)
                this.rect.y -= board.size.h + this.rect.h;

            if (this.rect.x + this.rect.w < board.size.x)
                this.rect.x += board.size.w + this.rect.w;

            if (this.rect.y + this.rect.h < board.size.y)
                this.rect.y += board.size.h + this.rect.h;
            

            base.Tick(seconds);

        }

        public override void Collide(FroggerObject fo, Rectangle intersection)
        {

            float common_area = intersection.Area() / fo.rect.Area();

            if (fo.level == Level.Walking && common_area > 0.75)
            {
                fo.MoveBy(this.delta);
            }
            else if (fo.level == Level.Floor && common_area > 0.1)
            {
                //w coś uderzyliśmy
                this.colision=true;
            }
        }

        public override string ToString()
        {
            return base.ToString() + " " + speed;
        }

    }

    /// <summary>
    /// ruchomy liść
    /// </summary>
    public class Leaf : MovingFloor
    {
        
        FroggerAnimation animation;

        /// <summary>
        /// czy zatopić liśc po zejściu żabki
        /// </summary>
        bool sinking = false;
        
        bool has_frog = false;

        bool had_frog = false;

        public Leaf(FroggerBoard board,Point p) : base(board,new Rectangle(p.x,p.y,1.0f,1.0f))
        {
            animation=new FroggerAnimation("leaf");
            
        }
        
        public override void Draw (Graphics g)
        {
            animation.Draw(g,rect.loc);
        }

        public override void Tick(float seconds)
        {
            animation.Tick(seconds);

            if (had_frog && !has_frog)
            {
                if (sinking)
                {

                    if (animation.IsStopped())
                        this.Remove();

                }
                else
                {
                    sinking = true;
                    animation.Seek(1);
                }
            }

            has_frog = false;


            base.Tick(seconds);
        }

        public override void Collide(FroggerObject fo, Rectangle intersection)
        {

            if (fo is Frog && intersection.Area() > 0.50f)
            {
                had_frog = true;
                has_frog = true;
            }

            base.Collide(fo, intersection);

        }

    }

    /// <summary>
    /// końcowy liść do którego trzeba dotrzeć
    /// </summary>
    public class Home : FroggerObject
    {
        
        FroggerAnimation animation;
        
        public Home(FroggerBoard board,Point p) : base(board,new Rectangle(p.x,p.y,2.0f,2.0f))
        {
            level=Level.Floor;
            animation=new FroggerAnimation("home");
        }
        
        public override void Draw (Graphics g)
        {
            animation.Draw(g,rect.loc);
        }
        
        public override void Collide (FroggerObject fo, Rectangle intersection)
        {
            if(fo is Frog && intersection.Area()/fo.rect.Area() > 0.75)
            {
                this.board.Win();
            }
        }
        
    }
    
    /// <summary>
    /// poruszająca się kłoda po której można chodzić
    /// </summary>
    public class Log : MovingFloor
    {
        
        FroggerAnimation animation;
        
        public Log(FroggerBoard board,Point p) : base(board,new Rectangle(p.x,p.y,3.0f,1.0f))
        {
            animation=new FroggerAnimation("log");
        }
        
        public override void Draw (Graphics g)
        {
            animation.Draw(g,rect.loc);
        }
        
    }
        
    /// <summary>
    /// żabka sterowana przez gracza
    /// </summary>
    public class Frog : FroggerObject
    {

        /// <summary>
        /// czy żabka się porusza
        /// </summary>
        bool is_moving=false;

        /// <summary>
        /// czy żabka tonie
        /// </summary>
        bool is_sinking = false;

        /// <summary>
        /// od jakiego czasu żabka nic nie robi
        /// </summary>
        float idle_time = 0.0f;

        /// <summary>
        /// powierzchnia ziemi na której stoi żabka
        /// jeżeli będzie zbyt niska to się utopi
        /// </summary>
        float floor_area = 0.0f;


        /// <summary>
        /// kierunek w który obrócona jest żabka
        /// </summary>
        Direction direction;
        
        /// <summary>
        /// animacja żabki
        /// </summary>
        FroggerAnimation animation;

        /// <summary>
        /// miejsce do którego żaba zostanie przeniesiona po utracie życia
        /// </summary>
        Point starting_location;

        /// <summary>
        /// ilość pozostałych żyć
        /// </summary>
        public int Lives = 0;

        public Frog(FroggerBoard board, Point p) : base(board,new Rectangle(p.x,p.y,1.0f,1.0f))
        {
            starting_location = p;
            level=Level.Walking;
            animation=new FroggerAnimation("frog");
            
        }

        /// <summary>
        /// wykonaj animację żeby żabka się nie nudziła
        /// </summary>
        public void DoIdleAnimation()
        {
            if (animation.IsStopped())
            {
                animation.Seek(8 + (int)direction * 4);
            }
        }

        /// <summary>
        /// każ żabce skoczyć
        /// </summary>
        /// <param name="dir">kierunek w którm żabka skoczy</param>
        public void Go(Direction dir)
        {

            idle_time = 0.0f;

            if(is_moving) // juz wykonujemy ruch
                return;

            is_moving=true;

            this.direction = dir;

            //znajdujemy obiekt który najbardziej
            //pokrywa się z przewidywaną pozycją żabki

            FroggerObject best_floor=null;
            float max_floor_area=float.NegativeInfinity;

            Point new_loc = new Point(dir) + Location;

            foreach(FroggerObject fo in board.objects)
            {
                if (fo.level == Level.Floor)
                {
                    float q = Rectangle.Area(fo.rect.Intersect(this.rect + new Point(dir)));

                    if (q > max_floor_area)
                    {
                        max_floor_area = q;
                        best_floor = fo;
                    }
                }

            }

            if (max_floor_area < 0.75f)
            {

                //prawdopodobnie żabka się utopi

            }
            else
            {
                //skaczemy do najbliższych współrzędnych odległych o całkowite
                //wielkości od best_floor

                new_loc = best_floor.Location.Align(new_loc, 1.0f);

            }

            Point speed = new_loc - Location;
            Go(speed * 10.0f, speed.Length());


            animation.Seek((int)dir * 2);

        }
        

        /// <summary>
        /// rysuj żabkę
        /// </summary>
        /// <param name="g"></param>
        public override void Draw (Graphics g)
        {
            animation.Draw(g,rect.loc);
        }
    
        public override void Tick (float seconds)
        {

            idle_time += seconds;

            if (idle_time >= 2.0f && Utils.rand.NextDouble() > 0.995)
                this.DoIdleAnimation();

            is_sinking = floor_area < Config.FrogFloorArea;

            if(!is_moving && is_sinking)
            {

                if (Lives-- == 0)
                    this.board.Lost();
                else
                    this.rect.loc = starting_location;
            }
            
            animation.Tick(seconds);
            
            base.Tick(seconds);
            is_moving=this.distance>0.0f;
            is_sinking=true;

            //resetuj wartości obliczane w kolizjach

            floor_area = 0.0f;

        }
    
        public override void Collide (FroggerObject fo, Rectangle intersection)
        {

            float intersection_area=intersection.Area()/rect.Area();

            if (fo.level==Level.Floor && !is_moving && intersection.Area() / rect.Area() > 0.50)
            {
                this.Location = fo.Location.Align(this.Location, 1.0f);
                is_moving = true;
            }

            if(fo.level==Level.Floor &&  intersection_area > 0.10)
            {
                this.floor_area+=intersection_area;
            }

        }
        
    }

    /// <summary>
    /// mostek
    /// </summary>
    public class Bridge : FroggerObject
    {

        bool is_locked = true;
        bool has_frog = false;
        bool just_played = false;

        FroggerBoard inner_level=null;

        FroggerAnimation animation;

        public string board_name;



        public Bridge(FroggerBoard board, Point p, string board_name) : base(board,new Rectangle(p.x,p.y,1.0f,3.0f))
        {
            level=Level.Floor;
            animation=new FroggerAnimation("bridge");
            this.board_name = board_name;
        }

        public override void Tick(float seconds)
        {

            //jeżeli właśnie wróciliśmy z innego levelu to sprawdzamy jego stan
            if (inner_level != null)
            {
                if (inner_level.State==BoardState.Won)
                {
                    is_locked = false;
                    animation.Seek(1);
                }

                inner_level = null;

            }

            animation.Tick(seconds);

            if (is_locked && has_frog && !just_played)
            {

                just_played = true;

                inner_level = BoardLoader.LoadBoard(board_name+".conf");

                board.PushBoard(inner_level);

            }

            if (!has_frog)
                just_played = false;

            has_frog = false;
        }

        public override void Draw(Graphics g)
        {
            animation.Draw(g, rect.loc);
        }

        public override void Collide(FroggerObject fo, Rectangle intersection)
        {
            if (fo is Frog )
            {
                if (intersection.Area() > 0.75f)
                    has_frog=true;

                if (is_locked && intersection.Area() > 0.25f)
                {

                    Rectangle locked = new Rectangle(0.0f, 1.0f, 1.0f, 1.0f);
                    locked += rect.loc;

                    if(Rectangle.Area(locked.Intersect(fo.rect))<Rectangle.Area(locked.Intersect(fo.rect + fo.delta)))
                    {
                        fo.delta *= 0.0f;
                        fo.Stop();
                    }

                }

            }
        }

        public override string ToString()
        {
            return base.ToString() + " " + board_name;
        }

    }

}
