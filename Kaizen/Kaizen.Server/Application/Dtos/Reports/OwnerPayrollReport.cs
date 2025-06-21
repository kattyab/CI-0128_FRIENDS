namespace Kaizen.Server.Application.Dtos.Reports
{
    public class OwnerPayrollReport
    {
        public string Period { get; set; }
        public DateTime ExecutedOn { get; set; }
        public string OwnerFullName { get; set; }
        public decimal TotalLaborCharges { get; set; }
        public decimal TotalMoneyPaid { get; set; }
        public decimal ServiciosProfesionalesAmount { get; set; }
        public decimal PorHorasAmount { get; set; }
        public decimal TiempoCompletoAmount { get; set; }
         public decimal SEM { get; set; }
        public decimal IVM { get; set; }
        public decimal CuotaPatronalBancoPopular { get; set; }
        public decimal AsignacionesFamiliares { get; set; }
        public decimal IMAS { get; set; }
        public decimal INA { get; set; }
        public decimal AporteBancoPopular { get; set; }
        public decimal FCL { get; set; }
        public decimal FondoPensionesComplementarias { get; set; }
        public decimal INS { get; set; }
    }
}
