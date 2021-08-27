using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Lexer.Tokens
{
    public class Token
    {
        public TipoToken tipoToken { get; set; }
        public int? posicionEnTablaSimbolos { get; set; }
        public string Lexema { get; set; }
        public int fila { get; set; }
        public int columna { get; set; }
        public override string ToString()
        {
            return $"{Lexema}         Linea: {fila}  Columna: {columna} \n";
        }

    }
}
