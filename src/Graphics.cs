
using Tao.Sdl;
using System;

using SysRectangle = System.Drawing.Rectangle;

namespace Frogger
{
    
    /// <summary>
    /// interfejs do rysowania po ekranie
    /// w tym przypadku jest bezpośrednio implementowany przez Screen
    /// </summary>
    public interface Graphics
    {

        void DrawImage(Image img, SysRectangle src, SysRectangle dst);
        void DrawImage(Image img, SysRectangle src, SysRectangle dst,object ignore_me);

    }
    
    public class Image : IDisposable
    {
        
        public IntPtr surface;
        
        public static Image FromFile(string filename)
        {
            
            Image img=new Image();
            
            img.surface=SdlImage.IMG_Load(filename);
            
            if(img.surface==IntPtr.Zero)
                return null;
            else
                return img;
            
        }
        
        public void Dispose ()
        {
            Sdl.SDL_FreeSurface(surface);
        }

        
    }
    
    public class Screen : IDisposable , Graphics
    {

        /// <summary>
        /// blokowany przy zmianach is_locked
        /// </summary>
        private static object mutex=new object();

        /// <summary>
        /// jeżeli true to jakiś screen jest aktywny
        /// </summary>
        private static bool is_locked=false;
        
        /// <summary>
        /// wskażnik do ekranu SDL
        /// </summary>
        private IntPtr screen;
        
        /// <summary>
        /// jeżeli false MainLoop przerywa działanie
        /// </summary>
        private bool Running=true;
        
        /// <summary>
        /// Tworzy nowy ekran
        /// </summary>
        /// <param name="w">wysokość</param>
        /// <param name="h">szerokość</param>
        public Screen(int w,int h)
        {

            lock (mutex)
            {
                if (is_locked)
                    throw new Exception("SDL already initialised");
                else
                    is_locked = true;
            }

            Sdl.SDL_Init(Sdl.SDL_INIT_VIDEO);
            screen=Sdl.SDL_SetVideoMode(w,h,32,0);

            if (screen == IntPtr.Zero)
                throw new Exception("Unable to initialize video");

            Sdl.SDL_WM_SetCaption(GetTitle(),null);
            Sdl.SDL_EnableKeyRepeat(10, 250);

        }

        /// <summary>
        /// usuwa naciśnięte klawisze oczekujące w kolejce
        /// </summary>
        protected void ClearEvents()
        {
            Sdl.SDL_Event e;

            while (Sdl.SDL_PollEvent(out e) != 0)
            {
                switch (e.type)
                {
                    case Sdl.SDL_QUIT:
                        this.Running = false;
                        return;
                }
            }
        }

        private void ProcessEvents()
        {

            Sdl.SDL_Event e;

            while (Sdl.SDL_PollEvent(out e) != 0)
            {
                switch (e.type)
                {
                    case Sdl.SDL_QUIT:
                        this.Running = false;
                        return;
                    case Sdl.SDL_KEYDOWN:
                        OnKeyDown(e.key.keysym.sym);
                        break;
                    case Sdl.SDL_KEYUP:
                        OnKeyUp(e.key.keysym.sym);
                        break;

                }
            }
        }

        private void QuitSDL()
        {
            lock (mutex)
            {
                if (is_locked)
                {
                    Sdl.SDL_Quit();
                    is_locked = false;
                }
            }
        }

        public void Flip()
        {
            Sdl.SDL_Flip(screen);
        }

        public void Sleep(int milis)
        {
            Sdl.SDL_Delay(milis);
        }

        public void Dispose()
        {
            QuitSDL();
        }

        /// <summary>
        /// Główna pętla
        /// </summary>
        public void MainLoop()
        {
            
            this.Running=true;

            try
            {

                while (this.Running)
                {

                    OnPaint();
                    if (!this.Running)
                        return;

                    Flip();

                    Sdl.SDL_Delay(10);

                    ProcessEvents();
                    if (!this.Running)
                        return;

                }

            }
            finally
            {
                this.QuitSDL();
            }

        }

        /// <summary>
        /// przerywa główną pętlę
        /// </summary>
        public void Quit()
        {
            this.Running = false;
        }

        /// <summary>
        /// czyści ekran
        /// </summary>
        /// <param name="color">kolor którym zostanie wypełniony ekran</param>
        public void Clear(int color)
        {
            Sdl.SDL_Rect rect;

            rect.x = 0;
            rect.y = 0;
            rect.w = -1;
            rect.h = -1;

            Sdl.SDL_FillRect(screen, ref rect, color);
        }

        /// <summary>
        /// rysuje obrazek na ekranie
        /// nie obsługuje skalowania
        /// </summary>
        /// <param name="img">obrazek</param>
        /// <param name="src">wycinek obrazka</param>
        /// <param name="dst">docelowe miejsce na ekranie</param>
        /// <param name="ignore_me">dodatkowy parametr dla zachowania kompatybilności</param>
        public void DrawImage(Image img, SysRectangle src, SysRectangle dst,object ignore_me)
        {
            DrawImage(img,src,dst);
        }

        /// <summary>
        /// rysuje obrazek na ekranie
        /// nie obsługuje skalowania
        /// </summary>
        /// <param name="img">obrazek</param>
        /// <param name="src">wycinek obrazka</param>
        /// <param name="dst">docelowe miejsce na ekranie</param>
        public void DrawImage(Image img, SysRectangle src, SysRectangle dst)
        {
            
            if(src.Width != dst.Width || src.Height != dst.Height)
                throw new ArgumentException("src and dst size must be equal");
            
            
            Sdl.SDL_Rect srcrect;
            Sdl.SDL_Rect dstrect;
            
            srcrect.x=(short)src.X;
            srcrect.y=(short)src.Y;
            srcrect.w=(short)src.Width;
            srcrect.h=(short)src.Height;

            dstrect.x=(short)dst.X;
            dstrect.y=(short)dst.Y;
            dstrect.w=(short)dst.Width;
            dstrect.h=(short)dst.Height;
            
            Sdl.SDL_BlitSurface(img.surface,ref srcrect,screen,ref dstrect);
            
            
        }

        public void DrawImage(Image img)
        {


            Sdl.SDL_Rect srcrect;
            Sdl.SDL_Rect dstrect;

            srcrect.x =0;
            srcrect.y =0;
            srcrect.w =-1;
            srcrect.h =-1;

            dstrect.x =0;
            dstrect.y =0;
            dstrect.w =-1;
            dstrect.h =-1;

            Sdl.SDL_BlitSurface(img.surface, ref srcrect, screen, ref dstrect);


        }

        public void SetCaption(string title)
        {
            Sdl.SDL_WM_SetCaption(title, null);
        }

        protected virtual string GetTitle()
        {
            return "SDL_app";
        }

        protected virtual void OnKeyDown(int keycode){}
        protected virtual void OnKeyUp(int keycode){}
        protected virtual void OnPaint(){}

    }
    
}
