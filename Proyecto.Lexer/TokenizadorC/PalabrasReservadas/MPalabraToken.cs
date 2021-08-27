using Proyecto.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Lexer.TokenizadorC.PalabrasReservadas
{
    public class MPalabraToken : IToken
    {
        public ResultadoTokenizador verificarToken(Entrada e, string tok)
        {
            string alfa = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";
            string tokCompleto = "";
            if (tok == "m" || tok == "M")
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

                if (tokCompleto.Equals("main", StringComparison.OrdinalIgnoreCase))
                {
                    var t = new Token
                    {
                        Lexema = tokCompleto,
                        fila = e.posicion.linea,
                        columna = (e.posicion.columna - cont),
                        tipoToken = TipoToken.pMain
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
