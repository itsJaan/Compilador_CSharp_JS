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
            string numeros = "1234567890";
            string operador = "+-/*.() ";
            string tokCompleto = "";
            
            if (tok == "(")
            {
                int cont = 0;
                tokCompleto += tok;
                var esOperacion = false;
                var sig = e.charProximo();

                if (numeros.Contains(sig.valor))
                {
                    while (numeros.Contains(sig.valor) || operador.Contains(sig.valor))
                    {
                        if (operador.Contains(sig.valor))
                            esOperacion = true;

                        tokCompleto += sig.valor;
                        e = sig.restante;
                        sig = e.charProximo();
                        cont++;
                    }
                }

                if (esOperacion)
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
                else
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
