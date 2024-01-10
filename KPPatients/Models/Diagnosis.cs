using System;
using System.Collections.Generic;

namespace KPPatients.Models
{
    public partial class Diagnosis
    {
        public Diagnosis()
        {
            PatientDiagnoses = new HashSet<PatientDiagnosis>();
            Treatments = new HashSet<Treatment>();
        }

        public int DiagnosisId { get; set; }
        public string Name { get; set; } 
        public int DiagnosisCategoryId { get; set; }

        public virtual DiagnosisCategory DiagnosisCategory { get; set; } 
        public virtual ICollection<PatientDiagnosis> PatientDiagnoses { get; set; }
        public virtual ICollection<Treatment> Treatments { get; set; }
    }
}
