using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Lexer
{
    public class Resultado_T_<T>
    {
        internal Resultado_T_(T v, Entrada r)
        {
            valor = v;
            restante = r;
            tieneValor = true;
        }internal Resultado_T_(Entrada r)
        {
            restante = r;
            tieneValor = false;
        }
        public T valor { get; set; }
        public Entrada  restante { get; set; }
        public bool tieneValor { get; set; }
    }
}
