using Proyecto.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.ArbolAbstracto
{
    public class Ambiente
    {
        public Ambiente anterior { get; set;}
        public Dictionary<string, Nodo> tabla;
        public Dictionary<string, Nodo> tablaAsign;

        public Ambiente(Ambiente a)
        {
            anterior = a;
            tabla = new Dictionary<string, Nodo>();
            tablaAsign = new Dictionary<string, Nodo>();
        }

        public bool Add(string l, Nodo n)
        {

            try
            {
                tabla.Add(l, n);
                return false;    
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"Ya existe {n.Name} {l}");
                return true;
            }
        }
        public Nodo GetEnvironment( string l)
        {
            Ambiente ambienteActual = this;
            while (ambienteActual!= null) {
                if (tabla.TryGetValue(l, out var found))
                    return found;
                else
                    ambienteActual = anterior;
            }   
            return null;
        }
        public void ValidImport(string key, Nodo nodo)
        {
            Token inicial = new Token();
            Token id = new Token();
            Token final = new Token();

            foreach (Nodo hijo in nodo.Hijos)
            {
                if (hijo.Token.tipoToken == TipoToken.pUsing)
                {
                    inicial = hijo.Token;
                }
                else if (hijo.Token.tipoToken == TipoToken.identificador)
                {
                    id = hijo.Token;
                }
                else if (hijo.Token.tipoToken == TipoToken.sPuntoComa)
                {
                    final = hijo.Token;
                }
            }
            if (inicial.tipoToken == TipoToken.pUsing)
                Console.WriteLine($"Import \"{key}\"{final.Lexema}");

        }
        public void ValidClassDeclaration(string key, Nodo nodo)
        {
            Token Tpublic = new Token();
            Token Tclass = new Token();
            Token id = new Token();
            Token final = new Token();

            foreach (Nodo hijo in nodo.Hijos)
            {
                if (hijo.Token != null)
                {
                    if (hijo.Token.tipoToken == TipoToken.pPublic)
                    {
                        Tpublic = hijo.Token;
                    }
                    else if (hijo.Token.tipoToken == TipoToken.pClass)
                    {
                        Tclass = hijo.Token;
                    }
                    else if (hijo.Token.tipoToken == TipoToken.identificador)
                    {
                        id = hijo.Token;
                    }
                    else if (hijo.Token.tipoToken == TipoToken.llaveA)
                    {
                        final = hijo.Token;
                    }
                }
            }
            if (Tpublic.tipoToken == TipoToken.pPublic)
                Console.WriteLine($"{Tclass.Lexema} {key} {final.Lexema}");

        }
        public void ValidFunctionDecl(string key, Nodo nodo, int t)
        {
            List<TipoToken> tipos = new List<TipoToken>() { TipoToken.pInt, TipoToken.pFloat, TipoToken.pDatetime, TipoToken.pBool, TipoToken.pVoid };
            Token inicial = new Token();
            Token tipo = new Token();
            Token id = new Token();
            Token parentesis = new Token();
            Token llave = new Token();
            string param = "";

            foreach (Nodo hijo in nodo.Hijos)
            {
                if (hijo.Token != null)
                {
                    if (hijo.Token.tipoToken == TipoToken.pPublic)
                    {
                        inicial = hijo.Token;
                    }
                    else if (tipos.Contains(hijo.Token.tipoToken))
                    {
                        tipo = hijo.Token;
                    }
                    else if (hijo.Token.tipoToken == TipoToken.identificador)
                    {
                        id = hijo.Token;
                    }
                    else if (hijo.Token.tipoToken == TipoToken.parentesisA)
                    {
                        parentesis = hijo.Token;
                    }
                    else if (hijo.Token.tipoToken == TipoToken.parentesisC)
                    {
                        parentesis = hijo.Token;
                    }
                    else if (hijo.Token.tipoToken == TipoToken.llaveA)
                    {
                        parentesis = hijo.Token;
                    }
                }
                else if (hijo.Name == "Parametros")
                {
                    param = Parametros(hijo);
                }
            }
            string tabs = "";
            for (int i = 0; i < t; i++)
                tabs += "\t";
            if (inicial.tipoToken == TipoToken.pPublic)
                Console.WriteLine($"{tabs}{key} ({param})"+"{");

        }
        public string Parametros(Nodo nodo)
        {
            List<TipoToken> tipos = new List<TipoToken>() { TipoToken.pInt, TipoToken.pFloat, TipoToken.pDatetime, TipoToken.pBool};
            string param = "";
            foreach(Nodo n in nodo.Hijos)
            {
                if (n.Token != null)
                {
                    
                    if (n.Token.tipoToken == TipoToken.sComa)
                    {
                        param += n.Token.Lexema;
                    }
                    else if (n.Token.tipoToken == TipoToken.identificador)
                    {
                        param += n.Token.Lexema;
                    }
                }
            }

            return param;
        }
        public void ValidDeclaration(string key, Nodo nodo, int t)
        {
            List<TipoToken> tipos = new List<TipoToken>() { TipoToken.pInt, TipoToken.pFloat, TipoToken.pDatetime, TipoToken.pBool};
            List<TipoToken> tipoIgual = new List<TipoToken>() { TipoToken.numeroEntero, TipoToken.numeroFloat, TipoToken.operacionAritmetica, TipoToken.pFalse, TipoToken.pTrue };
            bool esPub = false;
            bool wiNew = false;
            Token Ttipo = new Token();
            bool wIgual = false;    
            Token asign = new Token();
            Token fecha = new Token();
            bool esLista = false;
            bool param = false;
            string asignLista = "";
            foreach (Nodo hijo in nodo.Hijos)
            {
                if (hijo.Token != null)
                {
                    if (tipos.Contains(hijo.Token.tipoToken))
                    {
                        Ttipo = hijo.Token;
                    }
                    else if (hijo.Token.tipoToken == TipoToken.sIgual)
                    {
                        wIgual = true;
                    }
                    else if (tipoIgual.Contains(hijo.Token.tipoToken))
                    {
                        asign = hijo.Token;
                    }
                    else if(hijo.Token.tipoToken == TipoToken.pNew)
                    {
                        wiNew = true; 
                    }
                    else if (hijo.Token.tipoToken == TipoToken.nDate)
                    {
                        fecha = hijo.Token;
                    }
                    else if (hijo.Token.tipoToken == TipoToken.pList)
                    {
                        esLista = true;
                    }
                    else if (hijo.Token.tipoToken == TipoToken.pPublic)
                    {
                        esPub = true;
                    }
                    
                    if (hijo.Token.tipoToken == TipoToken.llaveC)
                    {
                        param = false;
                    }
                    else if (param)
                    {
                        asignLista += hijo.Token.Lexema;
                    }
                    else if(hijo.Token.tipoToken == TipoToken.llaveA)
                    {
                        param = true;
                    }
                }
            }

            string tabs = "";
            for (int i = 0; i < t; i++)
                tabs += "\t";

            //if (asignLista == "")
            //{
                asignLista = fecha.Lexema;
            //}
            if (esPub)
            {
                if (wIgual)
                {
                    if (Ttipo.tipoToken == TipoToken.identificador || Ttipo.tipoToken == TipoToken.pDatetime || esLista)
                    {
                        if (wiNew && Ttipo.tipoToken == TipoToken.pDatetime)
                        {
                            Console.WriteLine($"{tabs}var {key} = new Date({fecha.Lexema});");
                        }
                        else if (wiNew && Ttipo.tipoToken == TipoToken.identificador)
                        {
                            Console.WriteLine($"{tabs}var {key} = new {Ttipo.Lexema};");
                        }
                        else if(wiNew  && esLista)
                        {
                            Console.WriteLine($"{tabs}var {key} = [{asignLista}];");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"{tabs}var {key} = {asign};");
                    }
                }
                else
                {
                    Console.WriteLine($"{tabs}var {key};");
                }
            }
            else
            {
                if (wIgual)
                {
                    if (Ttipo.tipoToken == TipoToken.identificador || Ttipo.tipoToken == TipoToken.pDatetime || esLista)
                    {
                        if (wiNew && Ttipo.tipoToken == TipoToken.pDatetime)
                        {
                            Console.WriteLine($"{tabs}var {key} = new Date({fecha.Lexema});");
                        }
                        else if (wiNew && Ttipo.tipoToken == TipoToken.identificador)
                        {
                            Console.WriteLine($"{tabs}var {key} = new {Ttipo.Lexema};");
                        }
                        else if (wiNew && esLista)
                        {
                            Console.WriteLine($"{tabs}var {key} = [{asignLista}];");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"{tabs}var {key} = {asign};");
                    }
                }
                else
                {
                    Console.WriteLine($"{tabs}var {key};");
                }
            }
        }
        public void ValidIf(Nodo nodo, int t)
        {
            bool elseIf = false;
            List<Token> condicion = new List<Token>();
            foreach(Nodo hijo in nodo.Hijos)
            {
                if(hijo.Name =="Condicion IF")
                {
                    foreach(Nodo n  in hijo.Hijos)
                    {
                        if(n.Token!= null)
                        {
                            condicion.Add(n.Token);
                        }
                            

                    }
                }
                else if (hijo.Token!= null)
                {
                    if(hijo.Token.tipoToken == TipoToken.pElse)
                    {
                        elseIf = true;
                        condicion.Clear();
                    }
                }
            }
            //VALIDAR CONDICION

            string condi = "";
            foreach(Token tok in condicion)
            {
                condi += tok.Lexema+" ";
            }


            string tabs = "";
            for (int i = 0; i < t; i++)
                tabs += "\t";
            if (elseIf)
                Console.WriteLine($"{tabs}else if({condi}){"{"}");
            else
                Console.WriteLine($"{tabs}if({condi}){"{"}");

        }
        public void ValidForeach(Nodo nodo , int t)
        {
            List<Token> condicion = new List<Token>();
            foreach (Nodo hijo in nodo.Hijos)
            {
                if (hijo.Name == "Condicion Foreach")
                {
                    foreach (Nodo n in hijo.Hijos)
                    {
                        if (n.Token != null)
                            condicion.Add(n.Token);
                    }
                }
            }
            //VALIDAR CONDICION

            string condi = "";
            foreach (Token tok in condicion)
            {
                condi += tok.Lexema + " ";
            }

            string tabs = "";
            for (int i = 0; i < t; i++)
                tabs += "\t";
            Console.WriteLine($"{tabs}foreach({condi}){"{"}");
        }
        public void ValidWhile(Nodo nodo, int t)
        {
            List<Token> condicion = new List<Token>();
            foreach (Nodo hijo in nodo.Hijos)
            {
                if (hijo.Name == "Condicion While")
                {
                    foreach (Nodo n in hijo.Hijos)
                    {
                        if (n.Token != null)
                            condicion.Add(n.Token);
                    }
                }
            }
            //VALIDAR CONDICION

            string condi = "";
            foreach (Token tok in condicion)
            {
                condi += tok.Lexema + " ";
            }

            string tabs = "";
            for (int i = 0; i < t; i++)
                tabs += "\t";
            Console.WriteLine($"{tabs}while({condi}){"{"}");

        }
        public void ValidIncDec(string key, Nodo nodo , int t)
        {
            bool isId = false;
            bool incremento = false;
            foreach(Nodo hijo in nodo.Hijos)
            {
                if(hijo.Token!= null)
                {
                    if(hijo.Token.tipoToken == TipoToken.identificador)
                    {
                        isId = true;
                    }
                    else if(hijo.Token.tipoToken == TipoToken.sMas)
                    {
                        incremento = true;
                    }
                }
            }
            string tabs = "";
            for (int i = 0; i < t; i++)
                tabs += "\t";

            if (isId)
            {
                if (incremento)
                {
                    Console.WriteLine($"{tabs}{key}++;");
                }
                else
                {
                    Console.WriteLine($"{tabs}{key}--;");
                }

            }
        }
        public void ValidReturn(string key , Nodo nodo , int t)
        {
            Token Treturn = new Token();
            Token Tkey = new Token();

            foreach (Nodo hijo in nodo.Hijos)
            {
                if (hijo.Token != null)
                {
                    if (hijo.Token.tipoToken == TipoToken.pReturn)
                    {
                        Treturn = hijo.Token;
                    }
                    else if (hijo.Token.tipoToken == TipoToken.identificador || hijo.Token.tipoToken == TipoToken.pNull || hijo.Token.tipoToken == TipoToken.numeroEntero)
                    {
                        Tkey = hijo.Token;
                    }
                }
            }
            string tabs = "";
            for (int i = 0; i < t; i++)
                tabs += "\t";

            if (Tkey != null)
                Console.WriteLine($"{tabs}return {Tkey.Lexema};");
            else
                Console.WriteLine($"{tabs}return;");



        }
        public void ValidBreak(string key, Nodo nodo, int t)
        {
            string tabs = "";
            for (int i = 0; i < t; i++)
                tabs += "\t";
            Console.WriteLine($"{tabs}break;");
        }
        public void ValidConsole(string key, Nodo nodo, int t)
        {
            string tabs = "";
            for (int i = 0; i < t; i++) 
            { 
                tabs += "\t";
            }
            Console.WriteLine($"{tabs}console.log({key});");
        }
        public void ValidFunctionCall(string key, Nodo nodo, int t)
        {
            string strParams = "";
            Nodo nParams = new Nodo(null);
            foreach (Nodo hijo in nodo.Hijos)
            {
                if (hijo.Name == "Parametros")
                {
                    nParams = hijo;
                    foreach (Nodo par in hijo.Hijos)
                    {
                        if (par.Token != null)
                        {
                            strParams += par.Token.Lexema;
                        }
                    }
                }
                else if (hijo.Token != null)
                {
                    if(hijo.Token.tipoToken == TipoToken.nDate)
                    {
                        strParams = hijo.Token.Lexema;
                    }
                }
            }
            string tabs = "";
            for (int i = 0; i < t; i++)
            {
                tabs += "\t";
            }
            Console.WriteLine($"{tabs}{key}({strParams});");
        }
        public void ValidAssignation(string key, Nodo nodo, int t)
        {
            string asignacion = "";
            bool despuesIgual = false;
            foreach(Nodo hijo in nodo.Hijos)
            {
                if(hijo.Token!= null)
                {
                    if (despuesIgual)
                    {
                        asignacion += hijo.Token.Lexema;
                    }
                    if (hijo.Token.tipoToken == TipoToken.sIgual)
                    {
                        despuesIgual = true;
                    }
                }
            }

            string tabs = "";
            for (int i = 0; i < t; i++)
            {
                tabs += "\t";
            }
            Console.WriteLine($"{tabs}{key} = {asignacion}");
        }
        public void Close(int t)
        { string tab ="";
            for (int i = 0; i<t; i++)
            {
                tab+= "\t";
            }
            Console.WriteLine(tab+"}");
        }
    }
}
