using Proyecto.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Lexer.TokenizadorC
{
    public class SlashToken : IToken
    {
        public ResultadoTokenizador verificarToken(Entrada e, string tok)
        {
            if (tok == "/")
            {
                var sig = e.charProximo();
                if (sig.valor == '*')
                {
                    string com = "";
                    while (sig.valor != '/') {
                        com = sig.valor.ToString();
                        sig = e.charProximo();
                        e = sig.restante;
                    }
                    var t = new Token
                    {
                        Lexema = com,
                        fila = e.posicion.linea,
                        columna = e.posicion.columna,
                        tipoToken = TipoToken.comentario
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
                        Lexema = "/",
                        fila = e.posicion.linea,
                        columna = e.posicion.columna,
                        tipoToken = TipoToken.sDiv
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
