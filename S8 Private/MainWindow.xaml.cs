using System.Threading;
using System.Windows;
using System.ComponentModel;
using System.Windows.Media;
using System;
using Memory;
using System.Windows.Input;
using System.Diagnostics;
using Gh0st_Helper_PRO;
using Microsoft.Office.Interop.Excel;

namespace S8_Private
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public Mem m = new Mem();
        private BackgroundWorker BGW = new BackgroundWorker();
        private LowLevelKeyboardListener _listener;

        readonly _Application exc = new Microsoft.Office.Interop.Excel.Application();
        Workbook excWb;
        Worksheet excWs;

        double mapa;
        double tee1, tee2, tee3, pin1, pin2, pin3, eixox, eixoy, cosBola, senoBola;

        double pbtoyards, atanpbtoyards, gridPersonagem;

        double senoAngulo, cosAngulo;
        string vento, backoufront, esquerdaoudireita;

        int estadoBola, terreno;
        bool Aberto = false, fastdunk = false;

        public MainWindow()
        {
            InitializeComponent();
            BGW.DoWork += BGW_DoWork;
            BGW.RunWorkerCompleted += BGW_RunWorkerCompleted;
            BGW.ProgressChanged += BGW_ProgressChanged;
            BGW.WorkerReportsProgress = true;
            BGW.WorkerSupportsCancellation = true;
        }
        private void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BGW.RunWorkerAsync();
            mapa = m.ReadFloat("00EC97A0", "", false);
            if (mapa != 1)
            {
                Memorias();
                direcaoVento(senoAngulo, cosAngulo);
                mapaTEXT.Content = "Boa Sorte!";
                mapaTEXT.Foreground = Brushes.Green;
                distanciaTEXT.Content = Convert.ToString(Distancia(pin1, tee1, pin3, tee3));
                alturaTEXT.Content = Convert.ToString(Altura(tee2, pin2));
                ventoTEXT.Content = vento;
                anguloTEXT.Content = Convert.ToString(Angulo(cosAngulo, senoAngulo));
                if(esquerdaoudireita == "Direita")
                    quebraTEXT.Content = Convert.ToString(quebraBola(senoBola, cosBola, eixox, eixoy) * -1);
                else
                    quebraTEXT.Content = Convert.ToString(quebraBola(senoBola, cosBola, eixox, eixoy));
                terrenoTEXT.Content = Convert.ToString(Terreno(terreno) + "%");
                pbTEXT.Content = Convert.ToString(pbTirado(pin1, tee1, pin3, tee3));
                if(fastdunk == true)
                {
                    calcular.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    fastdunk = false;
                }
                if(estadoBola == 255)
                {
                    m.WriteMemory("ProjectG.exe+00AC79E0,0x8,0x58,0x10,0x0,0x0,0x14,0xE8", "float", "140");
                }
            }
            else
            {
                mapaTEXT.Content = "Esperando Partida...";
                mapaTEXT.Foreground = Brushes.Red;
                ventoTEXT.Content = "0";
            }  
        }
        private void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            Aberto = m.OpenProcess("ProjectG");
            if (!Aberto)
            {
                Thread.Sleep(1000);
                return;
            }

            Thread.Sleep(10);
            BGW.ReportProgress(0);
        }
        private void BGW_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (Aberto)
                processoTEXT.Content = "PangYa S8 Aberto";

            processoTEXT.Foreground = Brushes.Green;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            exc.Visible = true;
            excWb = exc.Workbooks.Open(@"C:\280uk.xlsx");
            excWs = excWb.Worksheets[1];
            excWs.Activate();
            _listener = new LowLevelKeyboardListener();
            _listener.OnKeyPressed += _listener_OnKeyPressed; 
            _listener.HookKeyboard();
            BGW.RunWorkerAsync();
        }
        void Memorias()
        {
            tee1 = m.ReadFloat("00E1107C", "", false);
            tee2 = m.ReadFloat("00E11080", "", false);
            tee3 = m.ReadFloat("00E11084", "", false);
            pin1 = m.ReadFloat("00EC445C", "", false);
            pin2 = m.ReadFloat("00EC4460", "", false);
            pin3 = m.ReadFloat("00EC4464", "", false);
            senoAngulo = m.ReadFloat("ProjectG.exe+AC79E0,0x1C,0x20,0x14,0x18,0x0,0x234,0xB4", "", false);
            cosAngulo = m.ReadFloat("ProjectG.exe+AC79E0,0x1C,0x20,0x14,0x18,0x0,0x234,0xAC", "", false);
            eixox = m.ReadFloat("ProjectG.exe+00AC79E0,0xA4,0x14,0x10,0x30,0x0,0x21C,0x1C", "", false);
            eixoy = m.ReadFloat("ProjectG.exe+00AC79E0,0x8,0x10,0x28,0x0,0x0,0x21C,0x24", "", false);
            cosBola = m.ReadFloat("00EC97A0", "", false);
            senoBola = m.ReadFloat("00EC97A8", "", false);
            vento = m.ReadString("ProjectG.exe+00AC79E0,0x8,0x10,0x30,0x0,0x220,0x28,0x0", "");
            estadoBola = m.ReadByte("ProjectG.exe+00AC79E0,0x0,0x58,0x10,0x0,0x0,0x14,0xF8");   
            gridPersonagem = m.ReadFloat("ProjectG.exe+00A3D3A8,0xBC,0x0,0x0,0x0,0x4,0x6C,0x68", "", false);
            terreno = m.ReadInt("ProjectG.exe+AC79E0,0x1C,0x0,0x10,0x18,0x0,0x21C,0xAC", "");
        }
        double quebraBola(double x, double y, double bolax, double bolay)
        {
            double radianusSeno, radianusCos, senoInverso, radianusPosicao, posicao, resultadoautoquebra, cos;
            radianusSeno = Math.Asin(x) * 180 / Math.PI;
            radianusCos = Math.Acos(y) * 180 / Math.PI;
            if (radianusSeno < 0.0)
            {
                posicao = 180 - (radianusCos - 180);
            }
            else
            {
                posicao = radianusCos;
            }
            radianusPosicao = posicao * Math.PI / 180;
            radianusPosicao *= -1;
            senoInverso = Math.Sin(radianusPosicao) * -1;
            cos = Math.Cos(radianusPosicao);
            resultadoautoquebra = Math.Round(((bolax * cos) + (bolay * senoInverso)) * -1 * (1 / 0.00875), 2); //0.00875 
            return resultadoautoquebra;
        }
        double autoPB(double d, double mh, double pbresultado)
        {
            pbtoyards = 0.2167 * pbresultado;
            atanpbtoyards = Math.Atan(pbtoyards / d) * 1.5;
            if (esquerdaoudireita == "Direita")
            {
                return gridPersonagem + atanpbtoyards;
            }
            else
            {
                return gridPersonagem - atanpbtoyards;
            }
        }
        double pbTirado(double x1, double x2, double z1, double z2)
        {
            double anguloCamera, distanciaRaiz, rad2, rad, pb2;

            anguloCamera = Math.Atan2(x2 - x1, z2 - z1);
            distanciaRaiz = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(z2 - z1, 2));
            rad2 = gridPersonagem;
            rad = Math.Abs(rad2) % 6.28318530717659;
            if (rad2 <= 0)
                rad *= -1;

            pb2 = ((distanciaRaiz * 0.3125) * Math.Tan(rad + anguloCamera)) / 1.5 / 0.2167 * -1;
            if (pb2 < 0)
            {
                pb2 *= -1;
            }
            return Math.Round(pb2,2);
        }
        double Angulo(double x, double y)
        {
            if (x < 0)
                x *= -1;
            if (y < 0)
                y *= -1;
            return Math.Round(((Math.Asin(x) * 180 / Math.PI) + (Math.Acos(y) * 180 / Math.PI)) / 2,2);
        }
        double Distancia (double x1, double x2, double y1, double y2)
        {
            return Math.Round(Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2)) * 0.312495, 2);
        }
        double Altura(double x1, double x2)
        {
            return Math.Round((pin2 - tee2 + 0.14) * (0.312495 * 0.914),2);
        }
        private void direcaoVento(double seno, double cos)
        {
            if (cos < 0)
                esquerdaoudireita = "Esquerda";
            else
                esquerdaoudireita = "Direita";

            if (seno > 0 && cos < 0 || seno > 0 && cos > 0)
                backoufront = "Front";
            else
                backoufront = "Back";
        }
        int Terreno(int x) 
        {
            x = 100 - x;
            return x;
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        void _listener_OnKeyPressed(object sender, KeyPressedArgs e)
        {
            if (e.KeyPressed == Key.F1)
            {
                fastdunk = true;
            } 
            if(e.KeyPressed == Key.F2)
            {
                if (mapa != 1)
                {
                    double p = Convert.ToDouble(resultadoTEXT.Content);
                    string j;
                    j = Convert.ToString(autoPB(Distancia(pin1, tee1, pin3, tee3), gridPersonagem, p));
                    m.WriteMemory("ProjectG.exe+00A3D3A8,0xBC,0x0,0x0,0x0,0x4,0x6C,0x68", "float", j);
                }
            }
            if(e.KeyPressed == Key.F3)
            {
                m.WriteMemory("ProjectG.exe+00AC79E0,0x8,0x58,0x10,0x0,0x0,0x14,0xE8", "float", "105");
            }
        }
        void Valores(double controle)
        {
            if (controle != 1)
            {
                excWs.Cells[1, 2].Value = distanciaTEXT.Content;
                excWs.Cells[2, 2].Value = alturaTEXT.Content;
                vento = vento.Substring(0, 1);
                excWs.Cells[3, 2].Value = vento;
                excWs.Cells[4, 2].Value = anguloTEXT.Content;
                excWs.Cells[5, 2].Value = quebraTEXT.Content;
                excWs.Cells[6, 2].Value = terrenoTEXT.Content;
            }
        }
        void Dunk1w(string x, double controle)
        {
            string spin;
            if (controle != 1)
            {
                if (x == "Front")
                {
                    resultadoTEXT.Content = Convert.ToString(excWs.Cells[2, 10].Value);
                    calibradorTEXT.Content = Convert.ToString(excWs.Cells[4, 10].Value);
                    Calibrador(Convert.ToDouble(excWs.Cells[3,10].Value));
                    spin = Convert.ToString(excWs.Cells[5, 10].Value);
                    m.WriteMemory("ProjectG.exe+00AC79E0,0x1C,0x20,0xC,0x2C,0x30,0x0,0x1C", "float", spin);
                }
                else
                {
                    resultadoTEXT.Content = Convert.ToString(excWs.Cells[2, 11].Value);
                    calibradorTEXT.Content = Convert.ToString(excWs.Cells[4, 11].Value);
                    Calibrador(Convert.ToDouble(excWs.Cells[3, 11].Value));
                    spin = Convert.ToString(excWs.Cells[5, 11].Value);
                    m.WriteMemory("ProjectG.exe+00AC79E0,0x1C,0x20,0xC,0x2C,0x30,0x0,0x1C", "float", spin);
                }
            }
        }
        private void buttondunk_Click(object sender, RoutedEventArgs e)
        {
            Valores(mapa);
            Dunk1w(backoufront,mapa);
        }
        private void Calibrador(double porcentagem)
        {
            /*
             * CREDITOS SERA!!!
             * CREDITOS SERA!!!
             * CREDITOS SERA!!!
            */
            string x;
            if(mapa != 1)
            {
                x = Convert.ToString(Math.Round(500.0 - (100.0 - porcentagem) * 3.6, 2));
                m.WriteMemory("ProjectG.exe+AC79E0,0x1C,0x20,0x14,0x18,0x0,0x46C,0x52C", "float", x);
                m.WriteMemory("ProjectG.exe+AC79E0,0x1C,0x20,0x14,0x18,0x0,0x46C,0x530", "float", x);
                m.WriteMemory("ProjectG.exe+AC79E0,0x1C,0x20,0x14,0x48,0x0,0x14,0x100", "float", x);
            } 
        }
    }
}
