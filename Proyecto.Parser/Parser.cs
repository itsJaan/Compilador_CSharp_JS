using Proyecto.ArbolAbstracto;
using Proyecto.ArbolAbstracto.Imports;
using Proyecto.ArbolAbstracto.Sentencias;
using Proyecto.Lexer;
using Proyecto.Lexer.TokenizadorC;
using Proyecto.Lexer.Tokens;
using System;
using System.Collections.Generic;


namespace Proyecto.Parser
{
    
    public class Parser
    {
        public List<TipoToken> tipoDato = new List<TipoToken> { TipoToken.pBool , TipoToken.pDatetime , TipoToken.pInt , TipoToken.pFloat , TipoToken.pVar};
        public List<TipoToken> tipoStates = new List<TipoToken> { TipoToken.pFor, TipoToken.pForeach, TipoToken.pIf , TipoToken.pWhile};
        public List<TipoToken> tipoOperador = new List<TipoToken> { TipoToken.sIgual, TipoToken.sExcla, TipoToken.sMayor};
        public List<TipoToken> tipoAritm = new List<TipoToken> { TipoToken.sMas, TipoToken.sMenos, TipoToken.sMod, TipoToken.sMult, TipoToken.sDiv ,TipoToken.parentesisA , TipoToken.parentesisC};

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
            move();
            UC();
            /*if (this.errores.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("Succesful");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                foreach(Error e in this.errores)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"Syntax Error Linea: {e.fila} Columna: {e.columna} se esperaba un {e.lexema}");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }*/

        }
        private void UC()
        {
            IMPORTS();
            CLASSES();
            //var imports = IMPORTS();
            //var clases = CLASSES();
        }
        private void IMPORTS()
        {

            if (tokSig.token.tipoToken != TipoToken.pUsing)
            {
                return;
            }
            IMPORT();
            IMPORTS();
            //return new ImportSecuence(IMPORT(), IMPORTS());
        }
        private void IMPORT()
        {
            match(TipoToken.pUsing);
            IDENTIFICADOR();
            match(TipoToken.sPuntoComa);

        }
        private void IDENTIFICADOR()
        {
            match(TipoToken.identificador);
            if (tokSig.token.tipoToken == TipoToken.sPunto)
            {
                match(TipoToken.sPunto);
                IDENTIFICADOR();
            }
        }
        private void CLASSES()
        {
            if (tokSig.token.tipoToken == TipoToken.finArchivo)
            {
                return;
            }
            CLASS();
            CLASSES();
        }
        private void CLASS()
        {
            match(TipoToken.pPublic);
            match(TipoToken.pClass);
            IDENTIFICADOR();
            match(TipoToken.llaveA);
            while(tokSig.token.tipoToken != TipoToken.llaveC)
            {
                INCLASS();
            }
            match(TipoToken.llaveC);
        }
        private void INCLASS()
        {
            //SI ES MAIN
            if (this.tokSig.token.tipoToken == TipoToken.pStatic)
            {
                match(TipoToken.pStatic);
                match(TipoToken.pVoid);
                match(TipoToken.pMain);
                match(TipoToken.parentesisA);
                match(TipoToken.parentesisC);
                match(TipoToken.llaveA);
                while (tokSig.token.tipoToken != TipoToken.llaveC)
                {
                    INMAIN();
                }
                match(TipoToken.llaveC);
                return; 
            }
            //SI NO ES MAIN
            else if (this.tokSig.token.tipoToken == TipoToken.pPublic)
            {
                match(TipoToken.pPublic);
                if (this.tokSig.token.tipoToken == TipoToken.pFloat)
                    match(TipoToken.pFloat);
                else if (this.tokSig.token.tipoToken == TipoToken.pBool)
                    match(TipoToken.pBool);
                else if (this.tokSig.token.tipoToken == TipoToken.pInt)
                    match(TipoToken.pInt);
                else if (this.tokSig.token.tipoToken == TipoToken.pDatetime)
                    match(TipoToken.pDatetime);
                else if (tokSig.token.tipoToken == TipoToken.pVoid)
                    match(TipoToken.pVoid);
              
                IDENTIFICADOR();
                //FUNCION
                if (tokSig.token.tipoToken == TipoToken.parentesisA)
                {
                    match(TipoToken.parentesisA);
                    if (tokSig.token.tipoToken != TipoToken.parentesisC)
                    {
                        PARAMETROS();
                    }
                    match(TipoToken.parentesisC);
                    match(TipoToken.llaveA);
                    while (tokSig.token.tipoToken != TipoToken.llaveC)
                    {
                        INMAIN();
                    }
                    match(TipoToken.llaveC);
                    return;
                }
                //DECLARACION PROP
                else if (tokSig.token.tipoToken == TipoToken.llaveA)
                {
                    match(TipoToken.llaveA);
                    match(TipoToken.pGet);
                    match(TipoToken.sPuntoComa);

                    if (tokSig.token.tipoToken == TipoToken.pSet)
                    {
                        match(TipoToken.pSet);
                        match(TipoToken.sPuntoComa);
                    }
                    match(TipoToken.llaveC);
                    return;
                }
                //DECLARACION ASIGNADA
                else if(tokSig.token.tipoToken == TipoToken.sIgual)
                {
                    match(TipoToken.sIgual);
                    DECLARACION_IGUAL();
                    return;   
                }
                //DECLARACION NORMAL
                else if (tokSig.token.tipoToken == TipoToken.sPuntoComa)
                {
                    match(TipoToken.sPuntoComa);
                    return;
                }
                return;
            }
        }
        private void INMAIN()
        {
            if (tipoDato.Contains(tokSig.token.tipoToken))
            {
                DECLARACION();
            }
            else if (tipoStates.Contains(tokSig.token.tipoToken))
            {
                SENTENCIA();
            }
            else if (tokSig.token.tipoToken == TipoToken.pConsole)
            {
                match(TipoToken.pConsole);
                match(TipoToken.sPunto);
                match(TipoToken.pWriteLine);
                match(TipoToken.parentesisA);
                match(TipoToken.sComillaD);
                IDENTIFICADOR();
                match(TipoToken.sComillaD);
                match(TipoToken.parentesisC);
                match(TipoToken.sPuntoComa);
                //FALTA OPCION DE CONCATENACION
                
            }else if (tokSig.token.tipoToken == TipoToken.identificador)
            {
                IDENTIFICADOR();
                FUNCIONCALL();
            }
            else if(tokSig.token.tipoToken == TipoToken.pBreak)
            {
                match(TipoToken.pBreak);
                match(TipoToken.sPuntoComa);
            }
                
        }
        private void FUNCIONCALL()
        {
            match(TipoToken.parentesisA);
            while(tokSig.token.tipoToken != TipoToken.parentesisC)
            {
                PARAMETROSCALL();
            }
            match(TipoToken.parentesisC);
            match(TipoToken.sPuntoComa);
        }
        private void PARAMETROS()
        {
            if (this.tokSig.token.tipoToken == TipoToken.pFloat)
                match(TipoToken.pFloat);
            else if (this.tokSig.token.tipoToken == TipoToken.pBool)
                match(TipoToken.pBool);
            else if (this.tokSig.token.tipoToken == TipoToken.pInt)
                match(TipoToken.pInt);
            else if (this.tokSig.token.tipoToken == TipoToken.pDatetime)
                match(TipoToken.pDatetime);
            else
                IDENTIFICADOR();
            IDENTIFICADOR();
            
            if(tokSig.token.tipoToken == TipoToken.sComa)
            {
                match(TipoToken.sComa);
                PARAMETROS();
            }
            return;
        }
        private void PARAMETROSCALL()
        {
            if (tokSig.token.tipoToken == TipoToken.numeroEntero)
            {
                match(TipoToken.numeroEntero);
                if (tokSig.token.tipoToken != TipoToken.parentesisC)
                    match(TipoToken.sComa);
            }
            else if (tokSig.token.tipoToken == TipoToken.numeroFloat)
            {
                match(TipoToken.numeroFloat);
                if (tokSig.token.tipoToken != TipoToken.parentesisC)
                    match(TipoToken.sComa);
            }
            else 
            {
                IDENTIFICADOR();
                if (tokSig.token.tipoToken != TipoToken.parentesisC)
                    match(TipoToken.sComa);
            }
        }
        private void SENTENCIA()
        {
            if(tokSig.token.tipoToken == TipoToken.pIf)
            {
                match(TipoToken.pIf);
                match(TipoToken.parentesisA);
                CONDICION_IF();
                match(TipoToken.parentesisC);
                match(TipoToken.llaveA);
                INMAIN();
                match(TipoToken.llaveC);
                
                if(tokSig.token.tipoToken == TipoToken.pElse)
                {
                    match(TipoToken.pElse);
                    match(TipoToken.llaveA);
                    INMAIN();
                    match(TipoToken.llaveC);
                }
                //Else if
            }
            else if (tokSig.token.tipoToken == TipoToken.pFor)
            {
                match(TipoToken.pFor);
                match(TipoToken.parentesisA);
                CONDICION_FOR();
                match(TipoToken.parentesisC);
                match(TipoToken.llaveA);
                INMAIN();
                match(TipoToken.llaveC);
            }
            else if (tokSig.token.tipoToken == TipoToken.pForeach)
            {
                match(TipoToken.pForeach);
                match(TipoToken.parentesisA);
                CONDICION_FOREACH();
                match(TipoToken.parentesisC);
                match(TipoToken.llaveA);
                INMAIN();
                match(TipoToken.llaveC);
            }
            else if (tokSig.token.tipoToken == TipoToken.pWhile)
            {
                match(TipoToken.pWhile);
                match(TipoToken.parentesisA);
                CONDICION_IF();
                match(TipoToken.parentesisC);
                match(TipoToken.llaveA);
                INMAIN();
                match(TipoToken.llaveC);
            }
            
        }
        private void CONDICION_IF()
        {
            if (tokSig.token.tipoToken == TipoToken.numeroEntero)
            {
                match(TipoToken.numeroEntero);
                if (tipoOperador.Contains(tokSig.token.tipoToken))
                {
                    if (tokSig.token.tipoToken == TipoToken.sIgual)
                    {
                        match(TipoToken.sIgual);
                        match(TipoToken.sIgual);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sMayor)
                    {
                        match(TipoToken.sMayor);
                        if (tokSig.token.tipoToken == TipoToken.sIgual)
                            match(TipoToken.sIgual);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sMenor)
                    {
                        match(TipoToken.sMenor);
                        if (tokSig.token.tipoToken == TipoToken.sIgual)
                            match(TipoToken.sIgual);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sExcla)
                    {
                        match(TipoToken.sExcla);
                        match(TipoToken.sIgual);
                    }

                    if (tokSig.token.tipoToken == TipoToken.numeroEntero)
                        match(TipoToken.numeroEntero);
                    else if (tokSig.token.tipoToken == TipoToken.numeroFloat)
                        match(TipoToken.numeroFloat);
                    else if (tokSig.token.tipoToken == TipoToken.pFalse)
                        match(TipoToken.pFalse);
                    else if (tokSig.token.tipoToken == TipoToken.pTrue)
                        match(TipoToken.pTrue);
                    else
                        IDENTIFICADOR();
                }
            }
            else if (tokSig.token.tipoToken == TipoToken.numeroFloat)
            {
                match(TipoToken.numeroFloat);
                if (tipoOperador.Contains(tokSig.token.tipoToken))
                {
                    if (tokSig.token.tipoToken == TipoToken.sIgual)
                    {
                        match(TipoToken.sIgual);
                        match(TipoToken.sIgual);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sMayor)
                    {
                        match(TipoToken.sMayor);
                        if (tokSig.token.tipoToken == TipoToken.sIgual)
                            match(TipoToken.sIgual);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sMenor)
                    {
                        match(TipoToken.sMenor);
                        if (tokSig.token.tipoToken == TipoToken.sIgual)
                            match(TipoToken.sIgual);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sExcla)
                    {
                        match(TipoToken.sExcla);
                        match(TipoToken.sIgual);
                    }

                    if (tokSig.token.tipoToken == TipoToken.numeroEntero)
                        match(TipoToken.numeroEntero);
                    else if (tokSig.token.tipoToken == TipoToken.numeroFloat)
                        match(TipoToken.numeroFloat);
                    else if (tokSig.token.tipoToken == TipoToken.pFalse)
                        match(TipoToken.pFalse);
                    else if (tokSig.token.tipoToken == TipoToken.pTrue)
                        match(TipoToken.pTrue);
                    else
                        IDENTIFICADOR();
                }
            }
            else if (tokSig.token.tipoToken == TipoToken.pFalse)
            {
                match(TipoToken.pFalse);
                if (tipoOperador.Contains(tokSig.token.tipoToken))
                {
                    if (tokSig.token.tipoToken == TipoToken.sIgual)
                    {
                        match(TipoToken.sIgual);
                        match(TipoToken.sIgual);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sMayor)
                    {
                        match(TipoToken.sMayor);
                        if (tokSig.token.tipoToken == TipoToken.sIgual)
                            match(TipoToken.sIgual);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sMenor)
                    {
                        match(TipoToken.sMenor);
                        if (tokSig.token.tipoToken == TipoToken.sIgual)
                            match(TipoToken.sIgual);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sExcla)
                    {
                        match(TipoToken.sExcla);
                        match(TipoToken.sIgual);
                    }

                    if (tokSig.token.tipoToken == TipoToken.numeroEntero)
                        match(TipoToken.numeroEntero);
                    else if (tokSig.token.tipoToken == TipoToken.numeroFloat)
                        match(TipoToken.numeroFloat);
                    else if (tokSig.token.tipoToken == TipoToken.pFalse)
                        match(TipoToken.pFalse);
                    else if (tokSig.token.tipoToken == TipoToken.pTrue)
                        match(TipoToken.pTrue);
                    else
                        IDENTIFICADOR();
                }
            }
            else if (tokSig.token.tipoToken == TipoToken.pTrue)
            {
                match(TipoToken.pTrue);
                if (tipoOperador.Contains(tokSig.token.tipoToken))
                {
                    if (tokSig.token.tipoToken == TipoToken.sIgual)
                    {
                        match(TipoToken.sIgual);
                        match(TipoToken.sIgual);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sMayor)
                    {
                        match(TipoToken.sMayor);
                        if (tokSig.token.tipoToken == TipoToken.sIgual)
                            match(TipoToken.sIgual);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sMenor)
                    {
                        match(TipoToken.sMenor);
                        if (tokSig.token.tipoToken == TipoToken.sIgual)
                            match(TipoToken.sIgual);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sExcla)
                    {
                        match(TipoToken.sExcla);
                        match(TipoToken.sIgual);
                    }

                    if (tokSig.token.tipoToken == TipoToken.numeroEntero)
                        match(TipoToken.numeroEntero);
                    else if (tokSig.token.tipoToken == TipoToken.numeroFloat)
                        match(TipoToken.numeroFloat);
                    else if (tokSig.token.tipoToken == TipoToken.pFalse)
                        match(TipoToken.pFalse);
                    else if (tokSig.token.tipoToken == TipoToken.pTrue)
                        match(TipoToken.pTrue);
                    else
                        IDENTIFICADOR();
                }
            }
            else if(tokSig.token.tipoToken == TipoToken.identificador)
            {
                IDENTIFICADOR();
                if (tipoOperador.Contains(tokSig.token.tipoToken))
                {
                    if (tokSig.token.tipoToken == TipoToken.sIgual)
                    {
                        match(TipoToken.sIgual);
                        match(TipoToken.sIgual);
                    }else if (tokSig.token.tipoToken == TipoToken.sMayor)
                    {
                        match(TipoToken.sMayor);
                        if (tokSig.token.tipoToken == TipoToken.sIgual)
                            match(TipoToken.sIgual);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sMenor)
                    {
                        match(TipoToken.sMenor);
                        if (tokSig.token.tipoToken == TipoToken.sIgual)
                            match(TipoToken.sIgual);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sExcla)
                    {
                        match(TipoToken.sExcla);
                        match(TipoToken.sIgual);
                    }

                    if (tokSig.token.tipoToken == TipoToken.numeroEntero)
                        match(TipoToken.numeroEntero);
                    else if (tokSig.token.tipoToken == TipoToken.numeroFloat)
                        match(TipoToken.numeroFloat);
                    else if (tokSig.token.tipoToken == TipoToken.pFalse)
                        match(TipoToken.pFalse);
                    else if (tokSig.token.tipoToken == TipoToken.pTrue)
                        match(TipoToken.pTrue);
                    else
                        IDENTIFICADOR();
                }
            }
            else if(tokSig.token.tipoToken == TipoToken.sExcla)
            {
                match(TipoToken.sExcla);
                IDENTIFICADOR();
            }

            if (tokSig.token.tipoToken == TipoToken.sAmpr)
            {
                match(TipoToken.sAmpr);
                match(TipoToken.sAmpr);
                CONDICION_IF();
            }
            else if (tokSig.token.tipoToken == TipoToken.sOr)
            {
                match(TipoToken.sOr);
                match(TipoToken.sOr);
                CONDICION_IF();
            }
            else 
                return;



        }
        private void CONDICION_FOR()

