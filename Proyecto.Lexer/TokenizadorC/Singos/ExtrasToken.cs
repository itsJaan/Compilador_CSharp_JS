using Proyecto.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Lexer.TokenizadorC.Singos
{
    public class ExtrasToken : IToken
    {
        public ResultadoTokenizador verificarToken(Entrada e, string tok)
        {
            if(tok==" ")
            {
                var t = new Token
                {
                    Lexema = " ",
                    fila = e.posicion.linea,
                    columna = e.posicion.columna,
                    tipoToken = TipoToken.espacio
                };
                return new ResultadoTokenizador
                {
                    entrada = e,
                    token = t
                };
            }
            else if (tok == "\n")
            {
                var t = new Token
                {
                    Lexema = "\n",
                    fila = e.posicion.linea,
                    columna = e.posicion.columna,
                    tipoToken = TipoToken.finLinea
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
