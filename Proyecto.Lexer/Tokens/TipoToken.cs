using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Lexer.Tokens
{
    public enum TipoToken
    {
        identificador,
        numeroEntero,
        comentario,
        nDate,
        operacionAritmetica,
        numeroFloat,
        sMas, sMenos, sDiv, sMult, sIgual,
        sMod, sExcla, sMayor, sMenor, sAmpr, sOr,
        pInt, pFloat, pBool, pDatetime,
        pPublic,
        pPrivate,
        pClass,
        pNew,
        pFor,
        pForeach,
        pFalse,
        pWhile,
        pIf,
        pElse,
        pBreak,
        pConsole,
        pMain,
        pNamespace,
        pWriteLine,
        pReadLine,
        pTrue,
        pReturn,
        pVoid,
        pVar,
        pUsing,
        pGet,
        pSet,
        pProtected,
        pAbstract,
        pStatic,
        pThis,
        parentesisA, parentesisC,
        bracketA, bracketC,
        sPunto,
        sPuntoComa,
        sComa,
        sComillaS,
        sComillaD,
        llaveA, llaveC,
        sGuionBajo,
        espacio,
        finLinea,
        finArchivo,
        tokenInvalido
    }
}
