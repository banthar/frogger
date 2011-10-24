
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;


using SysRectangle = System.Drawing.Rectangle;
using SysPoint = System.Drawing.Point;

namespace Frogger
{
    
    public class FroggerAnimationSpec
    {
        
        private static Dictionary<string, Image> tiles=new Dictionary<string, Image>();
        private static Dictionary<string, FroggerAnimationSpec> specs;
        
        public SysRectangle[] frames;
        public float[] times;
        public Image image;
        
        public static FroggerAnimationSpec fromString(string name)
        {
            if(specs==null)
            {
                specs=load_animations("animations.conf");
            }
            
            FroggerAnimationSpec s=specs[name];

            return s;    
        }
        
        private static Dictionary<string, FroggerAnimationSpec> load_animations(string filename)
        {
            
            Dictionary<string, FroggerAnimationSpec> specs=new Dictionary<string, FroggerAnimationSpec>();
        
            
            StreamReader reader=new StreamReader(Config.GetDataDir()+filename);
            
            String s=reader.ReadLine();
            
            while(s!=null)
            {

                //jeżeli linia kończy się \ to dołącz następną linię
                if(s.EndsWith("\\"))
                {

                    //s = s.Remove(s.Length - 1);
                    string tmp = reader.ReadLine();

                    if (tmp == null)
                        s = null;
                    else
                        s += tmp;

                    continue;

                }
                
                String[] fields=Regex.Split(s,"[^\\w\\._]+");
                
                if(fields.Length>=3 && !s.StartsWith("#"))
                {
                
                    
                    String name=fields[0];            
                    String img_name=fields[1];            
                    int length=int.Parse(fields[2]);
    
                    FroggerAnimationSpec spec=new FroggerAnimationSpec();
                    
                    spec.image=load_image(img_name);
                    spec.frames=new SysRectangle[length];
                    spec.times=new float[length];
    
                    for(int i=0;i<length;i++)
                    {
                        
                        if("INF".Equals(fields[i*5+3]))
                            spec.times[i]=float.PositiveInfinity;    
                        else
                            spec.times[i]=float.Parse(fields[i*5+3].Replace('.',','));
                        
                        spec.frames[i]=new SysRectangle(
                                                        int.Parse(fields[i*5+4]),
                                                        int.Parse(fields[i*5+5]),
                                                        int.Parse(fields[i*5+6]),
                                                        int.Parse(fields[i*5+7])
                                                        );
                    }
                    
                    specs.Add(name,spec);
    
                }
                    
                s=reader.ReadLine();
        
            }
            



            return specs;
            
        }
        
        private static Image load_image(string filename)
        {

            
            if(tiles.ContainsKey(filename))
            {
                return tiles[filename];
            }
            {
                Image image=Image.FromFile(Config.GetDataDir()+filename);
                tiles[filename]=image;
                return image;
            }
            
        }

        
    }
    
    public class FroggerAnimation
    {
        
        public int frame;
        float state;
        
        readonly FroggerAnimationSpec spec;
        
        public FroggerAnimation(string spec) : this(FroggerAnimationSpec.fromString(spec))
        {
        }

        public FroggerAnimation(FroggerAnimationSpec spec)
        {
            this.spec=spec;
            this.frame=0;
            this.state=0.0f;
                        
        }
        
        public void Tick(float seconds)
        {
            this.state+=seconds;
            
            while(state>=spec.times[frame])
            {
                state-=spec.times[frame++];
                frame%=spec.times.Length;
            }
        }
        
        public void Draw(Graphics g,Point p)
        {
            
            SysPoint sysp=p.toSysPoint(32);
            
            SysRectangle dst=new SysRectangle(sysp.X,sysp.Y,spec.frames[frame].Width,spec.frames[frame].Height);
            
            g.DrawImage(spec.image, spec.frames[frame],dst);
            
        }
        
        public void Seek(int frame)
        {
            this.state=0.0f;
            this.frame=frame%(spec.frames.Length);
        }

        public bool IsStopped()
        {
            return spec.times[this.frame] == float.PositiveInfinity;
        }

    }

}
