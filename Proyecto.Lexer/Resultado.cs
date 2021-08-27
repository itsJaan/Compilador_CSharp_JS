using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Lexer
{
    public static class Resultado
    {
        public static Resultado_T_<T> vacio<T>(Entrada restante) => new Resultado_T_<T>(restante);
        public static Resultado_T_<T> valor<T>(T valor ,Entrada restante) => new Resultado_T_<T>(valor, restante);

    }
}
