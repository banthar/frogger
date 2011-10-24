
using System;
using System.Threading;

using System.Collections.Generic;

namespace Frogger
{

    public class Frogger
    {

        public string getGameTitle()
        {
            return "Frogger";
        }

        public void resetGame()
        {
        }

        public void Show()
        {
            Thread t = new Thread(Main);
            t.Name = "Frogger";
            t.IsBackground = false;
            t.Start();
        }

        public static void Main()
        {

            if (Config.IsDebug)
            {
                FroggerScreen fs = new FroggerScreen();
                fs.Start();
            }
            else
            {
                try
                {
                    FroggerScreen fs = new FroggerScreen();
                    fs.Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

        }

    }

    public class FroggerScreen : Screen
    {
        Stack<FroggerBoard> boards;

        int time;
        Frog frog;
        Image help_image,victory_image;

        bool is_help_visible = true;
        bool is_victory_visible = false;

        protected override string GetTitle()
        {
            return "Frogger";
        }
            
        protected override void OnPaint()
        {

            if (is_help_visible)
            {
                this.DrawImage(help_image);
                return;
            }

            if (is_victory_visible)
            {
                this.DrawImage(victory_image);
                return;
            }

            Clear(0x2061af);

            float dt=((float)(System.Environment.TickCount-time))/1000.0f;
            time=System.Environment.TickCount;
            
            boards.Peek().Tick(dt);

            FroggerBoard board = boards.Peek();
            boards.Peek().Draw(this);

            if (board.State != BoardState.Active)
            {


                switch (board.State)
                {
                    case BoardState.InnerLevel:
                        Clear(0x000000);
                        boards.Push(board.inner_board);
                        break;
                    case BoardState.Lost:
                        Clear(0x880000);
                        boards.Pop();
                        break;
                    case BoardState.Won:
                        Clear(0xffffff);
                        boards.Pop();
                        if (boards.Count == 0)
                            is_victory_visible = true;
                        break;
                }

                if (boards.Count == 0)
                    boards.Push(BoardLoader.LoadBoard("main_board.conf"));

                boards.Peek().State = BoardState.Active;
                frog = boards.Peek().Get<Frog>();

                Flip();
                Sleep(100);

                //usuń oczekujące klawisze
                ClearEvents();

            }

        }
        
        protected override void OnKeyDown (int keycode)
        {

            if (is_help_visible || is_victory_visible)
            {
                is_help_visible = false;
                is_victory_visible = false;
                //zresetuj czas
                time = System.Environment.TickCount;
                return;
            }

            switch(keycode)
            {
                 case Tao.Sdl.Sdl.SDLK_RIGHT:
                    frog.Go(Direction.Right);
                    break;
                 case Tao.Sdl.Sdl.SDLK_DOWN:
                    frog.Go(Direction.Down);
                    break;
                 case Tao.Sdl.Sdl.SDLK_LEFT:
                    frog.Go(Direction.Left);
                    break;
                case Tao.Sdl.Sdl.SDLK_UP:
                    frog.Go(Direction.Up);
                    break;
                 case Tao.Sdl.Sdl.SDLK_ESCAPE:
                    this.Quit();
                    break;
                case Tao.Sdl.Sdl.SDLK_n:
                    if (Config.IsDebug)
                        this.boards.Peek().State = BoardState.Won;
                    break;
            }
        }


        public FroggerScreen() : base(640, 480)
        {

            help_image = Image.FromFile(Config.GetDataDir() + "help.png");
            victory_image = Image.FromFile(Config.GetDataDir() + "victory.png");

            this.boards = new Stack<FroggerBoard>();

            this.boards.Push(BoardLoader.LoadBoard("main_board.conf"));

            this.frog = boards.Peek().Get<Frog>();

        }
        
        public void Start()
        {
            time = System.Environment.TickCount;
            MainLoop();
        }
        
    }
}
