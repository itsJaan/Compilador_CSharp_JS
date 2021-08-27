using Proyecto.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Lexer.TokenizadorC.Singos
{
    public class GuionBajoToken : IToken
    {
        public ResultadoTokenizador verificarToken(Entrada e, string tok)
        {
            if (tok == "_")
            {
                var t = new Token
                {
                    Lexema = "_",
                    fila = e.posicion.linea,
                    columna = e.posicion.columna,
                    tipoToken = TipoToken.sGuionBajo
                };
                return new ResultadoTokenizador
                {
                    entrada = e,
                    token = t
                };
            }
            return null;
        }
    }
}
