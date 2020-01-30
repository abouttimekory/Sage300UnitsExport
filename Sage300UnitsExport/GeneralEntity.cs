using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sage300UnitsExport
{

    class GeneralEntity
    {
        private int id;
        private string code;
        private string extraCode;

        public GeneralEntity(int id, string code, string extraCode)
        {
            this.id = id;
            this.code = code;
            this.extraCode = extraCode;
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Code
        {
            get { return code; }
            set { code = value; }
        }

        public string ExtraCode
        {
            get { return extraCode; }
            set { extraCode = value; }
        }
    }
}
