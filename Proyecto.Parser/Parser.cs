using Proyecto.ArbolAbstracto;
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
        public List<TipoToken> tipoOperador = new List<TipoToken> { TipoToken.sIgualIgual, TipoToken.sExcla, TipoToken.sMayorIgual, TipoToken.sDistintoQue, TipoToken.sMayor , TipoToken.sMenor , TipoToken.sMenorIgual};
        public List<TipoToken> tipoAritm = new List<TipoToken> { TipoToken.sMas, TipoToken.sMenos, TipoToken.sMod, TipoToken.sMult, TipoToken.sDiv ,TipoToken.parentesisA , TipoToken.parentesisC};

        private readonly IEscaner escaner;
        private ResultadoTokenizador tokSig;
        private List<Error> errores;
        private Nodo Raiz;
        
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
                this.move();
            }
            this.move();
        }
        public void Parsear()
        {
            move();
            Raiz = new Nodo(null);
            UC();

            if (this.errores.Count == 0)
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
            }

        }
        private void UC()
        {
            IMPORTS(Raiz);
            CLASSES(Raiz);
        }
        private void IMPORTS(Nodo padre)
        {
            Nodo nImport = new Nodo(padre);
            nImport.Name = "Import";

            if (tokSig.token.tipoToken != TipoToken.pUsing)
            {
                return;
            }
            var hijo = IMPORT(nImport);
            padre.Hijos.Add(hijo);
            IMPORTS(padre);
        }
        private Nodo IMPORT(Nodo padre)
        {
            Nodo hijo = new Nodo(padre);

            hijo.Token = tokSig.token;
            padre.Hijos.Add(hijo);
            match(TipoToken.pUsing);

            padre = IDENTIFICADOR(padre);
            
            Nodo hijoaux = new Nodo(padre);
            hijoaux.Token = tokSig.token;
            padre.Hijos.Add(hijoaux);
            match(TipoToken.sPuntoComa);

            return padre;
        }
        private Nodo IDENTIFICADOR(Nodo padre)
        {
            Nodo hijo = new Nodo(padre);
            hijo.Token = tokSig.token;
            padre.Hijos.Add(hijo);
            match(TipoToken.identificador);

            if (tokSig.token.tipoToken == TipoToken.sPunto)
            {
                Nodo hijoaux = new Nodo(padre);
                hijoaux.Token = tokSig.token;
                padre.Hijos.Add(hijoaux);
                match(TipoToken.sPunto);
                padre = IDENTIFICADOR(padre);
            }
            return padre;
        }
        private void CLASSES(Nodo padre)
        {
            Nodo nClase = new Nodo(padre);
            nClase.Name = "Clase";
            if (tokSig.token.tipoToken == TipoToken.finArchivo)
            {
                return;
            }
            var hijo = CLASS(nClase);
            padre.Hijos.Add(hijo);
            CLASSES(padre);
        }
        private Nodo CLASS(Nodo padre)
        {
            Nodo hijo = new Nodo(padre);
            hijo.Token = tokSig.token;
            padre.Hijos.Add(hijo);
            match(TipoToken.pPublic);

            Nodo hijoClass = new Nodo(padre);
            hijoClass.Token = tokSig.token;
            padre.Hijos.Add(hijoClass);
            match(TipoToken.pClass);

            padre = IDENTIFICADOR(padre);

            Nodo hijoLlaveA = new Nodo(padre);
            hijoLlaveA.Token = tokSig.token;
            padre.Hijos.Add(hijoLlaveA);
            match(TipoToken.llaveA);

            Nodo hijoInClass = new Nodo(padre);
            hijoInClass.Name = "InClass";
            hijoInClass = INCLASSN(hijoInClass);
            padre.Hijos.Add(hijoInClass);

            Nodo hijoLlaveC = new Nodo(padre);
            hijoLlaveC.Token = tokSig.token;
            padre.Hijos.Add(hijoLlaveC);
            match(TipoToken.llaveC);

            return padre;
        }
        private Nodo INCLASSN(Nodo padre)
        {
            Nodo hijo = new Nodo(padre);
            if (tokSig.token.tipoToken == TipoToken.llaveC)
            {
                return padre;
            }
            hijo = INCLASS(hijo);
            padre.Hijos.Add(hijo);
            return INCLASSN(padre);
        }
        private Nodo INCLASS(Nodo padre)
        {
            //SI ES MAIN
            if (this.tokSig.token.tipoToken == TipoToken.pStatic)
            {
                Nodo hijoStatic = new Nodo(padre);
                hijoStatic.Token = tokSig.token;
                padre.Hijos.Add(hijoStatic);
                match(TipoToken.pStatic);

                Nodo hijoVoid = new Nodo(padre);
                hijoVoid.Token = tokSig.token;
                padre.Hijos.Add(hijoVoid);
                match(TipoToken.pVoid);

                Nodo hijoMain = new Nodo(padre);
                hijoMain.Token = tokSig.token;
                padre.Hijos.Add(hijoMain);
                match(TipoToken.pMain);

                Nodo hijoParA = new Nodo(padre);
                hijoParA.Token = tokSig.token;
                padre.Hijos.Add(hijoParA);
                match(TipoToken.parentesisA);

                Nodo hijoParC = new Nodo(padre);
                hijoParC.Token = tokSig.token;
                padre.Hijos.Add(hijoParC);
                match(TipoToken.parentesisC);

                Nodo hijoLlaveA = new Nodo(padre);
                hijoLlaveA.Token = tokSig.token;
                padre.Hijos.Add(hijoLlaveA);
                match(TipoToken.llaveA);


                Nodo hijoInMain = new Nodo(padre);
                hijoInMain.Name = "InMain";
                hijoInMain = INMAINN(hijoInMain);
                padre.Hijos.Add(hijoInMain);

                Nodo hijoLlaveC = new Nodo(padre);
                hijoLlaveC.Token = tokSig.token;
                padre.Hijos.Add(hijoLlaveC);
                match(TipoToken.llaveC);
                return padre; 
            }
            //SI NO ES MAIN
            else if (this.tokSig.token.tipoToken == TipoToken.pPublic)
            {
                Nodo hijoPublic = new Nodo(padre);
                hijoPublic.Token = tokSig.token;
                padre.Hijos.Add(hijoPublic);
                match(TipoToken.pPublic);

                if (this.tokSig.token.tipoToken == TipoToken.pFloat)
                {
                    Nodo hijoFloat = new Nodo(padre);
                    hijoFloat.Token = tokSig.token;
                    padre.Hijos.Add(hijoFloat);
                    match(TipoToken.pFloat);
                }
                else if (this.tokSig.token.tipoToken == TipoToken.pBool)
                {
                    Nodo hijoBool = new Nodo(padre);
                    hijoBool.Token = tokSig.token;
                    padre.Hijos.Add(hijoBool);
                    match(TipoToken.pBool);
                }
                else if (this.tokSig.token.tipoToken == TipoToken.pInt)
                {
                    Nodo hijoInt = new Nodo(padre);
                    hijoInt.Token = tokSig.token;
                    padre.Hijos.Add(hijoInt);
                    match(TipoToken.pInt);
                }
                else if (this.tokSig.token.tipoToken == TipoToken.pDatetime)
                {
                    Nodo hijoDateTime = new Nodo(padre);
                    hijoDateTime.Token = tokSig.token;
                    padre.Hijos.Add(hijoDateTime);
                    match(TipoToken.pDatetime);
                }
                else if (tokSig.token.tipoToken == TipoToken.pVoid)
                {
                    Nodo hijoVoid = new Nodo(padre);
                    hijoVoid.Token = tokSig.token;
                    padre.Hijos.Add(hijoVoid);
                    match(TipoToken.pVoid);
                }

                padre = IDENTIFICADOR(padre);

                //FUNCION
                if (tokSig.token.tipoToken == TipoToken.parentesisA)
                {
                    Nodo hijoParA = new Nodo(padre);
                    hijoParA.Token = tokSig.token;
                    padre.Hijos.Add(hijoParA);
                    match(TipoToken.parentesisA);

                    if (tokSig.token.tipoToken != TipoToken.parentesisC)
                    {
                        Nodo hijoParametros = new Nodo(padre);
                        hijoParametros.Name = "Parametros";
                        hijoParametros = PARAMETROS(hijoParametros);
                        padre.Hijos.Add(hijoParametros);
                    }
                    Nodo hijoParC = new Nodo(padre);
                    hijoParC.Token = tokSig.token;
                    padre.Hijos.Add(hijoParC);
                    match(TipoToken.parentesisC);

                    Nodo hijoLlaveA = new Nodo(padre);
                    hijoLlaveA.Token = tokSig.token;
                    padre.Hijos.Add(hijoLlaveA);
                    match(TipoToken.llaveA);

                    Nodo hijoInMain = new Nodo(padre);
                    hijoInMain.Name = "InMain";
                    hijoInMain = INMAINN(hijoInMain);
                    padre.Hijos.Add(hijoInMain);

                    Nodo hijoLlaveC = new Nodo(padre);
                    hijoLlaveC.Token = tokSig.token;
                    padre.Hijos.Add(hijoLlaveC);
                    match(TipoToken.llaveC);

                    return padre;
                }
                //DECLARACION PROP
                else if (tokSig.token.tipoToken == TipoToken.llaveA)
                {
                    Nodo hijoLlaveA = new Nodo(padre);
                    hijoLlaveA.Token = tokSig.token;
                    padre.Hijos.Add(hijoLlaveA);
                    match(TipoToken.llaveA);

                    Nodo hijoGet = new Nodo(padre);
                    hijoGet.Token = tokSig.token;
                    padre.Hijos.Add(hijoGet);
                    match(TipoToken.pGet);

                    Nodo hijoPc = new Nodo(padre);
                    hijoPc.Token = tokSig.token;
                    padre.Hijos.Add(hijoPc);
                    match(TipoToken.sPuntoComa);

                    if (tokSig.token.tipoToken == TipoToken.pSet)
                    {
                        Nodo hijoSet = new Nodo(padre);
                        hijoSet.Token = tokSig.token;
                        padre.Hijos.Add(hijoSet);
                        match(TipoToken.pSet);

                        Nodo hijoPc2 = new Nodo(padre);
                        hijoPc2.Token = tokSig.token;
                        padre.Hijos.Add(hijoPc2);
                        match(TipoToken.sPuntoComa);
                    }

                    Nodo hijoLlaveC = new Nodo(padre);
                    hijoLlaveC.Token = tokSig.token;
                    padre.Hijos.Add(hijoLlaveC);
                    match(TipoToken.llaveC);

                    return padre;
                }
                //DECLARACION ASIGNADA
                else if(tokSig.token.tipoToken == TipoToken.sIgual)
                {
                    Nodo hijoIgual = new Nodo(padre);
                    hijoIgual.Token = tokSig.token;
                    padre.Hijos.Add(hijoIgual);
                    match(TipoToken.sIgual);

                    padre = DECLARACION_IGUAL(padre);
                    
                    return padre;   
                }
                //DECLARACION NORMAL
                else if (tokSig.token.tipoToken == TipoToken.sPuntoComa)
                {
                    Nodo hijoPc = new Nodo(padre);
                    hijoPc.Token = tokSig.token;
                    padre.Hijos.Add(hijoPc);
                    match(TipoToken.sPuntoComa);

                    return padre;
                }
            }
            return null;
        }
        private Nodo INMAINN(Nodo padre)
        {
            Nodo hijo = new Nodo(padre);
            if (tokSig.token.tipoToken == TipoToken.llaveC)
            {
                return padre;
            }
            hijo = INMAIN(hijo);
            padre.Hijos.Add(hijo);
            return INMAINN(padre);

        }
        private Nodo INMAIN(Nodo padre)
        {
            if (tipoDato.Contains(tokSig.token.tipoToken))
            {
                Nodo hijoDecl = new Nodo(padre);
                hijoDecl.Name = "Declaracion";
                hijoDecl = DECLARACION(hijoDecl);
                padre.Hijos.Add(hijoDecl);
                return padre;
            }
            else if (tipoStates.Contains(tokSig.token.tipoToken))
            {
                Nodo hijoSentencia = new Nodo(padre);
                hijoSentencia.Name = "Sentencia";
                hijoSentencia = SENTENCIA(hijoSentencia);
                padre.Hijos.Add(hijoSentencia);
                return padre;
            }
            else if (tokSig.token.tipoToken == TipoToken.pConsole)
            {

                Nodo hijoConsole = new Nodo(padre);
                hijoConsole.Token = tokSig.token;
                padre.Hijos.Add(hijoConsole);
                match(TipoToken.pConsole);

                Nodo hijoP = new Nodo(padre);
                hijoP.Token = tokSig.token;
                padre.Hijos.Add(hijoP);
                match(TipoToken.sPunto);

                Nodo hijoWriteline = new Nodo(padre);
                hijoWriteline.Token = tokSig.token;
                padre.Hijos.Add(hijoWriteline);
                match(TipoToken.pWriteLine);

                Nodo hijoParA = new Nodo(padre);
                hijoParA.Token = tokSig.token;
                padre.Hijos.Add(hijoParA); 
                match(TipoToken.parentesisA);

                Nodo hijoComilla = new Nodo(padre);
                hijoComilla.Token = tokSig.token;
                padre.Hijos.Add(hijoComilla);
                match(TipoToken.sComillaD);

                padre = IDENTIFICADOR(padre);

                Nodo hijoCommillaD = new Nodo(padre);
                hijoCommillaD.Token = tokSig.token;
                padre.Hijos.Add(hijoCommillaD);
                match(TipoToken.sComillaD);

                Nodo hijoParC = new Nodo(padre);
                hijoParC.Token = tokSig.token;
                padre.Hijos.Add(hijoParC);
                match(TipoToken.parentesisC);

                Nodo hijoPc = new Nodo(padre);
                hijoPc.Token = tokSig.token;
                padre.Hijos.Add(hijoPc);
                match(TipoToken.sPuntoComa);
                
                return padre;

            }
            else if (tokSig.token.tipoToken == TipoToken.identificador)
            {
                padre = IDENTIFICADOR(padre);
                if(tokSig.token.tipoToken == TipoToken.sIgual)
                {
                    Nodo hijoIgual = new Nodo(padre);
                    hijoIgual.Token = tokSig.token;
                    padre.Hijos.Add(hijoIgual);
                    match(TipoToken.sIgual);
                    padre = DECLARACION_IGUAL(padre);
                
                }
                else if (tokSig.token.tipoToken == TipoToken.sMas)
                {
                    Nodo hijoMas = new Nodo(padre);
                    hijoMas.Token = tokSig.token;
                    padre.Hijos.Add(hijoMas);
                    match(TipoToken.sMas);
                    
                    Nodo hijoMas2 = new Nodo(padre);
                    hijoMas2.Token = tokSig.token;
                    padre.Hijos.Add(hijoMas2);
                    match(TipoToken.sMas);

                }
                else if (tokSig.token.tipoToken == TipoToken.sMenos)
                {
                    Nodo hijoMenos = new Nodo(padre);
                    hijoMenos.Token = tokSig.token;
                    padre.Hijos.Add(hijoMenos);
                    match(TipoToken.sMenos);

                    Nodo hijoMenos2 = new Nodo(padre);
                    hijoMenos2.Token = tokSig.token;
                    padre.Hijos.Add(hijoMenos2);
                    match(TipoToken.sMenos);

                }
                else
                    padre = FUNCIONCALL(padre);

                return padre;
            }
            else if(tokSig.token.tipoToken == TipoToken.pBreak)
            {
                Nodo hijoBreak = new Nodo(padre);
                hijoBreak.Token = tokSig.token;
                padre.Hijos.Add(hijoBreak);
                match(TipoToken.pBreak);

                Nodo hijoPc = new Nodo(padre);
                hijoPc.Token = tokSig.token;
                padre.Hijos.Add(hijoPc);
                match(TipoToken.sPuntoComa);

                return padre;
            }
            return null;

        }
        private Nodo FUNCIONCALL(Nodo padre)
        {
            Nodo hijoParA = new Nodo(padre);
            hijoParA.Token = tokSig.token;
            padre.Hijos.Add(hijoParA);
            match(TipoToken.parentesisA);

            padre = PARAMETROSCALLN(padre);
            
            Nodo hijoParC = new Nodo(padre);
            hijoParC.Token = tokSig.token;
            padre.Hijos.Add(hijoParC);
            match(TipoToken.parentesisC);

            Nodo hijoPc = new Nodo(padre);
            hijoPc.Token = tokSig.token;
            padre.Hijos.Add(hijoPc);
            match(TipoToken.sPuntoComa);

            return padre;
        }
        private Nodo PARAMETROSCALLN(Nodo padre)
        {
            Nodo hijoParametros = new Nodo(padre);
            hijoParametros.Name = "Parametros";
            if( tokSig.token.tipoToken == TipoToken.parentesisC)
            {
                return padre;
            }
            var hijo = PARAMETROSCALL(hijoParametros);
            padre.Hijos.Add(hijo);
            return PARAMETROSCALLN(padre);
        }
        private Nodo PARAMETROSCALL(Nodo padre)
        {
            if (tokSig.token.tipoToken == TipoToken.numeroEntero)
            {
                Nodo hijoNumeroEntero = new Nodo(padre);
                hijoNumeroEntero.Token = tokSig.token;
                padre.Hijos.Add(hijoNumeroEntero);
                match(TipoToken.numeroEntero);

                if (tokSig.token.tipoToken != TipoToken.parentesisC)
                {
                    Nodo hijoPc = new Nodo(padre);
                    hijoPc.Token = tokSig.token;
                    padre.Hijos.Add(hijoPc);
                    match(TipoToken.sComa);
                }
                return padre;

            }
            else if (tokSig.token.tipoToken == TipoToken.numeroFloat)
            {
                Nodo hijoNumeroFloat = new Nodo(padre);
                hijoNumeroFloat.Token = tokSig.token;
                padre.Hijos.Add(hijoNumeroFloat);
                match(TipoToken.numeroFloat);
                if (tokSig.token.tipoToken != TipoToken.parentesisC)
                {
                    Nodo hijoPc = new Nodo(padre);
                    hijoPc.Token = tokSig.token;
                    padre.Hijos.Add(hijoPc);
                    match(TipoToken.sComa);
                }
                return padre;
            }
            else 
            {
                padre = IDENTIFICADOR(padre);
                if (tokSig.token.tipoToken != TipoToken.parentesisC)
                {
                    Nodo hijoPc = new Nodo(padre);
                    hijoPc.Token = tokSig.token;
                    padre.Hijos.Add(hijoPc);
                    match(TipoToken.sComa);
                }
                return padre;
            }
            
        }
        private Nodo PARAMETROS(Nodo padre)
        {
            if (this.tokSig.token.tipoToken == TipoToken.pFloat)
            {
                Nodo hijoParA = new Nodo(padre);
                hijoParA.Token = tokSig.token;
                padre.Hijos.Add(hijoParA);
                match(TipoToken.pFloat);
            }
            else if (this.tokSig.token.tipoToken == TipoToken.pBool)
            {
                Nodo hijoParA = new Nodo(padre);
                hijoParA.Token = tokSig.token;
                padre.Hijos.Add(hijoParA);
                match(TipoToken.pBool);
            }
            else if (this.tokSig.token.tipoToken == TipoToken.pInt)
            {
                Nodo hijoParA = new Nodo(padre);
                hijoParA.Token = tokSig.token;
                padre.Hijos.Add(hijoParA);
                match(TipoToken.pInt);
            }
            else if (this.tokSig.token.tipoToken == TipoToken.pDatetime)
            {
                Nodo hijoParA = new Nodo(padre);
                hijoParA.Token = tokSig.token;
                padre.Hijos.Add(hijoParA);
                match(TipoToken.pDatetime);
            }
            else
            {
                padre = IDENTIFICADOR(padre);
            }
            padre = IDENTIFICADOR(padre);
            
            if(tokSig.token.tipoToken == TipoToken.sComa)
            {
                Nodo hijoParA = new Nodo(padre);
                hijoParA.Token = tokSig.token;
                padre.Hijos.Add(hijoParA);
                match(TipoToken.sComa);
                padre = PARAMETROS(padre);
            }
            return padre;
        }
        private Nodo SENTENCIA(Nodo padre)
        {
            if(tokSig.token.tipoToken == TipoToken.pIf)
            {
                
                Nodo hijoIf = new Nodo(padre);
                hijoIf.Token = tokSig.token;
                padre.Hijos.Add(hijoIf);
                match(TipoToken.pIf);

                Nodo hijoParA = new Nodo(padre);
                hijoParA.Token = tokSig.token;
                padre.Hijos.Add(hijoParA);
                match(TipoToken.parentesisA);

                Nodo hijoCondicion = new Nodo(padre);
                hijoCondicion.Name = "Condicion IF";
                hijoCondicion = CONDICION_IF(hijoCondicion);
                padre.Hijos.Add(hijoCondicion);

                Nodo hijoParC = new Nodo(padre);
                hijoParC.Token = tokSig.token;
                padre.Hijos.Add(hijoParC);
                match(TipoToken.parentesisC);

                Nodo hijoLlaveA = new Nodo(padre);
                hijoLlaveA.Token = tokSig.token;
                padre.Hijos.Add(hijoLlaveA);
                match(TipoToken.llaveA);

                Nodo hijoInMain = new Nodo(padre);
                hijoInMain.Name = "In IF";
                hijoInMain = INMAINN(hijoInMain);
                padre.Hijos.Add(hijoInMain);

                Nodo hijoLlaveC = new Nodo(padre);
                hijoLlaveC.Token = tokSig.token;
                padre.Hijos.Add(hijoLlaveC);
                match(TipoToken.llaveC);

                
                if(tokSig.token.tipoToken == TipoToken.pElse)
                {
                    padre = ELSEIFN(padre);
                }
                
            }
            else if (tokSig.token.tipoToken == TipoToken.pForeach)
            {
                Nodo hijoLlaveCelse = new Nodo(padre);
                hijoLlaveCelse.Token = tokSig.token;
                padre.Hijos.Add(hijoLlaveCelse);
                match(TipoToken.pForeach);

                Nodo hijoParA = new Nodo(padre);
                hijoParA.Token = tokSig.token;
                padre.Hijos.Add(hijoParA);
                match(TipoToken.parentesisA);

                Nodo hijoCondicion = new Nodo(padre);
                hijoCondicion.Name = "Condicion Foreach";
                hijoCondicion =CONDICION_FOREACH(hijoCondicion);
                padre.Hijos.Add(hijoCondicion);
                
                Nodo hijoParC = new Nodo(padre);
                hijoParC.Token = tokSig.token;
                padre.Hijos.Add(hijoParC);
                match(TipoToken.parentesisC);

                Nodo hijoLlaveA = new Nodo(padre);
                hijoLlaveA.Token = tokSig.token;
                padre.Hijos.Add(hijoLlaveA);
                match(TipoToken.llaveA);

                Nodo hijoInMain = new Nodo(padre);
                hijoInMain.Name = "In Foreach";
                hijoInMain = INMAINN(hijoInMain);
                padre.Hijos.Add(hijoInMain);


                Nodo hijoLlaveC = new Nodo(padre);
                hijoLlaveC.Token = tokSig.token;
                padre.Hijos.Add(hijoLlaveC);
                match(TipoToken.llaveC);
                
            }
            else if (tokSig.token.tipoToken == TipoToken.pWhile)
            {
                Nodo hijoWhile = new Nodo(padre);
                hijoWhile.Token = tokSig.token;
                padre.Hijos.Add(hijoWhile);
                match(TipoToken.pWhile);

                Nodo hijoParentesisA = new Nodo(padre);
                hijoParentesisA.Token = tokSig.token;
                padre.Hijos.Add(hijoParentesisA);
                match(TipoToken.parentesisA);
                
                Nodo hijoCondicion = new Nodo(padre);
                hijoCondicion.Name = "Condicion While";
                hijoCondicion = CONDICION_IF(hijoCondicion);
                padre.Hijos.Add(hijoCondicion);

                Nodo hijoParentesisC = new Nodo(padre);
                hijoParentesisC.Token = tokSig.token;
                padre.Hijos.Add(hijoParentesisC);
                match(TipoToken.parentesisC);

                Nodo hijoLlaveA = new Nodo(padre);
                hijoLlaveA.Token = tokSig.token;
                padre.Hijos.Add(hijoLlaveA);
                match(TipoToken.llaveA);

                Nodo hijoInMain = new Nodo(padre);
                hijoInMain.Name = "In Foreach";
                hijoInMain = INMAINN(hijoInMain);
                padre.Hijos.Add(hijoInMain);

                Nodo hijoLlaveC = new Nodo(padre);
                hijoLlaveC.Token = tokSig.token;
                padre.Hijos.Add(hijoLlaveC);
                match(TipoToken.llaveC);

            }
            return padre;
            
        }
        private Nodo ELSEIFN(Nodo padre)
        {
            
            if(tokSig.token.tipoToken != TipoToken.pElse)
            {
                return padre;
            }
            padre = ELSEIF(padre);
            return ELSEIFN(padre);
        }
        private Nodo ELSEIF(Nodo padre)
        {
            Nodo hijoElse = new Nodo(padre);
            hijoElse.Token = tokSig.token;
            padre.Hijos.Add(hijoElse);
            match(TipoToken.pElse);
            
            if(tokSig.token.tipoToken == TipoToken.pIf)
            {
                Nodo hijoIf = new Nodo(padre);
                hijoIf.Token = tokSig.token;
                padre.Hijos.Add(hijoIf);
                match(TipoToken.pIf);

                Nodo hijoParA = new Nodo(padre);
                hijoParA.Token = tokSig.token;
                padre.Hijos.Add(hijoParA);
                match(TipoToken.parentesisA);

                Nodo hijoCondicion = new Nodo(padre);
                hijoCondicion.Name = "Condicion IF";
                hijoCondicion = CONDICION_IF(hijoCondicion);
                padre.Hijos.Add(hijoCondicion);

                Nodo hijoParC = new Nodo(padre);
                hijoParC.Token = tokSig.token;
                padre.Hijos.Add(hijoParC);
                match(TipoToken.parentesisC);

                Nodo hijoLlaveA = new Nodo(padre);
                hijoLlaveA.Token = tokSig.token;
                padre.Hijos.Add(hijoLlaveA);
                match(TipoToken.llaveA);

                Nodo hijoInMain = new Nodo(padre);
                hijoInMain.Name = "In Else If";
                hijoInMain = INMAINN(hijoInMain);
                padre.Hijos.Add(hijoInMain);

                Nodo hijoLlaveC = new Nodo(padre);
                hijoLlaveC.Token = tokSig.token;
                padre.Hijos.Add(hijoLlaveC);
                match(TipoToken.llaveC);
            }
            else
            {
                Nodo hijoLlaveAElse = new Nodo(padre);
                hijoLlaveAElse.Token = tokSig.token;
                padre.Hijos.Add(hijoLlaveAElse);
                match(TipoToken.llaveA);

                Nodo hijoInMain2 = new Nodo(padre);
                hijoInMain2.Name = "In Else";
                hijoInMain2 = INMAINN(hijoInMain2);
                padre.Hijos.Add(hijoInMain2);

                Nodo hijoLlaveCelse = new Nodo(padre);
                hijoLlaveCelse.Token = tokSig.token;
                padre.Hijos.Add(hijoLlaveCelse);
                match(TipoToken.llaveC);
            }
            return padre;            
        }
        private Nodo CONDICION_IF(Nodo padre)
        {
            if (tokSig.token.tipoToken == TipoToken.numeroEntero)
            {
                Nodo hijonumeroEntero = new Nodo(padre);
                hijonumeroEntero.Token = tokSig.token;
                padre.Hijos.Add(hijonumeroEntero);
                match(TipoToken.numeroEntero);

                if (tipoOperador.Contains(tokSig.token.tipoToken))
                {
                    if (tokSig.token.tipoToken == TipoToken.sIgualIgual)
                    {
                        Nodo hijosIgualIgual = new Nodo(padre);
                        hijosIgualIgual.Token = tokSig.token;
                        padre.Hijos.Add(hijosIgualIgual);
                        match(TipoToken.sIgualIgual);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sMayor)
                    {
                        Nodo hijosMayor = new Nodo(padre);
                        hijosMayor.Token = tokSig.token;
                        padre.Hijos.Add(hijosMayor);
                        match(TipoToken.sMayor);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sMayorIgual)
                    {
                        Nodo hijosMayorIgual = new Nodo(padre);
                        hijosMayorIgual.Token = tokSig.token;
                        padre.Hijos.Add(hijosMayorIgual);
                        match(TipoToken.sMayorIgual);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sMenor)
                    {
                        Nodo hijosMenor = new Nodo(padre);
                        hijosMenor.Token = tokSig.token;
                        padre.Hijos.Add(hijosMenor);
                        match(TipoToken.sMenor);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sMenorIgual)
                    {
                        Nodo hijosMenorIgual = new Nodo(padre);
                        hijosMenorIgual.Token = tokSig.token;
                        padre.Hijos.Add(hijosMenorIgual);
                        match(TipoToken.sMenorIgual);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sDistintoQue)
                    {
                        Nodo hijosDistintoQue = new Nodo(padre);
                        hijosDistintoQue.Token = tokSig.token;
                        padre.Hijos.Add(hijosDistintoQue);
                        match(TipoToken.sDistintoQue);
                    }


                    if (tokSig.token.tipoToken == TipoToken.numeroEntero)
                    {
                        Nodo hijonumeroEntero2 = new Nodo(padre);
                        hijonumeroEntero2.Token = tokSig.token;
                        padre.Hijos.Add(hijonumeroEntero2);
                        match(TipoToken.numeroEntero);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.numeroFloat)
                    {
                        Nodo hijonumeroFloat = new Nodo(padre);
                        hijonumeroFloat.Token = tokSig.token;
                        padre.Hijos.Add(hijonumeroFloat);
                        match(TipoToken.numeroFloat);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.pFalse)
                    {
                        Nodo hijopFalse = new Nodo(padre);
                        hijopFalse.Token = tokSig.token;
                        padre.Hijos.Add(hijopFalse);
                        match(TipoToken.pFalse);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.pTrue)
                    {
                        Nodo hijopTrue = new Nodo(padre);
                        hijopTrue.Token = tokSig.token;
                        padre.Hijos.Add(hijopTrue);
                        match(TipoToken.pTrue);
                    }
                    else
                    {
                       padre = IDENTIFICADOR(padre);
                    }
                }
            }
            else if (tokSig.token.tipoToken == TipoToken.numeroFloat)
            {
                Nodo hijonumeroFloat = new Nodo(padre);
                hijonumeroFloat.Token = tokSig.token;
                padre.Hijos.Add(hijonumeroFloat);
                match(TipoToken.numeroFloat);

                if (tipoOperador.Contains(tokSig.token.tipoToken))
                {
                    if (tokSig.token.tipoToken == TipoToken.sIgualIgual)
                    {
                        Nodo hijosIgualIgual = new Nodo(padre);
                        hijosIgualIgual.Token = tokSig.token;
                        padre.Hijos.Add(hijosIgualIgual);
                        match(TipoToken.sIgualIgual);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sMayor)
                    {
                        Nodo hijosMayor = new Nodo(padre);
                        hijosMayor.Token = tokSig.token;
                        padre.Hijos.Add(hijosMayor);
                        match(TipoToken.sMayor);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sMayorIgual)
                    {
                        Nodo hijosMayorIgual = new Nodo(padre);
                        hijosMayorIgual.Token = tokSig.token;
                        padre.Hijos.Add(hijosMayorIgual);
                        match(TipoToken.sMayorIgual);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sMenor)
                    {
                        Nodo hijosMenor = new Nodo(padre);
                        hijosMenor.Token = tokSig.token;
                        padre.Hijos.Add(hijosMenor);
                        match(TipoToken.sMenor);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sMenorIgual)
                    {
                        Nodo hijosMenorIgual = new Nodo(padre);
                        hijosMenorIgual.Token = tokSig.token;
                        padre.Hijos.Add(hijosMenorIgual);
                        match(TipoToken.sMenorIgual);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sDistintoQue)
                    {
                        Nodo hijosDistintoQue = new Nodo(padre);
                        hijosDistintoQue.Token = tokSig.token;
                        padre.Hijos.Add(hijosDistintoQue);
                        match(TipoToken.sDistintoQue);
                    }


                    if (tokSig.token.tipoToken == TipoToken.numeroEntero)
                    {
                        Nodo hijonumeroEntero2 = new Nodo(padre);
                        hijonumeroEntero2.Token = tokSig.token;
                        padre.Hijos.Add(hijonumeroEntero2);
                        match(TipoToken.numeroEntero);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.numeroFloat)
                    {
                        Nodo hijonumeroFloat2 = new Nodo(padre);
                        hijonumeroFloat2.Token = tokSig.token;
                        padre.Hijos.Add(hijonumeroFloat2);
                        match(TipoToken.numeroFloat);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.pFalse)
                    {
                        Nodo hijopFalse = new Nodo(padre);
                        hijopFalse.Token = tokSig.token;
                        padre.Hijos.Add(hijopFalse);
                        match(TipoToken.pFalse);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.pTrue)
                    {
                        Nodo hijopTrue = new Nodo(padre);
                        hijopTrue.Token = tokSig.token;
                        padre.Hijos.Add(hijopTrue);
                        match(TipoToken.pTrue);
                    }
                    else
                    {
                        padre = IDENTIFICADOR(padre);
                    }
                }
            }
            else if (tokSig.token.tipoToken == TipoToken.pFalse)
            {
                Nodo hijoFalse = new Nodo(padre);
                hijoFalse.Token = tokSig.token;
                padre.Hijos.Add(hijoFalse);
                match(TipoToken.pFalse);

                if (tipoOperador.Contains(tokSig.token.tipoToken))
                {
                    if (tokSig.token.tipoToken == TipoToken.sIgualIgual)
                    {
                        Nodo hijosIgualIgual = new Nodo(padre);
                        hijosIgualIgual.Token = tokSig.token;
                        padre.Hijos.Add(hijosIgualIgual);
                        match(TipoToken.sIgualIgual);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sMayor)
                    {
                        Nodo hijosMayor = new Nodo(padre);
                        hijosMayor.Token = tokSig.token;
                        padre.Hijos.Add(hijosMayor);
                        match(TipoToken.sMayor);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sMayorIgual)
                    {
                        Nodo hijosMayorIgual = new Nodo(padre);
                        hijosMayorIgual.Token = tokSig.token;
                        padre.Hijos.Add(hijosMayorIgual);
                        match(TipoToken.sMayorIgual);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sMenor)
                    {
                        Nodo hijosMenor = new Nodo(padre);
                        hijosMenor.Token = tokSig.token;
                        padre.Hijos.Add(hijosMenor);
                        match(TipoToken.sMenor);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sMenorIgual)
                    {
                        Nodo hijosMenorIgual = new Nodo(padre);
                        hijosMenorIgual.Token = tokSig.token;
                        padre.Hijos.Add(hijosMenorIgual);
                        match(TipoToken.sMenorIgual);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sDistintoQue)
                    {
                        Nodo hijosDistintoQue = new Nodo(padre);
                        hijosDistintoQue.Token = tokSig.token;
                        padre.Hijos.Add(hijosDistintoQue);
                        match(TipoToken.sDistintoQue);
                    }


                    if (tokSig.token.tipoToken == TipoToken.numeroEntero)
                    {
                        Nodo hijonumeroEntero2 = new Nodo(padre);
                        hijonumeroEntero2.Token = tokSig.token;
                        padre.Hijos.Add(hijonumeroEntero2);
                        match(TipoToken.numeroEntero);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.numeroFloat)
                    {
                        Nodo hijonumeroFloat2 = new Nodo(padre);
                        hijonumeroFloat2.Token = tokSig.token;
                        padre.Hijos.Add(hijonumeroFloat2);
                        match(TipoToken.numeroFloat);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.pFalse)
                    {
                        Nodo hijopFalse = new Nodo(padre);
                        hijopFalse.Token = tokSig.token;
                        padre.Hijos.Add(hijopFalse);
                        match(TipoToken.pFalse);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.pTrue)
                    {
                        Nodo hijopTrue = new Nodo(padre);
                        hijopTrue.Token = tokSig.token;
                        padre.Hijos.Add(hijopTrue);
                        match(TipoToken.pTrue);
                    }
                    else
                    {
                        padre = IDENTIFICADOR(padre);
                    }
                }
            }
            else if (tokSig.token.tipoToken == TipoToken.pTrue)
            {
                Nodo hijoTrue = new Nodo(padre);
                hijoTrue.Token = tokSig.token;
                padre.Hijos.Add(hijoTrue);
                match(TipoToken.pTrue);

                if (tipoOperador.Contains(tokSig.token.tipoToken))
                {
                    if (tokSig.token.tipoToken == TipoToken.sIgualIgual)
                    {
                        Nodo hijosIgualIgual = new Nodo(padre);
                        hijosIgualIgual.Token = tokSig.token;
                        padre.Hijos.Add(hijosIgualIgual);
                        match(TipoToken.sIgualIgual);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sMayor)
                    {
                        Nodo hijosMayor = new Nodo(padre);
                        hijosMayor.Token = tokSig.token;
                        padre.Hijos.Add(hijosMayor);
                        match(TipoToken.sMayor);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sMayorIgual)
                    {
                        Nodo hijosMayorIgual = new Nodo(padre);
                        hijosMayorIgual.Token = tokSig.token;
                        padre.Hijos.Add(hijosMayorIgual);
                        match(TipoToken.sMayorIgual);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sMenor)
                    {
                        Nodo hijosMenor = new Nodo(padre);
                        hijosMenor.Token = tokSig.token;
                        padre.Hijos.Add(hijosMenor);
                        match(TipoToken.sMenor);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sMenorIgual)
                    {
                        Nodo hijosMenorIgual = new Nodo(padre);
                        hijosMenorIgual.Token = tokSig.token;
                        padre.Hijos.Add(hijosMenorIgual);
                        match(TipoToken.sMenorIgual);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sDistintoQue)
                    {
                        Nodo hijosDistintoQue = new Nodo(padre);
                        hijosDistintoQue.Token = tokSig.token;
                        padre.Hijos.Add(hijosDistintoQue);
                        match(TipoToken.sDistintoQue);
                    }


                    if (tokSig.token.tipoToken == TipoToken.numeroEntero)
                    {
                        Nodo hijonumeroEntero2 = new Nodo(padre);
                        hijonumeroEntero2.Token = tokSig.token;
                        padre.Hijos.Add(hijonumeroEntero2);
                        match(TipoToken.numeroEntero);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.numeroFloat)
                    {
                        Nodo hijonumeroFloat2 = new Nodo(padre);
                        hijonumeroFloat2.Token = tokSig.token;
                        padre.Hijos.Add(hijonumeroFloat2);
                        match(TipoToken.numeroFloat);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.pFalse)
                    {
                        Nodo hijopFalse = new Nodo(padre);
                        hijopFalse.Token = tokSig.token;
                        padre.Hijos.Add(hijopFalse);
                        match(TipoToken.pFalse);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.pTrue)
                    {
                        Nodo hijopTrue = new Nodo(padre);
                        hijopTrue.Token = tokSig.token;
                        padre.Hijos.Add(hijopTrue);
                        match(TipoToken.pTrue);
                    }
                    else
                    {
                        padre = IDENTIFICADOR(padre);
                    }
                }
            }
            else if(tokSig.token.tipoToken == TipoToken.identificador)
            {
                padre = IDENTIFICADOR(padre);

                if (tipoOperador.Contains(tokSig.token.tipoToken))
                {
                    if (tokSig.token.tipoToken == TipoToken.sIgualIgual)
                    {
                        Nodo hijosIgualIgual = new Nodo(padre);
                        hijosIgualIgual.Token = tokSig.token;
                        padre.Hijos.Add(hijosIgualIgual);
                        match(TipoToken.sIgualIgual);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sMayor)
                    {
                        Nodo hijosMayor = new Nodo(padre);
                        hijosMayor.Token = tokSig.token;
                        padre.Hijos.Add(hijosMayor);
                        match(TipoToken.sMayor);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sMayorIgual)
                    {
                        Nodo hijosMayorIgual = new Nodo(padre);
                        hijosMayorIgual.Token = tokSig.token;
                        padre.Hijos.Add(hijosMayorIgual);
                        match(TipoToken.sMayorIgual);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sMenor)
                    {
                        Nodo hijosMenor = new Nodo(padre);
                        hijosMenor.Token = tokSig.token;
                        padre.Hijos.Add(hijosMenor);
                        match(TipoToken.sMenor);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sMenorIgual)
                    {
                        Nodo hijosMenorIgual = new Nodo(padre);
                        hijosMenorIgual.Token = tokSig.token;
                        padre.Hijos.Add(hijosMenorIgual);
                        match(TipoToken.sMenorIgual);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sDistintoQue)
                    {
                        Nodo hijosDistintoQue = new Nodo(padre);
                        hijosDistintoQue.Token = tokSig.token;
                        padre.Hijos.Add(hijosDistintoQue);
                        match(TipoToken.sDistintoQue);
                    }


                    if (tokSig.token.tipoToken == TipoToken.numeroEntero)
                    {
                        Nodo hijonumeroEntero2 = new Nodo(padre);
                        hijonumeroEntero2.Token = tokSig.token;
                        padre.Hijos.Add(hijonumeroEntero2);
                        match(TipoToken.numeroEntero);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.numeroFloat)
                    {
                        Nodo hijonumeroFloat2 = new Nodo(padre);
                        hijonumeroFloat2.Token = tokSig.token;
                        padre.Hijos.Add(hijonumeroFloat2);
                        match(TipoToken.numeroFloat);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.pFalse)
                    {
                        Nodo hijopFalse = new Nodo(padre);
                        hijopFalse.Token = tokSig.token;
                        padre.Hijos.Add(hijopFalse);
                        match(TipoToken.pFalse);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.pTrue)
                    {
                        Nodo hijopTrue = new Nodo(padre);
                        hijopTrue.Token = tokSig.token;
                        padre.Hijos.Add(hijopTrue);
                        match(TipoToken.pTrue);
                    }
                    else
                    {
                        padre = IDENTIFICADOR(padre);
                    }
                }
            }
            else if(tokSig.token.tipoToken == TipoToken.sExcla)
            {
                Nodo hijosExcla = new Nodo(padre);
                hijosExcla.Token = tokSig.token;
                padre.Hijos.Add(hijosExcla);
                match(TipoToken.sExcla);

                padre = IDENTIFICADOR(padre);
            }

            if (tokSig.token.tipoToken == TipoToken.sAmpr)
            {
                match(TipoToken.sAmpr);
                match(TipoToken.sAmpr);
                padre = CONDICION_IF(padre);
            }
            else if (tokSig.token.tipoToken == TipoToken.sOr)
            {
                match(TipoToken.sOr);
                match(TipoToken.sOr);
                padre = CONDICION_IF(padre);
            }
            return padre;


        }
        private Nodo CONDICION_FOREACH(Nodo padre)
        {
            if (tokSig.token.tipoToken == TipoToken.identificador)
            {
                padre = IDENTIFICADOR(padre);
            }
            else if (tokSig.token.tipoToken == TipoToken.pInt)
            {
                Nodo hijoInt = new Nodo(padre);
                hijoInt.Token = tokSig.token;
                padre.Hijos.Add(hijoInt);
                match(TipoToken.pInt);
            }
            else if (tokSig.token.tipoToken == TipoToken.pBool)
            {
                Nodo pBool = new Nodo(padre);
                pBool.Token = tokSig.token;
                padre.Hijos.Add(pBool);
                match(TipoToken.pBool);
            }
            else if (tokSig.token.tipoToken == TipoToken.pDatetime)
            {
                Nodo pDatetime = new Nodo(padre);
                pDatetime.Token = tokSig.token;
                padre.Hijos.Add(pDatetime);
                match(TipoToken.pDatetime);
            }
            else if (tokSig.token.tipoToken == TipoToken.pFloat)
            {
                Nodo hijopFloat = new Nodo(padre);
                hijopFloat.Token = tokSig.token;
                padre.Hijos.Add(hijopFloat);
                match(TipoToken.pFloat);
            }

            padre = IDENTIFICADOR(padre);

            Nodo hijoIn = new Nodo(padre);
            hijoIn.Token = tokSig.token;
            padre.Hijos.Add(hijoIn);
            match(TipoToken.pIn);

            padre = IDENTIFICADOR(padre);
            return padre;


        }
        private Nodo DECLARACION(Nodo padre)
        {
            if (tokSig.token.tipoToken == TipoToken.pInt)
            {
                Nodo hijoInt = new Nodo(padre);
                hijoInt.Token = tokSig.token;
                padre.Hijos.Add(hijoInt);
                match(TipoToken.pInt);
            }
            else if (tokSig.token.tipoToken == TipoToken.pBool)
            {
                Nodo hijoBool = new Nodo(padre);
                hijoBool.Token = tokSig.token;
                padre.Hijos.Add(hijoBool);
                match(TipoToken.pBool);
            }
            else if (tokSig.token.tipoToken == TipoToken.pFloat)
            {
                Nodo hijoFloat = new Nodo(padre);
                hijoFloat.Token = tokSig.token;
                padre.Hijos.Add(hijoFloat);
                match(TipoToken.pFloat);
            }
            else if (tokSig.token.tipoToken == TipoToken.pDatetime)
            {
                Nodo hijoDateTime = new Nodo(padre);
                hijoDateTime.Token = tokSig.token;
                padre.Hijos.Add(hijoDateTime);
                match(TipoToken.pDatetime);
            }
            else if (tokSig.token.tipoToken == TipoToken.pVar)
            {
                Nodo hijoVar = new Nodo(padre);
                hijoVar.Token = tokSig.token;
                padre.Hijos.Add(hijoVar);
                match(TipoToken.pVar);
            }
            
            
            if (tokSig.token.tipoToken == TipoToken.bracketA)
            {
                Nodo hijobracketA = new Nodo(padre);
                hijobracketA.Token = tokSig.token;
                padre.Hijos.Add(hijobracketA);
                match(TipoToken.bracketA);

                Nodo hijobracketC = new Nodo(padre);
                hijobracketC.Token = tokSig.token;
                padre.Hijos.Add(hijobracketC);
                match(TipoToken.bracketC);
            }

            padre = IDENTIFICADOR(padre);

            if(tokSig.token.tipoToken == TipoToken.sPuntoComa)
            {
                Nodo hijoPc = new Nodo(padre);
                hijoPc.Token = tokSig.token;
                padre.Hijos.Add(hijoPc);
                match(TipoToken.sPuntoComa);
            }             
            else if(tokSig.token.tipoToken == TipoToken.sIgual)
            {
                Nodo hijoIgual = new Nodo(padre);
                hijoIgual.Token = tokSig.token;
                padre.Hijos.Add(hijoIgual);
                match(TipoToken.sIgual);

                padre = DECLARACION_IGUAL(padre);
            }
            return padre;

        }
        private Nodo DECLARACION_IGUAL(Nodo padre)
        {
            if (this.tokSig.token.tipoToken == TipoToken.numeroEntero)
            {
                Nodo hijoNumeroEntero = new Nodo(padre);
                hijoNumeroEntero.Token = tokSig.token;
                padre.Hijos.Add(hijoNumeroEntero);
                match(TipoToken.numeroEntero);

                if (tipoAritm.Contains(tokSig.token.tipoToken))
                {
                    padre = OPERACIONARITMETICA(padre);
                }

                Nodo hijoPc = new Nodo(padre);
                hijoPc.Token = tokSig.token;
                padre.Hijos.Add(hijoPc);
                match(TipoToken.sPuntoComa);

                return padre;
            }
            else if (this.tokSig.token.tipoToken == TipoToken.numeroFloat)
            {
                Nodo hijoNumeroFloat = new Nodo(padre);
                hijoNumeroFloat.Token = tokSig.token;
                padre.Hijos.Add(hijoNumeroFloat);
                match(TipoToken.numeroFloat);
                
                if (tipoAritm.Contains(tokSig.token.tipoToken))
                {
                    padre = OPERACIONARITMETICA(padre);
                }

                Nodo hijoPc = new Nodo(padre);
                hijoPc.Token = tokSig.token;
                padre.Hijos.Add(hijoPc);
                match(TipoToken.sPuntoComa);

                return padre; ;
            }
            else if (this.tokSig.token.tipoToken == TipoToken.pFalse)
            {
                Nodo hijoFalse = new Nodo(padre);
                hijoFalse.Token = tokSig.token;
                padre.Hijos.Add(hijoFalse);
                match(TipoToken.pFalse);

                Nodo hijoPc = new Nodo(padre);
                hijoPc.Token = tokSig.token;
                padre.Hijos.Add(hijoPc);
                match(TipoToken.sPuntoComa);

                return padre;
            }
            else if (this.tokSig.token.tipoToken == TipoToken.pTrue)
            {
                Nodo hijoTrue = new Nodo(padre);
                hijoTrue.Token = tokSig.token;
                padre.Hijos.Add(hijoTrue);
                match(TipoToken.pTrue);

                Nodo hijoPc = new Nodo(padre);
                hijoPc.Token = tokSig.token;
                padre.Hijos.Add(hijoPc);
                match(TipoToken.sPuntoComa);

                return padre;
            }
            else if (this.tokSig.token.tipoToken == TipoToken.pNew)
            {
                //new DateTime(año, mes, dia, hora, minuto, segundo);
                Nodo hijoNew = new Nodo(padre);
                hijoNew.Token = tokSig.token;
                padre.Hijos.Add(hijoNew);
                match(TipoToken.pNew);

                Nodo hijoDateTime = new Nodo(padre);
                hijoDateTime.Token = tokSig.token;
                padre.Hijos.Add(hijoDateTime);
                match(TipoToken.pDatetime);

                Nodo hijoParA = new Nodo(padre);
                hijoParA.Token = tokSig.token;
                padre.Hijos.Add(hijoParA);
                match(TipoToken.parentesisA);

                Nodo hijoNumeroEntero = new Nodo(padre);
                hijoNumeroEntero.Token = tokSig.token;
                padre.Hijos.Add(hijoNumeroEntero);
                match(TipoToken.numeroEntero);

                Nodo hijoComa1 = new Nodo(padre);
                hijoComa1.Token = tokSig.token;
                padre.Hijos.Add(hijoComa1);
                match(TipoToken.sComa);

                Nodo hijoNumeroEntero1 = new Nodo(padre);
                hijoNumeroEntero1.Token = tokSig.token;
                padre.Hijos.Add(hijoNumeroEntero1);
                match(TipoToken.numeroEntero);

                Nodo hijoComa2 = new Nodo(padre);
                hijoComa2.Token = tokSig.token;
                padre.Hijos.Add(hijoComa2);
                match(TipoToken.sComa);

                Nodo hijoNumeroEntero2 = new Nodo(padre);
                hijoNumeroEntero2.Token = tokSig.token;
                padre.Hijos.Add(hijoNumeroEntero2);
                match(TipoToken.numeroEntero);

                Nodo hijoComa3 = new Nodo(padre);
                hijoComa3.Token = tokSig.token;
                padre.Hijos.Add(hijoComa3);
                match(TipoToken.sComa);

                Nodo hijoNumeroEntero3 = new Nodo(padre);
                hijoNumeroEntero3.Token = tokSig.token;
                padre.Hijos.Add(hijoNumeroEntero3);
                match(TipoToken.numeroEntero);

                Nodo hijoComa4 = new Nodo(padre);
                hijoComa4.Token = tokSig.token;
                padre.Hijos.Add(hijoComa4);
                match(TipoToken.sComa);

                Nodo hijoNumeroEntero4 = new Nodo(padre);
                hijoNumeroEntero4.Token = tokSig.token;
                padre.Hijos.Add(hijoNumeroEntero4);
                match(TipoToken.numeroEntero);

                Nodo hijoComa5 = new Nodo(padre);
                hijoComa5.Token = tokSig.token;
                padre.Hijos.Add(hijoComa5);
                match(TipoToken.sComa);

                Nodo hijoNumeroEntero5 = new Nodo(padre);
                hijoNumeroEntero5.Token = tokSig.token;
                padre.Hijos.Add(hijoNumeroEntero5);
                match(TipoToken.numeroEntero);

                Nodo hijoParC = new Nodo(padre);
                hijoParC.Token = tokSig.token;
                padre.Hijos.Add(hijoParC);
                match(TipoToken.parentesisC);

                Nodo hijoPc = new Nodo(padre);
                hijoPc.Token = tokSig.token;
                padre.Hijos.Add(hijoPc);
                match(TipoToken.sPuntoComa);

                return padre;
            }
            else if (tokSig.token.tipoToken == TipoToken.pConsole)
            {
                Nodo hijoConsole = new Nodo(padre);
                hijoConsole.Token = tokSig.token;
                padre.Hijos.Add(hijoConsole);
                match(TipoToken.pConsole);

                Nodo hijoP = new Nodo(padre);
                hijoP.Token = tokSig.token;
                padre.Hijos.Add(hijoP);
                match(TipoToken.sPunto);

                Nodo hijoReadLine = new Nodo(padre);
                hijoReadLine.Token = tokSig.token;
                padre.Hijos.Add(hijoReadLine);
                match(TipoToken.pReadLine);

                Nodo hijoParA = new Nodo(padre);
                hijoParA.Token = tokSig.token;
                padre.Hijos.Add(hijoParA);
                match(TipoToken.parentesisA);

                Nodo hijoParC = new Nodo(padre);
                hijoParC.Token = tokSig.token;
                padre.Hijos.Add(hijoParC);
                match(TipoToken.parentesisC);

                Nodo hijoPc = new Nodo(padre);
                hijoPc.Token = tokSig.token;
                padre.Hijos.Add(hijoPc);
                match(TipoToken.sPuntoComa);

                return padre;
            }
            else if (tokSig.token.tipoToken == TipoToken.parentesisA)
            {
                padre = OPERACIONARITMETICA(padre);

                Nodo hijoPc = new Nodo(padre);
                hijoPc.Token = tokSig.token;
                padre.Hijos.Add(hijoPc);
                match(TipoToken.sPuntoComa);
                
                return padre;
            }
            else if (tokSig.token.tipoToken == TipoToken.identificador)
            {
                padre = IDENTIFICADOR(padre);
                if (tokSig.token.tipoToken == TipoToken.parentesisA)
                {
                    padre = FUNCIONCALL(padre);
                    return padre;
                }
                else if (tipoAritm.Contains(tokSig.token.tipoToken))
                {
                    padre = OPERACIONARITMETICA(padre);

                    Nodo hijoPc = new Nodo(padre);
                    hijoPc.Token = tokSig.token;
                    padre.Hijos.Add(hijoPc);
                    match(TipoToken.sPuntoComa);
                    
                    return padre;
                }
                else
                {
                    Nodo hijoPc = new Nodo(padre);
                    hijoPc.Token = tokSig.token;
                    padre.Hijos.Add(hijoPc);
                    match(TipoToken.sPuntoComa);

                    return padre;
                }
            }
            else
                return padre;

        }
        private Nodo OPERACIONARITMETICA(Nodo padre)
        {
            while(tokSig.token.tipoToken==TipoToken.numeroEntero || tokSig.token.tipoToken == TipoToken.numeroFloat || tipoAritm.Contains(tokSig.token.tipoToken))
            {
                if (tokSig.token.tipoToken == TipoToken.parentesisA)
                {
                    Nodo hijoParA = new Nodo(padre);
                    hijoParA.Token = tokSig.token;
                    padre.Hijos.Add(hijoParA);
                    match(TipoToken.parentesisA);

                    padre = OPERACIONARITMETICA(padre);

                    Nodo hijoParC = new Nodo(padre);
                    hijoParC.Token = tokSig.token;
                    padre.Hijos.Add(hijoParC);
                    match(TipoToken.parentesisC);
                }
                else if (tokSig.token.tipoToken == TipoToken.numeroEntero)
                {
                    Nodo hijoParnumeroEntero = new Nodo(padre);
                    hijoParnumeroEntero.Token = tokSig.token;
                    padre.Hijos.Add(hijoParnumeroEntero);
                    match(TipoToken.numeroEntero);
                }
                else if (tokSig.token.tipoToken == TipoToken.numeroFloat)
                {
                    Nodo hijonumeroFloat = new Nodo(padre);
                    hijonumeroFloat.Token = tokSig.token;
                    padre.Hijos.Add(hijonumeroFloat);
                    match(TipoToken.numeroFloat);
                }
                else if (tokSig.token.tipoToken == TipoToken.sMas)
                {
                    Nodo hijosMas = new Nodo(padre);
                    hijosMas.Token = tokSig.token;
                    padre.Hijos.Add(hijosMas);
                    match(TipoToken.sMas);
                }
                else if (tokSig.token.tipoToken == TipoToken.sMenos)
                {
                    Nodo hijosMenos = new Nodo(padre);
                    hijosMenos.Token = tokSig.token;
                    padre.Hijos.Add(hijosMenos);
                    match(TipoToken.sMenos);
                }
                else if (tokSig.token.tipoToken == TipoToken.sMult)
                {
                    Nodo hijoMult = new Nodo(padre);
                    hijoMult.Token = tokSig.token;
                    padre.Hijos.Add(hijoMult);
                    match(TipoToken.sMult);
                }
                else if (tokSig.token.tipoToken == TipoToken.sDiv)
                {
                    Nodo hijoDiv = new Nodo(padre);
                    hijoDiv.Token = tokSig.token;
                    padre.Hijos.Add(hijoDiv);
                    match(TipoToken.sDiv);
                }
                else if (tokSig.token.tipoToken == TipoToken.sMod)
                {
                    Nodo hijoMod = new Nodo(padre);
                    hijoMod.Token = tokSig.token;
                    padre.Hijos.Add(hijoMod);
                    match(TipoToken.sMod);
                }
                else
                    return padre;
            }
            return padre;
        }
    }
} 