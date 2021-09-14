using Proyecto.Lexer.TokenizadorC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Lexer
{
    public interface IToken
    {
        ResultadoTokenizador verificarToken(Entrada e, string tok);
    }
}
