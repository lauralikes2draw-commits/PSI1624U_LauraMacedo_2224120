using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace ProjetoFinal
{
    internal static class AdminDialogos
    {
        public static void MostrarDetalheCliente(Form owner, AdminCliente c)
        {
            if (c == null) return;
            using (Form f = BaseForm("Detalhes da cliente", 700, 640))
            {
                AddHeader(f, "Detalhes da cliente", "Perfil, histórico e ações rápidas.");

                PictureBox pic = new PictureBox();
                pic.Image = AdminSharedUi.CarregarImagemPerfil(c.Foto);
                pic.SizeMode = PictureBoxSizeMode.Zoom;
                pic.Location = new Point(38, 112);
                pic.Size = new Size(108, 108);
                f.Controls.Add(pic);

                f.Controls.Add(AdminSharedUi.MakeLabel(c.Nome, 170, 118, 430, 32, 17, true, AdminSharedUi.Texto));
                f.Controls.Add(AdminSharedUi.MakeLabel(c.Email + "  •  " + c.Telefone, 170, 154, 430, 24, 10, false, AdminSharedUi.Cinza));
                f.Controls.Add(AdminSharedUi.MakeLabel("Estado: " + (c.Ativo ? "Ativo" : "Inativo"), 170, 184, 220, 24, 10, true, c.Ativo ? AdminSharedUi.Verde : AdminSharedUi.Vermelho));

                string ultimo = c.UltimaMarcacaoData.HasValue ? c.UltimaMarcacaoData.Value.ToString("dd/MM/yyyy") + (c.UltimaMarcacaoHora.HasValue ? " às " + c.UltimaMarcacaoHora.Value.ToString(@"hh\:mm") : "") : "Sem serviços registados";
                AddInfo(f, "Serviços realizados", c.TotalServicos.ToString(), 38, 255);
                AddInfo(f, "Total gasto", AdminRepository.Money(c.TotalGasto), 246, 255);
                AddInfo(f, "Marcações este mês", c.MarcacoesMes.ToString(), 454, 255);
                AddInfo(f, "Último serviço", string.IsNullOrWhiteSpace(c.UltimoServico) ? "Nenhum" : c.UltimoServico, 38, 355);
                AddInfo(f, "Data e hora", ultimo, 246, 355);
                AddInfo(f, "Vem todos os meses?", c.MarcacoesMes > 0 ? "Sim" : "Não", 454, 355);

                Button btnAgenda = Button("Ver agenda", 38, 505);
                btnAgenda.Click += delegate { MostrarAgendaCliente(owner, c.IdUsuario, c.Nome); };
                Button btnEditar = Button("Editar perfil", 196, 505);
                btnEditar.Click += delegate { EditarCliente(c); f.Close(); };
                Button btnEliminar = Button("Eliminar", 354, 505);
                btnEliminar.BackColor = AdminSharedUi.Vermelho;
                btnEliminar.Click += delegate { if (Confirm("Deseja eliminar/desativar esta conta?")) { AdminRepository.DesativarUsuario(c.IdUsuario, false); MessageBox.Show("Cliente desativada com sucesso."); f.Close(); } };
                Button btnMensagem = Button("Contactar", 512, 505);
                btnMensagem.Click += delegate { AdminSharedUi.MostrarContacto(c); };
                f.Controls.Add(btnAgenda); f.Controls.Add(btnEditar); f.Controls.Add(btnEliminar); f.Controls.Add(btnMensagem);
                f.ShowDialog(owner);
            }
        }

        public static void MostrarAgendaCliente(Form owner, int idCliente, string nome)
        {
            using (Form f = BaseForm("Agenda de " + nome, 1080, 740))
            {
                AddHeader(f, "Agenda de " + nome, "Agenda diária da cliente no mesmo estilo das marcações.");
                DateTime dataAtual = DateTime.Today;

                DateTimePicker picker = new DateTimePicker();
                picker.Format = DateTimePickerFormat.Long;
                picker.Value = dataAtual;
                picker.Location = new Point(42, 112);
                picker.Size = new Size(260, 34);
                picker.Font = new Font("Segoe UI", 9.5f);

                Button prev = Button("‹", 318, 108);
                prev.Size = new Size(44, 38);
                prev.Font = new Font("Segoe UI", 16, FontStyle.Bold);

                Button next = Button("›", 370, 108);
                next.Size = new Size(44, 38);
                next.Font = new Font("Segoe UI", 16, FontStyle.Bold);

                f.Controls.Add(picker);
                f.Controls.Add(prev);
                f.Controls.Add(next);

                Panel agenda = Card(42, 166, 970, 500);
                agenda.AutoScroll = true;
                agenda.BackColor = Color.White;
                f.Controls.Add(agenda);

                Action carregar = delegate
                {
                    dataAtual = picker.Value.Date;
                    agenda.Controls.Clear();
                    List<AdminMarcacao> todas = AdminRepository.GetMarcacoes(nome, "Todos", dataAtual, dataAtual, 500);
                    List<AdminMarcacao> marcacoes = todas.Where(m => m.IdCliente == idCliente || string.Equals(m.Cliente, nome, StringComparison.OrdinalIgnoreCase)).OrderBy(m => m.Hora).ToList();

                    DesenharLinhasAgenda(agenda);
                    if (marcacoes.Count == 0)
                    {
                        Panel vazio = Card(120, 90, 780, 150);
                        Label icon = AdminSharedUi.MakeLabel("♡", 0, 14, 780, 55, 34, true, AdminSharedUi.Rosa);
                        icon.TextAlign = ContentAlignment.MiddleCenter;
                        vazio.Controls.Add(icon);
                        Label msg = AdminSharedUi.MakeLabel("Não há marcações para esta cliente neste dia.", 0, 75, 780, 32, 14, true, AdminSharedUi.Texto);
                        msg.TextAlign = ContentAlignment.MiddleCenter;
                        vazio.Controls.Add(msg);
                        Label sub = AdminSharedUi.MakeLabel("Use as setas para navegar pelos dias da agenda.", 0, 110, 780, 26, 9.5f, false, AdminSharedUi.Cinza);
                        sub.TextAlign = ContentAlignment.MiddleCenter;
                        vazio.Controls.Add(sub);
                        agenda.Controls.Add(vazio);
                        vazio.BringToFront();
                        return;
                    }

                    int ultimoFim = 0;
                    foreach (AdminMarcacao m in marcacoes)
                    {
                        int top = 18 + Math.Max(0, (int)((m.Hora.TotalMinutes - 9 * 60) * 58 / 60));
                        if (top < ultimoFim + 8) top = ultimoFim + 8;
                        Panel card = AgendaMarcacaoCard(m);
                        card.Location = new Point(120, top);
                        TornarAgendaClicavel(card, f, m);
                        agenda.Controls.Add(card);
                        card.BringToFront();
                        ultimoFim = top + card.Height;
                    }
                };

                prev.Click += delegate { picker.Value = picker.Value.AddDays(-1); };
                next.Click += delegate { picker.Value = picker.Value.AddDays(1); };
                picker.ValueChanged += delegate { carregar(); };
                carregar();
                f.ShowDialog(owner);
            }
        }

        public static void MostrarDetalheProfissional(Form owner, AdminProfissional p)
        {
            if (p == null) return;
            using (Form f = BaseForm("Detalhes da profissional", 680, 540))
            {
                AddHeader(f, "Detalhes da profissional", "Perfil, comissão e desempenho.");
                PictureBox pic = new PictureBox();
                pic.Image = AdminSharedUi.CarregarImagemPerfil(p.Foto);
                pic.SizeMode = PictureBoxSizeMode.Zoom;
                pic.Location = new Point(38, 115);
                pic.Size = new Size(108, 108);
                f.Controls.Add(pic);
                f.Controls.Add(AdminSharedUi.MakeLabel(p.Nome, 170, 122, 410, 30, 17, true, AdminSharedUi.Texto));
                f.Controls.Add(AdminSharedUi.MakeLabel(p.Especialidade + " • " + p.Email, 170, 158, 410, 24, 10, false, AdminSharedUi.Cinza));
                f.Controls.Add(AdminSharedUi.MakeLabel("Comissão: " + p.ComissaoPercentual.ToString("0.##") + "%", 170, 188, 220, 24, 10, true, AdminSharedUi.Rosa));
                AddInfo(f, "Avaliação", p.Avaliacao.ToString("0.0") + "/5", 38, 260);
                AddInfo(f, "Atendimentos este mês", p.AtendimentosMes.ToString(), 246, 260);
                AddInfo(f, "Faturação este mês", AdminRepository.Money(p.FaturacaoMes), 454, 260);
                Button msg = Button("Enviar mensagem", 38, 420);
                msg.Click += delegate { MostrarEnviarMensagem(owner, new List<int> { p.IdUsuario }); };
                Button des = Button(p.Ativo ? "Desativar" : "Ativar", 210, 420);
                des.BackColor = p.Ativo ? AdminSharedUi.Vermelho : AdminSharedUi.Verde;
                des.Click += delegate { AdminRepository.DesativarUsuario(p.IdUsuario, !p.Ativo); MessageBox.Show("Estado atualizado."); f.Close(); };
                f.Controls.Add(msg); f.Controls.Add(des);
                f.ShowDialog(owner);
            }
        }

        public static void MostrarDetalheServico(Form owner, AdminServico s)
        {
            if (s == null) return;
            using (Form f = BaseForm("Detalhes do serviço", 680, 560))
            {
                AddHeader(f, "Detalhes do serviço", "Imagem, preço, duração e estatísticas.");
                PictureBox pic = new PictureBox();
                pic.Image = AdminSharedUi.CarregarImagemServico(s.Foto, s.Categoria);
                pic.SizeMode = PictureBoxSizeMode.Zoom;
                pic.Location = new Point(38, 115);
                pic.Size = new Size(110, 110);
                f.Controls.Add(pic);
                f.Controls.Add(AdminSharedUi.MakeLabel(s.Nome, 170, 120, 430, 30, 17, true, AdminSharedUi.Texto));
                f.Controls.Add(AdminSharedUi.MakeLabel(s.Categoria + " • " + s.DuracaoMinutos + " min", 170, 156, 430, 24, 10, false, AdminSharedUi.Cinza));
                f.Controls.Add(AdminSharedUi.MakeLabel(AdminRepository.Money(s.Preco), 170, 186, 220, 28, 14, true, AdminSharedUi.Rosa));
                AddInfo(f, "Popularidade", s.TotalMarcacoes + " marcações", 38, 260);
                AddInfo(f, "Total faturado", AdminRepository.Money(s.TotalFaturado), 246, 260);
                AddInfo(f, "Avaliação", s.Avaliacao.ToString("0.0") + "/5", 454, 260);
                Label desc = AdminSharedUi.MakeLabel("Descrição: " + s.Descricao, 38, 375, 580, 70, 10, false, AdminSharedUi.Texto);
                f.Controls.Add(desc);
                Button des = Button(s.Ativo ? "Desativar" : "Reativar", 38, 465);
                des.BackColor = s.Ativo ? AdminSharedUi.Laranja : AdminSharedUi.Verde;
                des.Click += delegate { AdminRepository.DesativarServico(s.IdServico, !s.Ativo); MessageBox.Show(s.Ativo ? "Serviço desativado temporariamente." : "Serviço reativado."); f.Close(); };
                Button del = Button("Eliminar", 205, 465);
                del.BackColor = AdminSharedUi.Vermelho;
                del.Click += delegate { if (Confirm("Deseja eliminar este serviço do catálogo? Ele sai das categorias e das marcações futuras, mas o histórico antigo fica guardado.")) { AdminRepository.EliminarServico(s.IdServico); MessageBox.Show("Serviço eliminado do catálogo."); f.Close(); } };
                f.Controls.Add(des);
                f.Controls.Add(del);
                f.ShowDialog(owner);
            }
        }

        public static void MostrarDetalheFatura(Form owner, AdminFatura fat)
        {
            if (fat == null) return;
            using (Form f = BaseForm("Fatura / Recibo", 680, 720))
            {
                Label logo = AdminSharedUi.MakeLabel("BEAUTECARE", 0, 28, 650, 34, 20, true, AdminSharedUi.Rosa);
                logo.TextAlign = ContentAlignment.MiddleCenter;
                Label subt = AdminSharedUi.MakeLabel("Fatura / Recibo", 0, 62, 650, 26, 12, true, AdminSharedUi.Texto);
                subt.TextAlign = ContentAlignment.MiddleCenter;
                f.Controls.Add(logo); f.Controls.Add(subt);
                Button close = SmallCloseButton(f.Width - 76, 26); close.Click += delegate { f.Close(); }; f.Controls.Add(close);

                Panel linha = new Panel(); linha.Location = new Point(64, 110); linha.Size = new Size(540, 1); linha.BackColor = Color.FromArgb(235, 210, 220); f.Controls.Add(linha);
                f.Controls.Add(AdminSharedUi.MakeLabel("N.º", 64, 132, 130, 24, 10, true, AdminSharedUi.Texto));
                f.Controls.Add(AdminSharedUi.MakeLabel(string.IsNullOrWhiteSpace(fat.NumeroFatura) ? "Sem número" : fat.NumeroFatura, 238, 132, 340, 24, 10, true, AdminSharedUi.Texto));
                f.Controls.Add(AdminSharedUi.MakeLabel("Data", 64, 170, 130, 24, 10, true, AdminSharedUi.Texto));
                f.Controls.Add(AdminSharedUi.MakeLabel(fat.DataFatura.ToString("dd/MM/yyyy"), 238, 170, 340, 24, 10, false, AdminSharedUi.Texto));
                f.Controls.Add(AdminSharedUi.MakeLabel("Cliente", 64, 208, 130, 24, 10, true, AdminSharedUi.Texto));
                f.Controls.Add(AdminSharedUi.MakeLabel(fat.Cliente, 238, 208, 340, 24, 10, false, AdminSharedUi.Texto));

                Panel linha2 = new Panel(); linha2.Location = new Point(64, 258); linha2.Size = new Size(540, 1); linha2.BackColor = Color.FromArgb(235, 210, 220); f.Controls.Add(linha2);
                f.Controls.Add(AdminSharedUi.MakeLabel("Serviço(s)", 64, 278, 320, 26, 11, true, AdminSharedUi.Texto));
                f.Controls.Add(AdminSharedUi.MakeLabel("Valor", 492, 278, 100, 26, 11, true, AdminSharedUi.Texto));
                Label serv = AdminSharedUi.MakeLabel(string.IsNullOrWhiteSpace(fat.Servicos) ? "Serviço não indicado" : fat.Servicos, 64, 322, 390, 64, 10, false, AdminSharedUi.Texto);
                f.Controls.Add(serv);
                Label valor = AdminSharedUi.MakeLabel(AdminRepository.Money(fat.Subtotal), 474, 322, 120, 24, 10, false, AdminSharedUi.Texto);
                valor.TextAlign = ContentAlignment.MiddleRight;
                f.Controls.Add(valor);

                Panel totals = new Panel();
                totals.Location = new Point(64, 405);
                totals.Size = new Size(540, 105);
                totals.BackColor = Color.FromArgb(255, 240, 247);
                totals.Controls.Add(AdminSharedUi.MakeLabel("Subtotal", 250, 18, 130, 22, 10, true, AdminSharedUi.Cinza));
                Label subVal = AdminSharedUi.MakeLabel(AdminRepository.Money(fat.Subtotal), 400, 18, 110, 22, 10, true, AdminSharedUi.Texto); subVal.TextAlign = ContentAlignment.MiddleRight; totals.Controls.Add(subVal);
                totals.Controls.Add(AdminSharedUi.MakeLabel("Desconto", 250, 45, 130, 22, 10, true, AdminSharedUi.Cinza));
                Label descVal = AdminSharedUi.MakeLabel(AdminRepository.Money(fat.Desconto), 400, 45, 110, 22, 10, true, AdminSharedUi.Texto); descVal.TextAlign = ContentAlignment.MiddleRight; totals.Controls.Add(descVal);
                totals.Controls.Add(AdminSharedUi.MakeLabel("TOTAL", 28, 36, 180, 34, 18, true, AdminSharedUi.Rosa));
                Label totalVal = AdminSharedUi.MakeLabel(AdminRepository.Money(fat.Total), 360, 70, 150, 26, 13, true, AdminSharedUi.Rosa); totalVal.TextAlign = ContentAlignment.MiddleRight; totals.Controls.Add(totalVal);
                f.Controls.Add(totals);

                f.Controls.Add(AdminSharedUi.MakeLabel("Método de pagamento", 64, 535, 190, 24, 10, true, AdminSharedUi.Texto));
                f.Controls.Add(AdminSharedUi.MakeLabel(string.IsNullOrWhiteSpace(fat.MetodoPagamento) ? "Multibanco" : fat.MetodoPagamento, 270, 535, 220, 24, 10, false, AdminSharedUi.Texto));
                f.Controls.Add(AdminSharedUi.MakeLabel("Estado", 64, 570, 190, 24, 10, true, AdminSharedUi.Texto));
                Label estado = AdminSharedUi.MakeLabel(fat.Estado, 270, 570, 220, 24, 10, true, EstadoCor(fat.Estado));
                f.Controls.Add(estado);

                Button imprimir = Button("Imprimir", 64, 630);
                imprimir.Click += delegate { ImprimirFatura(fat); };
                Button paga = Button("Marcar paga", 222, 630);
                paga.Click += delegate { AdminRepository.SetEstadoFatura(fat.IdFatura, "Paga"); MessageBox.Show("Fatura atualizada."); f.Close(); };
                Button fechar = Button("Fechar", 380, 630); fechar.BackColor = Color.FromArgb(230, 230, 230); fechar.ForeColor = AdminSharedUi.Texto; fechar.Click += delegate { f.Close(); };
                f.Controls.Add(imprimir); f.Controls.Add(paga); f.Controls.Add(fechar);
                Label obrigada = AdminSharedUi.MakeLabel("Obrigada pela preferência ♡", 0, 670, 650, 26, 10, true, AdminSharedUi.Rosa);
                obrigada.TextAlign = ContentAlignment.MiddleCenter;
                f.Controls.Add(obrigada);
                f.ShowDialog(owner);
            }
        }

        public static void MostrarDetalheMarcacao(Form owner, AdminMarcacao m)
        {
            if (m == null) return;
            using (Form f = BaseForm("Detalhes da marcação", 640, 500))
            {
                AddHeader(f, "Detalhes da marcação", "Cliente, profissional, serviço e estado.");
                f.Controls.Add(AdminSharedUi.MakeLabel(m.Cliente, 38, 118, 520, 32, 16, true, AdminSharedUi.Rosa));
                f.Controls.Add(AdminSharedUi.MakeLabel("Serviço: " + m.Servico, 38, 165, 520, 24, 10, false, AdminSharedUi.Texto));
                f.Controls.Add(AdminSharedUi.MakeLabel("Profissional: " + m.Profissional, 38, 195, 520, 24, 10, false, AdminSharedUi.Texto));
                f.Controls.Add(AdminSharedUi.MakeLabel("Data/Hora: " + m.DataMarcacao.ToString("dd/MM/yyyy") + " às " + m.Hora.ToString(@"hh\:mm"), 38, 225, 520, 24, 10, false, AdminSharedUi.Texto));
                f.Controls.Add(AdminSharedUi.MakeLabel("Valor: " + AdminRepository.Money(m.Valor), 38, 255, 520, 24, 10, true, AdminSharedUi.Texto));
                f.Controls.Add(AdminSharedUi.MakeLabel("Estado: " + m.Estado, 38, 285, 520, 24, 10, true, EstadoCor(m.Estado)));
                f.Controls.Add(AdminSharedUi.MakeLabel("Observações: " + m.Observacoes, 38, 318, 520, 55, 10, false, AdminSharedUi.Texto));
                Button confirmar = Button("Confirmar", 38, 405); confirmar.BackColor = AdminSharedUi.Verde;
                confirmar.Click += delegate { AdminRepository.SetEstadoMarcacao(m.IdMarcacao, "Confirmada"); f.Close(); };
                Button cancelar = Button("Cancelar", 196, 405); cancelar.BackColor = AdminSharedUi.Vermelho;
                cancelar.Click += delegate { AdminRepository.SetEstadoMarcacao(m.IdMarcacao, "Cancelada"); f.Close(); };
                f.Controls.Add(confirmar); f.Controls.Add(cancelar);
                f.ShowDialog(owner);
            }
        }

        public static void CriarCliente(Form owner, Action refresh)
        {
            using (Form f = BaseForm("Novo cliente", 760, 600))
            {
                AddHeader(f, "+ Novo cliente", "Crie uma conta para a cliente e entregue os dados de acesso.");
                Panel card = Card(38, 125, 660, 330);
                f.Controls.Add(card);
                TextBox nome = Input(card, "Nome completo", 28, 50, 280);
                TextBox email = Input(card, "Email", 350, 50, 280);
                TextBox tel = Input(card, "Telefone", 28, 140, 280);
                TextBox obs = Input(card, "Observações internas", 350, 140, 280); obs.Multiline = true; obs.Height = 74;
                Label info = AdminSharedUi.MakeLabel("Depois de criar, o sistema mostra o email e a senha temporária da cliente.", 28, 255, 590, 26, 10, false, AdminSharedUi.Cinza);
                card.Controls.Add(info);
                Button criar = Button("Criar cliente", 442, 495);
                criar.Click += delegate
                {
                    if (string.IsNullOrWhiteSpace(nome.Text) || string.IsNullOrWhiteSpace(email.Text)) { MessageBox.Show("Preencha o nome e o email."); return; }
                    try
                    {
                        string senha = AdminRepository.CriarCliente(nome.Text.Trim(), email.Text.Trim(), tel.Text.Trim());
                        MessageBox.Show("Novo cliente criado com sucesso.\n\nEmail: " + email.Text.Trim() + "\nSenha: " + senha, "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        if (refresh != null) refresh();
                        f.Close();
                    }
                    catch (Exception ex) { MessageBox.Show("Erro ao criar cliente: " + ex.Message); }
                };
                Button cancelar = Button("Cancelar", 590, 495); cancelar.BackColor = Color.FromArgb(235, 235, 235); cancelar.ForeColor = AdminSharedUi.Texto; cancelar.Click += delegate { f.Close(); };
                f.Controls.Add(criar); f.Controls.Add(cancelar);
                f.ShowDialog(owner);
            }
        }

        public static void EditarCliente(AdminCliente c)
        {
            if (c == null) return;
            using (Form f = BaseForm("Editar cliente", 560, 470))
            {
                AddHeader(f, "Editar cliente", "Atualize os dados principais da cliente.");
                TextBox nome = Input(f, "Nome completo", 42, 135, 460); nome.Text = c.Nome;
                TextBox email = Input(f, "Email", 42, 210, 460); email.Text = c.Email;
                TextBox tel = Input(f, "Telefone", 42, 285, 460); tel.Text = c.Telefone;
                Button guardar = Button("Guardar", 42, 370);
                guardar.Click += delegate
                {
                    AdminRepository.ExecuteNonQuery("UPDATE dbo.Usuarios SET Nome=@Nome, Email=@Email, Telefone=@Telefone WHERE IdUsuario=@Id", new Dictionary<string, object> { { "@Nome", nome.Text.Trim() }, { "@Email", email.Text.Trim() }, { "@Telefone", tel.Text.Trim() }, { "@Id", c.IdUsuario } });
                    MessageBox.Show("Cliente atualizado.");
                    f.Close();
                };
                f.Controls.Add(guardar);
                f.ShowDialog();
            }
        }

        public static void CriarProfissional(Form owner, Action refresh)
        {
            using (Form f = BaseForm("Nova profissional", 780, 660))
            {
                AddHeader(f, "+ Nova profissional", "Crie o acesso da profissional, defina especialidade e comissão.");
                Panel card = Card(38, 125, 680, 400);
                f.Controls.Add(card);
                TextBox nome = Input(card, "Nome completo", 28, 50, 290);
                TextBox email = Input(card, "Email", 350, 50, 290);
                TextBox tel = Input(card, "Telefone", 28, 135, 290);
                TextBox esp = Input(card, "Especialidade", 350, 135, 290);
                TextBox com = Input(card, "Comissão (%)", 28, 220, 290); com.Text = "40";
                TextBox ava = Input(card, "Avaliação inicial", 350, 220, 290); ava.Text = "5";
                card.Controls.Add(AdminSharedUi.MakeLabel("A comissão será usada nos cálculos das faturas e faturação da profissional.", 28, 315, 600, 24, 10, false, AdminSharedUi.Cinza));
                Button criar = Button("Criar profissional", 420, 555);
                criar.Click += delegate
                {
                    if (string.IsNullOrWhiteSpace(nome.Text) || string.IsNullOrWhiteSpace(email.Text)) { MessageBox.Show("Preencha o nome e o email."); return; }
                    decimal dcom = AdminRepository.ToDecimal(com.Text); if (dcom <= 0) dcom = 40;
                    decimal dava = AdminRepository.ToDecimal(ava.Text); if (dava <= 0) dava = 5;
                    try
                    {
                        string senha = AdminRepository.CriarProfissional(nome.Text.Trim(), email.Text.Trim(), tel.Text.Trim(), esp.Text.Trim(), dcom, dava);
                        MessageBox.Show("Nova profissional criada com sucesso.\n\nEmail: " + email.Text.Trim() + "\nSenha: " + senha, "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        if (refresh != null) refresh();
                        f.Close();
                    }
                    catch (Exception ex) { MessageBox.Show("Erro ao criar profissional: " + ex.Message); }
                };
                Button cancelar = Button("Cancelar", 585, 555); cancelar.BackColor = Color.FromArgb(235, 235, 235); cancelar.ForeColor = AdminSharedUi.Texto; cancelar.Click += delegate { f.Close(); };
                f.Controls.Add(criar); f.Controls.Add(cancelar);
                f.ShowDialog(owner);
            }
        }

        public static void CriarServico(Form owner, Action refresh)
        {
            using (Form f = BaseForm("Novo serviço", 780, 680))
            {
                AddHeader(f, "+ Novo serviço", "Adicione foto, categoria, duração e preço. O serviço fica disponível nas marcações.");
                string fotoPath = "";
                Panel card = Card(38, 125, 680, 420);
                f.Controls.Add(card);
                PictureBox preview = new PictureBox(); preview.Location = new Point(28, 38); preview.Size = new Size(120, 120); preview.SizeMode = PictureBoxSizeMode.Zoom; preview.Image = Properties.Resources.icons8_spa_mask_50; card.Controls.Add(preview);
                TextBox nome = Input(card, "Nome do serviço", 178, 50, 210);
                ComboBox cat = CategoriaCombo(card, "Categoria", 420, 50, 210, "");
                TextBox dur = Input(card, "Duração em minutos", 178, 135, 210); dur.Text = "60";
                TextBox preco = Input(card, "Preço (€)", 420, 135, 210);
                TextBox desc = Input(card, "Descrição", 28, 235, 600); desc.Height = 90; desc.Multiline = true;
                Button foto = Button("Escolher foto", 28, 175);
                foto.Click += delegate
                {
                    using (OpenFileDialog ofd = new OpenFileDialog())
                    {
                        ofd.Filter = "Imagens|*.jpg;*.jpeg;*.png;*.bmp";
                        if (ofd.ShowDialog(f) == DialogResult.OK)
                        {
                            string pasta = Path.Combine(Application.StartupPath, "FotosServicos");
                            if (!Directory.Exists(pasta)) Directory.CreateDirectory(pasta);
                            fotoPath = Path.Combine(pasta, "servico_" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(ofd.FileName));
                            File.Copy(ofd.FileName, fotoPath, true);
                            using (Image img = Image.FromFile(fotoPath)) preview.Image = new Bitmap(img);
                        }
                    }
                };
                card.Controls.Add(foto);
                Button criar = Button("Criar serviço", 420, 575);
                criar.Click += delegate
                {
                    if (string.IsNullOrWhiteSpace(nome.Text)) { MessageBox.Show("Preencha o nome do serviço."); return; }
                    int minutos; if (!int.TryParse(dur.Text, out minutos) || minutos <= 0) minutos = 60;
                    decimal valor = AdminRepository.ToDecimal(preco.Text);
                    try
                    {
                        AdminRepository.CriarServico(nome.Text.Trim(), cat.Text.Trim(), minutos, valor, desc.Text.Trim(), fotoPath);
                        MessageBox.Show("Serviço criado com sucesso. Ele já fica disponível nas marcações.", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        if (refresh != null) refresh();
                        f.Close();
                    }
                    catch (Exception ex) { MessageBox.Show("Erro ao criar serviço: " + ex.Message); }
                };
                Button cancelar = Button("Cancelar", 585, 575); cancelar.BackColor = Color.FromArgb(235, 235, 235); cancelar.ForeColor = AdminSharedUi.Texto; cancelar.Click += delegate { f.Close(); };
                f.Controls.Add(criar); f.Controls.Add(cancelar);
                f.ShowDialog(owner);
            }
        }

        public static void EditarProfissional(Form owner, AdminProfissional p, Action refresh)
        {
            if (p == null) return;
            using (Form f = BaseForm("Editar profissional", 780, 660))
            {
                AddHeader(f, "Editar profissional", "Altere apenas os campos que pretende atualizar.");
                Panel card = Card(38, 125, 680, 400); f.Controls.Add(card);
                PictureBox foto = new PictureBox(); foto.Image = AdminSharedUi.CarregarImagemPerfil(p.Foto); foto.SizeMode = PictureBoxSizeMode.Zoom; foto.Location = new Point(28, 38); foto.Size = new Size(112, 112); card.Controls.Add(foto);
                string fotoPath = p.Foto;
                Button trocarFoto = Button("Trocar foto", 22, 165); trocarFoto.Size = new Size(125, 34); trocarFoto.Click += delegate
                {
                    using (OpenFileDialog ofd = new OpenFileDialog())
                    {
                        ofd.Filter = "Imagens|*.jpg;*.jpeg;*.png;*.bmp";
                        if (ofd.ShowDialog(f) == DialogResult.OK)
                        {
                            string pasta = Path.Combine(Application.StartupPath, "FotosPerfis"); if (!Directory.Exists(pasta)) Directory.CreateDirectory(pasta);
                            fotoPath = Path.Combine(pasta, "prof_" + p.IdUsuario + Path.GetExtension(ofd.FileName)); File.Copy(ofd.FileName, fotoPath, true);
                            using (Image img = Image.FromFile(fotoPath)) foto.Image = new Bitmap(img);
                        }
                    }
                }; card.Controls.Add(trocarFoto);
                TextBox nome = Input(card, "Nome completo", 178, 50, 210); nome.Text = p.Nome;
                TextBox email = Input(card, "Email", 420, 50, 210); email.Text = p.Email;
                TextBox tel = Input(card, "Telefone", 178, 135, 210); tel.Text = p.Telefone;
                TextBox esp = Input(card, "Especialidade", 420, 135, 210); esp.Text = p.Especialidade;
                TextBox com = Input(card, "Comissão (%)", 178, 220, 210); com.Text = p.ComissaoPercentual.ToString("0.##");
                TextBox ava = Input(card, "Avaliação", 420, 220, 210); ava.Text = p.Avaliacao.ToString("0.0");
                Button guardar = Button("Guardar alterações", 410, 555); guardar.Size = new Size(165, 42);
                guardar.Click += delegate
                {
                    decimal dcom = AdminRepository.ToDecimal(com.Text); if (dcom <= 0) dcom = 40;
                    decimal dava = AdminRepository.ToDecimal(ava.Text); if (dava <= 0) dava = 5;
                    AdminRepository.AtualizarProfissional(p.IdUsuario, nome.Text.Trim(), email.Text.Trim(), tel.Text.Trim(), esp.Text.Trim(), dcom, dava, fotoPath);
                    MessageBox.Show("Profissional atualizado."); if (refresh != null) refresh(); f.Close();
                };
                Button cancelar = Button("Cancelar", 585, 555); cancelar.BackColor = Color.FromArgb(235, 235, 235); cancelar.ForeColor = AdminSharedUi.Texto; cancelar.Click += delegate { f.Close(); };
                f.Controls.Add(guardar); f.Controls.Add(cancelar); f.ShowDialog(owner);
            }
        }

        public static void EditarServico(Form owner, AdminServico s, Action refresh)
        {
            if (s == null) return;
            using (Form f = BaseForm("Editar serviço", 780, 680))
            {
                AddHeader(f, "Editar serviço", "Os campos já vêm preenchidos. Altere apenas o necessário.");
                string fotoPath = s.Foto;
                Panel card = Card(38, 125, 680, 420); f.Controls.Add(card);
                PictureBox preview = new PictureBox(); preview.Location = new Point(28, 38); preview.Size = new Size(120, 120); preview.SizeMode = PictureBoxSizeMode.Zoom; preview.Image = AdminSharedUi.CarregarImagemServico(s.Foto, s.Categoria); card.Controls.Add(preview);
                TextBox nome = Input(card, "Nome do serviço", 178, 50, 210); nome.Text = s.Nome;
                ComboBox cat = CategoriaCombo(card, "Categoria", 420, 50, 210, s.Categoria);
                TextBox dur = Input(card, "Duração em minutos", 178, 135, 210); dur.Text = s.DuracaoMinutos.ToString();
                TextBox preco = Input(card, "Preço (€)", 420, 135, 210); preco.Text = s.Preco.ToString("0.00");
                TextBox desc = Input(card, "Descrição", 28, 235, 600); desc.Text = s.Descricao; desc.Height = 90; desc.Multiline = true;
                Button foto = Button("Escolher foto", 28, 175); foto.Click += delegate
                {
                    using (OpenFileDialog ofd = new OpenFileDialog())
                    {
                        ofd.Filter = "Imagens|*.jpg;*.jpeg;*.png;*.bmp";
                        if (ofd.ShowDialog(f) == DialogResult.OK)
                        {
                            string pasta = Path.Combine(Application.StartupPath, "FotosServicos"); if (!Directory.Exists(pasta)) Directory.CreateDirectory(pasta);
                            fotoPath = Path.Combine(pasta, "servico_" + s.IdServico + Path.GetExtension(ofd.FileName)); File.Copy(ofd.FileName, fotoPath, true);
                            using (Image img = Image.FromFile(fotoPath)) preview.Image = new Bitmap(img);
                        }
                    }
                }; card.Controls.Add(foto);
                Button guardar = Button("Guardar serviço", 420, 575); guardar.Size = new Size(150, 42); guardar.Click += delegate
                {
                    int minutos; if (!int.TryParse(dur.Text, out minutos) || minutos <= 0) minutos = 60;
                    decimal valor = AdminRepository.ToDecimal(preco.Text);
                    AdminRepository.AtualizarServico(s.IdServico, nome.Text.Trim(), cat.Text.Trim(), minutos, valor, desc.Text.Trim(), fotoPath);
                    MessageBox.Show("Serviço atualizado."); if (refresh != null) refresh(); f.Close();
                };
                Button cancelar = Button("Cancelar", 585, 575); cancelar.BackColor = Color.FromArgb(235, 235, 235); cancelar.ForeColor = AdminSharedUi.Texto; cancelar.Click += delegate { f.Close(); };
                f.Controls.Add(guardar); f.Controls.Add(cancelar); f.ShowDialog(owner);
            }
        }

        public static void MostrarCategoriasServicos(Form owner)
        {
            using (Form f = BaseForm("Categorias de serviços", 840, 660))
            {
                AddHeader(f, "Categorias de serviços", "Categorias com foto, nome e quantidade de serviços ativos.");
                FlowLayoutPanel flow = new FlowLayoutPanel(); flow.Location = new Point(36, 118); flow.Size = new Size(760, 480); flow.AutoScroll = true; flow.WrapContents = true; flow.BackColor = Color.White; f.Controls.Add(flow);
                Dictionary<string, List<AdminServico>> grupos = new Dictionary<string, List<AdminServico>>(StringComparer.OrdinalIgnoreCase);
                foreach (AdminServico s in AdminRepository.GetServicos("", "Todos", 5000))
                {
                    string cat = string.IsNullOrWhiteSpace(s.Categoria) ? "Outros" : s.Categoria.Trim();
                    if (!grupos.ContainsKey(cat)) grupos[cat] = new List<AdminServico>(); grupos[cat].Add(s);
                }
                foreach (string cat in grupos.Keys.OrderBy(x => x)) flow.Controls.Add(CategoriaServicoCard(owner, cat, grupos[cat]));
                f.ShowDialog(owner);
            }
        }

        private static Control CategoriaServicoCard(Form owner, string categoria, List<AdminServico> servicos)
        {
            Panel card = Card(0, 0, 225, 205); card.Margin = new Padding(0, 0, 18, 18);
            PictureBox img = new PictureBox(); img.Location = new Point(75, 18); img.Size = new Size(76, 76); img.SizeMode = PictureBoxSizeMode.Zoom; img.Image = AdminSharedUi.CarregarImagemServico(servicos.Count > 0 ? servicos[0].Foto : "", categoria); card.Controls.Add(img);
            Label nome = AdminSharedUi.MakeLabel(categoria, 16, 104, 190, 28, 12, true, AdminSharedUi.Texto); nome.TextAlign = ContentAlignment.MiddleCenter; card.Controls.Add(nome);
            int ativos = servicos.Count(x => x.Ativo);
            int inativos = servicos.Count - ativos;
            Label qtd = AdminSharedUi.MakeLabel(ativos + " ativo(s)" + (inativos > 0 ? " · " + inativos + " desativado(s)" : ""), 16, 136, 190, 24, 9.2f, true, AdminSharedUi.Rosa); qtd.TextAlign = ContentAlignment.MiddleCenter; card.Controls.Add(qtd);
            string exemplos = string.Join(", ", servicos.Take(2).Select(x => x.Nome).ToArray());
            Label sub = AdminSharedUi.MakeLabel(exemplos, 16, 162, 190, 28, 8.5f, false, AdminSharedUi.Cinza); sub.TextAlign = ContentAlignment.MiddleCenter; card.Controls.Add(sub);
            Label abrir = AdminSharedUi.MakeLabel("Clique para ver", 16, 184, 190, 18, 8.2f, true, AdminSharedUi.Rosa); abrir.TextAlign = ContentAlignment.MiddleCenter; card.Controls.Add(abrir);
            card.Cursor = Cursors.Hand;
            card.Click += delegate { MostrarServicosDaCategoria(owner, categoria, servicos); };
            img.Click += delegate { MostrarServicosDaCategoria(owner, categoria, servicos); };
            nome.Click += delegate { MostrarServicosDaCategoria(owner, categoria, servicos); };
            qtd.Click += delegate { MostrarServicosDaCategoria(owner, categoria, servicos); };
            sub.Click += delegate { MostrarServicosDaCategoria(owner, categoria, servicos); };
            abrir.Click += delegate { MostrarServicosDaCategoria(owner, categoria, servicos); };
            return card;
        }

        private static void MostrarServicosDaCategoria(Form owner, string categoria, List<AdminServico> servicos)
        {
            using (Form f = BaseForm("Serviços - " + categoria, 900, 700))
            {
                AddHeader(f, categoria, "Todos os serviços dentro desta categoria.");
                FlowLayoutPanel flow = new FlowLayoutPanel();
                flow.Location = new Point(36, 118);
                flow.Size = new Size(800, 500);
                flow.AutoScroll = true;
                flow.WrapContents = true;
                flow.BackColor = Color.White;
                f.Controls.Add(flow);
                foreach (AdminServico s in servicos.OrderBy(x => x.Nome))
                    flow.Controls.Add(ServicoCategoriaCard(s, f));
                f.ShowDialog(owner);
            }
        }

        private static Control ServicoCategoriaCard(AdminServico s, Form owner)
        {
            Panel card = Card(0, 0, 245, 245);
            card.Margin = new Padding(0, 0, 18, 18);
            PictureBox img = new PictureBox(); img.Location = new Point(78, 16); img.Size = new Size(82, 82); img.SizeMode = PictureBoxSizeMode.Zoom; img.Image = AdminSharedUi.CarregarImagemServico(s.Foto, s.Categoria); card.Controls.Add(img);
            Label nome = AdminSharedUi.MakeLabel(s.Nome, 18, 105, 205, 38, 11, true, AdminSharedUi.Texto); nome.TextAlign = ContentAlignment.MiddleCenter; card.Controls.Add(nome);
            Label info = AdminSharedUi.MakeLabel(s.DuracaoMinutos + " min  •  " + AdminRepository.Money(s.Preco), 18, 145, 205, 24, 9.2f, true, AdminSharedUi.Rosa); info.TextAlign = ContentAlignment.MiddleCenter; card.Controls.Add(info);
            Label aval = AdminSharedUi.MakeLabel("★★★★★ " + s.Avaliacao.ToString("0.0"), 18, 168, 205, 22, 9, true, Color.FromArgb(200, 160, 70)); aval.TextAlign = ContentAlignment.MiddleCenter; card.Controls.Add(aval);
            Label estado = AdminSharedUi.MakeLabel(s.Ativo ? "Ativo" : "Desativado", 18, 188, 205, 18, 8.3f, true, s.Ativo ? AdminSharedUi.Verde : AdminSharedUi.Vermelho); estado.TextAlign = ContentAlignment.MiddleCenter; card.Controls.Add(estado);
            Button ver = Button("Ver", 26, 202); ver.Size = new Size(86, 32); ver.Click += delegate { MostrarDetalheServico(owner, s); };
            Button editar = Button("Editar", 128, 202); editar.Size = new Size(86, 32); editar.Click += delegate { EditarServico(owner, s, null); };
            card.Controls.Add(ver); card.Controls.Add(editar);
            return card;
        }

        public static void CriarMarcacao(Form owner, Action refresh)
        {
            using (NovaMarcacaoAdmin f = new NovaMarcacaoAdmin())
            {
                if (f.ShowDialog(owner) == DialogResult.OK)
                {
                    MessageBox.Show("Marcação criada com sucesso.", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (refresh != null) refresh();
                }
            }
        }

        public static void MostrarEnviarMensagem(Form owner, List<int> preSelecionados)
        {
            using (Form f = BaseForm("Enviar mensagem", 900, 700))
            {
                AddHeader(f, "Enviar mensagem", "Envie notificações pelo app ou por email para clientes e profissionais.");
                string tipoInicial = "Cliente";
                if (owner is ProfissionaisAdmin) tipoInicial = "Profissional";
                if (preSelecionados != null && preSelecionados.Count > 0)
                {
                    string tipo = AdminRepository.GetTipoUsuario(preSelecionados[0]);
                    if (!string.IsNullOrWhiteSpace(tipo)) tipoInicial = tipo;
                }

                Panel left = Card(34, 118, 330, 480);
                Panel right = Card(390, 118, 450, 480);
                f.Controls.Add(left); f.Controls.Add(right);

                left.Controls.Add(AdminSharedUi.MakeLabel("Destinatários", 22, 18, 260, 26, 13, true, AdminSharedUi.Texto));
                left.Controls.Add(AdminSharedUi.MakeLabel("Escolha quem vai receber a mensagem.", 22, 48, 270, 24, 9, false, AdminSharedUi.Cinza));
                ComboBox tipoDest = new ComboBox();
                tipoDest.Location = new Point(22, 90); tipoDest.Size = new Size(270, 32);
                tipoDest.DropDownStyle = ComboBoxStyle.DropDownList; tipoDest.FlatStyle = FlatStyle.Flat; tipoDest.Font = new Font("Segoe UI", 10);
                tipoDest.Items.AddRange(new object[] { "Cliente", "Profissional", "Todos" }); tipoDest.SelectedItem = tipoInicial;
                left.Controls.Add(tipoDest);

                TextBox filtro = new TextBox();
                filtro.BorderStyle = BorderStyle.None; filtro.Font = new Font("Segoe UI", 10); filtro.Location = new Point(14, 10); filtro.Size = new Size(240, 24);
                Panel shellFiltro = Card(22, 134, 270, 42); shellFiltro.BackColor = Color.White; shellFiltro.Controls.Add(filtro); left.Controls.Add(shellFiltro); filtro.BringToFront();
                filtro.Text = "";

                CheckedListBox lista = new CheckedListBox();
                lista.Location = new Point(22, 196); lista.Size = new Size(270, 250); lista.CheckOnClick = true; lista.BorderStyle = BorderStyle.None;
                lista.Font = new Font("Segoe UI", 9.8F); lista.BackColor = Color.FromArgb(255, 248, 251);
                left.Controls.Add(lista);

                right.Controls.Add(AdminSharedUi.MakeLabel("Mensagem", 26, 18, 260, 28, 15, true, AdminSharedUi.Texto));
                right.Controls.Add(AdminSharedUi.MakeLabel("Escolha o canal. O email é enviado imediatamente usando a configuração SMTP do App.config.", 26, 50, 375, 40, 9, false, AdminSharedUi.Cinza));

                FlowLayoutPanel canais = new FlowLayoutPanel();
                canais.Location = new Point(26, 104); canais.Size = new Size(385, 46); canais.WrapContents = false; canais.BackColor = Color.Transparent;
                right.Controls.Add(canais);
                string canalSelecionado = "App";
                Action atualizarCanais = null;
                List<Button> canalBotoes = new List<Button>();
                atualizarCanais = delegate
                {
                    foreach (Button b in canalBotoes)
                    {
                        bool ativo = b.Text == canalSelecionado;
                        b.BackColor = ativo ? AdminSharedUi.Rosa : AdminSharedUi.RosaClaro;
                        b.ForeColor = ativo ? Color.White : AdminSharedUi.Rosa;
                    }
                };
                foreach (string c in new[] { "App", "Email" })
                {
                    Button b = Button(c, 0, 0); b.Size = new Size(110, 38); b.Margin = new Padding(0, 0, 12, 0); b.Text = c;
                    b.Click += delegate { canalSelecionado = ((Button)b).Text; atualizarCanais(); };
                    canalBotoes.Add(b); canais.Controls.Add(b);
                }
                atualizarCanais();

                TextBox assunto = Input(right, "Assunto", 26, 190, 375);
                TextBox msg = Input(right, "Mensagem", 26, 280, 375); msg.Multiline = true; msg.Height = 118;
                right.Controls.Add(AdminSharedUi.MakeLabel("Resultado: a mensagem fica também registada no histórico e nas notificações do destinatário.", 26, 420, 380, 45, 9, false, AdminSharedUi.Cinza));

                List<AdminOpcao> pessoasAtuais = new List<AdminOpcao>();
                Action carregar = delegate
                {
                    lista.Items.Clear();
                    string tipo = tipoDest.SelectedItem == null ? "Cliente" : tipoDest.SelectedItem.ToString();
                    pessoasAtuais = AdminRepository.GetUsuariosMensagemOpcoes(tipo);
                    string termo = filtro.Text.Trim().ToLowerInvariant();
                    foreach (AdminOpcao p in pessoasAtuais)
                    {
                        if (termo.Length > 0 && !p.Nome.ToLowerInvariant().Contains(termo) && !p.Tipo.ToLowerInvariant().Contains(termo)) continue;
                        int idx = lista.Items.Add(p);
                        if (preSelecionados != null && preSelecionados.Contains(p.Id)) lista.SetItemChecked(idx, true);
                    }
                };
                tipoDest.SelectedIndexChanged += delegate { carregar(); };
                filtro.TextChanged += delegate { carregar(); };
                carregar();

                Button enviar = Button("Enviar mensagem", 555, 620); enviar.Size = new Size(165, 42);
                enviar.Click += delegate
                {
                    List<int> ids = new List<int>();
                    foreach (object item in lista.CheckedItems) ids.Add(((AdminOpcao)item).Id);
                    if (ids.Count == 0 || string.IsNullOrWhiteSpace(msg.Text)) { MessageBox.Show("Escolha pelo menos uma pessoa e escreva a mensagem."); return; }
                    string resultadoEnvio = AdminRepository.EnviarMensagem(ids, canalSelecionado, assunto.Text.Trim(), msg.Text.Trim());
                    MessageBox.Show(resultadoEnvio, "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    f.Close();
                };
                Button cancelar = Button("Cancelar", 732, 620); cancelar.BackColor = Color.FromArgb(235, 235, 235); cancelar.ForeColor = AdminSharedUi.Texto; cancelar.Click += delegate { f.Close(); };
                f.Controls.Add(enviar); f.Controls.Add(cancelar);
                f.ShowDialog(owner);
            }
        }


        public static void MostrarTopProfissionais(Form owner)
        {
            using (Form f = BaseForm("Top 10 profissionais", 780, 680))
            {
                AddHeader(f, "Top 10 profissionais", "Ranking por atendimentos, avaliação e faturação do mês.");
                FlowLayoutPanel flow = new FlowLayoutPanel();
                flow.Location = new Point(36, 118); flow.Size = new Size(700, 490); flow.AutoScroll = true; flow.WrapContents = false; flow.FlowDirection = FlowDirection.TopDown; flow.BackColor = Color.White;
                f.Controls.Add(flow);
                int pos = 1;
                foreach (AdminProfissional p in AdminRepository.GetTopProfissionais(10))
                {
                    flow.Controls.Add(TopProfissionalCard(p, pos++));
                }
                f.ShowDialog(owner);
            }
        }

        private static Control TopProfissionalCard(AdminProfissional p, int pos)
        {
            Panel card = Card(0, 0, 660, 92);
            card.Margin = new Padding(0, 0, 0, 12);
            Label medalha = AdminSharedUi.MakeLabel(pos.ToString(), 18, 22, 44, 44, 15, true, Color.White);
            medalha.TextAlign = ContentAlignment.MiddleCenter; medalha.BackColor = AdminSharedUi.Rosa; ApplyRoundedRegion(medalha, 22); medalha.Resize += delegate { ApplyRoundedRegion(medalha, 22); };
            card.Controls.Add(medalha);
            PictureBox foto = new PictureBox(); foto.Image = AdminSharedUi.CarregarImagemPerfil(p.Foto); foto.SizeMode = PictureBoxSizeMode.Zoom; foto.Location = new Point(78, 17); foto.Size = new Size(58, 58); card.Controls.Add(foto);
            card.Controls.Add(AdminSharedUi.MakeLabel(p.Nome, 150, 16, 230, 26, 11.5f, true, AdminSharedUi.Texto));
            card.Controls.Add(AdminSharedUi.MakeLabel(string.IsNullOrWhiteSpace(p.Especialidade) ? "Profissional BeauteCare" : p.Especialidade, 150, 45, 230, 24, 9, false, AdminSharedUi.Cinza));
            card.Controls.Add(AdminSharedUi.MakeLabel("★ " + p.Avaliacao.ToString("0.0"), 395, 22, 80, 24, 10, true, Color.FromArgb(200, 160, 70)));
            card.Controls.Add(AdminSharedUi.MakeLabel(p.Servicos + " atendimentos", 480, 18, 150, 24, 9, true, AdminSharedUi.Texto));
            card.Controls.Add(AdminSharedUi.MakeLabel(AdminRepository.Money(p.FaturacaoMes), 480, 46, 150, 24, 10, true, AdminSharedUi.Rosa));
            return card;
        }


        public static void MostrarAgendaDia(Form owner)
        {
            using (Form f = BaseForm("Agenda", 1080, 740))
            {
                AddHeader(f, "Agenda", "Todas as marcações de todas as profissionais no dia escolhido.");
                DateTime dataAtual = DateTime.Today;

                DateTimePicker picker = new DateTimePicker();
                picker.Format = DateTimePickerFormat.Long; picker.Value = dataAtual; picker.Location = new Point(42, 112); picker.Size = new Size(260, 34); picker.Font = new Font("Segoe UI", 9.5f);
                Button prev = Button("‹", 318, 108); prev.Size = new Size(44, 38); prev.Font = new Font("Segoe UI", 16, FontStyle.Bold);
                Button next = Button("›", 370, 108); next.Size = new Size(44, 38); next.Font = new Font("Segoe UI", 16, FontStyle.Bold);
                f.Controls.Add(picker); f.Controls.Add(prev); f.Controls.Add(next);

                Panel agenda = Card(42, 166, 970, 500);
                agenda.AutoScroll = true; agenda.BackColor = Color.White;
                f.Controls.Add(agenda);

                Action carregar = delegate
                {
                    dataAtual = picker.Value.Date;
                    agenda.Controls.Clear();
                    List<AdminMarcacao> marcacoes = AdminRepository.GetMarcacoes("", "Todos", dataAtual, dataAtual, 500);
                    DesenharLinhasAgenda(agenda);
                    if (marcacoes.Count == 0)
                    {
                        Panel vazio = Card(38, 40, 875, 150);
                        Label icon = AdminSharedUi.MakeLabel("♡", 0, 14, 875, 55, 34, true, AdminSharedUi.Rosa); icon.TextAlign = ContentAlignment.MiddleCenter; vazio.Controls.Add(icon);
                        Label msg = AdminSharedUi.MakeLabel("Não há marcações para este dia.", 0, 75, 875, 32, 14, true, AdminSharedUi.Texto); msg.TextAlign = ContentAlignment.MiddleCenter; vazio.Controls.Add(msg);
                        Label sub = AdminSharedUi.MakeLabel("Use “+ Nova Marcação” para criar um agendamento.", 0, 110, 875, 26, 9.5f, false, AdminSharedUi.Cinza); sub.TextAlign = ContentAlignment.MiddleCenter; vazio.Controls.Add(sub);
                        agenda.Controls.Add(vazio);
                        return;
                    }

                    int ultimoFim = 0;
                    foreach (AdminMarcacao m in marcacoes.OrderBy(x => x.Hora))
                    {
                        int top = 18 + Math.Max(0, (int)((m.Hora.TotalMinutes - 9 * 60) * 58 / 60));
                        if (top < ultimoFim + 8) top = ultimoFim + 8;
                        Panel card = AgendaMarcacaoCard(m);
                        card.Location = new Point(120, top);
                        TornarAgendaClicavel(card, f, m);
                        agenda.Controls.Add(card);
                        card.BringToFront();
                        ultimoFim = top + card.Height;
                    }
                };

                prev.Click += delegate { picker.Value = picker.Value.AddDays(-1); };
                next.Click += delegate { picker.Value = picker.Value.AddDays(1); };
                picker.ValueChanged += delegate { carregar(); };
                carregar();
                f.ShowDialog(owner);
            }
        }


        private static void DesenharLinhasAgenda(Panel agenda)
        {
            int y = 18;
            for (TimeSpan h = new TimeSpan(9, 0, 0); h <= new TimeSpan(19, 0, 0); h = h.Add(TimeSpan.FromHours(1)))
            {
                Label hora = AdminSharedUi.MakeLabel(h.ToString(@"hh\:mm"), 18, y + 6, 70, 24, 10, true, AdminSharedUi.Texto);
                agenda.Controls.Add(hora);
                Panel line = new Panel(); line.Location = new Point(92, y + 18); line.Size = new Size(820, 1); line.BackColor = Color.FromArgb(245, 215, 225); agenda.Controls.Add(line);
                y += 58;
            }
        }

        private static void TornarAgendaClicavel(Control card, Form owner, AdminMarcacao m)
        {
            if (card == null || m == null) return;
            card.Cursor = Cursors.Hand;
            card.Click += delegate { MostrarDetalheMarcacao(owner, m); };
            foreach (Control c in card.Controls) TornarAgendaClicavel(c, owner, m);
        }

        private static Panel AgendaMarcacaoCard(AdminMarcacao m)
        {
            Panel card = Card(0, 0, 780, 74);
            card.BackColor = Color.FromArgb(255, 248, 251);
            Panel faixa = new Panel(); faixa.Location = new Point(0, 0); faixa.Size = new Size(6, 74); faixa.BackColor = EstadoCor(m.Estado); card.Controls.Add(faixa);
            card.Controls.Add(AdminSharedUi.MakeLabel(m.Hora.ToString(@"hh\:mm") + " - " + m.Hora.Add(TimeSpan.FromMinutes(m.DuracaoMinutos)).ToString(@"hh\:mm"), 20, 10, 120, 22, 8.5f, false, AdminSharedUi.Cinza));
            card.Controls.Add(AdminSharedUi.MakeLabel(string.IsNullOrWhiteSpace(m.Servico) ? "Serviço" : m.Servico, 20, 34, 250, 26, 10.5f, true, AdminSharedUi.Texto));
            card.Controls.Add(AdminSharedUi.MakeLabel(m.Cliente, 285, 34, 190, 26, 10, true, AdminSharedUi.Texto));
            card.Controls.Add(AdminSharedUi.MakeLabel(m.Profissional, 485, 34, 170, 26, 9, false, AdminSharedUi.Cinza));
            Label estado = AdminSharedUi.MakeLabel(m.Estado, 650, 25, 105, 28, 9, true, EstadoCor(m.Estado)); estado.TextAlign = ContentAlignment.MiddleCenter; estado.BackColor = Color.White; ApplyRoundedRegion(estado, 14); card.Controls.Add(estado);
            return card;
        }


        public static void ImprimirFatura(AdminFatura fat)
        {
            if (fat == null) return;
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += delegate (object s, PrintPageEventArgs e)
            {
                Font title = new Font("Segoe UI", 18, FontStyle.Bold);
                Font normal = new Font("Segoe UI", 11);
                float y = 60;
                e.Graphics.DrawString("BeauteCare", title, Brushes.Black, 60, y); y += 45;
                e.Graphics.DrawString("Fatura Nº " + fat.NumeroFatura, normal, Brushes.Black, 60, y); y += 28;
                e.Graphics.DrawString("Cliente: " + fat.Cliente, normal, Brushes.Black, 60, y); y += 28;
                e.Graphics.DrawString("Data: " + fat.DataFatura.ToString("dd/MM/yyyy"), normal, Brushes.Black, 60, y); y += 28;
                e.Graphics.DrawString("Serviços: " + fat.Servicos, normal, Brushes.Black, 60, y); y += 40;
                e.Graphics.DrawString("Subtotal: " + AdminRepository.Money(fat.Subtotal), normal, Brushes.Black, 60, y); y += 28;
                e.Graphics.DrawString("Desconto: " + AdminRepository.Money(fat.Desconto), normal, Brushes.Black, 60, y); y += 28;
                e.Graphics.DrawString("Total: " + AdminRepository.Money(fat.Total), title, Brushes.Black, 60, y); y += 42;
                e.Graphics.DrawString("Estado: " + fat.Estado + "   Método: " + fat.MetodoPagamento, normal, Brushes.Black, 60, y);
            };
            using (PrintPreviewDialog pp = new PrintPreviewDialog())
            {
                pp.Document = pd;
                pp.Width = 900; pp.Height = 700;
                pp.ShowDialog();
            }
        }

        public static void ExportarGrid(DataGridView dgv, string titulo)
        {
            if (dgv == null) return;
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV|*.csv";
                sfd.FileName = titulo + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".csv";
                if (sfd.ShowDialog() != DialogResult.OK) return;
                StringBuilder sb = new StringBuilder();
                List<DataGridViewColumn> cols = dgv.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible && !(c is DataGridViewImageColumn) && !(c is DataGridViewButtonColumn)).ToList();
                sb.AppendLine(string.Join(";", cols.Select(c => Csv(c.HeaderText)).ToArray()));
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.IsNewRow) continue;
                    sb.AppendLine(string.Join(";", cols.Select(c => Csv(Convert.ToString(row.Cells[c.Name].Value))).ToArray()));
                }
                File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show("Exportação concluída.", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private static string Csv(string v)
        {
            if (v == null) return "";
            return "\"" + v.Replace("\"", "\"\"") + "\"";
        }

        private static Form BaseForm(string title, int w, int h)
        {
            Form f = new Form();
            f.Text = title;
            f.Size = new Size(w, h);
            f.StartPosition = FormStartPosition.CenterParent;
            f.BackColor = Color.White;
            f.FormBorderStyle = FormBorderStyle.None;
            f.MaximizeBox = false;
            f.MinimizeBox = false;
            f.AutoScroll = true;
            ApplyRoundedRegion(f, 26);
            f.Resize += delegate { ApplyRoundedRegion(f, 26); };
            return f;
        }

        private static void AddHeader(Form f, string title, string subtitle)
        {
            Label t = AdminSharedUi.MakeLabel(title, 34, 26, f.Width - 120, 42, 19, true, AdminSharedUi.Texto);
            Label sub = AdminSharedUi.MakeLabel(subtitle, 36, 70, f.Width - 120, 28, 10, true, AdminSharedUi.Rosa);
            sub.Name = "lblSubHeaderAdminDialog";
            Button fechar = SmallCloseButton(f.Width - 76, 26);
            fechar.Click += delegate { f.Close(); };
            f.Controls.Add(t); f.Controls.Add(sub); f.Controls.Add(fechar);
        }

        private static Button SmallCloseButton(int x, int y)
        {
            Button b = new Button();
            b.Text = "×";
            b.Location = new Point(x, y);
            b.Size = new Size(40, 34);
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.BackColor = Color.FromArgb(245, 245, 245);
            b.ForeColor = AdminSharedUi.Texto;
            b.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            b.Cursor = Cursors.Hand;
            ApplyRoundedRegion(b, 14);
            b.Resize += delegate { ApplyRoundedRegion(b, 14); };
            return b;
        }

        private static Panel Card(int x, int y, int w, int h)
        {
            Panel p = new Panel();
            p.Location = new Point(x, y);
            p.Size = new Size(w, h);
            p.BackColor = Color.FromArgb(255, 248, 251);
            ApplyRoundedRegion(p, 18);
            p.Resize += delegate { ApplyRoundedRegion(p, 18); };
            return p;
        }

        private static TextBox Input(Control parent, string label, int x, int y, int w)
        {
            parent.Controls.Add(AdminSharedUi.MakeLabel(label, x, y - 24, w, 20, 9, true, AdminSharedUi.Texto));
            Panel shell = new Panel();
            shell.Location = new Point(x, y);
            shell.Size = new Size(w, 40);
            shell.BackColor = Color.White;
            ApplyRoundedRegion(shell, 14);
            shell.Resize += delegate { ApplyRoundedRegion(shell, 14); };
            TextBox t = new TextBox();
            t.BorderStyle = BorderStyle.None;
            t.Location = new Point(12, 10);
            t.Size = new Size(w - 24, 22);
            t.Font = new Font("Segoe UI", 10);
            t.BackColor = Color.White;
            t.ForeColor = AdminSharedUi.Texto;
            t.SizeChanged += delegate
            {
                if (t.Height + 20 > shell.Height)
                {
                    shell.Height = t.Height + 20;
                    ApplyRoundedRegion(shell, 14);
                }
                t.Width = Math.Max(20, shell.Width - 24);
            };
            shell.Controls.Add(t);
            parent.Controls.Add(shell);
            return t;
        }

        private static ComboBox CategoriaCombo(Control parent, string label, int x, int y, int w, string atual)
        {
            parent.Controls.Add(AdminSharedUi.MakeLabel(label, x, y - 24, w, 20, 9, true, AdminSharedUi.Texto));
            ComboBox c = new ComboBox();
            c.Location = new Point(x, y);
            c.Size = new Size(w, 34);
            c.Font = new Font("Segoe UI", 10);
            c.DropDownStyle = ComboBoxStyle.DropDown;
            c.FlatStyle = FlatStyle.Flat;
            c.BackColor = Color.White;
            c.ForeColor = AdminSharedUi.Texto;
            HashSet<string> cats = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (AdminServico s in AdminRepository.GetServicos("", "Todos", 5000))
            {
                string cat = string.IsNullOrWhiteSpace(s.Categoria) ? "Outros" : s.Categoria.Trim();
                if (cats.Add(cat)) c.Items.Add(cat);
            }
            if (!string.IsNullOrWhiteSpace(atual)) c.Text = atual;
            else if (c.Items.Count > 0) c.SelectedIndex = 0;
            parent.Controls.Add(c);
            return c;
        }

        private static ComboBox ComboBox(Control f, string label, int x, int y, int w, List<AdminOpcao> items)
        {
            f.Controls.Add(AdminSharedUi.MakeLabel(label, x, y - 24, w, 20, 9, true, AdminSharedUi.Texto));
            ComboBox c = new ComboBox();
            c.Location = new Point(x, y);
            c.Size = new Size(w, 34);
            c.Font = new Font("Segoe UI", 10);
            c.DropDownStyle = ComboBoxStyle.DropDownList;
            c.FlatStyle = FlatStyle.Flat;
            c.BackColor = Color.White;
            c.ForeColor = AdminSharedUi.Texto;
            foreach (AdminOpcao op in items) c.Items.Add(op);
            if (c.Items.Count > 0) c.SelectedIndex = 0;
            f.Controls.Add(c);
            return c;
        }

        private static Button Button(string text, int x, int y)
        {
            Button b = new Button();
            b.Text = text;
            b.Location = new Point(x, y);
            b.Size = new Size(140, 40);
            b.BackColor = AdminSharedUi.Rosa;
            b.ForeColor = Color.White;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            b.Cursor = Cursors.Hand;
            ApplyRoundedRegion(b, 18);
            b.Resize += delegate { ApplyRoundedRegion(b, 18); };
            return b;
        }

        private static void AddInfo(Form f, string title, string value, int x, int y)
        {
            Panel p = Card(x, y, 180, 82);
            p.Controls.Add(AdminSharedUi.MakeLabel(title, 10, 10, 158, 20, 8.5f, false, AdminSharedUi.Cinza));
            p.Controls.Add(AdminSharedUi.MakeLabel(value, 10, 38, 158, 30, 11, true, AdminSharedUi.Texto));
            f.Controls.Add(p);
        }

        private static Color EstadoCor(string estado)
        {
            if (estado == null) return AdminSharedUi.Texto;
            string e = estado.ToLowerInvariant();
            if (e.Contains("paga") || e.Contains("pago") || e.Contains("confirm")) return AdminSharedUi.Verde;
            if (e.Contains("pend") || e.Contains("aguard")) return AdminSharedUi.Laranja;
            if (e.Contains("não") || e.Contains("nao") || e.Contains("cancel")) return AdminSharedUi.Vermelho;
            return AdminSharedUi.Texto;
        }

        private static Panel ServiceCard(AdminServico s, Action<AdminServico> add)
        {
            Panel card = Card(0, 0, 250, 185);
            card.Margin = new Padding(8);
            PictureBox img = new PictureBox(); img.Location = new Point(16, 18); img.Size = new Size(58, 58); img.SizeMode = PictureBoxSizeMode.Zoom; img.Image = AdminSharedUi.CarregarImagemServico(s.Foto, s.Categoria); card.Controls.Add(img);
            card.Controls.Add(AdminSharedUi.MakeLabel(s.Nome, 88, 20, 145, 42, 10, true, AdminSharedUi.Texto));
            card.Controls.Add(AdminSharedUi.MakeLabel(AdminRepository.Money(s.Preco), 88, 64, 130, 22, 9.5f, true, AdminSharedUi.Rosa));
            card.Controls.Add(AdminSharedUi.MakeLabel("Duração: " + s.DuracaoMinutos + " min", 16, 94, 180, 22, 9, false, AdminSharedUi.Cinza));
            card.Controls.Add(AdminSharedUi.MakeLabel("★★★★★ " + s.Avaliacao.ToString("0.0"), 16, 118, 180, 22, 9, true, Color.FromArgb(200, 160, 70)));
            Button b = Button("Adicionar", 45, 142); b.Size = new Size(160, 32); b.Click += delegate { if (add != null) add(s); };
            card.Controls.Add(b);
            return card;
        }


        private static void ApplyRoundedRegion(Control control, int radius)
        {
            if (control == null || control.Width <= 0 || control.Height <= 0) return;
            try
            {
                using (GraphicsPath path = RoundedPath(new Rectangle(0, 0, control.Width, control.Height), radius))
                    control.Region = new Region(path);
            }
            catch { }
        }

        private static GraphicsPath RoundedPath(Rectangle bounds, int radius)
        {
            int d = Math.Max(1, radius * 2);
            GraphicsPath path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static bool Confirm(string msg)
        {
            return MessageBox.Show(msg, "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }
    }
}
