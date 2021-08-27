using Proyecto.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Proyecto.Lexer.TokenizadorC
{
    public class NumeroToken : IToken
    {
        public ResultadoTokenizador verificarToken(Entrada e, string tok)
        {
            string tokCompleto = "";
            if (tok == "0" || tok == "1" || tok == "2" || tok == "3"|| tok == "4" || tok == "5"
                || tok == "6" || tok == "7" ||tok == "8" || tok == "9" )
            {
                int cont = 0;
                tokCompleto += tok;
                var esFloat = false;
                var sig = e.charProximo();

                while(sig.valor =='0'|| sig.valor == '1' || sig.valor == '2' || sig.valor == '3' || sig.valor == '4' ||
                        sig.valor == '5' || sig.valor == '6' || sig.valor == '7' || sig.valor == '8' || sig.valor == '9' || sig.valor=='.')
                {
                    if (sig.valor == '.') esFloat =true;
                    tokCompleto += sig.valor;
                    e = sig.restante;
                    sig = e.charProximo();
                    cont++;
                }
                if (!esFloat) {
                    var t = new Token
                    {
                        Lexema = tokCompleto,
                        fila = e.posicion.linea,
                        columna = (e.posicion.columna - cont),
                        tipoToken = TipoToken.numeroEntero
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
                        tipoToken = TipoToken.numeroFloat
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
