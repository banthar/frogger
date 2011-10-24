using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Frogger
{
    class BoardLoader
    {
        public static FroggerBoard LoadBoard(string filename)
        {

            FroggerBoard board = new FroggerBoard();

            StreamReader reader=new StreamReader(Config.GetDataDir()+filename);
            
            String s=reader.ReadLine();
            
            int line = 0;

            while (s != null)
            {

                line++;

                try
                {

                    String[] fields = Regex.Split(s, "[^\\w\\._-]+");

                    if (s.Length == 0 || s.StartsWith("#") || fields[0].Length == 0)
                        continue;

                    FroggerObject o = ObjectFromFields(board, fields);

                    if (o != null)
                        board.Add(o);
                    else
                        Console.WriteLine(filename + ":" + line + " Parse Error:\n\t" + s);


                }
                finally
                {
                    s = reader.ReadLine();
                }

            }

            reader.Close();

            return board;

        }

        private static float ParseFloat(string s)
        {
            return Utils.ParseFloat(s);
        }


        private static FroggerObject ObjectFromFields_real(FroggerBoard board, string[] fields)
        {

            FroggerObject fo = null;

            Point p = new Point(ParseFloat(fields[1]), ParseFloat(fields[2]));

            switch (fields[0].ToLower().Replace("_",""))
            {
                case "frog":
                    fo = new Frog(board, p);
                    break;
                case "rock":
                    fo = new Rock(board, p);
                    break;
                case "log":
                    fo = new Log(board, p);
                    fo.Go(new Point(ParseFloat(fields[3]), ParseFloat(fields[4])), float.PositiveInfinity);
                    break;
                case "littlefrog":
                    fo = new LittleFrog(board, p);
                    break;
                case "fly":
                    fo = new Fly(board, p);
                    break;
                case "leaf":
                    fo = new Leaf(board, p);
                    fo.Go(new Point(ParseFloat(fields[3]), ParseFloat(fields[4])), float.PositiveInfinity);
                    break;
                case "home":
                    fo = new Home(board, p);
                    break;
                case "bridge":
                    fo = new Bridge(board, p, fields[3]);
                    break;
                default:
                    return null;
            }

            return fo;

        }

        public static FroggerObject ObjectFromFields(FroggerBoard board, string[] fields)
        {
            if (!Config.IsDebug)
                try
                {
                    return ObjectFromFields_real(board, fields);
                }
                catch (Exception)
                {
                    return null;
                }
            else
                return ObjectFromFields_real(board, fields);

        }

    }
}
