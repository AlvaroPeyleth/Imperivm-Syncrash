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
    private readonly Label status = new Label();
    private bool busy;
    private Image banner;
    private Image brand;

    internal SyncrashWindow()
    {
        Text = "Syncrash v1 · Imperivm";
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
        AddRow(body, Copy("Protección de cierres disponible. Desyncs en desarrollo.", 10F));

        var editions = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, Margin = new Padding(0, 12, 0, 12) };
        editions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        editions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        editions.Controls.Add(Edition("Steam vanilla", "Juego base de Steam", true), 0, 0);
        editions.Controls.Add(Edition("Community Mod", "Próximamente · requiere el mod", false), 1, 0);
        AddRow(body, editions);

        AddRow(body, Copy("Instalación del juego", 9F, true));
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

        AddRow(body, Copy("Solo modifica gbr.exe. Sin envío de datos.", 9.5F));
        var recovery = Copy("Para quitarlo, verifica los archivos del juego en Steam.\nNo crea copia de seguridad.", 9.5F);
        recovery.Margin = new Padding(0, 9, 0, 14);
        AddRow(body, recovery);

        var repo = new LinkLabel { Text = Repository.Replace("https://", ""), AutoSize = true, Dock = DockStyle.Top, LinkColor = Crimson, ActiveLinkColor = Ink, VisitedLinkColor = Crimson, Margin = Padding.Empty, AccessibleName = "Abrir repositorio de Syncrash" };
        repo.LinkClicked += delegate
        {
            try { Process.Start(new ProcessStartInfo(Repository) { UseShellExecute = true }); }
            catch (Exception) { status.Text = "No se pudo abrir el navegador. El enlace está en el README del proyecto."; }
        };
        AddRow(body, repo);
        AddRow(body, Copy("Código y documentación · privado por ahora", 8.5F));

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

    private static Control Edition(string title, string description, bool active)
    {
        var box = new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, ColumnCount = 1, Padding = new Padding(12), Margin = new Padding(0, 0, active ? 12 : 0, 0), BackColor = active ? Color.FromArgb(240, 235, 230) : Color.FromArgb(239, 238, 235), CellBorderStyle = TableLayoutPanelCellBorderStyle.None };
        var option = new RadioButton { Text = title, Checked = active, Enabled = active, AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold), Margin = new Padding(0, 0, 0, 5) };
        var explanation = Copy(description, 9F);
        explanation.ForeColor = Color.FromArgb(85, 78, 69);
        box.Controls.Add(option, 0, 0);
        box.Controls.Add(explanation, 0, 1);
        return box;
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

    private async Task ApplyAsync()
    {
        if (busy) return;
        if (string.IsNullOrWhiteSpace(path.Text) && !PickGame()) return;
        string target = path.Text;
        busy = true;
        apply.Enabled = browse.Enabled = false;
        apply.Text = "Aplicando…";
        status.Text = "Comprobando archivos y aplicando Syncrash…";
        status.ForeColor = Ink;
        try
        {
            status.Text = await Task.Run(() => Syncrash.PatchGame(target));
            status.ForeColor = Color.FromArgb(34, 87, 58);
        }
        catch (UnauthorizedAccessException)
        {
            status.ForeColor = Crimson;
            status.Text = "Sin permiso para escribir. Cierra Syncrash y ábrelo como administrador.";
        }
        catch (Exception error)
        {
            status.ForeColor = Crimson;
            status.Text = error.Message;
            MessageBox.Show(this, error.Message, "No se pudo aplicar Syncrash", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally
        {
            busy = false;
            apply.Enabled = browse.Enabled = true;
            apply.Text = "Aplicar parche";
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && banner != null) { banner.Dispose(); banner = null; }
        if (disposing && brand != null) { brand.Dispose(); brand = null; }
        base.Dispose(disposing);
    }
}
