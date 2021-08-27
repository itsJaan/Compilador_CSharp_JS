using Proyecto.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Lexer.TokenizadorC
{
    public interface IToken
    {
        ResultadoTokenizador verificarToken(Entrada e, string tok);
    }
}
