using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Model
{
    public abstract class Entity<Tkey>
    {
        public Tkey Id { get; set; }

        [Column("CREATION_DATE")]
        public DateTime CreationDate { get; set; }

        [Column("EDIT_TIME")]
        public DateTime EditTime { get; set; }

        /// <summary>
        /// Identificativo del parent di questa entità.
        /// </summary>
        public Tkey ParentId { get; set; }

        /// <summary>
        /// Identificativo del figlio di questa entità.
        /// </summary>
        public Tkey ChildId { get; set; }

        /// <summary>
        /// Clona l'entità.
        /// </summary>
        /// <returns></returns>
        public virtual Entity<Tkey> Clone()
        {
            return (Entity<Tkey>)this.MemberwiseClone();
        }
    }
}
