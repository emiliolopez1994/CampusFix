using CampusFix.Api.Models;
using CampusFix.Api.Validators;
using CampusFix.Api.DTOs;
var checks = 0;
for(var a=1;a<=4;a++) for(var b=0;b<=5;b++) {
    bool accepted;
    try { ReporteValidator.ValidarTransicion((EstadoReporte)a,(EstadoReporte)b); accepted=true; }
    catch(InvalidOperationException) { accepted=false; }
    if(accepted != (a<4 && b==a+1)) throw new Exception($"Transición incorrecta: {a} → {b}");
    checks++;
}
ReporteValidator.Validar(new ReporteEntrada("Proyector averiado","Aula 204",null,PrioridadReporte.Alta,1));
foreach(var d in new[]{new ReporteEntrada(" ","Aula",null,PrioridadReporte.Alta,1),new ReporteEntrada("Problema"," ",null,PrioridadReporte.Alta,1),new ReporteEntrada("Problema","Aula",null,(PrioridadReporte)9,1),new ReporteEntrada("Problema","Aula",null,PrioridadReporte.Alta,0)}) {
    try { ReporteValidator.Validar(d); throw new Exception("Se aceptó una entrada inválida"); }
    catch(ArgumentException) { checks++; }
}
Console.WriteLine($"PASS: {checks} casos de transición y validación; entrada válida aceptada.");
