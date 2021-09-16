
using Proyecto.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto.ArbolAbstracto
{
    public class Nodo
    {
        public Nodo Padre { get; set; }
        public List<Nodo> Hijos { get; set; }

        public Token Token { get; set; }

        public String Name { get; set; }
        public Nodo(Nodo p)
        {
            Padre = p;
            Hijos = new List<Nodo>();
        }
    }
}
