using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

internal sealed class SyncrashWindow : Form
{
    private const string Repository = "https://github.com/AlvaroPeyleth/Imperivm-Syncrash";
    private static readonly Color Ink = Color.FromArgb(40, 36, 31);
    private static readonly Color Paper = Color.FromArgb(247, 246, 243);
    private static readonly Color Crimson = Color.FromArgb(112, 43, 42);
    private readonly TextBox path = new TextBox();
    private readonly Button browse = new Button();
    private readonly Button apply = new Button();
    private readonly Button restoreScreen = new Button();
    private readonly CheckBox adaptiveScreen = new CheckBox();
    private readonly Label status = new Label();
    private readonly ToolTip tips = new ToolTip { AutoPopDelay = 15000, InitialDelay = 350, ReshowDelay = 100 };
    private readonly bool screenAvailable = ScreenCompatibility.HasEmbeddedScreen();
    private bool busy;
    private Image banner;
    private Image brand;

    internal SyncrashWindow()
    {
        Text = "Syncrash 1.0.4 · Prueba local";
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Font = new Font("Segoe UI", 9F);
        ForeColor = Ink;
        BackColor = Paper;
        ClientSize = new Size(780, 566);
        MinimumSize = new Size(710, 520);
        StartPosition = FormStartPosition.CenterScreen;

        var frame = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Margin = Padding.Empty };
        frame.RowStyles.Add(new RowStyle(SizeType.Absolute, 128));
        frame.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        frame.RowStyles.Add(new RowStyle(SizeType.Absolute, 76));
        Controls.Add(frame);

        var hero = new BannerPanel { Dock = DockStyle.Fill, Margin = Padding.Empty, BackColor = Color.FromArgb(30, 27, 24) };
        using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Syncrash.Banner"))
        {
            if (stream != null) { using (var original = Image.FromStream(stream)) banner = new Bitmap(original); }
        }
        hero.BackgroundImage = banner;
        using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Syncrash.Icon"))
        {
            if (stream != null) Icon = new Icon(stream, 32, 32);
        }
        using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Syncrash.Mark"))
        {
            if (stream != null) { using (var original = Image.FromStream(stream)) brand = new Bitmap(original); }
        }
        var mark = new PictureBox { Image = brand, BackColor = Color.Transparent, SizeMode = PictureBoxSizeMode.Zoom, Location = new Point(26, 36), Size = new Size(52, 52), TabStop = false };
        var title = new Label { Text = "Syncrash", Font = new Font("Georgia", 28F), ForeColor = Color.FromArgb(248, 242, 229), BackColor = Color.Transparent, AutoSize = true, Location = new Point(90, 28) };
        var subtitle = new Label { Text = "Estabilidad para Imperivm", Font = new Font("Segoe UI", 10F), ForeColor = Color.FromArgb(235, 227, 213), BackColor = Color.Transparent, AutoSize = true, Location = new Point(94, 78) };
        hero.Controls.Add(mark);
        hero.Controls.Add(title);
        hero.Controls.Add(subtitle);
        frame.Controls.Add(hero, 0, 0);

