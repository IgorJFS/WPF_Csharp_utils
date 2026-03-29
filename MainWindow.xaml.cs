using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.VisualBasic.FileIO;

namespace WPF_utils;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void ProcurarOrigem_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFolderDialog
        {
            Title = "Selecione a pasta de origem"
        };

        if (dialog.ShowDialog() == true)
        {
            txtOrigem.Text = dialog.FolderName;
        }
    }

    private void ProcurarDestino_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFolderDialog
        {
            Title = "Selecione a pasta de destino"
        };

        if (dialog.ShowDialog() == true)
        {
            txtDestino.Text = dialog.FolderName;
        }
    }

    private void MoverArquivos_Click(object sender, RoutedEventArgs e)
    {
        string origem = txtOrigem.Text;
        string extensao = txtExtensao.Text;
        string destino = txtDestino.Text;

        txtStatus.Text = string.Empty;

        if (string.IsNullOrWhiteSpace(origem) || string.IsNullOrWhiteSpace(extensao) || string.IsNullOrWhiteSpace(destino))
        {
            txtStatus.Text = "Por favor, preencha todos os campos.";
            txtStatus.Foreground = Brushes.Red;
            return;
        }

        // Normaliza a extensão para conter o ".". Ex: "txt" vira ".txt"
        if (!extensao.StartsWith("."))
        {
            extensao = "." + extensao;
        }

        try
        {
            if (!Directory.Exists(origem))
            {
                txtStatus.Text = "A pasta de origem não existe ou é inválida.";
                txtStatus.Foreground = Brushes.Red;
                return;
            }

            // Busca todos os arquivos com a extensão fornecida
            string[] arquivos = Directory.GetFiles(origem, "*" + extensao);
            int contador = 0;
            int contadorDuplicatas = 0;

            foreach (var arquivo in arquivos)
            {
                string nomeArquivo = System.IO.Path.GetFileName(arquivo);
                string caminhoDestino = System.IO.Path.Combine(destino, nomeArquivo);

                // Mover o arquivo para o destino
                // Caso já exista no destino, você pode optar por criar uma exceção, ignorar, ou sobrescrever
                if (!File.Exists(caminhoDestino))
                {
                    File.Move(arquivo, caminhoDestino);
                    contador++;
                }
                else
                {
                    FileSystem.DeleteFile(arquivo, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                    contadorDuplicatas++;
                }
            }
            if (contador <= 0 && contadorDuplicatas <= 0)
            {
                txtStatus.Text = "Nenhum arquivo encontrado com a extensão especificada.";
                txtStatus.Foreground = Brushes.Orange;
                return;
            }
            txtStatus.Text = $"{contador} arquivo(s) movido(s) e {contadorDuplicatas} arquivo(s) duplicado(s) movidos para a lixeira.";
            txtStatus.Foreground = Brushes.Green;
        }
        catch (Exception ex)
        {
            txtStatus.Text = $"Erro ao tentar mover arquivos: {ex.Message}";
            txtStatus.Foreground = Brushes.Red;
        }
    }
}