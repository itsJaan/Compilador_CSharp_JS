using Proyecto.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Lexer.TokenizadorC
{
    public class IgualToken : IToken
    {
        public ResultadoTokenizador verificarToken(Entrada e, string tok)
        {
            if (tok == "=")
            {
                var sig = e.charProximo();

                if (sig.valor == '=')
                {
                    e = sig.restante;

                    var t = new Token
                    {
                        Lexema = "==",
                        fila = e.posicion.linea,
                        columna = e.posicion.columna-1,
                        tipoToken = TipoToken.sIgualIgual
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
                        Lexema = "=",
                        fila = e.posicion.linea,
                        columna = e.posicion.columna,
                        tipoToken = TipoToken.sIgual
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
