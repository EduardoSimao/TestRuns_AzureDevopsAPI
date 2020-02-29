using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROBOTESTE_ILibrarySystem.Model
{
    class Construtor
    {
        //Classe com os atributos id's que serão utilizados para montar a URL da API
        public string RunsID { get; set; }
        public string ResulID { get; set; }
    }

    class Attachments
    {
        //Classe com os atributos necessarios para passar via json para API
        public string Stream { get; set; }
        public string FileName { get; set; }
        public string Comment { get; set; }
        public string AttachmentType { get; set; }

    }
}
