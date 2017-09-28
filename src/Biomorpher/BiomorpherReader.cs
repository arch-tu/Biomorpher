﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino;
using System.Windows.Forms;
using System.Drawing;
using Rhino.Geometry;
using Biomorpher.IGA;
using Grasshopper.Kernel.Special;
using GalapagosComponents;
using Grasshopper.Kernel.Data;

namespace Biomorpher
{
    /// <summary>
    /// The Grasshopper component
    /// </summary>
    public class BiomorpherReader: GH_Component
    {
        public Grasshopper.GUI.Canvas.GH_Canvas canvas;
        private static readonly object syncLock = new object();
        private IGH_DataAccess deej;

        private OutputData localData = new OutputData();

        private List<GH_NumberSlider> sliders = new List<GH_NumberSlider>();
        private List<GalapagosGeneListObject> genepools = new List<GalapagosGeneListObject>();

        private GH_Integer branch, designID;
        private GH_Path historicPath;

        /// <summary>
        /// Main constructor
        /// </summary>
        public BiomorpherReader()
            : base("BiomorpherReader", "BiomorpherReader", "Uses Biomorpher data to display paramter states", "Params", "Util")
        {    
            canvas = Instances.ActiveCanvas;
            this.IconDisplayMode = GH_IconDisplayMode.icon;
        }

        /// <summary>
        /// Register component inputs
        /// </summary>
        /// <param name="pm"></param>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pm)
        {
            pm.AddGenericParameter("Data", "Data", "Biomorpher saved data", GH_ParamAccess.item);
            pm.AddPathParameter("HistoricPath", "HistoricPath", "Optional: if none provided, final population is used", GH_ParamAccess.item);
            pm.AddIntegerParameter("DesignID", "DesignID", "Design ID from the population", GH_ParamAccess.item, 0);

            pm[1].Optional = true;
        }
        
        /// <summary>
        /// Register component outputs
        /// </summary>
        /// <param name="pm"></param>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pm)
        {
           
            //pm[1].Simplify = true;
        }

        /// <summary>
        /// Grasshopper solveinstance
        /// </summary>
        /// <param name="DA"></param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            if (!DA.GetData<OutputData>("Data", ref localData)) { return; }
            if (!DA.GetData<GH_Path>("HistoricPath", ref historicPath))
            {
                historicPath = null;
            }
            if (!DA.GetData<GH_Integer>("DesignID", ref designID)) { return; };






            if(historicPath == null)
            {
                this.Message = "Final population";

                GH_Structure<GH_Number> numberTree = localData.GetPopulationData();
                
                //GH_Path 
                List<double> genes = new List<double>();
                for (int i = 0; i < numberTree.get_Branch(i).Count; i++)
                {
                    double myDouble;
                    GH_Convert.ToDouble(numberTree.get_Branch(designID.Value)[i], out myDouble, GH_Conversion.Primary);
                    genes.Add(myDouble);
                }

                //lock (syncLock)
                //{
                this.Locked = true;

                canvas.Document.Enabled = false;
                SetSliders(genes, localData.GetSliders(), localData.GetGenePools());
                canvas.Document.Enabled = true;
                
                ExpireSolution(true);
                canvas.Document.SolutionEnd += new GH_Document.SolutionEndEventHandler(EnableComponent);
                //}
            }

            else
            {
                this.Message = "Historic population";
            }

            //GH_Path myPath = new GH_Path(i, j, k);

        }

        public void EnableComponent(object sender, GH_SolutionEventArgs e)
        {
            this.Locked = false;
        }


        /// <summary>
        /// Sets the current slider values for a geven input chromosome
        /// </summary>
        /// <param name="chromo"></param>
        /// <param name="sliders"></param>
        /// <param name="genePools"></param>
        public void SetSliders(List<double> genes, List<GH_NumberSlider> sliders, List<GalapagosGeneListObject> genePools)
        {

            int sCount = sliders.Count;

            for (int i = 0; i < sCount; i++)
            {
                double min = (double)sliders[i].Slider.Minimum;
                double max = (double)sliders[i].Slider.Maximum;
                double range = max - min;

                sliders[i].Slider.Value = (decimal)(genes[i] * range + min);
            }

            // Set the gene pool values
            // Note that we use the back end of the genes, beyond the slider count
            int geneIndex = sCount;

            for (int i = 0; i < genePools.Count; i++)
            {
                for (int j = 0; j < genePools[i].Count; j++)
                {
                    Decimal myDecimal = System.Convert.ToDecimal(genes[geneIndex]);
                    genePools[i].set_NormalisedValue(j, myDecimal);

                    geneIndex++;
                }
                genePools[i].ExpireSolution(false);
            }
        }






        /// <summary>
        /// Gets the component guid
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("6DD8D538-EF87-453B-BF1B-77AD316FF9A0"); }
        }



        /// <summary>
        /// Locate the component with the rest of the rif raf
        /// </summary>
        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.senary;
            }
        }

        /// <summary>
        /// Icon icon what a lovely icon
        /// </summary>
        protected override Bitmap Icon
        {
            get
            {
                return Properties.Resources.BiomorpherReaderIcon_24;
            }
        }

        /// <summary>
        /// Extra fancy menu items
        /// </summary>
        /// <param name="menu"></param>
        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, @"Github source", GotoGithub);
        }

        /// <summary>
        /// Dare ye go to github?
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GotoGithub(Object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://github.com/johnharding/Biomorpher");
        }

        
    }
}
