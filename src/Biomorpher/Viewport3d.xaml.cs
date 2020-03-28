﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HelixToolkit.Wpf;
using System.Windows.Media.Media3D;
using Rhino.Geometry;
using Biomorpher.IGA;

namespace Biomorpher
{
    /// <summary>
    /// Interaction logic for Viewport3d.xaml
    /// </summary>
    public partial class Viewport3d : UserControl
    {
        private int ID;
        private BiomorpherWindow W;
        private HelixViewport3D hVp3D;
        //HelixToolkit.Wpf.SharpDX.Viewport3DX hVp3D;
        public Viewport3d(List<Mesh> meshes, int id, BiomorpherWindow w, bool hasViewcube)
        {
            ID = id;
            W = w;
            InitializeComponent();
            create3DViewPort(meshes, hasViewcube);
        }

        private void create3DViewPort(List<Mesh> rMesh, bool hasViewcube)
        {
            hVp3D = new HelixViewport3D();
            //hVp3D = new HelixToolkit.Wpf.SharpDX.Viewport3DX();
 
            //Settings
            hVp3D.ShowFrameRate = false;
            hVp3D.ViewCubeOpacity = 0.1;
            hVp3D.ViewCubeTopText = "T";
            hVp3D.ViewCubeBottomText = "B";
            hVp3D.ViewCubeFrontText = "E";
            hVp3D.ViewCubeRightText = "N";
            hVp3D.ViewCubeLeftText = "S";
            hVp3D.ViewCubeBackText = "W";
            hVp3D.ViewCubeHeight = 40;
            hVp3D.ViewCubeWidth = 40;
            hVp3D.ShowViewCube = hasViewcube;
            var lights = new DefaultLights();
            hVp3D.Children.Add(lights);
            hVp3D.IsInertiaEnabled = true;
            hVp3D.ZoomExtentsWhenLoaded = true;
            hVp3D.Background = new SolidColorBrush(Color.FromArgb(255, 157, 163, 170));
            //hVp3D.Camera = Friends.dummyHelix.Camera;

            List<ModelVisual3D> vis = new List<ModelVisual3D>();

            for (int i = 0; i < rMesh.Count; i++)
            {
                if(rMesh[i] != null)
                {
                    MeshGeometry3D wMesh = new MeshGeometry3D();
                    DiffuseMaterial material = new DiffuseMaterial();
                    Friends.ConvertRhinotoWpfMesh(rMesh[i], wMesh, material);
                    GeometryModel3D model = new GeometryModel3D(wMesh, material);

                    model.BackMaterial = material;
                    ModelVisual3D v = new ModelVisual3D();
                    v.Content = model;
                    
                    vis.Add(v);

                }
            }

            for (int i = 0; i < vis.Count; i++)
            {
                hVp3D.Children.Add(vis[i]);
            }

            //Add viewport to user control
            this.AddChild(hVp3D);


            /*
            ContextMenu myMenu = new ContextMenu();

            MenuItem item1 = new MenuItem();
            MenuItem item2 = new MenuItem();

            item1.Header = "item1";
            //item1.Click += new RoutedEventHandler(item1_Click);
            myMenu.Items.Add(item1);

            item2.Header = "item2";
            //item2.Click += new RoutedEventHandler(item2_Click);
            myMenu.Items.Add(item2);

            //this.ContextMenu = myMenu;
            //myMenu.IsOpen = true;
            hVp3D.ContextMenu = myMenu;
             */

        }

        


        public void SetCamera(ProjectionCamera cam)
        {
            hVp3D.Camera = cam;
        }

        public ProjectionCamera GetCamera()
        {
            return hVp3D.Camera;
        }


        /// <summary>
        /// Matches this camera to the others
        /// </summary>
        public void MatchCamera()
        {
            try
            {
                for (int i = 0; i < 12; i++)
                {
                    string dp_name = "dp_tab2_" + i;

                    DockPanel dp = (DockPanel)W.GetControls()[dp_name];
   
                    Viewport3d myViewport = (Viewport3d)dp.Children[1];
                    myViewport.SetCamera(this.GetCamera());

                }
            }
            catch
            {
            }

        }

        /// <summary>
        /// Double click to set the Grasshopper instance
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            Population pop = W.GetPopulation();

            foreach (Chromosome jimmy in pop.chromosomes)
            {
                if(jimmy.isRepresentative && jimmy.clusterId==ID)
                {
                    W.SetInstance(jimmy);
                    break;
                }
            }

        }

        /*
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);
        }
         */

    }
}
