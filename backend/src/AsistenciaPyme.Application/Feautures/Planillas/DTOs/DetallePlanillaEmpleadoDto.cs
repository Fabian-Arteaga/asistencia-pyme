namespace AsistenciaPyme.Application.Features.Planillas.DTOs;

public class DetallePlanillaEmpleadoDto
{
    public int IdPlanilla { get; set; }
    public int IdEmpleado { get; set; }
    public string CodigoEmpleado { get; set; } = string.Empty;
    public string NombreEmpleado { get; set; } = string.Empty;
    public string? NumeroINSS { get; set; }
    public string Departamento { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public decimal SalarioBase { get; set; }
    public int DiasLaborados { get; set; }
    public int MinutosLaborados { get; set; }
    public int CantidadTardanzas { get; set; }
    public int MinutosTardanza { get; set; }
    public decimal DescuentoTardanza { get; set; }
    public int MinutosExtrasDetectados { get; set; }
    public int MinutosExtrasAprobados { get; set; }
    public decimal MontoHorasExtras { get; set; }
    public decimal VacacionesAcumuladasPeriodo { get; set; }
    public decimal SaldoVacaciones { get; set; }
    public decimal TotalIngresos { get; set; }
    public decimal TotalDeducciones { get; set; }
    public decimal SalarioNeto { get; set; }
    public decimal? IndemnizacionProyectada { get; set; }
    public List<ConceptoDetallePlanillaDto> Conceptos { get; set; } = new();
    public decimal INSS { get; set; }
    public decimal Embargos { get; set; }
    public decimal OtrasDeducciones { get; set; }
}

public class ConceptoDetallePlanillaDto
{
    public int IdConceptoPlanilla { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public string? Descripcion { get; set; }
}
