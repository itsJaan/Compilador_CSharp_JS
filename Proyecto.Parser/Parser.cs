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
        private Ambiente ambienteRaiz;
        private int tabs;
        private Nodo nMain;
        
        public Parser(IEscaner sc)
        {
            escaner = sc;
            errores = new List<Error>();
            Raiz = new Nodo(null);
            Ambiente amb = new Ambiente(null, "Anterior Raiz");
            ambienteRaiz = new Ambiente(amb , "Raiz");
            tabs = 0;
            nMain = new Nodo(Raiz);
        }
        private void move()
        {
            this.tokSig = this.escaner.proximoToken();
        }
        private void match(TipoToken tipo)
        {
            if(this.tokSig.token.tipoToken != tipo)
            {
                Error error = new Error(tokSig.token.fila, tokSig.token.columna,tokSig.token.tipoToken.ToString(), tipo.ToString());
                errores.Add(error);
                return;
            }
            else
                move();
        }
        public void Parsear()
        {
            move();
            UC();
            if (errores.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                foreach(Error e in errores)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"Syntax Error: Se recibio {e.lexema} en Fila: {e.fila} Columna: {e.columna} se esperaba un {e.waiting}");
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


            string key = "";
            foreach(Nodo n in padre.Hijos)
            {
                if (n.Token != null)
                {
                    if (n.Token.tipoToken == TipoToken.identificador || n.Token.tipoToken == TipoToken.sPunto)
                    {
                        key += n.Token.Lexema;
                    }
                }
            }

            bool exists = ambienteRaiz.Add(key,padre , ambienteRaiz);
            if(!exists)
                ambienteRaiz.ValidImport(key ,padre);

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
            Ambiente ambienteClase = new Ambiente(ambienteRaiz , "Clase");
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

            string key = "";
            foreach (Nodo n in padre.Hijos)
            {
                if (n.Token != null) {
                    if (n.Token.tipoToken == TipoToken.identificador)
                    {
                        key += n.Token.Lexema;
                    }
                }
            }
            bool exists = ambienteRaiz.Add(key, padre , ambienteRaiz);
            if (!exists)
                ambienteRaiz.ValidClassDeclaration(key, padre);

            Nodo hijoInClass = new Nodo(padre);
            hijoInClass.Name = "InClass";
            hijoInClass = INCLASSN(hijoInClass, ambienteClase);
            padre.Hijos.Add(hijoInClass);

            Nodo hijoLlaveC = new Nodo(padre);
            hijoLlaveC.Token = tokSig.token;
            padre.Hijos.Add(hijoLlaveC);
            match(TipoToken.llaveC);


            if (!exists)
                ambienteRaiz.Close(tabs);
        
            return padre;
        }
        private Nodo INCLASSN(Nodo padre , Ambiente ambientePadre)
        {
            Nodo hijo = new Nodo(padre);
            if (tokSig.token.tipoToken == TipoToken.llaveC)
            {
                return padre;
            }
            hijo = INCLASS(hijo, ambientePadre);
            padre.Hijos.Add(hijo);
            return INCLASSN(padre , ambientePadre);
        }
        private Nodo INCLASS(Nodo padre, Ambiente ambientePadre)
        {
            tabs++;
            Ambiente ambienteInClass = new Ambiente(ambientePadre , "In Class");
            //SI ES MAIN
            if (this.tokSig.token.tipoToken == TipoToken.pStatic)
            {
                Console.WriteLine("//Main");
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
                hijoInMain = INMAINN(hijoInMain, ambienteInClass);
                padre.Hijos.Add(hijoInMain);

                Nodo hijoLlaveC = new Nodo(padre);
                hijoLlaveC.Token = tokSig.token;
                padre.Hijos.Add(hijoLlaveC);
                match(TipoToken.llaveC);

                nMain = padre;
                tabs--;
                return padre; 
            }
            //SI NO ES MAIN
            else if (this.tokSig.token.tipoToken == TipoToken.pPublic)
            {
                bool isList = false;

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
                else if (tokSig.token.tipoToken == TipoToken.pList)
                {
                    Nodo hijoList = new Nodo(padre);
                    hijoList.Token = tokSig.token;
                    padre.Hijos.Add(hijoList);
                    match(TipoToken.pList);

                    Nodo hijosMenor = new Nodo(padre);
                    hijosMenor.Token = tokSig.token;
                    padre.Hijos.Add(hijosMenor);
                    match(TipoToken.sMenor);

                    if (tokSig.token.tipoToken == TipoToken.identificador)
                    {
                        padre = IDENTIFICADOR(padre);
                    }
                    else if(tokSig.token.tipoToken == TipoToken.pInt)
                    {
                        Nodo hijoInt = new Nodo(padre);
                        hijoInt.Token = tokSig.token;
                        padre.Hijos.Add(hijoInt);
                        match(TipoToken.pInt);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.pFloat)
                    {
                        Nodo hijoInt = new Nodo(padre);
                        hijoInt.Token = tokSig.token;
                        padre.Hijos.Add(hijoInt);
                        match(TipoToken.pFloat);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.pDatetime)
                    {
                        Nodo hijoDateTime = new Nodo(padre);
                        hijoDateTime.Token = tokSig.token;
                        padre.Hijos.Add(hijoDateTime);
                        match(TipoToken.pDatetime);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.pBool)
                    {
                        Nodo hijoBool = new Nodo(padre);
                        hijoBool.Token = tokSig.token;
                        padre.Hijos.Add(hijoBool);
                        match(TipoToken.pBool);
                    }
                    Nodo hijoMayor = new Nodo(padre);
                    hijoMayor.Token = tokSig.token;
                    padre.Hijos.Add(hijoMayor);
                    match(TipoToken.sMayor);
                    isList = true;
                }


                padre = IDENTIFICADOR(padre);

                string key = "";
                foreach (Nodo n in padre.Hijos)
                {
                    if (n.Token != null)
                    {
                        if (n.Token.tipoToken == TipoToken.identificador)
                        {
                            key += n.Token.Lexema;
                        }
                    }
                }

                //FUNCION
                if (tokSig.token.tipoToken == TipoToken.parentesisA && !isList)
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

                        //Verificacion y asignacion parametros
                        foreach(Nodo hijo in hijoParametros.Hijos)
                        {
                            if (hijo.Token != null)
                            {
                                ambienteInClass.Add(hijo.Token.Lexema, hijo, ambienteInClass);
                            }
                        }
                    }
                    Nodo hijoParC = new Nodo(padre);
                    hijoParC.Token = tokSig.token;
                    padre.Hijos.Add(hijoParC);
                    match(TipoToken.parentesisC);

                    Nodo hijoLlaveA = new Nodo(padre);
                    hijoLlaveA.Token = tokSig.token;
                    padre.Hijos.Add(hijoLlaveA);
                    match(TipoToken.llaveA);

                    bool exists = ambientePadre.Add(key, padre,ambientePadre);
                    if (!exists)
                        ambientePadre.ValidFunctionDecl(key, padre , tabs);
                    
                    Nodo hijoInMain = new Nodo(padre);
                    hijoInMain.Name = "InFuncion";
                    hijoInMain = INMAINN(hijoInMain, ambienteInClass);
                    padre.Hijos.Add(hijoInMain);
                    
                    Nodo hijoLlaveC = new Nodo(padre);
                    hijoLlaveC.Token = tokSig.token;
                    padre.Hijos.Add(hijoLlaveC);
                    match(TipoToken.llaveC);

                    ambientePadre.Close(tabs);
                    tabs--;
                    return padre;
                }
                //DECLARACION PROP
                else if (tokSig.token.tipoToken == TipoToken.llaveA && !isList)
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

                    bool exists = ambientePadre.Add(key, padre, ambientePadre);
                    if (!exists)
                        ambientePadre.ValidDeclaration(key, padre, tabs);

                    tabs--;
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

                    bool exists = ambientePadre.Add(key, padre, ambientePadre);
                    if (!exists)
                        ambientePadre.ValidDeclaration(key, padre, tabs);
                    tabs--;
                    return padre;   
                }
                //DECLARACION NORMAL
                else if (tokSig.token.tipoToken == TipoToken.sPuntoComa)
                {
                    Nodo hijoPc = new Nodo(padre);
                    hijoPc.Token = tokSig.token;
                    padre.Hijos.Add(hijoPc);
                    match(TipoToken.sPuntoComa);

                    bool exists = ambientePadre.Add(key, padre, ambientePadre);
                    if (!exists)
                        ambientePadre.ValidDeclaration(key, padre, tabs);
                    
                    tabs--;
                    return padre;
                }
            }
            return null;
        }
        private Nodo INMAINN(Nodo padre , Ambiente ambientePadre)
        {
            Nodo hijo = new Nodo(padre);
            if (tokSig.token.tipoToken == TipoToken.llaveC)
            {
                return padre;
            }
            hijo = INMAIN(hijo, ambientePadre);
            padre.Hijos.Add(hijo);
            return INMAINN(padre, ambientePadre);

        }
        private Nodo INMAIN(Nodo padre ,Ambiente ambientePadre)
        {
            tabs++;
            
            
            if (tipoDato.Contains(tokSig.token.tipoToken))
            {
                Nodo hijoDecl = new Nodo(padre);
                hijoDecl.Name = "Declaracion";
                hijoDecl = DECLARACION(hijoDecl ,ambientePadre);
                padre.Hijos.Add(hijoDecl);
                tabs--;
                return padre;
            }
            else if (tipoStates.Contains(tokSig.token.tipoToken))
            {
                Nodo hijoSentencia = new Nodo(padre);
                hijoSentencia.Name = "Sentencia";
                hijoSentencia = SENTENCIA(hijoSentencia , ambientePadre);
                padre.Hijos.Add(hijoSentencia);
                tabs--;
                return padre;
            }
            else if (tokSig.token.tipoToken == TipoToken.pConsole)
            {
                bool existeVar = false;
                bool esId = false;
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

                string key = "";
                if(tokSig.token.tipoToken == TipoToken.strEntreComilla)
                {
                    Nodo hijoComilla = new Nodo(padre);
                    hijoComilla.Token = tokSig.token;
                    padre.Hijos.Add(hijoComilla);
                    key= hijoComilla.Token.Lexema;
                    match(TipoToken.strEntreComilla);
                }
                else if (tokSig.token.tipoToken == TipoToken.numeroEntero)
                {
                    Nodo hijoComilla = new Nodo(padre);
                    hijoComilla.Token = tokSig.token;
                    padre.Hijos.Add(hijoComilla);
                    key = hijoComilla.Token.Lexema;
                    match(TipoToken.numeroEntero);
                }
                else if (tokSig.token.tipoToken == TipoToken.identificador)
                {
                    Nodo hijoComilla = new Nodo(padre);
                    hijoComilla.Token = tokSig.token;
                    padre.Hijos.Add(hijoComilla);
                    key = hijoComilla.Token.Lexema;
                    match(TipoToken.identificador);

                    esId = true;
                    existeVar = ambientePadre.GetEnvironment(key, ambientePadre);


                }
                else if (tokSig.token.tipoToken == TipoToken.numeroFloat)
                {
                    Nodo hijoComilla = new Nodo(padre);
                    hijoComilla.Token = tokSig.token;
                    padre.Hijos.Add(hijoComilla);
                    key = hijoComilla.Token.Lexema;
                    match(TipoToken.numeroFloat);
                }
                else if (tokSig.token.tipoToken == TipoToken.operacionAritmetica)
                {
                    Nodo hijoComilla = new Nodo(padre);
                    hijoComilla.Token = tokSig.token;
                    padre.Hijos.Add(hijoComilla);
                    key = hijoComilla.Token.Lexema;
                    match(TipoToken.operacionAritmetica);
                }
                

                Nodo hijoParC = new Nodo(padre);
                hijoParC.Token = tokSig.token;
                padre.Hijos.Add(hijoParC);
                match(TipoToken.parentesisC);

                Nodo hijoPc = new Nodo(padre);
                hijoPc.Token = tokSig.token;
                padre.Hijos.Add(hijoPc);
                match(TipoToken.sPuntoComa);


                if (esId)
                {
                    if (existeVar)
                    {
                        ambientePadre.ValidConsole(key, padre, tabs);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"No existe variable {key}");
                    }
                }
                else
                    ambientePadre.ValidConsole(key, padre, tabs);
                

                tabs--;
                return padre;

            }
            else if (tokSig.token.tipoToken == TipoToken.identificador)
            {
                padre = IDENTIFICADOR(padre);
                string key = "";
                string keyAux = "";
                bool decl = false;
                bool asignCompleta = false;
                bool incDec = false;
                bool callF = false;
                bool existClase = false;
                foreach (Nodo n in padre.Hijos)
                {
                    if (n.Token != null)
                    {
                        if (n.Token.tipoToken == TipoToken.identificador || n.Token.tipoToken == TipoToken.sPunto)
                        {
                            key += n.Token.Lexema;
                        }
                    }
                }

                existClase = ambientePadre.GetEnvironment(key, ambientePadre);

                if (tokSig.token.tipoToken == TipoToken.sIgual)
                {
                    Nodo hijoIgual = new Nodo(padre);
                    hijoIgual.Token = tokSig.token;
                    padre.Hijos.Add(hijoIgual);
                    match(TipoToken.sIgual);
                    padre = DECLARACION_IGUAL(padre);

                    asignCompleta = true;
                
                }
                else if(tokSig.token.tipoToken == TipoToken.identificador)
                {
                    padre = IDENTIFICADOR(padre);

                    foreach (Nodo n in padre.Hijos)
                    {
                        if (n.Token != null)
                        {
                            if (n.Token.tipoToken == TipoToken.identificador || n.Token.tipoToken == TipoToken.sPunto)
                            {
                                if(n.Token.Lexema != key)
                                keyAux += n.Token.Lexema;
                            }
                        }
                    }

                    if (tokSig.token.tipoToken == TipoToken.sPuntoComa)
                    {
                        Nodo hijoPc = new Nodo(padre);
                        hijoPc.Token = tokSig.token;
                        padre.Hijos.Add(hijoPc);
                        match(TipoToken.sPuntoComa);
                        decl = true;
                    }
                    else if (tokSig.token.tipoToken == TipoToken.sIgual)
                    {
                        Nodo hijoIgual = new Nodo(padre);
                        hijoIgual.Token = tokSig.token;
                        padre.Hijos.Add(hijoIgual);
                        match(TipoToken.sIgual);

                        padre = DECLARACION_IGUAL(padre);

                        decl = true;
                    }

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

                    Nodo hijoPc = new Nodo(padre);
                    hijoPc.Token = tokSig.token;
                    padre.Hijos.Add(hijoPc);
                    match(TipoToken.sPuntoComa);

                    incDec = true;

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

                    Nodo hijoPc = new Nodo(padre);
                    hijoPc.Token = tokSig.token;
                    padre.Hijos.Add(hijoPc);
                    match(TipoToken.sPuntoComa);

                    incDec = true;

                }
                else
                {
                    padre = FUNCIONCALL(padre);
                    callF = true;
                }

                
                if (existClase)
                {
                    if (decl)
                    {
                        bool exists = ambientePadre.Add(keyAux, padre, ambientePadre);
                        if(!exists)
                            ambientePadre.ValidDeclaration(keyAux, padre, tabs);
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine($"Variable {key} ya existe.");
                        }
                    }
                    else if (incDec)
                    {
                        ambientePadre.ValidIncDec(key, padre, tabs);
                    }
                    else if(callF)
                    {
                        ambientePadre.ValidFunctionCall(key, padre, tabs);
                    }
                    else if (asignCompleta)
                    {
                        ambientePadre.ValidAssignation(key, padre, tabs);
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"No existe variable o clase {key}");
                }
                tabs--;
                return padre;
            }
            else if(tokSig.token.tipoToken == TipoToken.pBreak)
            {
                string key = "";
                Nodo hijoBreak = new Nodo(padre);
                hijoBreak.Token = tokSig.token;
                padre.Hijos.Add(hijoBreak);
                match(TipoToken.pBreak);

                Nodo hijoPc = new Nodo(padre);
                hijoPc.Token = tokSig.token;
                padre.Hijos.Add(hijoPc);
                match(TipoToken.sPuntoComa);


                key = "break" + tabs;
               
                bool exists = ambientePadre.Add(key, padre, ambientePadre);
                if (!exists)
                    ambientePadre.ValidBreak(key, padre, tabs);

                tabs--;
                return padre;
            }
            else if (tokSig.token.tipoToken == TipoToken.pReturn)
            {
                string key = "";
                bool esId = false;
                bool exists = false;
                Nodo hijoReturn = new Nodo(padre);
                hijoReturn.Token = tokSig.token;
                padre.Hijos.Add(hijoReturn);
                match(TipoToken.pReturn);

                if (tokSig.token.tipoToken == TipoToken.identificador)
                {
                    padre = IDENTIFICADOR(padre);
                    
                    foreach (Nodo n in padre.Hijos)
                    {
                        if (n.Token != null)
                        {
                            if (n.Token.tipoToken == TipoToken.identificador)
                            {
                                key += n.Token.Lexema;
                            }
                        }
                    }
                    esId = true;
                    exists = ambientePadre.GetEnvironment(key, ambientePadre);

                }
                else if (tokSig.token.tipoToken == TipoToken.numeroEntero)
                {
                    
                    Nodo hijoNum = new Nodo(padre);
                    hijoNum.Token = tokSig.token;
                    padre.Hijos.Add(hijoNum);

                    key = tokSig.token.Lexema;
                    match(TipoToken.numeroEntero);

                }
                else if (tokSig.token.tipoToken == TipoToken.pNull)
                {
                    Nodo hijoNull = new Nodo(padre);
                    hijoNull.Token = tokSig.token;
                    padre.Hijos.Add(hijoNull);
                    key = "nullReturn" + tabs;
                    match(TipoToken.pNull);
                }
                Nodo hijoPc = new Nodo(padre);
                hijoPc.Token = tokSig.token;
                padre.Hijos.Add(hijoPc);
                match(TipoToken.sPuntoComa);

                if (esId)
                {
                    if (exists)
                    {
                        ambientePadre.ValidReturn(key, padre, tabs);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"Variable {key} no existe.");
                    }

                }
                else
                {
                    ambientePadre.ValidReturn(key, padre, tabs);
                }
                    
                tabs--;
                return padre;
            }
            return padre;
        }
        private Nodo FUNCIONCALL(Nodo padre)
        {
            Nodo hijoParA = new Nodo(padre);
            hijoParA.Token = tokSig.token;
            padre.Hijos.Add(hijoParA);
            match(TipoToken.parentesisA);

            if (tokSig.token.tipoToken != TipoToken.nDate)
            {
                padre = PARAMETROSCALLN(padre);
            }else
            {
                Nodo nDate = new Nodo(padre);
                nDate.Token = tokSig.token;
                padre.Hijos.Add(nDate);
                match(TipoToken.nDate);
            }
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
            if (tokSig.token.tipoToken == TipoToken.numeroFloat)
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
            else if(tokSig.token.tipoToken ==TipoToken.identificador)
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
            return padre;
            
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
        private Nodo SENTENCIA(Nodo padre, Ambiente ambientePadre)
        {   
            Ambiente ambienteSentencia = new Ambiente(ambientePadre , "Sentencia");
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

                Token auxId = new Token();
                Token auxId2 = new Token();
                bool incId = false;
                int cont = 0;
                foreach (Nodo hijo in hijoCondicion.Hijos)
                {
                    if (hijo.Token!= null)
                    {
                        if (hijo.Token.tipoToken == TipoToken.identificador)
                        {
                            if (cont == 0)
                                auxId = hijo.Token;
                            else
                                auxId2 = hijo.Token;
                            incId = true;
                            cont++;
                        }
                    }
                }
                bool existeVar1 = false;
                bool existeVar2 = false;
                if (incId)
                {
                    existeVar1 = ambientePadre.GetEnvironment(auxId.Lexema , ambientePadre);
                    if (cont > 1)
                    {
                        existeVar2 = ambientePadre.GetEnvironment(auxId2.Lexema, ambientePadre);
                    }   
                } 
                ambientePadre.ValidIf(padre, tabs);
                if (!existeVar1)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"Variable {auxId.Lexema} no existe.");
                }
                if(cont>1 && !existeVar2)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"Variable {auxId2.Lexema} no existe.");
                }
               

                Nodo hijoInMain = new Nodo(padre);
                hijoInMain.Name = "In IF";
                hijoInMain = INMAINN(hijoInMain , ambienteSentencia);
                padre.Hijos.Add(hijoInMain);

                Nodo hijoLlaveC = new Nodo(padre);
                hijoLlaveC.Token = tokSig.token;
                padre.Hijos.Add(hijoLlaveC);
                match(TipoToken.llaveC);
                ambienteSentencia.Close(tabs);
                
                if(tokSig.token.tipoToken == TipoToken.pElse)
                {
                    padre = ELSEIFN(padre, ambientePadre);
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

                ambienteSentencia.ValidForeach(padre, tabs);

                Nodo hijoInMain = new Nodo(padre);
                hijoInMain.Name = "In Foreach";
                hijoInMain = INMAINN(hijoInMain, ambienteSentencia);
                padre.Hijos.Add(hijoInMain);


                Nodo hijoLlaveC = new Nodo(padre);
                hijoLlaveC.Token = tokSig.token;
                padre.Hijos.Add(hijoLlaveC);
                match(TipoToken.llaveC);

                ambienteSentencia.Close(tabs);
                
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
                Token auxId = new Token();
                Token auxId2 = new Token();
                bool incId = false;
                int cont = 0;
                foreach (Nodo hijo in hijoCondicion.Hijos)
                {
                    if (hijo.Token != null)
                    {
                        if (hijo.Token.tipoToken == TipoToken.identificador)
                        {
                            if (cont == 0)
                                auxId = hijo.Token;
                            else
                                auxId2 = hijo.Token;
                            incId = true;
                            cont++;
                        }
                    }
                }
                bool existeVar1 = false;
                bool existeVar2 = false;
                if (incId)
                {
                    existeVar1 = ambientePadre.GetEnvironment(auxId.Lexema, ambientePadre);
                    if (cont > 1)
                    {
                        existeVar2 = ambientePadre.GetEnvironment(auxId2.Lexema, ambientePadre);
                    }
                }
                ambienteSentencia.ValidWhile(padre, tabs);
                if (!existeVar1)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"Variable {auxId.Lexema} no existe.");
                }
                if (cont > 1 && !existeVar2)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"Variable {auxId2.Lexema} no existe.");
                }

                Nodo hijoInMain = new Nodo(padre);
                hijoInMain.Name = "In While";
                hijoInMain = INMAINN(hijoInMain, ambienteSentencia);
                padre.Hijos.Add(hijoInMain);

                Nodo hijoLlaveC = new Nodo(padre);
                hijoLlaveC.Token = tokSig.token;
                padre.Hijos.Add(hijoLlaveC);
                match(TipoToken.llaveC);

                ambienteSentencia.Close(tabs);
            }
            return padre;
            
        }
        private Nodo ELSEIFN(Nodo padre, Ambiente ambiente)
        {
            
            if(tokSig.token.tipoToken != TipoToken.pElse)
            {
                return padre;
            }
            padre = ELSEIF(padre , ambiente);
            return ELSEIFN(padre, ambiente);
        }
        private Nodo ELSEIF(Nodo padre, Ambiente ambiente)
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

                ambiente.ValidIf(padre, tabs);

                Nodo hijoInMain = new Nodo(padre);
                hijoInMain.Name = "In Else If";
                hijoInMain = INMAINN(hijoInMain, ambiente);
                padre.Hijos.Add(hijoInMain);

                Nodo hijoLlaveC = new Nodo(padre);
                hijoLlaveC.Token = tokSig.token;
                padre.Hijos.Add(hijoLlaveC);
                match(TipoToken.llaveC);

                ambiente.Close(tabs);

            }
            else
            {
                Nodo hijoLlaveAElse = new Nodo(padre);
                hijoLlaveAElse.Token = tokSig.token;
                padre.Hijos.Add(hijoLlaveAElse);
                match(TipoToken.llaveA);

                ambiente.ValidElse(padre, tabs);

                Nodo hijoInMain2 = new Nodo(padre);
                hijoInMain2.Name = "In Else";
                hijoInMain2 = INMAINN(hijoInMain2 , ambiente);
                padre.Hijos.Add(hijoInMain2);

                Nodo hijoLlaveCelse = new Nodo(padre);
                hijoLlaveCelse.Token = tokSig.token;
                padre.Hijos.Add(hijoLlaveCelse);
                match(TipoToken.llaveC);

                ambiente.Close(tabs);

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
                Nodo hijoAmpr = new Nodo(padre);
                hijoAmpr.Token = tokSig.token;
                padre.Hijos.Add(hijoAmpr);
                padre.Hijos.Add(hijoAmpr);
                match(TipoToken.sAmpr);
                match(TipoToken.sAmpr);
                padre = CONDICION_IF(padre);
            }
            else if (tokSig.token.tipoToken == TipoToken.sOr)
            {
                Nodo hijoOr = new Nodo(padre);
                hijoOr.Token = tokSig.token;
                padre.Hijos.Add(hijoOr);
                padre.Hijos.Add(hijoOr);
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
        private Nodo DECLARACION(Nodo padre , Ambiente ambientePadre)
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
            else if (tokSig.token.tipoToken == TipoToken.pList)
            {
                Nodo hijoVar = new Nodo(padre);
                hijoVar.Token = tokSig.token;
                padre.Hijos.Add(hijoVar);
                match(TipoToken.pList);

                Nodo hijoMenor = new Nodo(padre);
                hijoMenor.Token = tokSig.token;
                padre.Hijos.Add(hijoMenor);
                match(TipoToken.sMenor);

                if (tokSig.token.tipoToken == TipoToken.identificador)
                {
                    padre = IDENTIFICADOR(padre);
                }
                else if (tipoDato.Contains(tokSig.token.tipoToken))
                {
                    if(tokSig.token.tipoToken == TipoToken.pInt)
                    {
                        Nodo hijoInt = new Nodo(padre);
                        hijoInt.Token = tokSig.token;
                        padre.Hijos.Add(hijoInt);
                        match(TipoToken.pInt);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.pFloat)
                    {
                        Nodo hijoInt = new Nodo(padre);
                        hijoInt.Token = tokSig.token;
                        padre.Hijos.Add(hijoInt);
                        match(TipoToken.pFloat);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.pDatetime)
                    {
                        Nodo hijoDateTime = new Nodo(padre);
                        hijoDateTime.Token = tokSig.token;
                        padre.Hijos.Add(hijoDateTime);
                        match(TipoToken.pDatetime);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.pBool)
                    {
                        Nodo hijoBool = new Nodo(padre);
                        hijoBool.Token = tokSig.token;
                        padre.Hijos.Add(hijoBool);
                        match(TipoToken.pBool);
                    }
                    Nodo hijoMayor = new Nodo(padre);
                    hijoMayor.Token = tokSig.token;
                    padre.Hijos.Add(hijoMayor);
                    match(TipoToken.sMayor);

                }
            }

            padre = IDENTIFICADOR(padre);
            string key = "";
            foreach (Nodo n in padre.Hijos)
            {
                if (n.Token != null)
                {
                    if (n.Token.tipoToken == TipoToken.identificador)
                    {
                        key += n.Token.Lexema;
                    }
                }
            }

            if (tokSig.token.tipoToken == TipoToken.sPuntoComa)
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

            bool exists = ambientePadre.Add(key, padre, ambientePadre);
            if (!exists)
                ambientePadre.ValidDeclaration(key, padre, tabs);

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
            else if (this.tokSig.token.tipoToken == TipoToken.operacionAritmetica)
            {
                Nodo hijoTrue = new Nodo(padre);
                hijoTrue.Token = tokSig.token;
                padre.Hijos.Add(hijoTrue);
                match(TipoToken.operacionAritmetica);

                Nodo hijoPc = new Nodo(padre);
                hijoPc.Token = tokSig.token;
                padre.Hijos.Add(hijoPc);
                match(TipoToken.sPuntoComa);

                return padre;
            }
            else if (this.tokSig.token.tipoToken == TipoToken.pNew)
            {   
                //IDENTIFICADOR
                
                
                Nodo hijoNew = new Nodo(padre);
                hijoNew.Token = tokSig.token;
                padre.Hijos.Add(hijoNew);
                match(TipoToken.pNew);

                if (tokSig.token.tipoToken == TipoToken.pDatetime)
                {
                    //new DateTime(año, mes, dia, hora, minuto, segundo);
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
                    match(TipoToken.nDate);

                    Nodo hijoParC = new Nodo(padre);
                    hijoParC.Token = tokSig.token;
                    padre.Hijos.Add(hijoParC);
                    match(TipoToken.parentesisC);

                    
                }
                else if(tokSig.token.tipoToken == TipoToken.pList)
                {
                    Nodo hijoList = new Nodo(padre);
                    hijoList.Token = tokSig.token;
                    padre.Hijos.Add(hijoList);
                    match(TipoToken.pList);

                    Nodo hijosMenor = new Nodo(padre);
                    hijosMenor.Token = tokSig.token;
                    padre.Hijos.Add(hijosMenor);
                    match(TipoToken.sMenor);

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
                    else if (tokSig.token.tipoToken == TipoToken.pFloat)
                    {
                        Nodo hijoInt = new Nodo(padre);
                        hijoInt.Token = tokSig.token;
                        padre.Hijos.Add(hijoInt);
                        match(TipoToken.pFloat);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.pDatetime)
                    {
                        Nodo hijoDateTime = new Nodo(padre);
                        hijoDateTime.Token = tokSig.token;
                        padre.Hijos.Add(hijoDateTime);
                        match(TipoToken.pDatetime);
                    }
                    else if (tokSig.token.tipoToken == TipoToken.pBool)
                    {
                        Nodo hijoBool = new Nodo(padre);
                        hijoBool.Token = tokSig.token;
                        padre.Hijos.Add(hijoBool);
                        match(TipoToken.pBool);
                    }

                    Nodo hijoMayor = new Nodo(padre);
                    hijoMayor.Token = tokSig.token;
                    padre.Hijos.Add(hijoMayor);
                    match(TipoToken.sMayor);

                    Nodo hijoParA = new Nodo(padre);
                    hijoParA.Token = tokSig.token;
                    padre.Hijos.Add(hijoParA);
                    match(TipoToken.parentesisA);

                    Nodo hijoParC = new Nodo(padre);
                    hijoParC.Token = tokSig.token;
                    padre.Hijos.Add(hijoParC);
                    match(TipoToken.parentesisC);

                    Nodo hijollaveA = new Nodo(padre);
                    hijollaveA.Token = tokSig.token;
                    padre.Hijos.Add(hijollaveA);
                    match(TipoToken.llaveA);

                    if(tokSig.token.tipoToken != TipoToken.nDate)
                    {
                        padre = PARAMLISTAN(padre);
                    }
                    else
                    {
                        Nodo nDate = new Nodo(padre);
                        nDate.Token = tokSig.token;
                        padre.Hijos.Add(nDate);
                        match(TipoToken.nDate);
                    }
                   

                    Nodo hijollaveC = new Nodo(padre);
                    hijollaveC.Token = tokSig.token;
                    padre.Hijos.Add(hijollaveC);
                    match(TipoToken.llaveC);

                }
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
            else if (tokSig.token.tipoToken == TipoToken.identificador)
            {
                padre = IDENTIFICADOR(padre);

                if (tokSig.token.tipoToken == TipoToken.parentesisA)
                {
                    padre = FUNCIONCALL(padre);
                    return padre;
                }
                Nodo hijoPc = new Nodo(padre);
                hijoPc.Token = tokSig.token;
                padre.Hijos.Add(hijoPc);
                match(TipoToken.sPuntoComa);
                return padre;
            }
            else
                return padre;

        }
        private Nodo PARAMLISTAN(Nodo padre)
        {
            if (tokSig.token.tipoToken == TipoToken.llaveC)
            {
                return padre;
            }
            padre =  PARAMLISTA(padre);
            return PARAMLISTAN(padre);
        }
        private Nodo PARAMLISTA(Nodo padre)
        {
            if (tokSig.token.tipoToken == TipoToken.numeroFloat)
            {
                Nodo hijoNumeroEntero = new Nodo(padre);
                hijoNumeroEntero.Token = tokSig.token;
                padre.Hijos.Add(hijoNumeroEntero);
                match(TipoToken.numeroFloat);

                if (tokSig.token.tipoToken != TipoToken.llaveC)
                {
                    Nodo hijoPc = new Nodo(padre);
                    hijoPc.Token = tokSig.token;
                    padre.Hijos.Add(hijoPc);
                    match(TipoToken.sComa);
                }
                return padre;
            }
            else if (tokSig.token.tipoToken == TipoToken.identificador)
            {
                padre = IDENTIFICADOR(padre);

                if (tokSig.token.tipoToken != TipoToken.llaveC)
                {
                    Nodo hijoPc = new Nodo(padre);
                    hijoPc.Token = tokSig.token;
                    padre.Hijos.Add(hijoPc);
                    match(TipoToken.sComa);
                }
                return padre;
            }
            else if (tokSig.token.tipoToken == TipoToken.pFalse)
            {
                Nodo hijoNumeroEntero = new Nodo(padre);
                hijoNumeroEntero.Token = tokSig.token;
                padre.Hijos.Add(hijoNumeroEntero);
                match(TipoToken.pFalse);

                if (tokSig.token.tipoToken != TipoToken.llaveC)
                {
                    Nodo hijoPc = new Nodo(padre);
                    hijoPc.Token = tokSig.token;
                    padre.Hijos.Add(hijoPc);
                    match(TipoToken.sComa);
                }
                return padre;
            }
            else if (tokSig.token.tipoToken == TipoToken.pTrue)
            {
                Nodo hijoNumeroEntero = new Nodo(padre);
                hijoNumeroEntero.Token = tokSig.token;
                padre.Hijos.Add(hijoNumeroEntero);
                match(TipoToken.pTrue);

                if (tokSig.token.tipoToken != TipoToken.llaveC)
                {
                    Nodo hijoPc = new Nodo(padre);
                    hijoPc.Token = tokSig.token;
                    padre.Hijos.Add(hijoPc);
                    match(TipoToken.sComa);
                }
                return padre;
            }
            return padre;
        }
    }
} 