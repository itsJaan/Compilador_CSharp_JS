using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Parser
{
    public class Error
    {
        public int fila;
        public int columna;
        public string lexema;
        public string waiting;

        public Error( int f , int c, string l ,string w)
        {
            fila = f;
            columna = c;
            lexema = l;
            waiting = w;
        }
    }
}
