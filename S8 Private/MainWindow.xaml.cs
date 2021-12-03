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
using System.Linq;

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
        double tee1, tee2, tee3, pin1, pin2, pin3, eixox, eixoy, cosBola, senoBola, spin, curva;

        double pbtoyards, atanpbtoyards, gridPersonagem;

        double senoAngulo, cosAngulo;
        string vento, backoufront, esquerdaoudireita;

        int estadoBola, terreno, driver;
        bool Aberto = false, fastdunk = false, fasttoma = false;

        double[] porcentagemCali = new double[] { 30.00, 30.28, 30.56, 30.83, 31.11, 31.39, 31.67, 31.94, 32.22, 32.50, 32.78, 33.06, 33.33, 33.61, 33.89, 34.17, 34.44, 34.72, 35.00, 35.28, 35.56, 35.83, 36.11, 36.39, 36.67, 36.94, 37.22, 37.50, 37.78, 38.06, 38.33, 38.61, 38.89, 39.17, 39.44, 39.72, 40.00, 40.28, 40.56, 40.83, 41.11, 41.39, 41.67, 41.94, 42.22, 42.50, 42.78, 43.06, 43.33, 43.61, 43.89, 44.17, 44.44, 44.72, 45.00, 45.28, 45.56, 45.83, 46.11, 46.39, 46.67, 46.94, 47.22, 47.50, 47.78, 48.06, 48.33, 48.61, 48.89, 49.17, 49.44, 49.72, 50.00, 50.28, 50.56, 50.83, 51.11, 51.39, 51.67, 51.94, 52.22, 52.50, 52.78, 53.06, 53.33, 53.61, 53.89, 54.17, 54.44, 54.72, 55.00, 55.28, 55.56, 55.83, 56.11, 56.39, 56.67, 56.94, 57.22, 57.50, 57.78, 58.06, 58.33, 58.61, 58.89, 59.17, 59.44, 59.72, 60.00, 60.28, 60.56, 60.83, 61.11, 61.39, 61.67, 61.94, 62.22, 62.50, 62.78, 63.06, 63.33, 63.61, 63.89, 64.17, 64.44, 64.72, 65.00, 65.28, 65.56, 65.83, 66.11, 66.39, 66.67, 66.94, 67.22, 67.50, 67.78, 68.06, 68.33, 68.61, 68.89, 69.17, 69.44, 69.72, 70.00, 70.28, 70.56, 70.83, 71.11, 71.39, 71.67, 71.94, 72.22, 72.50, 72.78, 73.06, 73.33, 73.61, 73.89, 74.17, 74.44, 74.72, 75.00, 75.28, 75.56, 75.83, 76.11, 76.39, 76.67, 76.94, 77.22, 77.50, 77.78, 78.06, 78.33, 78.61, 78.89, 79.17, 79.44, 79.72, 80.00, 80.28, 80.56, 80.83, 81.11, 81.39, 81.67, 81.94, 82.22, 82.50, 82.78, 83.06, 83.33, 83.61, 83.89, 84.17, 84.44, 84.72, 85.00, 85.28, 85.56, 85.83, 86.11, 86.39, 86.67, 86.94, 87.22, 87.50, 87.78, 88.06, 88.33, 88.61, 88.89, 89.17, 89.44, 89.72, 90.00, 90.28, 90.56, 90.83, 91.11, 91.39, 91.67, 91.94, 92.22, 92.50, 92.78, 93.06, 93.33, 93.61, 93.89, 94.17, 94.44, 94.72, 95.00, 95.28, 95.56, 95.83, 96.11, 96.39, 96.67, 96.94, 97.22, 97.50, 97.78, 98.06, 98.33, 98.61, 98.89, 99.17, 99.44, 99.72, 100.00 };
        public MainWindow()
        {
            InitializeComponent();
            BGW.DoWork += BGW_DoWork;
            BGW.RunWorkerCompleted += BGW_RunWorkerCompleted;
            //BGW.ProgressChanged += BGW_ProgressChanged;
            BGW.WorkerReportsProgress = true;
            BGW.WorkerSupportsCancellation = true;
        }
        private void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BGW.RunWorkerAsync();
            mapa = m.ReadFloat("00EC97A0", "", false);
            if (mapa != 1)
            {
                helperTEXT.Foreground = Brushes.Black;
                tigerTEXT.Foreground = Brushes.Black;
                Memorias();
                direcaoVento(senoAngulo, cosAngulo);
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
                spinTEXT.Content = Convert.ToString(Math.Round(spin,2));
                curvaTEXT.Content = Convert.ToString(Math.Round(curva,2));
                //CONTROLE DUNK
                if(fastdunk == true)
                {
                    calcular.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    fastdunk = false;
                }
                //CONTROLE TOMA
                if (fasttoma == true)
                {
                    calcular.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    fasttoma = false;
                }
                //PIXEL PANGYA
                if (estadoBola == 255)
                {
                    m.WriteMemory("ProjectG.exe+00AC79E0,0x8,0x58,0x10,0x0,0x0,0x14,0xE8", "float", "140");
                }
            }
            else
            {
                tigerTEXT.Foreground = Brushes.Red;
                helperTEXT.Foreground = Brushes.Red;
                anguloTEXT.Content = "0";
                ventoTEXT.Content = "0";
                distanciaTEXT.Content = "0";
                terrenoTEXT.Content= "0";
                pbTEXT.Content = "0";
                calibradorTEXT.Content = "0";
                resultadoTEXT.Content = "0";
                alturaTEXT.Content = "0";
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
        /*
        private void BGW_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //VOU DESATIVAR NO MOMENTO JA QUE NO LAYOUT NAO TEM O QUE IMPLEMENTAR 
        }  
        */
        private void Window_Closed(object sender, EventArgs e)
        {
            Process[] runingProcess = Process.GetProcesses();
            for (int i = 0; i < runingProcess.Length; i++)
            {
                if (runingProcess[i].ProcessName == "EXCEL")
                {
                    runingProcess[i].Kill();
                }
            }
            Process.GetCurrentProcess().Kill();
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
            //gridPersonagem = m.ReadFloat("ProjectG.exe+00A3D3A8,0xBC,0x0,0x0,0x0,0x4,0x6C,0x68", "", false); //CASO NÃO FUNCIONE SEU AUTO PB SO USAR ESSE GRID!!!
            gridPersonagem = m.ReadFloat("ProjectG.exe+00AC79E0,0x0,0x40,0x10,0xC,0x30,0x0,0x68", "", false);
            terreno = m.ReadInt("ProjectG.exe+AC79E0,0x1C,0x0,0x10,0x18,0x0,0x21C,0xAC", "");
            driver = m.ReadByte("ProjectG.exe+A40359");
            spin = m.ReadFloat("ProjectG.exe+0xAC79E0,0x1C,0x20,0x14,0x28,0x0,0x0,0x1C");
            curva = m.ReadFloat("ProjectG.exe+0xAC79E0,0x1C,0x20,0x14,0x28,0x0,0x0,0x18");
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
            resultadoautoquebra = Math.Round(((bolax * cos) + (bolay * senoInverso)) * -1 * (1 / 0.00745), 2); //0.00875 
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

            if (seno > 0.00 && cos < 0.00 || seno > 0.00 && cos > 0.00)
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
            if (e.KeyPressed == Key.F2)
            {
                fasttoma = true;
            }
            if (e.KeyPressed == Key.F3)
            {
                if (mapa != 1)
                {
                    double p = Math.Round(Convert.ToDouble(resultadoTEXT.Content),2);
                    string j;
                    j = Convert.ToString(autoPB(Distancia(pin1, tee1, pin3, tee3), gridPersonagem, p));
                    m.WriteMemory("ProjectG.exe+00A3D3A8,0xBC,0x0,0x0,0x0,0x4,0x6C,0x68", "float", j);
                }
            }
            if(e.KeyPressed == Key.F4)
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
        void Dunk(string x, double controle)
        {
            string spin;
            if (controle != 1 && driver == 0)
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
            else if (controle != 1 && driver == 1)
            {
                if (x == "Front")
                {
                    resultadoTEXT.Content = Convert.ToString(excWs.Cells[10, 10].Value);
                    calibradorTEXT.Content = Convert.ToString(excWs.Cells[12, 10].Value);
                    Calibrador(Convert.ToDouble(excWs.Cells[11, 10].Value));
                    spin = Convert.ToString(excWs.Cells[13, 10].Value);
                    m.WriteMemory("ProjectG.exe+00AC79E0,0x1C,0x20,0xC,0x2C,0x30,0x0,0x1C", "float", spin);
                }
                else
                {
                    resultadoTEXT.Content = Convert.ToString(excWs.Cells[10, 11].Value);
                    calibradorTEXT.Content = Convert.ToString(excWs.Cells[12, 11].Value);
                    Calibrador(Convert.ToDouble(excWs.Cells[11, 11].Value));
                    spin = Convert.ToString(excWs.Cells[13, 11].Value);
                    m.WriteMemory("ProjectG.exe+00AC79E0,0x1C,0x20,0xC,0x2C,0x30,0x0,0x1C", "float", spin);
                }
            }
            else if (controle != 1 && driver == 2)
            {
                if (x == "Front")
                {
                    resultadoTEXT.Content = Convert.ToString(excWs.Cells[18, 10].Value);
                    calibradorTEXT.Content = Convert.ToString(excWs.Cells[20, 10].Value);
                    Calibrador(Convert.ToDouble(excWs.Cells[19, 10].Value));
                    spin = Convert.ToString(excWs.Cells[21, 10].Value);
                    m.WriteMemory("ProjectG.exe+00AC79E0,0x1C,0x20,0xC,0x2C,0x30,0x0,0x1C", "float", spin);
                }
                else
                {
                    resultadoTEXT.Content = Convert.ToString(excWs.Cells[18, 11].Value);
                    calibradorTEXT.Content = Convert.ToString(excWs.Cells[20, 11].Value);
                    Calibrador(Convert.ToDouble(excWs.Cells[19, 11].Value));
                    spin = Convert.ToString(excWs.Cells[21, 11].Value);
                    m.WriteMemory("ProjectG.exe+00AC79E0,0x1C,0x20,0xC,0x2C,0x30,0x0,0x1C", "float", spin);
                }
            }
        }
        void Toma(string x, double controle)
        {
            if (controle != 1 && driver == 0)
            {
                if (x == "Front")
                {
                    resultadoTEXT.Content = Convert.ToString(excWs.Cells[2, 6].Value);
                    calibradorTEXT.Content = Convert.ToString(excWs.Cells[4, 6].Value);
                    Calibrador(Convert.ToDouble(excWs.Cells[3, 6].Value));
                    m.WriteMemory("ProjectG.exe+00AC79E0,0x1C,0x20,0xC,0x2C,0x30,0x0,0x1C", "float", "7");
                }
                else
                {
                    resultadoTEXT.Content = Convert.ToString(excWs.Cells[2, 7].Value);
                    calibradorTEXT.Content = Convert.ToString(excWs.Cells[4, 7].Value);
                    Calibrador(Convert.ToDouble(excWs.Cells[3, 7].Value));
                    m.WriteMemory("ProjectG.exe+00AC79E0,0x1C,0x20,0xC,0x2C,0x30,0x0,0x1C", "float", "7");
                }
            }
            else if (controle != 1 && driver == 1)
            {
                if (x == "Front")
                {
                    resultadoTEXT.Content = Convert.ToString(excWs.Cells[9, 6].Value);
                    calibradorTEXT.Content = Convert.ToString(excWs.Cells[11, 6].Value);
                    Calibrador(Convert.ToDouble(excWs.Cells[10, 6].Value));
                    m.WriteMemory("ProjectG.exe+00AC79E0,0x1C,0x20,0xC,0x2C,0x30,0x0,0x1C", "float", "7");
                }
                else
                {
                    resultadoTEXT.Content = Convert.ToString(excWs.Cells[9, 7].Value);
                    calibradorTEXT.Content = Convert.ToString(excWs.Cells[11, 7].Value);
                    Calibrador(Convert.ToDouble(excWs.Cells[10, 7].Value));
                    m.WriteMemory("ProjectG.exe+00AC79E0,0x1C,0x20,0xC,0x2C,0x30,0x0,0x1C", "float", "7");
                }
            }
            else if (controle != 1 && driver == 2)
            {
                if (x == "Front")
                {
                    resultadoTEXT.Content = Convert.ToString(excWs.Cells[16, 6].Value);
                    calibradorTEXT.Content = Convert.ToString(excWs.Cells[18, 6].Value);
                    Calibrador(Convert.ToDouble(excWs.Cells[17, 6].Value));
                    m.WriteMemory("ProjectG.exe+00AC79E0,0x1C,0x20,0xC,0x2C,0x30,0x0,0x1C", "float", "7");
                }
                else
                {
                    resultadoTEXT.Content = Convert.ToString(excWs.Cells[16, 7].Value);
                    calibradorTEXT.Content = Convert.ToString(excWs.Cells[18, 7].Value);
                    Calibrador(Convert.ToDouble(excWs.Cells[17, 7].Value));
                    m.WriteMemory("ProjectG.exe+00AC79E0,0x1C,0x20,0xC,0x2C,0x30,0x0,0x1C", "float", "7");
                }
            }
        }
        private void buttondunk_Click(object sender, RoutedEventArgs e)
        {
            Valores(mapa);
            if (mapa != 1 && fastdunk == true)
                Dunk(backoufront, mapa);
            else if (mapa != 1 && fasttoma == true)
                Toma(backoufront, mapa);
        }
        private void Calibrador(double porcentagem)
        {
            /*
             * CREDITOS SERA!!!
             * CREDITOS SERA!!!
             * CREDITOS SERA!!!
            */
            string p;
            if(mapa != 1)
            {
                double calibradorCorreto = porcentagemCali.Aggregate((x, y) => Math.Abs(x - porcentagem) < Math.Abs(y - porcentagem) ? x : y);
                p = Convert.ToString(Math.Round(500.0 - (100.0 - calibradorCorreto) * 3.6, 2));
                m.WriteMemory("ProjectG.exe+AC79E0,0x1C,0x20,0x14,0x18,0x0,0x46C,0x52C", "float", p);
                m.WriteMemory("ProjectG.exe+AC79E0,0x1C,0x20,0x14,0x18,0x0,0x46C,0x530", "float", p);
                m.WriteMemory("ProjectG.exe+AC79E0,0x1C,0x20,0x14,0x48,0x0,0x14,0x100", "float", p);
            } 
        }
    }
}
