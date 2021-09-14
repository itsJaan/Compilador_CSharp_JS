//using Proyecto.Lexer.Tokens;

namespace Proyecto.ArbolAbstracto.Expresiones
{
    public class Identificador : Expresion
    {
        //public Proyecto.Lexer.Tokens.Token token { get; set; }
        public Identificador identificador { get; set; }
        
       // public Identificador(Proyecto.Lexer.Tokens.Token t , Identificador i)
        //{
        //    token = t;
        //    identificador = i;
        //}
        //public static Identificador Null => new Identificador(null , null);
    }
}
