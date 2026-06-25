using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBGilingPadi
{
    public class ListPenggilingan
    {
        // Sesuaikan nama dan tipe property dengan kolom di database (Modul Poin 8)
        public string ID_Giling { get; set; }
        public string NamaTempat { get; set; }
        public string Alamat { get; set; }
        public string Wilayah { get; set; }
        public string StatusOperasional { get; set; }
    }
}
