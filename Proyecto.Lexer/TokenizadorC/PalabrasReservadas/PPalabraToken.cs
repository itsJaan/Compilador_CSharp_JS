using Proyecto.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Lexer.TokenizadorC.PalabrasReservadas
{
    public class PPalabraToken : IToken
    {
        public ResultadoTokenizador verificarToken(Entrada e, string tok)
        {
            string alfa = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";
            string tokCompleto = "";
            if (tok == "p" || tok == "P")
            {
                int cont = 0;
                tokCompleto += tok;
                var sig = e.charProximo();

                while (alfa.Contains(sig.valor))
                {
                    tokCompleto += sig.valor;
                    e = sig.restante;
                    sig = e.charProximo();
                    cont++;
                }

                if (tokCompleto.Equals("public", StringComparison.OrdinalIgnoreCase))
                {
                    var t = new Token
                    {
                        Lexema = tokCompleto,
                        fila = e.posicion.linea,
                        columna = (e.posicion.columna - cont),
                        tipoToken = TipoToken.pPublic
                    };
                    return new ResultadoTokenizador
                    {
                        entrada = e,
                        token = t
                    };
                }
                else if (tokCompleto.Equals("private", StringComparison.OrdinalIgnoreCase))
                {
                    var t = new Token
                    {
                        Lexema = tokCompleto,
                        fila = e.posicion.linea,
                        columna = (e.posicion.columna - cont),
                        tipoToken = TipoToken.pPrivate
                    };
                    return new ResultadoTokenizador
                    {
                        entrada = e,
                        token = t
                    };
                }
                else if (tokCompleto.Equals("protected", StringComparison.OrdinalIgnoreCase))
                {
                    var t = new Token
                    {
                        Lexema = tokCompleto,
                        fila = e.posicion.linea,
                        columna = (e.posicion.columna - cont),
                        tipoToken = TipoToken.pProtected
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
                        Lexema = tokCompleto,
                        fila = e.posicion.linea,
                        columna = (e.posicion.columna - cont),
                        tipoToken = TipoToken.identificador
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
