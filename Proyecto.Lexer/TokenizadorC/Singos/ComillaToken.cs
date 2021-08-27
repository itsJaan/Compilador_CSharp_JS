using Proyecto.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Lexer.TokenizadorC.Singos
{
    public class ComillaToken : IToken
    {
        public ResultadoTokenizador verificarToken(Entrada e, string tok)
        {
            if (tok == "\'")
            {
                var t = new Token
                {
                    Lexema = "\'",
                    fila = e.posicion.linea,
                    columna = e.posicion.columna,
                    tipoToken = TipoToken.sComillaS
                };
                return new ResultadoTokenizador
                {
                    entrada = e,
                    token = t
                };
            }
            if (tok == "\"")
            {
                var t = new Token
                {
                    Lexema = "\"",
                    fila = e.posicion.linea,
                    columna = e.posicion.columna,
                    tipoToken = TipoToken.sComillaD
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
