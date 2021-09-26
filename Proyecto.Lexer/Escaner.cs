using Proyecto.Lexer.Tokens;
using System;
using Proyecto.Lexer.TokenizadorC;
using Proyecto.Lexer.TokenizadorC.Singos;
using Proyecto.Lexer.TokenizadorC.PalabrasReservadas;

namespace Proyecto.Lexer
{
    public class Escaner: IEscaner
    {
        private  Entrada entrada;
        public Escaner(Entrada e)
        {
            this.entrada = e;
        }
        public ResultadoTokenizador proximoToken()
        {
            IToken[] arr = new IToken[]
                {
                    new MasToken(),
                    new MenosToken(),
                    new SlashToken(),
                    new AsteriscoToken(),
                    new MayorToken(),
                    new GuionBajoToken(),
                    new MenorToken(),
                    new IgualToken(),
                    new ExclamacionToken(),
                    new AmperToken(),
                    new ModulerToken(),
                    new OrToken(),
                    new PuntoToken(),
                    new PuntoComaToken(),
                    new BracketToken(),
                    new ParenthesisToken(),
                    new LlaveToken(),
                    new ComaToken(),
                    new ComillaToken(),
                    new NumeroToken(),
                    new BPalabraToken(),
                    new CPalabraToken(),
                    new DPalabraToken(),
                    new EPalabraToken(),
                    new FPalabraToken(),
                    new GPalabraToken(),
                    new IPalabraToken(),
                    new LPalabraToken(),
                    new MPalabraToken(),
                    new NPalabraToken(),
                    new PPalabraToken(),
                    new RPalabraToken(),
                    new SPalabraToken(),
                    new TPalabraToken(),
                    new UPalabraToken(),
                    new VPalabraToken(),
                    new WPalabraToken(),
                    new IdentificadorToken(),
                    new ExtrasToken()
                };
            while (true)
            {

                var actual = siguienteChar();
                if ( actual == '\0')
                {
                    break;
                }
                ResultadoTokenizador t = new ResultadoTokenizador();
                foreach (var i in arr)
                {
                    t = i.verificarToken(entrada, actual.ToString());
                    Console.ForegroundColor = ConsoleColor.White;
                    if (t != null)
                    {
                        if (t.token.tipoToken != TipoToken.espacio && 
                            t.token.tipoToken != TipoToken.tabulador && 
                            t.token.tipoToken != TipoToken.finLinea)
                        {
                            //Console.WriteLine($"Token: {t.token.Lexema}    Linea: {t.token.fila} Columna: {t.token.columna}");
                            entrada = t.entrada;
                            return t;
                        }
                    }
                }
                if (t == null)
                {

                    var tok = new Token
                    {
                        Lexema = actual.ToString(),
                        fila = entrada.posicion.linea,
                        columna = entrada.posicion.columna,
                        tipoToken = TipoToken.tokenInvalido
                    };
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"El Simbolo: {tok.Lexema}    Linea: {tok.fila} Columna: {tok.columna} es invalido.");
                    Console.ForegroundColor = ConsoleColor.White;
                    return new ResultadoTokenizador
                    {
                        entrada = entrada,
                        token = tok
                    };
                }
            } 
            
            var to = new Token
            {
                Lexema = "finArchivo",
                fila = entrada.posicion.linea,
                columna = entrada.posicion.columna,
                tipoToken = TipoToken.finArchivo
            };
            //Console.WriteLine($"Token: {to.Lexema}    Linea: {to.fila} Columna: {to.columna}");
            return new ResultadoTokenizador
            {
                entrada = entrada,
                token = to
            };
        } 

        private char siguienteChar()
        {
            var siguiente = entrada.charProximo();
            entrada = siguiente.restante;
            return siguiente.valor;

        }
    }
}
