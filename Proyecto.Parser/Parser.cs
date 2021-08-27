using Proyecto.Lexer;
using Proyecto.Lexer.TokenizadorC;
using Proyecto.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Parser
{
    
    public class Parser
    {
        public List<TipoToken> tipoDato = new List<TipoToken> { TipoToken.pBool , TipoToken.pDatetime , TipoToken.pInt , TipoToken.pFloat};
        public List<TipoToken> tipoStates = new List<TipoToken> { TipoToken.pFor, TipoToken.pForeach, TipoToken.pIf , TipoToken.pWhile};

        private readonly IEscaner escaner;
        private ResultadoTokenizador tokSig;
        private List<Error> errores;

        public Parser(IEscaner sc)
        {
            this.escaner = sc;
            this.errores = new List<Error>();

        }
        private void move()
        {
            this.tokSig = this.escaner.proximoToken();
        }
        private void match(TipoToken tipo)
        {
            if(this.tokSig.token.tipoToken != tipo)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Error error = new Error(this.tokSig.token.fila, this.tokSig.token.columna, tipo.ToString());
                this.errores.Add(error);
            }
            this.move();
        }
        public void Parsear()
        {
            UC();
            //Evaluacion de errores
            if (this.errores.Count == 0)
                Console.WriteLine("Succesful");
            else
            {
                foreach(Error e in this.errores)
                {
                    Console.WriteLine($"Syntax Error Linea: {e.fila} Columna: {e.columna} se esperaba un {e.lexema}");
                }
            }
        }
        private void UC()
        {
            if (this.tokSig.token.tipoToken == TipoToken.pUsing)
                IMPORTS();
            else if (this.tokSig.token.tipoToken == TipoToken.pClass || this.tokSig.token.tipoToken == TipoToken.pPublic)
                CLASS();
            if (this.tokSig.token != null)
                UC();
        }
        private void IMPORTS()
        {
            match(TipoToken.pUsing);
            IDENTIFICADOR();
            match(TipoToken.sPuntoComa);
        }
        private void IDENTIFICADOR()
        {
            match(TipoToken.identificador);
            if (this.tokSig.token.tipoToken == TipoToken.sPunto) 
                IDENTIFICADOR();
        }
        private void CLASS()
        {
            if (this.tokSig.token.tipoToken == TipoToken.pPublic)
            {
                match(TipoToken.pPublic);
            }
            else if (this.tokSig.token.tipoToken == TipoToken.pPrivate)
            {
                match(TipoToken.pPrivate);
            }

            if (this.tokSig.token.tipoToken == TipoToken.pStatic)
            {
                match(TipoToken.pStatic);
            }

            match(TipoToken.pClass);
            IDENTIFICADOR();
            match(TipoToken.llaveA);
            INCLASS();
            match(TipoToken.llaveC);
        }
        private void INCLASS()
        {
            //Main
            if (this.tokSig.token.tipoToken == TipoToken.pStatic)
            {
                match(TipoToken.pStatic);
                if (this.tokSig.token.tipoToken == TipoToken.pVoid)
                {
                    match(TipoToken.pVoid);
                    match(TipoToken.pMain);
                    match(TipoToken.parentesisA);
                    match(TipoToken.parentesisC);
                    match(TipoToken.llaveA);
                    INMAIN();
                    match(TipoToken.llaveC);
                }
            }

            //DECLARACION DE PUBLIC O PRIVATE
            else if (this.tokSig.token.tipoToken == TipoToken.pPublic)
                match(TipoToken.pPublic);
            else if (this.tokSig.token.tipoToken == TipoToken.pPrivate)
                match(TipoToken.pPrivate);

            //if (this.tokSig.token.tipoToken == TipoToken.pVar)
            //    match(TipoToken.pVar);
            if (this.tokSig.token.tipoToken == TipoToken.pFloat)
                match(TipoToken.pFloat);
            else if (this.tokSig.token.tipoToken == TipoToken.pBool)
                match(TipoToken.pBool);
            else if (this.tokSig.token.tipoToken == TipoToken.pInt)
                match(TipoToken.pInt);
            else if (this.tokSig.token.tipoToken == TipoToken.pDatetime)
                match(TipoToken.pDatetime);
            else if (this.tokSig.token.tipoToken == TipoToken.pVoid)
                match(TipoToken.pVoid);

            IDENTIFICADOR();

            if (this.tokSig.token.tipoToken == TipoToken.sPuntoComa)
            {
                match(TipoToken.sPuntoComa);
            }
            else if (this.tokSig.token.tipoToken == TipoToken.sIgual)
            {
                match(TipoToken.sIgual);
                DECLARACION();
            }
            else if (this.tokSig.token.tipoToken == TipoToken.parentesisA)
            {
                match(TipoToken.parentesisA);
                FUNCION();
            }

            //RECURSION EN SI MISMO
            if(this.tokSig.token != null)
            {
                INCLASS();
            }
        }
        private void DECLARACION()
        {
            if (this.tokSig.token.tipoToken == TipoToken.numeroEntero)
            {
                match(TipoToken.numeroEntero);
                match(TipoToken.sPuntoComa);
            }
            else if (this.tokSig.token.tipoToken == TipoToken.operacionAritmetica)
            {
                match(TipoToken.operacionAritmetica);
                match(TipoToken.sPuntoComa);
            }
            else if (this.tokSig.token.tipoToken == TipoToken.numeroFloat)
            {
                match(TipoToken.numeroFloat);
                match(TipoToken.sPuntoComa);
            }
            else if (this.tokSig.token.tipoToken == TipoToken.pFalse)
            {
                match(TipoToken.pFalse);
                match(TipoToken.sPuntoComa);
            }
            else if (this.tokSig.token.tipoToken == TipoToken.pTrue)
            {
                match(TipoToken.pTrue);
                match(TipoToken.sPuntoComa);
            }
            else if (this.tokSig.token.tipoToken == TipoToken.pNew)
            {
                match(TipoToken.pNew);
                if (this.tokSig.token.tipoToken == TipoToken.pBool)
                {
                    //new Bool(false);
                    match(TipoToken.pBool);
                    match(TipoToken.parentesisA);
                    if (this.tokSig.token.tipoToken == TipoToken.pFalse) 
                        match(TipoToken.pFalse);
                    else 
                        match(TipoToken.pTrue);
                    match(TipoToken.parentesisC);
                    match(TipoToken.sPuntoComa);
                }
                else if (this.tokSig.token.tipoToken == TipoToken.pFloat)
                {
                    //new Float(200.11);
                    match(TipoToken.pFloat);
                    match(TipoToken.parentesisA);
                    if (this.tokSig.token.tipoToken == TipoToken.numeroFloat) match(TipoToken.numeroFloat);
                    else match(TipoToken.numeroEntero);
                    match(TipoToken.parentesisC);
                    match(TipoToken.sPuntoComa);
                    //OPERACIONES DE ARITMETICAS
                }
                else if (this.tokSig.token.tipoToken == TipoToken.pInt)
                {
                    //new Int(20011);
                    match(TipoToken.pInt);
                    match(TipoToken.parentesisA);
                    match(TipoToken.numeroEntero);
                    match(TipoToken.parentesisC);
                    match(TipoToken.sPuntoComa);
                   // OPERACIONES ARITMETICAS
                }
                else if (this.tokSig.token.tipoToken == TipoToken.pDatetime)
                {
                    //new DateTime(2001, 12, 12, 20, 00, 00);
                    match(TipoToken.pDatetime);
                    match(TipoToken.parentesisA);
                    if (this.tokSig.token.tipoToken == TipoToken.numeroEntero)
                    {
                        match(TipoToken.numeroEntero);
                        match(TipoToken.sComa);
                        match(TipoToken.numeroEntero);
                        match(TipoToken.sComa);
                        match(TipoToken.numeroEntero);
                        match(TipoToken.sComa);
                        match(TipoToken.numeroEntero);
                        match(TipoToken.sComa);
                        match(TipoToken.numeroEntero);
                        match(TipoToken.sComa);
                        match(TipoToken.numeroEntero);
                    }
                    match(TipoToken.parentesisC);
                    match(TipoToken.sPuntoComa);
                }
            }
        }

        private void FUNCION()
        {
            if(this.tokSig.token.tipoToken != TipoToken.parentesisC)
                PARAMETROS();
            match(TipoToken.parentesisC);
            match(TipoToken.llaveA);
            INFUNCION();
            match(TipoToken.llaveC);
        }
        private void INFUNCION()
        {
            if(tipoDato.Contains(this.tokSig.token.tipoToken))
            {
                if (this.tokSig.token.tipoToken == TipoToken.pFloat)
                    match(TipoToken.pFloat);
                else if (this.tokSig.token.tipoToken == TipoToken.pBool)
                    match(TipoToken.pBool);
                else if (this.tokSig.token.tipoToken == TipoToken.pInt)
                    match(TipoToken.pInt);
                else if (this.tokSig.token.tipoToken == TipoToken.pDatetime)
                    match(TipoToken.pDatetime);
                IDENTIFICADOR();
                if (this.tokSig.token.tipoToken == TipoToken.sPuntoComa)
                {
                    match(TipoToken.sPuntoComa);
                }
                else if (this.tokSig.token.tipoToken == TipoToken.sIgual)
                {
                    match(TipoToken.sIgual);
                    DECLARACION();
                }
            }
            else if (tipoStates.Contains(this.tokSig.token.tipoToken))
            {
                if (this.tokSig.token.tipoToken == TipoToken.pIf)
                {
                    match(TipoToken.pIf);
                    F_IF();
                }
                else if (this.tokSig.token.tipoToken == TipoToken.pFor)
                {
                    match(TipoToken.pFor);
                    F_FOR();
                }
                else if (this.tokSig.token.tipoToken == TipoToken.pForeach)
                {
                    match(TipoToken.pForeach);
                    F_FOREACH();
                }
                else if (this.tokSig.token.tipoToken == TipoToken.pWhile)
                {
                    match(TipoToken.pWhile);
                    F_WHILE();
                }
            }
            else if(this.tokSig.token.tipoToken == TipoToken.pConsole)
            {
                match(TipoToken.pConsole);
                match(TipoToken.sPunto);
                if (this.tokSig.token.tipoToken == TipoToken.pWriteLine)
                {
                    match(TipoToken.pWriteLine);
                    match(TipoToken.parentesisA);
                    //TEXTO;
                    match(TipoToken.parentesisC);
                    match(TipoToken.sPuntoComa);

                }
            }
        }

        private void PARAMETROS()
        {
            throw new NotImplementedException();
        }private void F_IF()
        {
            throw new NotImplementedException();
        }
        private void F_FOR()
        {
            throw new NotImplementedException();
        }
        private void F_FOREACH()
        {
            throw new NotImplementedException();
        }
        private void F_WHILE()
        {
            throw new NotImplementedException();
        }
        private void INMAIN()
        {
            throw new NotImplementedException();
        }
    }
}
