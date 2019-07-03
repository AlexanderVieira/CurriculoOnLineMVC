using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurriculoOnLineMVC.Models
{
    public class Candidato
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string Nome { get; set; }
        
        public virtual ICollection<CandidatoPerfil> Perfis { get; set; }

        public Candidato()
        {
            this.Perfis = new HashSet<CandidatoPerfil>();
        }

    }
}
