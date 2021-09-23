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
            string numeros = "1234567890";
            string operador = "+-/*., ";
            bool date = false;
            string tokCompleto = "";
            if (numeros.Contains(tok)) 
            {
                int cont = 0;
                tokCompleto += tok;
                var esFloat = false;
                var esOperacion = false;
                var sig = e.charProximo();

                while (numeros.Contains(sig.valor) || operador.Contains(sig.valor))
                {
                    if (sig.valor == '.')
                        esFloat = true;
                    else if (sig.valor == ',')
                        date = true;

                    

                    if (operador.Contains(sig.valor)) 
                        esOperacion = true;
                    if (date && sig.valor == ')')
                        break;
                    tokCompleto += sig.valor;
                    e = sig.restante;
                    sig = e.charProximo();
                    cont++;
                }

                if (date)
                {
                    var t = new Token
                    {
                        Lexema = tokCompleto,
                        fila = e.posicion.linea,
                        columna = (e.posicion.columna - cont),
                        tipoToken = TipoToken.nDate
                    };
                    return new ResultadoTokenizador
                    {
                        entrada = e,
                        token = t
                    };

                }
                else if (esOperacion)
                {
                    var t = new Token
                    {
                        Lexema = tokCompleto,
                        fila = e.posicion.linea,
                        columna = (e.posicion.columna - cont),
                        tipoToken = TipoToken.operacionAritmetica
                    };
                    return new ResultadoTokenizador
                    {
                        entrada = e,
                        token = t
                    };
                }
                else if (!esFloat) 
                {
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
