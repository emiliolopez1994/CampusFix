const { chromium } = require("playwright");
(async () => {
  require("fs").mkdirSync("artifacts", { recursive: true });
  const browser = await chromium.launch({ headless: true });
  const page = await browser.newPage({
    viewport: { width: 1440, height: 1000 },
  });
  const errors = [];
  page.on("pageerror", (e) => errors.push(e.message));
  let people = [
    { id: 1, nombre: "Carlos López", correo: "carlos@example.edu" },
  ];
  let rows = [
    {
      id: 1,
      problema: "El proyector no enciende",
      ubicacion: "Aula 204 · Edificio A",
      descripcion: "El equipo no responde al control ni al botón de encendido.",
      prioridad: 3,
      estado: 1,
      reportadoPorId: 1,
      reportadoPor: "Carlos López",
      fechaReporte: new Date().toISOString(),
      fechaActualizacion: new Date().toISOString(),
      historial: [],
    },
  ];
  await page.route("**/api/**", async (route) => {
    const req = route.request(),
      url = new URL(req.url()),
      body = req.postDataJSON();
    let data;
    if (url.pathname === "/api/usuarios") {
      if (req.method() === "POST") {
        const p = { id: people.length + 1, ...body };
        people.push(p);
        data = p;
      } else data = people;
    } else if (url.pathname.endsWith("/estado")) {
      const r = rows.find((x) => x.id === +url.pathname.split("/")[3]);
      r.historial.push({
        id: r.historial.length + 1,
        estadoAnterior: r.estado,
        estadoNuevo: body.estado,
        fechaCambio: new Date().toISOString(),
      });
      r.estado = body.estado;
      data = r;
    } else if (req.method() === "POST") {
      data = {
        id: rows.length + 1,
        ...body,
        estado: 1,
        reportadoPor: people.find((x) => x.id === body.reportadoPorId).nombre,
        fechaReporte: new Date().toISOString(),
        fechaActualizacion: new Date().toISOString(),
        historial: [],
      };
      rows.push(data);
    } else if (req.method() === "PUT") {
      const id = +url.pathname.split("/")[3];
      const i = rows.findIndex((r) => r.id === id);
      rows[i] = { ...rows[i], ...body };
      data = rows[i];
    } else data = rows;
    await route.fulfill({ json: data });
  });
  await page.goto("http://localhost:4200");
  await page
    .getByRole("button", { name: "El proyector no enciende" })
    .waitFor();
  await page.screenshot({ path: "artifacts/desktop.png", fullPage: true });
  await page.getByRole("button", { name: "Nuevo reporte" }).click();
  await page.locator("[name=problema]").fill("Ventana rota");
  await page.locator("[name=ubicacion]").fill("Biblioteca");
  await page
    .locator("[name=reportante]")
    .selectOption({ label: "Carlos López · carlos@example.edu" });
  await page
    .getByRole("button", { name: "Guardar reporte", exact: true })
    .click();
  await page.getByRole("heading", { name: "Ventana rota" }).waitFor();
  for (const label of [
    "Pasar a En revisión →",
    "Pasar a En reparación →",
    "Pasar a Solucionado →",
  ]) {
    await page.getByRole("button", { name: label, exact: true }).click();
  }
  await page.getByText("Incidencia solucionada", { exact: false }).waitFor();
  await page.getByRole("button", { name: "Editar datos" }).click();
  await page.locator("[name=ubicacion]").fill("Biblioteca central");
  await page
    .getByRole("button", { name: "Guardar reporte", exact: true })
    .click();
  await page
    .getByRole("dialog")
    .getByText("Biblioteca central", { exact: true })
    .waitFor();
  await page.getByRole("button", { name: "Cerrar", exact: true }).click();
  await page.getByRole("textbox", { name: "Buscar reportes" }).fill("zzzz");
  await page.getByRole("heading", { name: "No hay coincidencias" }).waitFor();
  await page.getByRole("textbox", { name: "Buscar reportes" }).fill("");
  await page.getByRole("link", { name: "Personas", exact: false }).click();
  await page.locator("[name=nombre]").fill("Ana Pérez");
  await page.locator("[name=correo]").fill("ana@example.edu");
  await page
    .getByRole("button", { name: "Registrar persona", exact: true })
    .click();
  await page.getByText("Ana Pérez", { exact: true }).waitFor();
  await page.getByRole("link", { name: "Tablero de estados" }).click();
  await page.locator(".ticket-card").first().waitFor();
  await page.screenshot({ path: "artifacts/kanban.png", fullPage: true });
  await page.setViewportSize({ width: 390, height: 844 });
  await page.getByRole("link", { name: "Vista general" }).click();
  await page
    .getByRole("heading", { name: "Cada incidencia, bajo control." })
    .waitFor();
  await page.screenshot({ path: "artifacts/mobile.png", fullPage: true });
  const overflow = await page.evaluate(
    () => document.documentElement.scrollWidth > innerWidth,
  );
  if (overflow) throw Error("Horizontal viewport overflow");
  if (errors.length) throw Error(errors.join("\n"));
  console.log(
    "PASS: navigation, creation, sequential states, history, editing, filters, persons, kanban, mobile overflow and browser console (mock API).",
  );
  await browser.close();
})().catch((e) => {
  console.error(e);
  process.exit(1);
});
