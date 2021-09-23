using Proyecto.Lexer;
using Proyecto.Lexer.Tokens;
using Proyecto.Parser;
using System;
using System.IO;

namespace Proyecto_Compiladores_1
{
    public class main
    {
        static void Main(string[] args)
        {
            var code = File.ReadAllText("D:\\Documentos\\Repos VS\\Proyecto Compiladores 1\\Proyecto Compiladores 1\\Test\\test1.txt").Replace(Environment.NewLine, "\n");
            var entrada = new Entrada(code);
            var escaner = new Escaner(entrada);
           
            //var res=escaner.proximoToken();
            //while (res.token.tipoToken != TipoToken.finArchivo)
            //{
            //    res=escaner.proximoToken();
            //}
            var escanerParser = new Escaner(entrada);
            var parser = new Parser(escanerParser);
            parser.Parsear();

        }
    }
}
