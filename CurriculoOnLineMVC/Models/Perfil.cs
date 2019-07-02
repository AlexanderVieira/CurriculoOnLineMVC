using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CurriculoOnLineMVC.Models
{
    public class Perfil
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(30)")]
        public string Descricao { get; set; }        
        public virtual ICollection<CandidatoPerfil> Candidatos { get; set; }

        public Perfil()
        {
            this.Candidatos = new HashSet<CandidatoPerfil>();
        }
    }
}