        {
            if (tokSig.token.tipoToken == TipoToken.pInt)
            {
                match(TipoToken.pInt);
                IDENTIFICADOR();
                match(TipoToken.sIgual);
                match(TipoToken.numeroEntero);
            }
            else if (tokSig.token.tipoToken == TipoToken.identificador)
            {
                IDENTIFICADOR();
            }
            match(TipoToken.sPuntoComa);

            if (tokSig.token.tipoToken == TipoToken.numeroEntero)
            {
                match(TipoToken.numeroEntero);
                if (tokSig.token.tipoToken == TipoToken.sIgual)
                {
                    match(TipoToken.sIgual);
                    match(TipoToken.sIgual);
                }
                else if (tokSig.token.tipoToken == TipoToken.sMayor)
                {
                    match(TipoToken.sMayor);
                    if (tokSig.token.tipoToken == TipoToken.sIgual)
                        match(TipoToken.sIgual);
                }
                else if (tokSig.token.tipoToken == TipoToken.sMenor)
                {
                    match(TipoToken.sMenor);
                    if (tokSig.token.tipoToken == TipoToken.sIgual)
                        match(TipoToken.sIgual);
                }
                else if (tokSig.token.tipoToken == TipoToken.sExcla)
                {
                    match(TipoToken.sExcla);
                    match(TipoToken.sIgual);
                }
                IDENTIFICADOR();
            }
            else if (tokSig.token.tipoToken == TipoToken.identificador)
            {
                IDENTIFICADOR();
                match(TipoToken.numeroEntero);
                if (tokSig.token.tipoToken == TipoToken.sIgual)
                {
                    match(TipoToken.sIgual);
                    match(TipoToken.sIgual);
                }
                else if (tokSig.token.tipoToken == TipoToken.sMayor)
                {
                    match(TipoToken.sMayor);
                    if (tokSig.token.tipoToken == TipoToken.sIgual)
                        match(TipoToken.sIgual);
                }
                else if (tokSig.token.tipoToken == TipoToken.sMenor)
                {
                    match(TipoToken.sMenor);
                    if (tokSig.token.tipoToken == TipoToken.sIgual)
                        match(TipoToken.sIgual);
                }
                else if (tokSig.token.tipoToken == TipoToken.sExcla)
                {
                    match(TipoToken.sExcla);
                    match(TipoToken.sIgual);
                }
                if (tokSig.token.tipoToken == TipoToken.numeroEntero)
                    match(TipoToken.numeroEntero);
                else if (tokSig.token.tipoToken == TipoToken.numeroFloat)
                    match(TipoToken.numeroFloat);
                else if (tokSig.token.tipoToken == TipoToken.pFalse)
                    match(TipoToken.pFalse);
                else if (tokSig.token.tipoToken == TipoToken.pTrue)
                    match(TipoToken.pTrue);
                else
                    IDENTIFICADOR();
            }
            match(TipoToken.sPuntoComa);
            
            IDENTIFICADOR();
            if(tokSig.token.tipoToken == TipoToken.sMas)
            {
                match(TipoToken.sMas);
                match(TipoToken.sMas);
            }
            else if(tokSig.token.tipoToken == TipoToken.sMenos)
            {
                match(TipoToken.sMenos);
                match(TipoToken.sMenos);
            }
        }
        private void CONDICION_FOREACH()
        {
            if (tokSig.token.tipoToken == TipoToken.identificador)
                IDENTIFICADOR();
            else if (tokSig.token.tipoToken == TipoToken.pInt)
                match(TipoToken.pInt);
            else if (tokSig.token.tipoToken == TipoToken.pBool)
                match(TipoToken.pBool);
            else if (tokSig.token.tipoToken == TipoToken.pDatetime)
                match(TipoToken.pDatetime);
            else if (tokSig.token.tipoToken == TipoToken.pFloat)
                match(TipoToken.pFloat);
            IDENTIFICADOR();
            match(TipoToken.pIn);
            IDENTIFICADOR();


        }
        private void DECLARACION()
        {
            if(tokSig.token.tipoToken == TipoToken.pInt)
                match(TipoToken.pInt);
            else if (tokSig.token.tipoToken == TipoToken.pBool)
                match(TipoToken.pBool);
            else if (tokSig.token.tipoToken == TipoToken.pFloat)
                match(TipoToken.pFloat);
            else if (tokSig.token.tipoToken == TipoToken.pDatetime)
                match(TipoToken.pDatetime);
            else if (tokSig.token.tipoToken == TipoToken.pVar)
                match(TipoToken.pVar);
            if (tokSig.token.tipoToken == TipoToken.bracketA)
            {
                match(TipoToken.bracketA);
                match(TipoToken.bracketC);
            }
            IDENTIFICADOR();

            if(tokSig.token.tipoToken == TipoToken.sPuntoComa)
               match(TipoToken.sPuntoComa);
             
            else if(tokSig.token.tipoToken == TipoToken.sIgual)
            {
                match(TipoToken.sIgual);
                DECLARACION_IGUAL();
            }


        }
        private void DECLARACION_IGUAL()
        {
            if (this.tokSig.token.tipoToken == TipoToken.numeroEntero)
            {
                match(TipoToken.numeroEntero);
                if (tipoAritm.Contains(tokSig.token.tipoToken))
                {
                    OPERACIONARITMETICA();
                }
                match(TipoToken.sPuntoComa);
                return;
            }
            else if (this.tokSig.token.tipoToken == TipoToken.numeroFloat)
            {
                match(TipoToken.numeroFloat);
                match(TipoToken.sPuntoComa);
                return;
            }
            else if (this.tokSig.token.tipoToken == TipoToken.pFalse)
            {
                match(TipoToken.pFalse);
                match(TipoToken.sPuntoComa);
                return;
            }
            else if (this.tokSig.token.tipoToken == TipoToken.pTrue)
            {
                match(TipoToken.pTrue);
                match(TipoToken.sPuntoComa);
                return;
            }
            else if (this.tokSig.token.tipoToken == TipoToken.pNew)
            {
                //new DateTime(año, mes, dia, hora, minuto, segundo);
                match(TipoToken.pNew);
                match(TipoToken.pDatetime);
                match(TipoToken.parentesisA);
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
                match(TipoToken.parentesisC);
                match(TipoToken.sPuntoComa);
                return;
            }
            else if (tokSig.token.tipoToken == TipoToken.pConsole)
            {
                match(TipoToken.pConsole);
                match(TipoToken.sPunto);
                match(TipoToken.pReadLine);
                match(TipoToken.parentesisA);
                match(TipoToken.parentesisC);
                match(TipoToken.sPuntoComa);
                return;
            }
            else if (tokSig.token.tipoToken == TipoToken.parentesisA)
            {
                OPERACIONARITMETICA();
                match(TipoToken.sPuntoComa);
                return;
            }
            else if (tokSig.token.tipoToken == TipoToken.identificador)
            {
                IDENTIFICADOR();
                if (tokSig.token.tipoToken == TipoToken.parentesisA)
                {
                    FUNCIONCALL();
                }
                else if (tipoAritm.Contains(tokSig.token.tipoToken))
                {
                    OPERACIONARITMETICA();
                    match(TipoToken.sPuntoComa);
                }
                else
                    match(TipoToken.sPuntoComa);
                return;
            }
            else
                return;

        }
        private void OPERACIONARITMETICA()
        {
            while(tokSig.token.tipoToken==TipoToken.numeroEntero || tipoAritm.Contains(tokSig.token.tipoToken))
            {
                if (tokSig.token.tipoToken == TipoToken.parentesisA)
                {
                    match(TipoToken.parentesisA);
                    OPERACIONARITMETICA();
                    match(TipoToken.parentesisC);
                }
                else if (tokSig.token.tipoToken == TipoToken.numeroEntero)
                    match(TipoToken.numeroEntero);
                else if (tokSig.token.tipoToken == TipoToken.sMas)
                    match(TipoToken.sMas);
                else if (tokSig.token.tipoToken == TipoToken.sMenos)
                    match(TipoToken.sMenos);
                else if (tokSig.token.tipoToken == TipoToken.sMult)
                    match(TipoToken.sMult);
                else if (tokSig.token.tipoToken == TipoToken.sDiv)
                    match(TipoToken.sDiv);
                else if (tokSig.token.tipoToken == TipoToken.sMod)
                    match(TipoToken.sMod);
                else
                    return;
            }
        }
    }
} 