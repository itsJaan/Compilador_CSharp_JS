using Proyecto.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Lexer.TokenizadorC.Singos
{
    public class ParenthesisToken : IToken
    {
        public ResultadoTokenizador verificarToken(Entrada e, string tok)
        {
            if (tok == "(")
            {
                var t = new Token
                {
                    Lexema = "(",
                    fila = e.posicion.linea,
                    columna = e.posicion.columna,
                    tipoToken = TipoToken.parentesisA
                };
                return new ResultadoTokenizador
                {
                    entrada = e,
                    token = t
                };
            }
            if (tok == ")")
            {
                var t = new Token
                {
                    Lexema = ")",
                    fila = e.posicion.linea,
                    columna = e.posicion.columna,
                    tipoToken = TipoToken.parentesisC
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
