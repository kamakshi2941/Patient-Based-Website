using System;
using System.Collections.Generic;

namespace KPPatients.Models
{
    public partial class PatientDiagnosis
    {
        public PatientDiagnosis()
        {
            PatientTreatments = new HashSet<PatientTreatment>();
        }

        public int PatientDiagnosisId { get; set; }
        public int PatientId { get; set; }
        public int DiagnosisId { get; set; }
        public string? Comments { get; set; }

        public virtual Diagnosis Diagnosis { get; set; }
        public virtual Patient Patient { get; set; } 
        public virtual ICollection<PatientTreatment> PatientTreatments { get; set; }
    }
}