        var scroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Margin = Padding.Empty };
        var body = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, Padding = new Padding(28, 20, 28, 14) };
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        scroll.Controls.Add(body);
        frame.Controls.Add(scroll, 0, 1);
        AddRow(body, Copy("Tu instalación de Imperivm", 12F, true));
        AddRow(body, Copy("Edición HD de Steam · juego base sin otros mods", 9F));
        var location = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, Margin = new Padding(0, 4, 0, 12) };
        location.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        location.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 102));
        path.ReadOnly = true;
        path.Dock = DockStyle.Fill;
        path.Margin = new Padding(0, 5, 10, 0);
        path.AccessibleName = "Ruta de gbr.exe";
        path.BackColor = Color.White;
        browse.Text = "Elegir…";
        browse.Dock = DockStyle.Fill;
        browse.Height = 32;
        browse.Margin = Padding.Empty;
        browse.Click += delegate { PickGame(); };
        location.Controls.Add(path, 0, 0);
        location.Controls.Add(browse, 1, 0);
        AddRow(body, location);

        var included = Copy("Incluido: protección de cierres y ampliación de memoria", 10F, true);
        included.Margin = new Padding(0, 4, 0, 16);
        AddRow(body, included);
        adaptiveScreen.Text = "Añadir pantalla adaptable";
        adaptiveScreen.AccessibleName = "Añadir pantalla adaptable con suavizado, opcional";
        adaptiveScreen.Checked = screenAvailable;
        adaptiveScreen.Enabled = screenAvailable;
        adaptiveScreen.AutoSize = true;
        adaptiveScreen.Anchor = AnchorStyles.Left;
        adaptiveScreen.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        adaptiveScreen.Margin = new Padding(0, 0, 0, 5);
        AddRow(body, adaptiveScreen);
        AddRow(body, Copy(screenAvailable ? "Incluye suavizado de imagen. Mantiene la resolución del escritorio." :
            "Esta compilación de desarrollo no incluye los componentes de pantalla.", 9F));
        restoreScreen.Text = "Restaurar pantalla original";
        restoreScreen.AutoSize = true;
        restoreScreen.Anchor = AnchorStyles.Left;
        restoreScreen.Margin = new Padding(0, 12, 0, 16);
        restoreScreen.AccessibleName = "Retirar la compatibilidad de pantalla conservando el parche de memoria y cierres";
        restoreScreen.Click += async delegate { await ApplyAsync(true); };
        AddRow(body, restoreScreen);

        var repo = new LinkLabel { Text = "Proyecto y código", AutoSize = true, LinkColor = Crimson, ActiveLinkColor = Ink, VisitedLinkColor = Crimson, Margin = new Padding(0, 0, 20, 0), AccessibleName = "Abrir repositorio de Syncrash" };
        repo.LinkClicked += delegate
        {
            try { Process.Start(new ProcessStartInfo(Repository) { UseShellExecute = true }); }
            catch (Exception) { status.Text = "No se pudo abrir el navegador. El enlace está en el README del proyecto."; }
        };
        var links = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Top, Margin = new Padding(0, 6, 0, 0) };
        var help = new LinkLabel { Text = "Información", AutoSize = true, LinkColor = Crimson, Margin = new Padding(0, 0, 20, 0), AccessibleName = "Mostrar información y recuperación del parche" };
        var sourceLink = new LinkLabel { Text = "Licencias y fuentes de pantalla", AutoSize = true, LinkColor = Crimson, Margin = Padding.Empty, Visible = screenAvailable };
        links.Controls.Add(help);
        links.Controls.Add(repo);
        links.Controls.Add(sourceLink);
        AddRow(body, links);
        var details = Copy("MEMORIA Y CIERRES\nProtección para tres rutas de cierre identificadas. Hasta 4 GB de memoria virtual en Windows de 64 bits; no garantiza más FPS ni corrige fugas. No corrige todavía las desincronizaciones.\n\nPANTALLA ADAPTABLE\nHasta 1080p internos, ampliados al monitor principal con márgenes negros. Conserva el modo del escritorio. Los componentes se incluyen en este EXE y se instalan junto al juego. No necesitas abrir Syncrash para jugar. Desmarcar la opción conserva una instalación anterior; para quitarla usa Restaurar pantalla original.\n\nSUAVIZADO INCLUIDO\nPantalla adaptable está marcada por defecto y es opcional. Incluye el filtro de GPU para la imagen ampliada. No cambia texturas ni añade resolución interna. Si no está disponible, utiliza la presentación sin filtro. Para cambiar una configuración de pantalla ya instalada, restaura la pantalla y vuelve a aplicar.\n\nRECUPERACIÓN\nPara quitar todo el parche, restaura primero la pantalla y después verifica los archivos en Steam. No se crea copia de gbr.exe. Solo se admiten los archivos originales o resultados de Syncrash reconocidos. Community Mod no está disponible.\n\nLICENCIAS\nCreado por AlvaroPeyleth · Discord: xtalvarotx\nAplicador MIT; pantalla con licencia independiente incluida. El enlace de fuentes guarda una copia local de su licencia y código. No descarga mods ni envía datos.", 9F);
        details.Margin = new Padding(0, 12, 0, 8);
        details.Visible = false;
        AddRow(body, details);
        help.LinkClicked += delegate
        {
            details.Visible = !details.Visible;
            help.Text = details.Visible ? "Ocultar información" : "Información";
            if (details.Visible) scroll.ScrollControlIntoView(details);
        };
        tips.SetToolTip(help, "Memoria, pantalla y cómo retirar el parche. Pulsa para leer la información completa.");
        tips.SetToolTip(adaptiveScreen, "Encaja el juego con márgenes negros y suavizado mediante la GPU, sin cambiar el escritorio. Puedes desmarcarla para aplicar solo memoria y cierres. Desmarcar no retira una instalación anterior.");
        tips.SetToolTip(included, "Hasta 4 GB de memoria virtual en Windows de 64 bits. No garantiza más FPS ni elimina todos los cierres.");
        tips.SetToolTip(restoreScreen, "Retira solo los archivos de pantalla registrados por Syncrash. Conserva memoria y protección de cierres.");
        sourceLink.LinkClicked += delegate { ExportScreenSources(); };

        var footer = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, BackColor = Color.FromArgb(236, 234, 229), Padding = new Padding(28, 16, 28, 16), Margin = Padding.Empty };
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 186));
        status.Dock = DockStyle.Fill;
        status.Text = "Cierra el juego antes de continuar.";
        status.TextAlign = ContentAlignment.MiddleLeft;
        status.Margin = new Padding(0, 0, 16, 0);
        status.AccessibleName = "Estado del parche";
        apply.Text = "Aplicar parche";
        apply.AccessibleName = "Aplicar parche Steam vanilla";
        apply.Dock = DockStyle.Fill;
        apply.Margin = Padding.Empty;
        apply.BackColor = Crimson;
        apply.ForeColor = Color.White;
        apply.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        apply.FlatStyle = FlatStyle.Flat;
        apply.FlatAppearance.BorderSize = 0;
        apply.Click += async delegate { await ApplyAsync(); };
        footer.Controls.Add(status, 0, 0);
        footer.Controls.Add(apply, 1, 0);
        frame.Controls.Add(footer, 0, 2);
        Shown += delegate
        {
            // Opening the window never changes game files.
            try
            {
                List<string> found = Syncrash.FindSteamGames();
                if (found.Count == 1) path.Text = found[0];
                else status.Text = "Elige el gbr.exe de la instalación que quieres parchear.";
            }
            catch (Exception) { status.Text = "Selecciona gbr.exe con el botón Elegir."; }
            Rectangle available = Screen.FromControl(this).WorkingArea;
            if (Height > available.Height - 30) Height = available.Height - 30;
        };
        FormClosing += delegate(object sender, FormClosingEventArgs args)
        {
            if (busy) { args.Cancel = true; status.Text = "Espera a que termine la aplicación del parche."; }
        };
    }

    private static Label Copy(string text, float size, bool bold = false)
    {
        return new Label { Text = text, AutoSize = true, Dock = DockStyle.Top, Font = new Font("Segoe UI", size, bold ? FontStyle.Bold : FontStyle.Regular), Margin = new Padding(0, 0, 0, 3) };
    }

    private sealed class BannerPanel : Panel
    {
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (BackgroundImage == null) { base.OnPaintBackground(e); return; }
            float scale = Math.Max((float)Width / BackgroundImage.Width, (float)Height / BackgroundImage.Height);
            float sourceWidth = Width / scale;
            float sourceHeight = Height / scale;
            e.Graphics.DrawImage(BackgroundImage, ClientRectangle,
                new RectangleF((BackgroundImage.Width - sourceWidth) / 2, (BackgroundImage.Height - sourceHeight) / 2, sourceWidth, sourceHeight), GraphicsUnit.Pixel);
        }
    }

    private static void AddRow(TableLayoutPanel table, Control control)
    {
        int row = table.RowCount++;
        table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        table.Controls.Add(control, 0, row);
    }

    private void ExportScreenSources()
    {
        using (var picker = new SaveFileDialog { Title = "Guardar licencias y fuentes de pantalla", Filter = "Archivo ZIP|*.zip", FileName = "Syncrash-fuentes-pantalla.zip", OverwritePrompt = false })
        {
            if (picker.ShowDialog(this) != DialogResult.OK) return;
            try { ScreenCompatibility.ExportScreenSources(picker.FileName); status.Text = "Licencias y fuentes guardadas."; }
            catch (Exception error) { status.Text = "No se pudo guardar: " + error.Message; MessageBox.Show(this, status.Text, "Licencias y fuentes", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }
    }

    private bool PickGame()
    {
        using (var picker = new OpenFileDialog())
        {
            picker.Title = "Selecciona gbr.exe de Imperivm Steam vanilla";
            picker.Filter = "Ejecutable de Imperivm (gbr.exe)|gbr.exe";
            picker.FileName = "gbr.exe";
            if (picker.ShowDialog(this) != DialogResult.OK) return false;
            path.Text = picker.FileName;
            status.Text = "Cierra Imperivm antes de aplicar el parche.";
            return true;
        }
    }

    private static bool HasAccessDenied(Exception error)
    {
        if (error == null) return false;
        if (error is UnauthorizedAccessException) return true;
        var aggregate = error as AggregateException;
        if (aggregate != null)
        {
            foreach (Exception inner in aggregate.InnerExceptions)
                if (HasAccessDenied(inner)) return true;
            return false;
        }
        return HasAccessDenied(error.InnerException);
    }

    internal static string DescribeApplyError(Exception error)
    {
        return HasAccessDenied(error)
            ? error.Message + "\n\nAcceso denegado. Comprueba permisos, bloqueos y el aviso de seguridad de Windows. No se ha forzado la aplicación."
            : error.Message;
    }

    private async Task ApplyAsync(bool removeScreen = false)
    {
        if (busy) return;
        if (string.IsNullOrWhiteSpace(path.Text) && !PickGame()) return;
        string target = path.Text;
        bool includeScreen = adaptiveScreen.Checked;
        busy = true;
        apply.Enabled = browse.Enabled = restoreScreen.Enabled = adaptiveScreen.Enabled = false;
        apply.Text = removeScreen ? "Restaurando…" : "Aplicando…";
        status.Text = removeScreen ? "Retirando la compatibilidad de pantalla…" : "Comprobando archivos y aplicando Syncrash…";
        status.ForeColor = Ink;
        try
        {
            status.Text = await Task.Run(() => removeScreen ? ScreenCompatibility.RemoveGame(target) : Syncrash.PatchGame(target, includeScreen, includeScreen));
            if (removeScreen) adaptiveScreen.Checked = false;
            status.ForeColor = Color.FromArgb(34, 87, 58);
        }
        catch (PatchBusyException error)
        {
            status.ForeColor = Crimson;
            status.Text = error.Message;
        }
        catch (Exception error)
        {
            status.ForeColor = Crimson;
            status.Text = DescribeApplyError(error);
            MessageBox.Show(this, status.Text, removeScreen ? "No se pudo restaurar la pantalla" : "No se pudo aplicar Syncrash", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally
        {
            busy = false;
            apply.Enabled = browse.Enabled = restoreScreen.Enabled = true;
            adaptiveScreen.Enabled = screenAvailable;
            apply.Text = "Aplicar parche";
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) tips.Dispose();
        if (disposing && banner != null) { banner.Dispose(); banner = null; }
        if (disposing && brand != null) { brand.Dispose(); brand = null; }
        base.Dispose(disposing);
    }
}
