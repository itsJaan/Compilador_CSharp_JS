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
        sIgualIgual , sMayorIgual , sMenorIgual , sDistintoQue,
        sMod, sExcla, sMayor, sMenor, sAmpr, sOr,
        pInt, pFloat, pBool, pDatetime,
        pIn,
        pPublic,
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
        pWriteLine,
        pReadLine,
        pTrue,
        pReturn,
        pVoid,
        pVar,
        pUsing,
        pGet,
        pSet,
        pStatic,
        pThis,
        parentesisA, parentesisC,
        bracketA, bracketC,
        sPunto,
        sDolar,
        sPuntoComa,
        sComa,
        sComillaS,
        sComillaD,
        llaveA, llaveC,
        sGuionBajo,
        espacio,
        tabulador,
        finLinea,
        finArchivo,
        tokenInvalido
    }
}
