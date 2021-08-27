using Proyecto.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Lexer.TokenizadorC
{
    public class IdentificadorToken : IToken
    {
        public ResultadoTokenizador verificarToken(Entrada e, string tok)
        {
            string alfa = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";

            if (tok == "g" || tok == "G"
               || tok == "h" || tok == "H"
               || tok == "j" || tok == "J"
               || tok == "k" || tok == "K"
               || tok == "l" || tok == "L"
               || tok == "o" || tok == "O"
               || tok == "q" || tok == "Q"
               || tok == "x" || tok == "X"
               || tok == "y" || tok == "Y"
               || tok == "z" || tok == "Z")
            {
                var sig = e.charProximo();
                string tokCompleto = tok;
                int cont = 0;
                if (alfa.Contains(sig.valor))
                {
                    while (alfa.Contains(sig.valor))
                    {
                        tokCompleto += sig.valor;
                        e = sig.restante;
                        sig = e.charProximo();
                        cont++;
                    }
                    var t = new Token
                    {
                        Lexema = tokCompleto,
                        fila = e.posicion.linea,
                        columna = e.posicion.columna - cont,
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
