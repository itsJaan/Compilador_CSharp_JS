using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Lexer
{
    public readonly struct Posicion
    {
        public int absoluta { get; }

        public int linea { get; }
        public int columna { get; }
        public Posicion(int a , int l , int c)
        {
            absoluta = a;
            linea = l;
            columna = c;
        }
        public static Posicion Inicio => new Posicion(0, 1, 0);

        public Posicion moverPuntero(char caracter)
        {
            return caracter == '\n' 
                ? new Posicion(absoluta + 1, linea + 1, 0) 
                : new Posicion(absoluta + 1, linea, columna+1);
        }
    }
}
