using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto.ArbolAbstracto.Imports
{
    public class ImportSecuence : Import
    {
        public Import PrimerImport { get; set; }
        public Import SiguienteImport { get; set; }

        public ImportSecuence(Import P, Import S)
        {
            PrimerImport = P;
            SiguienteImport = S;
        }
    }
}
