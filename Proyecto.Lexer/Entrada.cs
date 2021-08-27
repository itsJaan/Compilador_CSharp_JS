using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Lexer
{
    public readonly struct Entrada
    {
        public string codFuente  { get;}
        public int longitud { get; }
        public Posicion posicion { get; }

        public Entrada(string c)
            :this(c , Posicion.Inicio, c.Length)
        {

        }
        public Entrada(string c , Posicion p , int l)
        {
            codFuente = c;
            posicion = p;
            longitud = l;
        }
        public Resultado_T_<char> charProximo()
        {
            if(longitud == 0)
            {
                return Resultado.vacio<char>(this);
            }
            var caracter = codFuente[posicion.absoluta];
            var e = new Entrada(codFuente, posicion.moverPuntero(caracter), longitud - 1);
            return Resultado.valor(caracter,e);
        }
    }
}
