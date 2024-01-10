using System;
using System.Collections.Generic;

namespace KPPatients.Models
{
    public partial class MedicationType
    {
        internal int mTypeId;

        public MedicationType()
        {
            Medications = new HashSet<Medication>();
        }

        public int MedicationTypeId { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Medication> Medications { get; set; }
    }
}
