using grafo_pruebas_consola;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Grafo_Proyecto
{

    public partial class MainWindow : Window
    {
        //metodos del grafo
        Graph TheG = new Graph();
        Brush custcolor;
        Random r = new Random();
        //se encarga de conectar el principio de la linea con el final y se muevan a la ves, jungo al tringulo y el peso
        private Dictionary<Line, (Ellipse startelip, Ellipse endelip, Polygon pol, Label Wtx)> lineConnections = new Dictionary<Line, (Ellipse startelip, Ellipse endelip, Polygon pol, Label Wtx)>();
        //lo mismo que el de arriba pero agarra el circulo junto al texto de adentro
        private Dictionary<Ellipse, (Ellipse vis, Label Datatx,Node nodo)> elementnode = new Dictionary<Ellipse, (Ellipse vis, Label Datatx, Node nodo)>();
        //
        private double h = 40;
        private double w = 40;
        //
        private bool isDragging = false;
        private bool ArrowDrag = false;
        private Point clickPosition;
        private Line tempLine;
        char a = 'A';
        int peso;
        Ellipse actElip = null;
        bool flecha;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnCrear_Click(object sender, RoutedEventArgs e)
        {
            custcolor = new SolidColorBrush(Color.FromRgb((byte)r.Next(1, 255), (byte)r.Next(1, 255), (byte)r.Next(1, 255)));
            Ellipse ElipNode = new Ellipse
            {
                //rectangulo visible
                Height = h,
                Width = w,
                Fill = custcolor,
            };
            Label texto = new Label
            {
                Content = a,
                Foreground=Brushes.White,
                FontWeight = FontWeights.Bold
            };
            Ellipse over = new Ellipse
            {
                //invisible
                Height = 40,
                Width = 40,
                Fill = new SolidColorBrush(Colors.Transparent)
            };
            //incluye los eventos
            over.MouseLeftButtonDown += ElipNode_MouseLeftButtonDown;
            over.MouseMove += ElipNode_MouseMove;
            over.MouseLeftButtonUp += ElipNode_MouseLeftButtonUp;
            Canvas.SetLeft(ElipNode, 30);
            Canvas.SetTop(ElipNode, 30);
            cnvGraph.Children.Add(ElipNode);

            Canvas.SetLeft(texto, Canvas.GetLeft(ElipNode) + 10);
            Canvas.SetTop(texto, Canvas.GetTop(ElipNode) + 6);
            cnvGraph.Children.Add(texto);

            Canvas.SetLeft(over, 30);
            Canvas.SetTop(over, 30);
            cnvGraph.Children.Add(over);
            Node n = new Node(a);
            TheG.AddNode(n);
            elementnode[over] = (ElipNode, texto,n);
            a++;
        }

        private void ElipNode_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!flecha)
            {
                actElip = sender as Ellipse;
                isDragging = true;
                clickPosition = e.GetPosition(cnvGraph);
                elementnode[actElip].vis.CaptureMouse();
                elementnode[actElip].Datatx.CaptureMouse();
                actElip.CaptureMouse();
            }
            else
            {
                actElip = sender as Ellipse;
                ArrowDrag = true;
                clickPosition = e.GetPosition(cnvGraph);
                tempLine = new Line
                {
                    Stroke = elementnode[actElip].vis.Fill,
                    StrokeThickness = 3,
                    //coloca el extremo en el centro del nodo
                    X1 = Canvas.GetLeft(actElip) + w / 2,
                    Y1 = Canvas.GetTop(actElip) + h / 2,
                    X2 = clickPosition.X,
                    Y2 = clickPosition.Y
                };
                Canvas.SetZIndex(actElip, Canvas.GetZIndex(actElip) + 1);
                Canvas.SetZIndex(elementnode[actElip].vis, Canvas.GetZIndex(elementnode[actElip].vis) + 1);
                Canvas.SetZIndex(elementnode[actElip].Datatx, Canvas.GetZIndex(elementnode[actElip].Datatx) + 1);
                cnvGraph.Children.Add(tempLine);
                tempLine.CaptureMouse();
                tempLine.MouseMove += ArrowMove;
                tempLine.MouseLeftButtonUp += FinArrow;
            }
        }
        //movimiento dentro del nodo
        private void ElipNode_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && actElip != null)
            {
                var position = e.GetPosition(cnvGraph);
                var offset = position - clickPosition;
                //evita que se salga del canva
                double newX = Canvas.GetLeft(actElip) + offset.X;
                double newY = Canvas.GetTop(actElip) + offset.Y;
                newX = Math.Max(0, newX);
                newY = Math.Max(0, newY);
                newX = Math.Min(cnvGraph.ActualWidth - w, newX);
                newY = Math.Min(cnvGraph.ActualHeight - h, newY);

                Canvas.SetLeft(actElip, newX);
                Canvas.SetTop(actElip, newY);

                Canvas.SetLeft(elementnode[actElip].vis, newX);
                Canvas.SetTop(elementnode[actElip].vis, newY);

                Canvas.SetLeft(elementnode[actElip].Datatx, newX + 10);
                Canvas.SetTop(elementnode[actElip].Datatx, newY + 6);
                foreach (var entry in lineConnections)
                {
                    if (entry.Value.startelip == actElip || entry.Value.endelip == actElip)
                    {
                        Line line = entry.Key;
                        //posiciona el texto al 60% de la distancia
                        //aca se puede calcular la distancia y solo ir variando los porcentaje
                        double TextX = line.X1 + (line.X2 - line.X1) * 0.65;
                        double TextY = line.Y1 + (line.Y2 - line.Y1) * 0.65;
                        //diferencial para luego hacer el calculo de rotacion
                        double dx = Canvas.GetLeft(entry.Value.endelip) + w / 2 - (Canvas.GetLeft(entry.Value.startelip) + w / 2);
                        double dy = Canvas.GetTop(entry.Value.endelip) + h / 2 - (Canvas.GetTop(entry.Value.startelip) + h / 2);
                        double angle = Math.Atan2(dy, dx) * (180 / Math.PI);
                        //posicionamiento de la flecha o tringulo al 80% de la distancia
                        double ArX = line.X1 + (line.X2 - line.X1) * 0.85;
                        double ArY = line.Y1 + (line.Y2 - line.Y1) * 0.85;
                        RotateTransform rotateTransform = new RotateTransform(angle, 0, 0);
                        entry.Value.pol.RenderTransform = rotateTransform;
                        //seter ubicacion
                        Canvas.SetLeft(entry.Value.Wtx, TextX);
                        Canvas.SetTop(entry.Value.Wtx, TextY);
                        Canvas.SetLeft(entry.Value.pol, ArX);
                        Canvas.SetTop(entry.Value.pol, ArY);
                        //modificar el otro extremo de la linea
                        if (entry.Value.startelip == actElip)
                        {
                            line.X1 = Canvas.GetLeft(actElip) + w / 2;
                            line.Y1 = Canvas.GetTop(actElip) + h / 2;
                        }
                        if (entry.Value.endelip == actElip)
                        {
                            line.X2 = Canvas.GetLeft(actElip) + w / 2;
                            line.Y2 = Canvas.GetTop(actElip) + h / 2;
                        }
                    }
                }
                clickPosition = position;
            }
        }
        private void ElipNode_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            var algo = sender as Ellipse;
            elementnode[algo].vis.ReleaseMouseCapture();
            elementnode[algo].Datatx.ReleaseMouseCapture();
            algo.ReleaseMouseCapture();
            actElip = null;
        }
        //movimiento de las aristas
        private void ArrowMove(object sender, MouseEventArgs e)
        {
            if (ArrowDrag && tempLine != null)
            {
                clickPosition = e.GetPosition(cnvGraph);
                tempLine.X2 = clickPosition.X - 5;
                tempLine.Y2 = clickPosition.Y - 5;
            }
        }

        private void FinArrow(object sender, MouseButtonEventArgs e)
        {
            if (ArrowDrag)
            {
                var endPoint = e.GetPosition(cnvGraph);
                var hitTestResult = VisualTreeHelper.HitTest(cnvGraph, endPoint);
                DependencyObject hitObject;
                if (hitTestResult != null)
                {
                    hitObject = hitTestResult.VisualHit;
                    if (hitTestResult != null && hitObject is Ellipse elp && actElip != elp)
                    {
                        //&& TheG.Revisar(elementnode[actElip].nodo, elementnode[elp].nodo)
                        //agregar modificador de linea
                        Line inv=new Line();
                        bool f=false;
                        foreach (var item in lineConnections) {
                            if (item.Value.startelip == actElip && item.Value.endelip == elp) {
                                cnvGraph.Children.Remove(item.Key);
                                cnvGraph.Children.Remove(item.Value.Wtx);
                                cnvGraph.Children.Remove(item.Value.pol);
                                lineConnections.Remove(item.Key);
                            }
                        }
                        //confusiones
                        // act lip es el nodo inicial y elp es el 
                        //conectar grafo
                        peso = TheG.Conect(elementnode[actElip].nodo, elementnode[elp].nodo, peso);
                        //coloca el extremo en el centro del nodo
                        tempLine.X2 = Canvas.GetLeft(elp) + w / 2;
                        tempLine.Y2 = Canvas.GetTop(elp) + h / 2;
                        //reposiciona 
                        double dx = Canvas.GetLeft(elp) + w / 2 - (Canvas.GetLeft(actElip) + w / 2);
                        double dy = Canvas.GetTop(elp) + h / 2 - (Canvas.GetTop(actElip) + h / 2);
                        double angle = Math.Atan2(dy, dx) * (180 / Math.PI);
                        //crea triangulo
                        Polygon polygon = new Polygon
                        {
                            Stroke = elementnode[actElip].vis.Fill,
                            Fill = elementnode[actElip].vis.Fill,
                            StrokeThickness = 2,
                            //ni idea de porque lo hace de esta manera
                            Points = new PointCollection(new[] { new Point(0, 0), new Point(-10, -5), new Point(-10, 5) })
                        };
                        RotateTransform rotateTransform = new RotateTransform(angle, 0, 0);
                        polygon.RenderTransform = rotateTransform;
                        cnvGraph.Children.Add(polygon);
                        Label textBlock = new Label
                        {
                            FontSize =20,
                            Content = $"Peso {peso}",
                            Foreground=Brushes.White,
                        };
                        cnvGraph.Children.Add(textBlock);
                        //aca se puede calcular la distancia y solo ir variando los porcentaje
                        double TextX = tempLine.X1 + (tempLine.X2 - tempLine.X1) * 0.65;
                        double TextY = tempLine.Y1 + (tempLine.Y2 - tempLine.Y1) * 0.65;
                        double ArX = tempLine.X1 + (tempLine.X2 - tempLine.X1) * 0.85;
                        double ArY = tempLine.Y1 + (tempLine.Y2 - tempLine.Y1) * 0.85;
                        Canvas.SetLeft(textBlock, TextX);
                        Canvas.SetTop(textBlock, TextY);
                        //coloca encima de la flecha todos los elementos del nodo
                        Canvas.SetZIndex(elp, Canvas.GetZIndex(elp) + 1);
                        Canvas.SetZIndex(elementnode[elp].vis, Canvas.GetZIndex(elementnode[elp].vis) + 1);
                        Canvas.SetZIndex(elementnode[elp].Datatx, Canvas.GetZIndex(elementnode[elp].Datatx) + 1);
                        Canvas.SetLeft(polygon, ArX);
                        Canvas.SetTop(polygon, ArY);
                        ArrowDrag = false;
                        //desactiva el modo flecha
                        flecha = false;
                        //almacena los elemento que corresponde a la lina, nodo inicial, nodo final y el resto
                        lineConnections[tempLine] = (actElip, elp, polygon, textBlock);
                        //no se que hace esto pero 
                        tempLine.ReleaseMouseCapture();
                    }
                    else
                    {
                        cnvGraph.Children.Remove(tempLine);
                    }
                }
                else
                {
                    cnvGraph.Children.Remove(tempLine);
                }
            }
        }
        private void btnConectar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                peso = int.Parse(txbPeso.Text);
                flecha= !flecha;
                txbPeso.Clear();
            }
            catch (Exception f){
                MessageBox.Show("Invalido");
            }
        }

        private void btnMat_Click(object sender, RoutedEventArgs e)
        {
            TheG.setMat();
            lsvMat.View = CreateGridViewForMatrix(TheG.Matrix);
            lsvMat.ItemsSource = ConvertMatrixToList(TheG.Matrix);
        }
        private GridView CreateGridViewForMatrix(int[,] matrix)
        {
            var gridView = new GridView();

            // Agregar una columna para el índice de fila
            gridView.Columns.Add(new GridViewColumn
            {
                DisplayMemberBinding = new System.Windows.Data.Binding("[0]")
            });
            char a= 'A';
            for (int i = 0; i < matrix.GetLength(1); i++) // Columnas
            {
                gridView.Columns.Add(new GridViewColumn
                {
                    Header = $"{a}", // Actualizar para compensar la columna del índice
                    DisplayMemberBinding = new System.Windows.Data.Binding($"[{i + 1}]")
                });
                a++;
            }
            return gridView;
        }
        private List<List<string>> ConvertMatrixToList(int[,] matrix)
        {
            var list = new List<List<string>>();
            char a = 'A';
            for (int i = 0; i < matrix.GetLength(0); i++) 
            {
                var rowList = new List<string>();
                for (int j = 0; j < matrix.GetLength(1)+1; j++) 
                {
                    if (j == 0) {
                        rowList.Add(a.ToString());
                        a++;
                    }
                    else
                        rowList.Add(matrix[i, j-1].ToString());
                }
                list.Add(rowList);
            }
            return list;
        }

        private void btnList_Click(object sender, RoutedEventArgs e)
        {
            lsList.ItemsSource = null;
            lsList.Items.Clear();
            for (int i = 0; i < TheG.TheNode.Count; i++) {
                String texto = $"{TheG.TheNode[i].Data} -> ";
                for(int j = 0; j < TheG.TheNode[i].Vert.Count; j++)
                {
                    texto += $"{TheG.TheNode[i].Vert[j].Target.Data} -> ";
                }
                texto +="null";
                lsList.Items.Add(texto);
            }
        }

        private void btnRest_Click(object sender, RoutedEventArgs e)
        {
            cnvGraph.Children.Clear();
            TheG.TheNode.Clear();
            a = 'A';
        }

        private void btnstart_Click(object sender, RoutedEventArgs e)
        {
            lsbbfs.Items.Clear();
            lsbdfs.Items.Clear();
            if (TheG.TheNode.Count > 0) {

                Queue<Node> bfs = TheG.BFS();
                Stack<Node> dfs = TheG.DFS();
                int s = bfs.Count;
                for (int i = 0; i < s; i++) { 
                    Node temp=bfs.Dequeue();
                    lsbbfs.Items.Add(temp.Data);
                    temp = dfs.Pop();
                    lsbdfs.Items.Add(temp.Data);
                }
            }
        }

        private void btnDjk_Click(object sender, RoutedEventArgs e)
        {
            if (TheG.TheNode.Count>0)
            {
                try {
                    TheG.setMat();
                    char a = char.Parse(txtOri.Text);
                    a=char.ToUpper(a);
                    char b = char.Parse(txtDest.Text);
                    b = char.ToUpper(b);
                    lblDjk.Content = TheG.dijkstra(a, b)==int.MaxValue?0: TheG.dijkstra(a, b);
                }
                catch (Exception ex) {
                    MessageBox.Show("Ingrese Nodos Correctos");
                }
            }
        }
    }
}