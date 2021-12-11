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
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace S8_Private
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        public Mem m = new Mem();
        private BackgroundWorker BGW = new BackgroundWorker();
        private LowLevelKeyboardListener _listener;

        readonly _Application exc = new Microsoft.Office.Interop.Excel.Application();
        Workbook excWb;
        Worksheet excWs;

        double mapa;
        double tee1, tee2, tee3, pin1, pin2, pin3, eixox, eixoy, cosBola, senoBola, spin, curva, ball1, ball2, ball3, pangGanho;

        double pbtoyards, atanpbtoyards, gridPersonagem;

        double senoAngulo, cosAngulo;
        string vento, backoufront, esquerdaoudireita;

        int estadoBarra, terreno, driver, chipIN, controle, bolaEstado, pangBonusWater, controleKike;
        bool Aberto = false, fastdunk = false, fasttoma = false;      
        public MainWindow()

        {
            InitializeComponent();
            BGW.DoWork += BGW_DoWork;
            BGW.RunWorkerCompleted += BGW_RunWorkerCompleted;
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
                if (esquerdaoudireita == "Direita")
                    quebraTEXT.Content = Convert.ToString(quebraBola(senoBola, cosBola, eixox, eixoy) * -1);
                else
                    quebraTEXT.Content = Convert.ToString(quebraBola(senoBola, cosBola, eixox, eixoy));
                terrenoTEXT.Content = Convert.ToString(Terreno(terreno) + "%");
                pbTEXT.Content = Convert.ToString(pbTirado(pin1, tee1, pin3, tee3));
                spinTEXT.Content = Convert.ToString(Math.Round(spin, 2));
                curvaTEXT.Content = Convert.ToString(Math.Round(curva, 2));
                //CONTROLE DUNK
                if (fastdunk == true)
                {
                    calcular.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    controle = 0;
                    fastdunk = false;
                }
                //CONTROLE TOMA
                if (fasttoma == true)
                {
                    calcular.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    controle = 0;
                    fasttoma = false;
                }
                //PIXEL PANGYA
                if (estadoBarra == 255)
                {
                    //m.WriteMemory("ProjectG.exe+007229D8,0x1C,0x10,0x24,0x0,0x0,0x14,0xD8", "float", "140");  
                    m.WriteMemory("ProjectG.exe+00AC79E0,0x8,0x58,0x10,0x0,0x0,0x14,0xE8", "float", "140"); //PANGYA S8
                }
                if (bolaEstado != 0)
                {
                    controle++;
                    helperTEXT.Foreground = Brushes.Blue;
                    tigerTEXT.Foreground = Brushes.Blue;
                    if (pangGanho > 0 && controle == 1 && chipIN > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("Acertou!!");
                        Console.ResetColor();
                    }
                    else if (chipIN == 0 && bolaEstado != 0 && controle == 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Errou!!");
                        Console.ResetColor();
                        controleKike = 1;
                    }
                    else if(pangBonusWater == 1)
                    {
                        if(controleKike == 1)
                            Console.WriteLine("Bola kiko a: " + Distancia(pin1, ball1, pin3, ball3) + "y do Hole.");
                        controleKike = 0;
                    }     
                }
            }
            else
            {
                tigerTEXT.Foreground = Brushes.Red;
                helperTEXT.Foreground = Brushes.Red;
                anguloTEXT.Content = "0";
                ventoTEXT.Content = "0";
                distanciaTEXT.Content = "0";
                terrenoTEXT.Content = "0";
                pbTEXT.Content = "0";
                calibradorTEXT.Content = "0";
                resultadoTEXT.Content = "0";
                alturaTEXT.Content = "0";
            }
        }
        private void moveBUTTON_Click(object sender, RoutedEventArgs e)
        {
            string moveball1, moveball2, moveball3;
            moveball1 = Convert.ToString(Math.Round(pin1, 12));
            moveball3 = Convert.ToString(Math.Round(pin3, 12));
            moveball2 = Convert.ToString(Math.Round(pin2, 12));
            m.WriteMemory("ProjectG.exe+A10FB0", "float", moveball1);
            m.WriteMemory("ProjectG.exe+A10FB8", "float", moveball3);
            m.WriteMemory("ProjectG.exe+A10FB4", "float", moveball2);
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("-----------------------");
            Console.WriteLine("| HOLE POSITION X,Y,Z |");
            Console.WriteLine("| {0}    |", Math.Round(pin1, 12));
            Console.WriteLine("| {0}    |", Math.Round(pin2, 12));
            Console.WriteLine("| {0}    |", Math.Round(pin3, 12));
            Console.WriteLine("-----------------------");
            Console.WriteLine("| BALL POSITION X,Y,Z |");
            Console.WriteLine("| {0}    |", moveball1);
            Console.WriteLine("| {0}    |", moveball2);
            Console.WriteLine("| {0}    |", moveball3);
            Console.WriteLine("-----------------------");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Bola movida para o Hole");
            Console.ResetColor();
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
            try
            {
                exc.Visible = true;
                excWb = exc.Workbooks.Open(@"C:\280uk2.xlsx");
                excWs = excWb.Worksheets[1];
                excWs.Activate();
                _listener = new LowLevelKeyboardListener();
                _listener.OnKeyPressed += _listener_OnKeyPressed;
                _listener.HookKeyboard();
                AllocConsole();
                Console.WriteLine("=========== CRIADO POR GH0ST ===========");
                int penis = Console.WindowWidth;
                int penis2 = Console.WindowHeight;
                int penis3 = penis / 2;
                int penis4 = penis2;
                Console.SetWindowSize(penis3, penis4);
                BGW.RunWorkerAsync();

            }
            catch (Exception)
            {
                MessageBox.Show("Excel Not Found!, Excel Nao Encontrada!");
            }
        }
        void Memorias()
        {
            try
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
                estadoBarra = m.ReadByte("ProjectG.exe+00AC79E0,0x0,0x58,0x10,0x0,0x0,0x14,0xF8");
                //gridPersonagem = m.ReadFloat("ProjectG.exe+00A3D3A8,0xBC,0x0,0x0,0x0,0x4,0x6C,0x68", "", false); //CASO NÃO FUNCIONE SEU AUTO PB SO USAR ESSE GRID!!!
                gridPersonagem = m.ReadFloat("ProjectG.exe+00AC79E0,0x0,0x40,0x10,0xC,0x30,0x0,0x68", "", false);
                terreno = m.ReadInt("ProjectG.exe+AC79E0,0x1C,0x0,0x10,0x18,0x0,0x21C,0xAC", "");
                driver = m.ReadByte("ProjectG.exe+A40359");
                spin = m.ReadFloat("ProjectG.exe+0xAC79E0,0x1C,0x20,0x14,0x28,0x0,0x0,0x1C");
                curva = m.ReadFloat("ProjectG.exe+0xAC79E0,0x1C,0x20,0x14,0x28,0x0,0x0,0x18");
                ball1 = m.ReadFloat("ProjectG.exe+A10FB0", "", false);
                ball2 = m.ReadFloat("ProjectG.exe+A10FB4", "", false);
                ball3 = m.ReadFloat("ProjectG.exe+A10FB8", "", false);
                bolaEstado = m.ReadByte("00E10FC8", "");
                pangGanho = m.ReadInt("ProjectG.exe+0xAC79E0,0xA4,0x14,0x10,0x30,0x0,0x240,0x40", "");
                chipIN = m.ReadByte("ProjectG.exe+0xAC79E0,0xA4,0x14,0x10,0x30,0x0,0x240,0x41", "");
                pangBonusWater = m.ReadInt("ProjectG.exe+0xAC79E0,0x8,0x10,0x28,0x0,0x0,0x240,0x7D0", "");
            }
            catch (Exception)
            {
                Console.WriteLine("Algum endereco de memoria recebeu um valor que nao deveria");
            }
             /* PANGYA S4
            tee1 = m.ReadFloat("ProjectG.exe+670540", "", false);
            tee2 = m.ReadFloat("ProjectG.exe+670544", "", false);
            tee3 = m.ReadFloat("ProjectG.exe+670548", "", false);
            pin1 = m.ReadFloat("ProjectG.exe+71F33C", "", false);
            pin2 = m.ReadFloat("ProjectG.exe+71F340", "", false);
            pin3 = m.ReadFloat("ProjectG.exe+71F344", "", false);
            cosAngulo = m.ReadFloat("ProjectG.exe+007229D8,0x1C,0x20,0x10,0x0,0x0,0x1DC,0x68", "", false);  //Seno
            senoAngulo = m.ReadFloat("ProjectG.exe+007229D8,0x1C,0x10,0x10,0x30,0x0,0x1DC,0x70", "", false); //Cosseno
            eixox = m.ReadFloat("ProjectG.exe+007229D8,0x9C,0x28,0x20,0x18,0x0,0x1C4,0x1C", "", false); //Eixo X
            eixoy = m.ReadFloat("ProjectG.exe+007229D8,0x1C,0x20,0x3C,0x0,0x0,0x1C4,0x24", "", false); //Eixo Y
            cosBola = m.ReadFloat("ProjectG.exe+725398", "", false); // MIRA X
            senoBola = m.ReadFloat("ProjectG.exe+7253A0", "", false); // MIRA Y
            vento = m.ReadString("ProjectG.exe+007229D8,0x1C,0x20,0x18,0x0,0x1C8,0x14,0x0", "");
            estadoBarra = m.ReadByte("ProjectG.exe+007229D8,0x1C,0x10,0x24,0x0,0x0,0x14,0xE8");
            gridPersonagem = m.ReadFloat("ProjectG.exe+007229D8,0x8,0x40,0x44,0x40,0x30,0x0,0x140", "", false);
            terreno = m.ReadInt("ProjectG.exe+007229D8,0x1C,0x20,0x10,0x0,0x0,0x1C4,0x84");
            driver = m.ReadByte("ProjectG.exe+69B139");
            spin = m.ReadFloat("ProjectG.exe+007229D8,0x1C,0x34,0x14,0x18,0x0,0x24");
            curva = m.ReadFloat("ProjectG.exe+007229D8,0x1C,0x34,0x14,0x2C,0x30,0x0,0x20");
            */
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
            resultadoautoquebra = Math.Round(((bolax * cos) + (bolay * senoInverso)) * -1 * (1 / 0.00875), 2);
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
            return Math.Round(pb2, 2);
        }
        double Angulo(double x, double y)
        {
            if (x < 0)
                x *= -1;
            if (y < 0)
                y *= -1;
            return Math.Round(((Math.Asin(x) * 180 / Math.PI) + (Math.Acos(y) * 180 / Math.PI)) / 2, 2);
        }
        double Distancia(double x1, double x2, double y1, double y2)
        {
            return Math.Round(Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2)) * 0.312495, 2);
        }
        double Altura(double x1, double x2)
        {
            return Math.Round((pin2 - tee2 + 0.14) * (0.312495 * 0.914), 1);
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
                if (driver == 0)
                    escreverColorido("Calculando Dunk 1w.", ("{Dunk}", ConsoleColor.Green));
                else if (driver == 1)
                    escreverColorido("Calculando Dunk 2w.", ("{Dunk}", ConsoleColor.Green));
                else if (driver == 2)
                    escreverColorido("Calculando Dunk 3w.", ("{Dunk}", ConsoleColor.Green));
                else
                    Console.WriteLine("Penis");
                Console.WriteLine("");
                fastdunk = true;
            }
            if (e.KeyPressed == Key.F2)
            {
                if (driver == 0)
                    escreverColorido("Calculando Toma 1w.", ("{Toma}", ConsoleColor.Red));
                else if (driver == 1)
                    escreverColorido("Calculando Toma 2w.", ("{Toma}", ConsoleColor.Red));
                else if (driver == 2)
                    escreverColorido("Calculando Toma 3w.", ("{Toma}", ConsoleColor.Red));
                else
                    Console.WriteLine("Penis");
                Console.WriteLine("");
                fasttoma = true;
            }
            if (e.KeyPressed == Key.F3)
            {
                if (mapa != 1)//if (mapa != 1)
                {       
                    double p = Math.Round(Convert.ToDouble(resultadoTEXT.Content), 2);
                    string j,r;
                    r = Convert.ToString(p);
                    j = Convert.ToString(autoPB(Distancia(pin1, tee1, pin3, tee3), gridPersonagem, p));
                    //m.WriteMemory("ProjectG.exe+007229D8,0x8,0x40,0x44,0x40,0x30,0x0,0x140", "float", j);
                    m.WriteMemory("ProjectG.exe+00AC79E0,0x0,0x40,0x10,0xC,0x30,0x0,0x68", "float", j); //S8
                    //m.WriteMemory("ProjectG.exe+00A3D3A8,0xBC,0x0,0x0,0x0,0x4,0x6C,0x68", "float", j); //CASO SEU AUTO PB NAO FUNCIONE!
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine("------------------------------------");
                    Console.WriteLine("|         Tabela AUTO PB           |");
                    Console.WriteLine("| Grid Inicial: {0}", gridPersonagem + "   |");
                    Console.WriteLine("| Grid Final: {0}  ",j + "     |");
                    Console.WriteLine("|                                  |");
                    Console.WriteLine("------------------------------------");
                    Console.ResetColor();
                }
            }
            if (e.KeyPressed == Key.F4)
            {
                //m.WriteMemory("ProjectG.exe+007229D8,0x1C,0x10,0x24,0x0,0x0,0x14,0xD8", "float", "105"); //S4
                escreverColorido("Liberando movimento da barra.", ("{barra.}", ConsoleColor.Blue));
                Console.WriteLine("");
                m.WriteMemory("ProjectG.exe+00AC79E0,0x8,0x58,0x10,0x0,0x0,0x14,0xE8", "float", "105"); //S8
            }
        }
        void Valores(double controle)
        {
            double x;
            if (controle != 1)
            {
                excWs.Cells[4, 2].Value = distanciaTEXT.Content;
                x = Convert.ToDouble(alturaTEXT.Content);
                excWs.Cells[4, 3].Value = x;
                vento = vento.Substring(0, 1);
                excWs.Cells[4, 4].Value = vento;
                excWs.Cells[4, 5].Value = anguloTEXT.Content;
                excWs.Cells[6, 6].Value = quebraTEXT.Content;
                excWs.Cells[4, 8].Value = terrenoTEXT.Content;
            }
        }
        void Dunk(string x, double controle)
        {
            string spin;
            try
            {
                if (controle != 1 && driver == 0)
                {
                    if (x == "Front")
                    {
                        resultadoTEXT.Content = Convert.ToString(excWs.Cells[17, 3].Value);
                        calibradorTEXT.Content = Convert.ToString(excWs.Cells[21, 3].Value);
                        Calibrador(Convert.ToDouble(excWs.Cells[20, 3].Value));
                        spin = Convert.ToString(excWs.Cells[22, 3].Value);
                        //m.WriteMemory("ProjectG.exe+007229D8,0x1C,0x34,0x14,0x18,0x0,0x24", "float", spin);
                        m.WriteMemory("ProjectG.exe+00AC79E0,0x1C,0x20,0xC,0x2C,0x30,0x0,0x1C", "float", spin); //S8
                    }
                    else
                    {
                        resultadoTEXT.Content = Convert.ToString(excWs.Cells[17, 4].Value);
                        calibradorTEXT.Content = Convert.ToString(excWs.Cells[21, 4].Value);
                        Calibrador(Convert.ToDouble(excWs.Cells[20, 4].Value));
                        spin = Convert.ToString(excWs.Cells[22, 4].Value);
                        //m.WriteMemory("ProjectG.exe+007229D8,0x1C,0x34,0x14,0x18,0x0,0x24", "float", spin);
                        m.WriteMemory("ProjectG.exe+00AC79E0,0x1C,0x20,0xC,0x2C,0x30,0x0,0x1C", "float", spin); //S8
                    }
                }
                else if (controle != 1 && driver == 1)
                {
                    if (x == "Front")
                    {
                        resultadoTEXT.Content = Convert.ToString(excWs.Cells[17, 7].Value);
                        calibradorTEXT.Content = Convert.ToString(excWs.Cells[21, 7].Value);
                        Calibrador(Convert.ToDouble(excWs.Cells[20, 7].Value));
                        spin = Convert.ToString(excWs.Cells[22, 7].Value);
                        //m.WriteMemory("ProjectG.exe+007229D8,0x1C,0x34,0x14,0x18,0x0,0x24", "float", spin);
                        m.WriteMemory("ProjectG.exe+00AC79E0,0x1C,0x20,0xC,0x2C,0x30,0x0,0x1C", "float", spin); //S8
                    }
                    else
                    {
                        resultadoTEXT.Content = Convert.ToString(excWs.Cells[17, 8].Value);
                        calibradorTEXT.Content = Convert.ToString(excWs.Cells[21, 8].Value);
                        Calibrador(Convert.ToDouble(excWs.Cells[20, 8].Value));
                        spin = Convert.ToString(excWs.Cells[22, 8].Value);
                        //m.WriteMemory("ProjectG.exe+007229D8,0x1C,0x34,0x14,0x18,0x0,0x24", "float", spin);
                        m.WriteMemory("ProjectG.exe+00AC79E0,0x1C,0x20,0xC,0x2C,0x30,0x0,0x1C", "float", spin); //S8
                    }
                }
                else if (controle != 1 && driver == 2)
                {
                    if (x == "Front")
                    {
                        resultadoTEXT.Content = Convert.ToString(excWs.Cells[17, 11].Value);
                        calibradorTEXT.Content = Convert.ToString(excWs.Cells[21, 11].Value);
                        Calibrador(Convert.ToDouble(excWs.Cells[20, 11].Value));
                        spin = Convert.ToString(excWs.Cells[22, 11].Value);
                        //m.WriteMemory("ProjectG.exe+007229D8,0x1C,0x34,0x14,0x18,0x0,0x24", "float", spin);
                        m.WriteMemory("ProjectG.exe+00AC79E0,0x1C,0x20,0xC,0x2C,0x30,0x0,0x1C", "float", spin); //S8
                    }
                    else
                    {
                        resultadoTEXT.Content = Convert.ToString(excWs.Cells[17, 12].Value);
                        calibradorTEXT.Content = Convert.ToString(excWs.Cells[21, 12].Value);
                        Calibrador(Convert.ToDouble(excWs.Cells[20, 12].Value));
                        spin = Convert.ToString(excWs.Cells[22, 12].Value);
                        //m.WriteMemory("ProjectG.exe+007229D8,0x1C,0x34,0x14,0x18,0x0,0x24", "float", spin);
                        m.WriteMemory("ProjectG.exe+00AC79E0,0x1C,0x20,0xC,0x2C,0x30,0x0,0x1C", "float", spin); //S8
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Mude o driver para 1w, 2w ou 3w");
            }
        }
        void Toma(string x, double controle)
        {
            try
            {
                if (controle != 1 && driver == 0)
                {
                    if (x == "Front")
                    {
                        resultadoTEXT.Content = Convert.ToString(excWs.Cells[26, 3].Value);
                        calibradorTEXT.Content = Convert.ToString(excWs.Cells[30, 3].Value);
                        Calibrador(Convert.ToDouble(excWs.Cells[29, 3].Value));
                        //m.WriteMemory("ProjectG.exe+007229D8,0x1C,0x34,0x14,0x18,0x0,0x24", "float", "7"); //S4
                        m.WriteMemory("ProjectG.exe+00AC79E0,0x1C,0x20,0xC,0x2C,0x30,0x0,0x1C", "float", "7"); //S8
                    }
                    else
                    {
                        resultadoTEXT.Content = Convert.ToString(excWs.Cells[26, 4].Value);
                        calibradorTEXT.Content = Convert.ToString(excWs.Cells[30, 4].Value);
                        Calibrador(Convert.ToDouble(excWs.Cells[29, 4].Value));
                        //m.WriteMemory("ProjectG.exe+007229D8,0x1C,0x34,0x14,0x18,0x0,0x24", "float", "7"); //S4
                        m.WriteMemory("ProjectG.exe+00AC79E0,0x1C,0x20,0xC,0x2C,0x30,0x0,0x1C", "float", "7");
                    }
                }
                else if (controle != 1 && driver == 1)
                {
                    if (x == "Front")
                    {
                        resultadoTEXT.Content = Convert.ToString(excWs.Cells[26, 7].Value);
                        calibradorTEXT.Content = Convert.ToString(excWs.Cells[30, 7].Value);
                        Calibrador(Convert.ToDouble(excWs.Cells[29, 7].Value));
                        //m.WriteMemory("ProjectG.exe+007229D8,0x1C,0x34,0x14,0x18,0x0,0x24", "float", "7"); //S4
                        m.WriteMemory("ProjectG.exe+00AC79E0,0x1C,0x20,0xC,0x2C,0x30,0x0,0x1C", "float", "7");
                    }
                    else
                    {
                        resultadoTEXT.Content = Convert.ToString(excWs.Cells[26, 8].Value);
                        calibradorTEXT.Content = Convert.ToString(excWs.Cells[30, 8].Value);
                        Calibrador(Convert.ToDouble(excWs.Cells[29, 8].Value));
                        //m.WriteMemory("ProjectG.exe+007229D8,0x1C,0x34,0x14,0x18,0x0,0x24", "float", "7"); //S4
                        m.WriteMemory("ProjectG.exe+00AC79E0,0x1C,0x20,0xC,0x2C,0x30,0x0,0x1C", "float", "7");
                    }
                }
                else if (controle != 1 && driver == 2)
                {
                    if (x == "Front")
                    {
                        resultadoTEXT.Content = Convert.ToString(excWs.Cells[26, 11].Value);
                        calibradorTEXT.Content = Convert.ToString(excWs.Cells[30, 11].Value);
                        Calibrador(Convert.ToDouble(excWs.Cells[29, 11].Value));
                        //m.WriteMemory("ProjectG.exe+007229D8,0x1C,0x34,0x14,0x18,0x0,0x24", "float", "7"); //S4
                        m.WriteMemory("ProjectG.exe+00AC79E0,0x1C,0x20,0xC,0x2C,0x30,0x0,0x1C", "float", "7");
                    }
                    else
                    {
                        resultadoTEXT.Content = Convert.ToString(excWs.Cells[26, 12].Value);
                        calibradorTEXT.Content = Convert.ToString(excWs.Cells[30, 12].Value);
                        Calibrador(Convert.ToDouble(excWs.Cells[29, 12].Value));
                        //m.WriteMemory("ProjectG.exe+007229D8,0x1C,0x34,0x14,0x18,0x0,0x24", "float", "7"); //S4
                        m.WriteMemory("ProjectG.exe+00AC79E0,0x1C,0x20,0xC,0x2C,0x30,0x0,0x1C", "float", "7");
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Mude o driver para 1w, 2w ou 3w");
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
            string p;
            if(mapa != 1)//if (mapa == 171)
            {
                p = Convert.ToString(calibradorTEXT.Content);
                /*
                m.WriteMemory("ProjectG.exe+69B720", "float", p);
                m.WriteMemory("ProjectG.exe+007229D8,0x1C,0x10,0x48,0x0,0x3DC,0x20", "float", p);
                m.WriteMemory("ProjectG.exe+007229D8,0x1C,0x10,0x24,0x0,0x0,0x14,0xF0", "float", p);
                */ //PANGYA S4
                m.WriteMemory("ProjectG.exe+AC79E0,0x1C,0x20,0x14,0x18,0x0,0x46C,0x52C", "float", p);
                m.WriteMemory("ProjectG.exe+AC79E0,0x1C,0x20,0x14,0x18,0x0,0x46C,0x530", "float", p);
                m.WriteMemory("ProjectG.exe+AC79E0,0x1C,0x20,0x14,0x48,0x0,0x14,0x100", "float", p);
            }
        }
        void escreverColorido(string str, params (string substring, ConsoleColor color)[] colors)
        {
            var palavras = Regex.Split(str, @"( )");

            foreach (var palavra in palavras)
            {
                (string substring, ConsoleColor color) cl = colors.FirstOrDefault(x => x.substring.Equals("{" + palavra + "}"));
                if (cl.substring != null)
                {
                    Console.ForegroundColor = cl.color;
                    Console.Write(cl.substring.Substring(1, cl.substring.Length - 2));
                    Console.ResetColor();
                }
                else
                {
                    Console.Write(palavra);
                }
            }
        }
    }
}
