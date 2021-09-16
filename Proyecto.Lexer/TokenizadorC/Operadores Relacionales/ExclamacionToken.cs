using Proyecto.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Lexer.TokenizadorC
{
    public class ExclamacionToken : IToken
    {
        public ResultadoTokenizador verificarToken(Entrada e, string tok)
        {
            if (tok == "!")
            {
                var sig = e.charProximo();

                if (sig.valor == '=')
                {
                    e = sig.restante;

                    var to = new Token
                    {
                        Lexema = "!=",
                        fila = e.posicion.linea,
                        columna = e.posicion.columna - 1,
                        tipoToken = TipoToken.sDistintoQue

                    };
                    return new ResultadoTokenizador
                    {
                        entrada = e,
                        token = to
                    };

                }
                else
                {
                    var t = new Token
                    {
                        Lexema = "!",
                        fila = e.posicion.linea,
                        columna = e.posicion.columna,
                        tipoToken = TipoToken.sExcla
                    };
                    return new ResultadoTokenizador
                    {
                        entrada = e,
                        token = t
                    };
                }
            }
            return null;
        }
    }
}
