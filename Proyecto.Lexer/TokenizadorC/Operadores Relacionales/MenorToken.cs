using Proyecto.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Lexer.TokenizadorC
{
    public class MenorToken : IToken
    {
        public ResultadoTokenizador verificarToken(Entrada e, string tok)
        {
            if (tok == "<")
            {
                var sig = e.charProximo();

                if (sig.valor == '=')
                {
                    e = sig.restante;

                    var t = new Token
                    {
                        Lexema = "<=",
                        fila = e.posicion.linea,
                        columna = e.posicion.columna - 1,
                        tipoToken = TipoToken.sMenorIgual
                    };
                    return new ResultadoTokenizador
                    {
                        entrada = e,
                        token = t
                    };

                }
                else
                {
                    var t = new Token
                    {
                        Lexema = "<",
                        fila = e.posicion.linea,
                        columna = e.posicion.columna,
                        tipoToken = TipoToken.sMenor
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
